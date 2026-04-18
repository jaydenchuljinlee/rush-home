using NUnit.Framework;
using UnityEngine;

/// <summary>
/// DifficultyData 순수 로직 테스트.
/// ScriptableObject를 CreateInstance로 생성하여 Unity 에디터 없이 실행 가능.
/// </summary>
[TestFixture]
public class DifficultyDataTest
{
    private DifficultyData _difficultyData;

    [SetUp]
    public void SetUp()
    {
        _difficultyData = ScriptableObject.CreateInstance<DifficultyData>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_difficultyData);
    }

    // ---- GetStage ----

    [Test]
    public void 성공_0초경과시_첫번째단계_반환()
    {
        // Arrange
        float elapsed = 0f;

        // Act
        DifficultyStage stage = _difficultyData.GetStage(elapsed);

        // Assert
        Assert.AreEqual(0f, stage.startTime);
    }

    [Test]
    public void 성공_30초경과시_보통단계_반환()
    {
        // Arrange
        float elapsed = 30f;

        // Act
        DifficultyStage stage = _difficultyData.GetStage(elapsed);

        // Assert
        Assert.AreEqual(30f, stage.startTime);
    }

    [Test]
    public void 성공_60초경과시_어려운단계_반환()
    {
        // Arrange
        float elapsed = 60f;

        // Act
        DifficultyStage stage = _difficultyData.GetStage(elapsed);

        // Assert
        Assert.AreEqual(60f, stage.startTime);
    }

    [Test]
    public void 성공_120초경과시_지옥단계_반환()
    {
        // Arrange
        float elapsed = 120f;

        // Act
        DifficultyStage stage = _difficultyData.GetStage(elapsed);

        // Assert
        Assert.AreEqual(120f, stage.startTime);
    }

    [Test]
    public void 성공_200초경과시_마지막단계_유지()
    {
        // Arrange
        float elapsed = 200f;

        // Act
        DifficultyStage stage = _difficultyData.GetStage(elapsed);

        // Assert
        Assert.AreEqual(120f, stage.startTime,
            "200초에는 마지막 단계(120초)가 유지되어야 한다.");
    }

    // ---- GetScrollSpeed ----

    [Test]
    public void 성공_0초에_쉬움단계_스크롤속도_반환()
    {
        // Arrange & Act
        float speed = _difficultyData.GetScrollSpeed(0f);

        // Assert
        Assert.Greater(speed, 0f, "스크롤 속도는 양수여야 한다.");
        Assert.AreEqual(_difficultyData.Stages[0].scrollSpeed, speed);
    }

    [Test]
    public void 성공_난이도단계별_스크롤속도_증가()
    {
        // Arrange & Act
        float speed0 = _difficultyData.GetScrollSpeed(0f);
        float speed30 = _difficultyData.GetScrollSpeed(30f);
        float speed60 = _difficultyData.GetScrollSpeed(60f);
        float speed120 = _difficultyData.GetScrollSpeed(120f);

        // Assert — 각 단계는 이전 단계보다 빨라야 한다
        Assert.Less(speed0, speed30, "30초 단계는 0초 단계보다 빨라야 한다.");
        Assert.Less(speed30, speed60, "60초 단계는 30초 단계보다 빨라야 한다.");
        Assert.Less(speed60, speed120, "120초 단계는 60초 단계보다 빨라야 한다.");
    }

    // ---- GetRandomSpawnInterval ----

    [Test]
    public void 성공_스폰간격이_최솟값이상_최댓값이하()
    {
        // Arrange
        float elapsed = 0f;
        DifficultyStage stage = _difficultyData.GetStage(elapsed);

        // Act — 10회 반복하여 범위 검증
        for (int i = 0; i < 10; i++)
        {
            float interval = _difficultyData.GetRandomSpawnInterval(elapsed);

            // Assert
            Assert.GreaterOrEqual(interval, stage.spawnIntervalMin,
                $"스폰 간격 {interval}이 최솟값 {stage.spawnIntervalMin}보다 작다.");
            Assert.LessOrEqual(interval, stage.spawnIntervalMax,
                $"스폰 간격 {interval}이 최댓값 {stage.spawnIntervalMax}보다 크다.");
        }
    }

    [Test]
    public void 성공_후반단계_스폰간격이_초반보다_짧음()
    {
        // Arrange & Act
        float earlyMax = _difficultyData.GetStage(0f).spawnIntervalMax;
        float lateMax = _difficultyData.GetStage(120f).spawnIntervalMax;

        // Assert
        Assert.Less(lateMax, earlyMax,
            "120초 단계의 최대 스폰 간격은 0초 단계보다 짧아야 한다.");
    }

    // ---- GetMaxObstaclesPerChunk ----

    [Test]
    public void 성공_최대장애물수가_난이도따라_증가()
    {
        // Arrange & Act
        int count0 = _difficultyData.GetMaxObstaclesPerChunk(0f);
        int count120 = _difficultyData.GetMaxObstaclesPerChunk(120f);

        // Assert
        Assert.Less(count0, count120,
            "120초 단계의 청크당 최대 장애물 수는 0초보다 많아야 한다.");
    }

    // ---- 경계값 테스트 ----

    [Test]
    public void 성공_음수경과시간_첫번째단계_반환()
    {
        // Arrange
        float elapsed = -1f;

        // Act
        DifficultyStage stage = _difficultyData.GetStage(elapsed);

        // Assert
        Assert.AreEqual(0f, stage.startTime,
            "음수 경과 시간에는 첫 번째 단계를 반환해야 한다.");
    }

    [Test]
    public void 성공_단계경계값_정확히_다음단계_반환()
    {
        // 30초 정각에 30초 단계가 시작되어야 한다
        DifficultyStage stageBefore = _difficultyData.GetStage(29.9f);
        DifficultyStage stageAt = _difficultyData.GetStage(30f);

        Assert.AreNotEqual(stageBefore.startTime, stageAt.startTime,
            "29.9초와 30.0초는 서로 다른 단계여야 한다.");
        Assert.AreEqual(30f, stageAt.startTime);
    }
}
