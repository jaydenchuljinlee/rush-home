using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// 장애물 시스템 Play Mode 테스트.
/// 충돌 감지 및 풀 반환 동작 검증.
/// 각 테스트는 독립적인 GameManager를 생성하고 TearDown에서 정리한다.
/// </summary>
public class ObstaclePlayTest
{
    private GameObject _playerGo;
    private GameObject _obstacleGo;
    private GameObject _gameManagerGo;
    private bool _playerHitFired;

    [SetUp]
    public void SetUp()
    {
        _playerHitFired = false;
        PlayerController.OnPlayerHit += OnPlayerHit;
    }

    [TearDown]
    public void TearDown()
    {
        PlayerController.OnPlayerHit -= OnPlayerHit;

        if (_playerGo != null) Object.Destroy(_playerGo);
        if (_obstacleGo != null) Object.Destroy(_obstacleGo);
        if (_gameManagerGo != null) Object.Destroy(_gameManagerGo);

        // 싱글톤 Instance 초기화 (다음 테스트와 격리)
        _playerGo = null;
        _obstacleGo = null;
        _gameManagerGo = null;
    }

    private void OnPlayerHit()
    {
        _playerHitFired = true;
    }

    private GameManager CreatePlayingGameManager()
    {
        _gameManagerGo = new GameObject("GameManager");
        GameManager gm = _gameManagerGo.AddComponent<GameManager>();
        gm.StartGame();
        return gm;
    }

    [UnityTest]
    public IEnumerator 성공_장애물_충돌시_OnPlayerHit_발생()
    {
        // Arrange: GameManager 생성 (Playing 상태)
        CreatePlayingGameManager();

        // 플레이어 생성
        _playerGo = new GameObject("Player");
        _playerGo.tag = "Player";
        Rigidbody2D rb = _playerGo.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        BoxCollider2D playerCollider = _playerGo.AddComponent<BoxCollider2D>();
        playerCollider.isTrigger = false;

        // GroundCheck 자식 오브젝트 + SerializeField 주입
        GameObject groundCheckGo = new GameObject("GroundCheck");
        groundCheckGo.transform.SetParent(_playerGo.transform);
        groundCheckGo.transform.localPosition = new Vector3(0f, -0.5f, 0f);

        PlayerController pc = _playerGo.AddComponent<PlayerController>();
        var groundCheckField = typeof(PlayerController).GetField(
            "groundCheck",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        groundCheckField?.SetValue(pc, groundCheckGo.transform);

        // 장애물 생성
        _obstacleGo = new GameObject("Obstacle");
        _obstacleGo.tag = "Obstacle";
        BoxCollider2D obstacleCollider = _obstacleGo.AddComponent<BoxCollider2D>();
        obstacleCollider.isTrigger = true;
        Obstacle obstacle = _obstacleGo.AddComponent<Obstacle>();
        obstacle.Initialize(0f, null);

        // 같은 위치에 배치 (충돌 유도)
        _playerGo.transform.position = Vector3.zero;
        _obstacleGo.transform.position = Vector3.zero;

        // Act: 물리 처리 대기
        yield return new WaitForFixedUpdate();
        yield return null;

        // Assert
        Assert.IsTrue(_playerHitFired, "장애물과 충돌 시 OnPlayerHit 이벤트가 발생해야 한다.");
    }

    [UnityTest]
    public IEnumerator 성공_장애물_화면밖_이탈시_비활성화_또는_파괴()
    {
        // Arrange: 별도 GameManager 생성 (Playing 상태)
        CreatePlayingGameManager();

        // 장애물을 화면 밖 X 위치에 배치 (destroyX = -15f 기준)
        _obstacleGo = new GameObject("Obstacle");
        _obstacleGo.tag = "Obstacle";
        _obstacleGo.AddComponent<BoxCollider2D>().isTrigger = true;
        Obstacle obstacle = _obstacleGo.AddComponent<Obstacle>();
        obstacle.Initialize(1f, null); // 속도>0, 풀 없음 -> Destroy 경로

        // destroyX(-15)보다 훨씬 왼쪽에 배치 -> 즉시 파괴 조건 만족
        _obstacleGo.transform.position = new Vector3(-20f, 0f, 0f);

        Assert.AreEqual(GameState.Playing, GameManager.Instance.CurrentState,
            "GameManager가 Playing 상태여야 한다.");

        // Act: Update 프레임 실행 -> Obstacle.Update -> destroyX 체크 -> Destroy
        yield return null;

        // Assert: 파괴됨 (pool=null이므로 Destroy 호출)
        bool destroyed = _obstacleGo == null;
        Assert.IsTrue(destroyed, "화면 밖으로 나간 장애물은 비활성화 또는 파괴되어야 한다.");

        _obstacleGo = null;
    }
}
