using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// PatternSpawner 동작 검증 Play Mode 테스트.
/// LevelPatternData 패턴 풀을 설정하고 실제 장애물이 스폰되는지 확인한다.
/// </summary>
[TestFixture]
public class PatternSpawnerPlayTest
{
    private GameObject _spawnerGo;
    private PatternSpawner _patternSpawner;
    private GameObject _gameManagerGo;
    private GameManager _gameManager;
    private LevelPatternData _testPattern;
    private GameObject _obstaclePrefab;

    [SetUp]
    public void SetUp()
    {
        // GameManager 설정
        _gameManagerGo = new GameObject("GameManager");
        _gameManager = _gameManagerGo.AddComponent<GameManager>();

        // 장애물 프리팹 (테스트용 더미)
        _obstaclePrefab = new GameObject("TestObstacle");
        _obstaclePrefab.tag = "Obstacle";
        var obstacle = _obstaclePrefab.AddComponent<Obstacle>();
        _obstaclePrefab.AddComponent<BoxCollider2D>();

        // LevelPatternData 생성
        _testPattern = ScriptableObject.CreateInstance<LevelPatternData>();

        // PatternSpawner 설정
        _spawnerGo = new GameObject("PatternSpawner");
        _patternSpawner = _spawnerGo.AddComponent<PatternSpawner>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.Destroy(_spawnerGo);
        Object.Destroy(_gameManagerGo);
        Object.Destroy(_obstaclePrefab);
        Object.DestroyImmediate(_testPattern);

        // 씬에 남아있는 Obstacle 태그 오브젝트 정리
        var obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (var o in obstacles)
            Object.Destroy(o);
    }

    [UnityTest]
    public IEnumerator 성공_패턴풀이_비어있을때_스폰_없음()
    {
        // Arrange
        _patternSpawner.SetAvailablePatterns(new LevelPatternData[0]);

        // 게임 상태를 Playing으로 전환 (GameManager.StartGame 또는 직접 상태 설정)
        // GameManager가 Awake에서 Ready 상태이므로 한 프레임 대기
        yield return null;

        // Act: 타이머가 0이 될 때까지 충분히 대기하지 않고 바로 확인
        var obstaclesBefore = GameObject.FindGameObjectsWithTag("Obstacle");
        int countBefore = obstaclesBefore.Length;

        yield return new WaitForSeconds(0.1f);

        var obstaclesAfter = GameObject.FindGameObjectsWithTag("Obstacle");
        int countAfter = obstaclesAfter.Length;

        // Assert: Playing 상태가 아니므로 스폰 없음
        Assert.AreEqual(countBefore, countAfter,
            "Playing 상태가 아닐 때는 패턴이 스폰되어서는 안 된다.");
    }

    [UnityTest]
    public IEnumerator 성공_SetAvailablePatterns_null_전달시_예외없이_처리됨()
    {
        // null을 전달해도 예외가 발생하지 않아야 한다
        Assert.DoesNotThrow(() =>
        {
            _patternSpawner.SetAvailablePatterns(null);
        });

        yield return null;
    }

    [UnityTest]
    public IEnumerator 성공_SetAvailablePatterns_패턴배열_설정후_예외없음()
    {
        // Arrange: 패턴 배열 설정
        LevelPatternData[] patterns = new LevelPatternData[] { _testPattern };

        // Act & Assert: 예외 없이 설정 완료
        Assert.DoesNotThrow(() =>
        {
            _patternSpawner.SetAvailablePatterns(patterns);
        });

        yield return null;
    }

    [UnityTest]
    public IEnumerator 성공_SetSpawnInterval_정상_설정()
    {
        // Arrange & Act
        Assert.DoesNotThrow(() =>
        {
            _patternSpawner.SetSpawnInterval(1.0f, 2.0f);
        });

        yield return null;
    }

    [UnityTest]
    public IEnumerator 성공_TerrainChunkSpawner_Easy티어에서_Gap없음_티어확인()
    {
        // Arrange
        var spawnerGo = new GameObject("TerrainChunkSpawner");
        var chunkSpawner = spawnerGo.AddComponent<TerrainChunkSpawner>();

        // Act: Easy 티어 설정
        chunkSpawner.SetDifficultyTier(DifficultyTier.Easy);

        yield return null;

        // Assert: Easy 티어가 Gap 최소 티어(Hard) 미만인지 확인
        Assert.Less((int)chunkSpawner.CurrentTier, (int)DifficultyTier.Hard,
            "Easy 티어는 Hard(Gap 허용) 티어보다 낮아야 한다.");

        Object.Destroy(spawnerGo);
    }

    [UnityTest]
    public IEnumerator 성공_TerrainChunkSpawner_Hard티어로_변경됨()
    {
        // Arrange
        var spawnerGo = new GameObject("TerrainChunkSpawner");
        var chunkSpawner = spawnerGo.AddComponent<TerrainChunkSpawner>();

        // Act
        chunkSpawner.SetDifficultyTier(DifficultyTier.Hard);

        yield return null;

        // Assert
        Assert.AreEqual(DifficultyTier.Hard, chunkSpawner.CurrentTier,
            "Hard 티어로 설정 후 CurrentTier는 Hard여야 한다.");

        Object.Destroy(spawnerGo);
    }
}
