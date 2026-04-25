using NUnit.Framework;
using UnityEngine;

/// <summary>
/// bugfix-slope-below-ground: 경사가 지면 기준선(YOffset=0) 아래로 내려가지 않는지 검증
/// </summary>
[TestFixture]
public class GroundScrollerSlopeBelowGroundTest
{
    private GameObject _scrollerGo;
    private GroundScroller _scroller;

    [SetUp]
    public void SetUp()
    {
        _scrollerGo = new GameObject("GroundScroller");
        _scroller = _scrollerGo.AddComponent<GroundScroller>();
        // 수정된 기본값: minTerrainYOffset = 0f, maxTerrainYOffset = 1.6f
        SetPrivateField(_scroller, "minTerrainYOffset", 0f);
        SetPrivateField(_scroller, "maxTerrainYOffset", 1.6f);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_scrollerGo);
    }

    [Test]
    public void 성공_leftTopYOffset이_0일때_SlopeDown은_Flat으로_변환()
    {
        // Arrange: leftTopYOffset=0, SlopeDown → rightTopYOffset = 0 - 0.8 = -0.8 < 0 → Flat 변환
        // Act
        TerrainChunkType result = InvokeClampTerrainType(
            _scroller,
            TerrainChunkType.SlopeDown,
            leftTopYOffset: 0f,
            slopeHeightDelta: 0.8f
        );

        // Assert: 지면 기준선 아래로 내려가므로 Flat으로 변환
        Assert.AreEqual(TerrainChunkType.Flat, result,
            "leftTopYOffset=0 에서 SlopeDown은 rightTopYOffset=-0.8이 되어 Flat으로 변환되어야 한다.");
    }

    [Test]
    public void 성공_leftTopYOffset이_0일때_CurveDown은_Flat으로_변환()
    {
        // Arrange: leftTopYOffset=0, CurveDown → rightTopYOffset = 0 - 0.8 = -0.8 < 0 → Flat 변환
        TerrainChunkType result = InvokeClampTerrainType(
            _scroller,
            TerrainChunkType.CurveDown,
            leftTopYOffset: 0f,
            slopeHeightDelta: 0.8f
        );

        Assert.AreEqual(TerrainChunkType.Flat, result,
            "leftTopYOffset=0 에서 CurveDown은 Flat으로 변환되어야 한다.");
    }

    [Test]
    public void 성공_leftTopYOffset이_0이상_slopeDelta이상이면_SlopeDown_허용()
    {
        // Arrange: leftTopYOffset=0.8, SlopeDown → rightTopYOffset = 0.8 - 0.8 = 0.0 >= 0 → 허용
        TerrainChunkType result = InvokeClampTerrainType(
            _scroller,
            TerrainChunkType.SlopeDown,
            leftTopYOffset: 0.8f,
            slopeHeightDelta: 0.8f
        );

        Assert.AreEqual(TerrainChunkType.SlopeDown, result,
            "leftTopYOffset=0.8 에서 SlopeDown은 rightTopYOffset=0.0이 되어 허용되어야 한다.");
    }

    [Test]
    public void 성공_SlopeDown_연속2회시_두번째는_Flat으로_변환()
    {
        // Arrange: 1회차 SlopeDown → leftTopYOffset=0 → rightTopYOffset=-0.8 (Flat으로 강제)
        // 만약 1회차가 허용되려면 leftTopYOffset >= 0.8 이어야 함
        // leftTopYOffset=0.8 → 1회차 SlopeDown → rightTopYOffset=0.0 (허용)
        // leftTopYOffset=0.0 → 2회차 SlopeDown → rightTopYOffset=-0.8 (Flat으로 강제)

        TerrainChunkType first = InvokeClampTerrainType(
            _scroller,
            TerrainChunkType.SlopeDown,
            leftTopYOffset: 0.8f,
            slopeHeightDelta: 0.8f
        );

        // 2회차: 첫 번째 rightTopYOffset(0.0)이 다음 leftTopYOffset이 됨
        TerrainChunkType second = InvokeClampTerrainType(
            _scroller,
            TerrainChunkType.SlopeDown,
            leftTopYOffset: 0.0f,
            slopeHeightDelta: 0.8f
        );

        Assert.AreEqual(TerrainChunkType.SlopeDown, first,
            "1회차 SlopeDown(leftTopYOffset=0.8)은 허용되어야 한다.");
        Assert.AreEqual(TerrainChunkType.Flat, second,
            "2회차 SlopeDown(leftTopYOffset=0.0)은 Flat으로 변환되어야 한다. 지면 아래로 내려가면 안 된다.");
    }

    [Test]
    public void 성공_SlopeUp은_minTerrainYOffset과_무관하게_maxTerrainYOffset_초과시_Flat()
    {
        // leftTopYOffset=1.6, SlopeUp → rightTopYOffset=2.4 > maxTerrainYOffset(1.6) → Flat
        TerrainChunkType result = InvokeClampTerrainType(
            _scroller,
            TerrainChunkType.SlopeUp,
            leftTopYOffset: 1.6f,
            slopeHeightDelta: 0.8f
        );

        Assert.AreEqual(TerrainChunkType.Flat, result,
            "rightTopYOffset이 maxTerrainYOffset(1.6)을 초과하면 Flat으로 변환되어야 한다.");
    }

    [Test]
    public void 성공_Flat은_항상_허용()
    {
        // leftTopYOffset이 어떤 값이어도 Flat은 오프셋 변화 없음
        TerrainChunkType result = InvokeClampTerrainType(
            _scroller,
            TerrainChunkType.Flat,
            leftTopYOffset: 0f,
            slopeHeightDelta: 0.8f
        );

        Assert.AreEqual(TerrainChunkType.Flat, result,
            "Flat 타입은 항상 허용되어야 한다.");
    }

    [Test]
    public void 성공_TerrainTile_SlopeDown_leftTopYOffset이_0일때_rightTopYOffset이_음수()
    {
        // TerrainTile 단위 검증: SetType(SlopeDown, 0f) 시 RightTopYOffset < 0
        // 이 조건이 ClampTerrainTypeToHeightRange에서 걸러져야 함을 간접 확인
        GameObject tileGo = new GameObject("TerrainTile");
        TerrainTile tile = tileGo.AddComponent<TerrainTile>();

        // leftTopYOffset=0으로 SlopeDown 설정 (클램프 없이 직접)
        tile.SetType(TerrainChunkType.SlopeDown, 0f);

        float rightTopYOffset = tile.RightTopYOffset;
        Assert.Less(rightTopYOffset, 0f,
            "SlopeDown(leftTopYOffset=0)의 RightTopYOffset은 음수여야 한다 — 이 케이스가 ClampTerrainTypeToHeightRange에서 Flat으로 변환되어야 한다.");

        Object.DestroyImmediate(tileGo);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        target.GetType()
            .GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(target, value);
    }

    private static TerrainChunkType InvokeClampTerrainType(
        GroundScroller scroller,
        TerrainChunkType type,
        float leftTopYOffset,
        float slopeHeightDelta
    )
    {
        return (TerrainChunkType)typeof(GroundScroller)
            .GetMethod("ClampTerrainTypeToHeightRange",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(scroller, new object[] { type, leftTopYOffset, slopeHeightDelta });
    }
}
