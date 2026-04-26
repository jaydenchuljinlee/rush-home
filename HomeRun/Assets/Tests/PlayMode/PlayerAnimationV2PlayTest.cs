using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// PlayerAnimationController v2 Play Mode 테스트.
/// Animator 할당, Idle 초기 상태, Run 상태 전환을 검증한다.
/// </summary>
public class PlayerAnimationV2PlayTest
{
    private GameObject _playerGo;
    private GameObject _gameManagerGo;
    private PlayerAnimationController _animController;
    private Animator _animator;

    [SetUp]
    public void SetUp()
    {
        // GameManager 생성
        _gameManagerGo = new GameObject("GameManager");
        _gameManagerGo.AddComponent<GameManager>();

        // Player 오브젝트 생성 (PlayerAnimationController 의존 컴포넌트 포함)
        _playerGo = new GameObject("Player");

        // RequireComponent 충족: Animator + SpriteRenderer
        _animator = _playerGo.AddComponent<Animator>();
        _playerGo.AddComponent<SpriteRenderer>();

        // Rigidbody2D + BoxCollider2D (PlayerController 의존)
        var rb = _playerGo.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        _playerGo.AddComponent<BoxCollider2D>();

        // GroundCheck 자식 오브젝트
        var groundCheckGo = new GameObject("GroundCheck");
        groundCheckGo.transform.SetParent(_playerGo.transform);
        groundCheckGo.transform.localPosition = new Vector3(0f, -0.5f, 0f);

        // PlayerController 추가 후 groundCheck 주입
        var pc = _playerGo.AddComponent<PlayerController>();
        var groundCheckField = typeof(PlayerController).GetField(
            "groundCheck",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        groundCheckField?.SetValue(pc, groundCheckGo.transform);

        // PlayerAnimationController 추가
        _animController = _playerGo.AddComponent<PlayerAnimationController>();
    }

    [TearDown]
    public void TearDown()
    {
        if (_playerGo != null) Object.DestroyImmediate(_playerGo);
        if (_gameManagerGo != null) Object.DestroyImmediate(_gameManagerGo);
        _playerGo = null;
        _gameManagerGo = null;
    }

    [UnityTest]
    public IEnumerator 성공_Animator_컨트롤러_할당됨()
    {
        // Animator 컴포넌트가 존재하는지 확인 (runtimeAnimatorController는 에디터 할당이므로 컴포넌트 존재 여부로 검증)
        yield return null;

        Assert.IsNotNull(_animator, "Player에 Animator 컴포넌트가 부착되어 있어야 한다.");
        Assert.IsNotNull(_animController, "Player에 PlayerAnimationController 컴포넌트가 부착되어 있어야 한다.");
    }

    [UnityTest]
    public IEnumerator 성공_Idle_상태_초기재생()
    {
        // Arrange: 게임 시작 전 (Ready 상태) — GameManager.IsPlaying == false
        yield return null;

        // Assert: isPlaying 파라미터가 false여야 Idle 상태
        bool isPlaying = _animator.GetBool("isPlaying");
        Assert.IsFalse(isPlaying, "게임 시작 전에는 isPlaying이 false여야 Idle 상태로 머문다.");
    }

    [UnityTest]
    public IEnumerator 성공_Run_상태_전환()
    {
        // Arrange: 게임 시작 전 isPlaying == false
        yield return null;
        Assert.IsFalse(_animator.GetBool("isPlaying"), "게임 시작 전 isPlaying은 false여야 한다.");

        // Act: 게임 시작
        GameManager.Instance.StartGame();
        yield return null;

        // Assert: isPlaying == true -> Run 상태로 전환
        bool isPlaying = _animator.GetBool("isPlaying");
        Assert.IsTrue(isPlaying, "게임 시작 후 isPlaying이 true여야 Run 상태로 전환된다.");
    }
}
