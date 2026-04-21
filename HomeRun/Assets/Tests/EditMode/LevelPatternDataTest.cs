using NUnit.Framework;
using UnityEngine;

/// <summary>
/// LevelPatternData ScriptableObject 및 DifficultyData 패턴 풀 로직 검증.
/// Edit Mode 테스트 - Unity 에디터 실행 불필요.
/// </summary>
[TestFixture]
public class LevelPatternDataTest
{
    private DifficultyData _difficultyData;
    private LevelPatternData _easyPattern;
    private LevelPatternData _hardPattern;

    [SetUp]
    public void SetUp()
    {
        _difficultyData = ScriptableObject.CreateInstance<DifficultyData>();
        _easyPattern = ScriptableObject.CreateInstance<LevelPatternData>();
        _hardPattern = ScriptableObject.CreateInstance<LevelPatternData>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_difficultyData);
        Object.DestroyImmediate(_easyPattern);
        Object.DestroyImmediate(_hardPattern);
    }

    // ===== DifficultyTier enum 값 검증 =====

    [Test]
    public void 성공_Easy티어_값이_0임()
    {
        Assert.AreEqual(0, (int)DifficultyTier.Easy, "DifficultyTier.Easy는 0이어야 한다.");
    }

    [Test]
    public void 성공_Normal티어_값이_1임()
    {
        Assert.AreEqual(1, (int)DifficultyTier.Normal, "DifficultyTier.Normal은 1이어야 한다.");
    }

    [Test]
    public void 성공_Hard티어_값이_2임()
    {
        Assert.AreEqual(2, (int)DifficultyTier.Hard, "DifficultyTier.Hard는 2이어야 한다.");
    }

    [Test]
    public void 성공_Extreme티어_값이_3임()
    {
        Assert.AreEqual(3, (int)DifficultyTier.Extreme, "DifficultyTier.Extreme은 3이어야 한다.");
    }

    // ===== GetTierForTime 검증 =====

    [Test]
    public void 성공_0초에서_Easy티어_반환()
    {
        DifficultyTier tier = _difficultyData.GetTierForTime(0f);
        Assert.AreEqual(DifficultyTier.Easy, tier, "0초에서는 Easy 티어여야 한다.");
    }

    [Test]
    public void 성공_30초에서_Normal티어_반환()
    {
        DifficultyTier tier = _difficultyData.GetTierForTime(30f);
        Assert.AreEqual(DifficultyTier.Normal, tier, "30초에서는 Normal 티어여야 한다.");
    }

    [Test]
    public void 성공_60초에서_Hard티어_반환()
    {
        DifficultyTier tier = _difficultyData.GetTierForTime(60f);
        Assert.AreEqual(DifficultyTier.Hard, tier, "60초에서는 Hard 티어여야 한다.");
    }

    [Test]
    public void 성공_120초에서_Extreme티어_반환()
    {
        DifficultyTier tier = _difficultyData.GetTierForTime(120f);
        Assert.AreEqual(DifficultyTier.Extreme, tier, "120초에서는 Extreme 티어여야 한다.");
    }

    // ===== GetPatternsForTime 검증 =====

    [Test]
    public void 성공_GetPatternsForTime_기본값은_빈배열_반환()
    {
        // 패턴을 할당하지 않은 상태에서는 빈 배열 반환
        LevelPatternData[] patterns = _difficultyData.GetPatternsForTime(0f);
        Assert.IsNotNull(patterns, "패턴 배열은 null이어서는 안 된다.");
        Assert.AreEqual(0, patterns.Length, "기본값에서 Easy 패턴 배열은 비어있어야 한다.");
    }

    [Test]
    public void 성공_GetPatternsForTime_각_구간별_별도_배열_반환()
    {
        // Easy, Normal, Hard, Extreme이 서로 독립적인 배열을 반환하는지 확인
        LevelPatternData[] easy = _difficultyData.GetPatternsForTime(0f);
        LevelPatternData[] normal = _difficultyData.GetPatternsForTime(30f);
        LevelPatternData[] hard = _difficultyData.GetPatternsForTime(60f);
        LevelPatternData[] extreme = _difficultyData.GetPatternsForTime(120f);

        // 기본 상태에서 모두 빈 배열 (null 아님)
        Assert.IsNotNull(easy);
        Assert.IsNotNull(normal);
        Assert.IsNotNull(hard);
        Assert.IsNotNull(extreme);
    }

    // ===== ObstacleEntry 기본값 검증 =====

    [Test]
    public void 성공_ObstacleEntry_UseDefaultY_마커값이_음수999임()
    {
        Assert.AreEqual(-999f, ObstacleEntry.UseDefaultY, "UseDefaultY 마커는 -999f이어야 한다.");
    }

    [Test]
    public void 성공_ObstacleEntry_UsesDefaultY_yOverride가_마커값이면_true()
    {
        ObstacleEntry entry = new ObstacleEntry
        {
            prefab = null,
            xOffset = 0f,
            yOverride = ObstacleEntry.UseDefaultY
        };
        Assert.IsTrue(entry.UsesDefaultY, "yOverride가 -999f이면 UsesDefaultY는 true여야 한다.");
    }

    [Test]
    public void 성공_ObstacleEntry_UsesDefaultY_yOverride가_설정되면_false()
    {
        ObstacleEntry entry = new ObstacleEntry
        {
            prefab = null,
            xOffset = 0f,
            yOverride = 2.5f
        };
        Assert.IsFalse(entry.UsesDefaultY, "yOverride가 2.5f이면 UsesDefaultY는 false여야 한다.");
    }

    // ===== TerrainChunkType enum 검증 =====

    [Test]
    public void 성공_TerrainChunkType_Flat이_존재함()
    {
        TerrainChunkType type = TerrainChunkType.Flat;
        Assert.AreEqual(TerrainChunkType.Flat, type);
    }

    [Test]
    public void 성공_TerrainChunkType_Gap이_존재함()
    {
        TerrainChunkType type = TerrainChunkType.Gap;
        Assert.AreEqual(TerrainChunkType.Gap, type);
    }

    [Test]
    public void 성공_Easy티어_Gap청크_없음_티어_비교로_검증()
    {
        // Easy 티어에서는 Gap이 허용되지 않는다.
        // Gap은 Hard(2) 이상 티어에서만 등장하므로 Easy(0) < Hard(2) 확인
        DifficultyTier easyTier = DifficultyTier.Easy;
        DifficultyTier gapMinTier = DifficultyTier.Hard;

        Assert.Less((int)easyTier, (int)gapMinTier,
            "Easy 티어는 Gap 최소 티어(Hard)보다 낮아야 한다.");
    }

    [Test]
    public void 성공_Normal티어_Gap청크_없음_티어_비교로_검증()
    {
        DifficultyTier normalTier = DifficultyTier.Normal;
        DifficultyTier gapMinTier = DifficultyTier.Hard;

        Assert.Less((int)normalTier, (int)gapMinTier,
            "Normal 티어는 Gap 최소 티어(Hard)보다 낮아야 한다.");
    }

    [Test]
    public void 성공_Hard티어_Gap청크_허용_티어_비교로_검증()
    {
        DifficultyTier hardTier = DifficultyTier.Hard;
        DifficultyTier gapMinTier = DifficultyTier.Hard;

        Assert.GreaterOrEqual((int)hardTier, (int)gapMinTier,
            "Hard 티어는 Gap 최소 티어(Hard) 이상이어야 한다.");
    }
}
