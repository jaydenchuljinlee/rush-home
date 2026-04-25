using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class TerrainVariationTest
{
    [Test]
    public void 성공_Easy티어_항상Flat반환()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTypeSequencer");
        TerrainTypeSequencer sequencer = go.AddComponent<TerrainTypeSequencer>();
        sequencer.SetDifficultyTier(DifficultyTier.Easy);

        // Act / Assert
        for (int i = 0; i < 20; i++)
        {
            Assert.AreEqual(TerrainChunkType.Flat, sequencer.GetNextType());
        }

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_Normal티어_경사타입포함()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTypeSequencer");
        TerrainTypeSequencer sequencer = go.AddComponent<TerrainTypeSequencer>();
        sequencer.SetDifficultyTier(DifficultyTier.Normal);

        bool sawSlope = false;

        // Act
        for (int i = 0; i < 100; i++)
        {
            TerrainChunkType type = sequencer.GetNextType();
            Assert.AreNotEqual(TerrainChunkType.Gap, type);

            if (type == TerrainChunkType.SlopeUp || type == TerrainChunkType.SlopeDown
                || type == TerrainChunkType.CurveUp || type == TerrainChunkType.CurveDown)
            {
                sawSlope = true;
            }
        }

        // Assert
        Assert.IsTrue(sawSlope, "Normal 티어에서는 경사 또는 곡선 타입이 등장해야 한다.");

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_TerrainTile_Gap타입_SetType_비가시()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTile");
        TerrainTile terrainTile = go.AddComponent<TerrainTile>();

        // Act
        terrainTile.SetType(TerrainChunkType.Gap);

        // Assert
        Assert.IsFalse(go.GetComponent<MeshRenderer>().enabled);
        Assert.IsFalse(go.GetComponent<PolygonCollider2D>().enabled);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_TerrainTile_Flat타입_SetType_가시()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTile");
        TerrainTile terrainTile = go.AddComponent<TerrainTile>();
        terrainTile.SetType(TerrainChunkType.Gap);

        // Act
        terrainTile.SetType(TerrainChunkType.Flat);

        // Assert
        Assert.IsTrue(go.GetComponent<MeshRenderer>().enabled);
        Assert.IsTrue(go.GetComponent<PolygonCollider2D>().enabled);
        Assert.AreEqual(0f, terrainTile.GroundYOffset);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_TerrainTile_SlopeUp_GroundYOffset_양수()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTile");
        TerrainTile terrainTile = go.AddComponent<TerrainTile>();

        // Act
        terrainTile.SetType(TerrainChunkType.SlopeUp);

        // Assert
        Assert.Greater(terrainTile.GroundYOffset, 0f);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_TerrainTile_인접타일_좌우상단높이_연속()
    {
        // Arrange
        GameObject leftGo = new GameObject("LeftTerrainTile");
        TerrainTile leftTile = leftGo.AddComponent<TerrainTile>();

        GameObject rightGo = new GameObject("RightTerrainTile");
        TerrainTile rightTile = rightGo.AddComponent<TerrainTile>();

        // Act
        leftTile.SetType(TerrainChunkType.SlopeUp, 0f);
        rightTile.SetType(TerrainChunkType.Flat, leftTile.RightTopYOffset);

        // Assert
        Assert.AreEqual(leftTile.RightTopYOffset, rightTile.LeftTopYOffset, 0.001f);
        Assert.AreEqual(rightTile.LeftTopYOffset, rightTile.RightTopYOffset, 0.001f);

        // Cleanup
        Object.DestroyImmediate(leftGo);
        Object.DestroyImmediate(rightGo);
    }

    [Test]
    public void 성공_TerrainTile_SlopeUp_메시와콜라이더_같은상단면()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTile");
        TerrainTile terrainTile = go.AddComponent<TerrainTile>();

        // Act
        terrainTile.SetType(TerrainChunkType.SlopeUp, 0f);
        Vector3[] vertices = go.GetComponent<MeshFilter>().sharedMesh.vertices;
        Vector2[] colliderPath = go.GetComponent<PolygonCollider2D>().GetPath(0);

        // Assert
        Assert.AreEqual(vertices[1].y, colliderPath[1].y, 0.001f);
        Assert.AreEqual(vertices[2].y, colliderPath[2].y, 0.001f);
        Assert.Greater(vertices[2].y, vertices[1].y);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_SlopeUp_다음_SlopeDown_금지()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTypeSequencer");
        TerrainTypeSequencer sequencer = go.AddComponent<TerrainTypeSequencer>();
        sequencer.SetDifficultyTier(DifficultyTier.Normal);

        // Act / Assert
        for (int i = 0; i < 100; i++)
        {
            sequencer.SetLastType(TerrainChunkType.SlopeUp);
            Assert.AreNotEqual(TerrainChunkType.SlopeDown, sequencer.GetNextType());
        }

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_Gap_다음_반드시Flat()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTypeSequencer");
        TerrainTypeSequencer sequencer = go.AddComponent<TerrainTypeSequencer>();
        sequencer.SetDifficultyTier(DifficultyTier.Extreme);

        // Act / Assert
        for (int i = 0; i < 100; i++)
        {
            sequencer.SetLastType(TerrainChunkType.Gap);
            Assert.AreEqual(TerrainChunkType.Flat, sequencer.GetNextType());
        }

        // Cleanup
        Object.DestroyImmediate(go);
    }

    // ---- 곡선 타일 신규 테스트 ----

    [Test]
    public void 성공_CurveUp_타입_메시상단꼭짓점수_정상()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTile");
        TerrainTile terrainTile = go.AddComponent<TerrainTile>();

        // Act
        terrainTile.SetType(TerrainChunkType.CurveUp, 0f);
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;

        // Assert: 2 * (curveSegments + 1) 개 꼭짓점
        int expectedVertCount = 2 * (terrainTile.CurveSegments + 1);
        Assert.AreEqual(expectedVertCount, mesh.vertexCount);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_CurveUp_좌우끝꼭짓점_SlopeUp과_동일높이()
    {
        // Arrange
        GameObject goSlope = new GameObject("SlopeTile");
        TerrainTile slopeTile = goSlope.AddComponent<TerrainTile>();

        GameObject goCurve = new GameObject("CurveTile");
        TerrainTile curveTile = goCurve.AddComponent<TerrainTile>();

        // Act
        slopeTile.SetType(TerrainChunkType.SlopeUp, 0f);
        curveTile.SetType(TerrainChunkType.CurveUp, 0f);

        // Assert: 좌측(LeftTopYOffset)과 우측(RightTopYOffset) 모두 동일
        Assert.AreEqual(slopeTile.LeftTopYOffset, curveTile.LeftTopYOffset, 0.001f);
        Assert.AreEqual(slopeTile.RightTopYOffset, curveTile.RightTopYOffset, 0.001f);

        // Cleanup
        Object.DestroyImmediate(goSlope);
        Object.DestroyImmediate(goCurve);
    }

    [Test]
    public void 성공_CurveDown_좌우끝꼭짓점_SlopeDown과_동일높이()
    {
        // Arrange
        GameObject goSlope = new GameObject("SlopeTile");
        TerrainTile slopeTile = goSlope.AddComponent<TerrainTile>();

        GameObject goCurve = new GameObject("CurveTile");
        TerrainTile curveTile = goCurve.AddComponent<TerrainTile>();

        // Act
        slopeTile.SetType(TerrainChunkType.SlopeDown, 0f);
        curveTile.SetType(TerrainChunkType.CurveDown, 0f);

        // Assert
        Assert.AreEqual(slopeTile.LeftTopYOffset, curveTile.LeftTopYOffset, 0.001f);
        Assert.AreEqual(slopeTile.RightTopYOffset, curveTile.RightTopYOffset, 0.001f);

        // Cleanup
        Object.DestroyImmediate(goSlope);
        Object.DestroyImmediate(goCurve);
    }

    [Test]
    public void 성공_접합부_Flat_다음_CurveUp_상단면_연속()
    {
        // Arrange
        GameObject leftGo = new GameObject("LeftTile");
        TerrainTile leftTile = leftGo.AddComponent<TerrainTile>();

        GameObject rightGo = new GameObject("RightTile");
        TerrainTile rightTile = rightGo.AddComponent<TerrainTile>();

        // Act
        leftTile.SetType(TerrainChunkType.Flat, 0f);
        rightTile.SetType(TerrainChunkType.CurveUp, leftTile.RightTopYOffset);

        // Assert: 접합부 상단면 연속 (이전 오른쪽 == 다음 왼쪽)
        Assert.AreEqual(leftTile.RightTopYOffset, rightTile.LeftTopYOffset, 0.001f);

        // Cleanup
        Object.DestroyImmediate(leftGo);
        Object.DestroyImmediate(rightGo);
    }

    [Test]
    public void 성공_접합부_SlopeUp_다음_Flat_상단면_연속()
    {
        // Arrange
        GameObject leftGo = new GameObject("LeftTile");
        TerrainTile leftTile = leftGo.AddComponent<TerrainTile>();

        GameObject rightGo = new GameObject("RightTile");
        TerrainTile rightTile = rightGo.AddComponent<TerrainTile>();

        // Act
        leftTile.SetType(TerrainChunkType.SlopeUp, 0f);
        rightTile.SetType(TerrainChunkType.Flat, leftTile.RightTopYOffset);

        // Assert
        Assert.AreEqual(leftTile.RightTopYOffset, rightTile.LeftTopYOffset, 0.001f);
        Assert.AreEqual(rightTile.LeftTopYOffset, rightTile.RightTopYOffset, 0.001f);

        // Cleanup
        Object.DestroyImmediate(leftGo);
        Object.DestroyImmediate(rightGo);
    }

    [Test]
    public void 성공_접합부_SlopeDown_다음_Flat_상단면_연속()
    {
        // Arrange
        GameObject leftGo = new GameObject("LeftTile");
        TerrainTile leftTile = leftGo.AddComponent<TerrainTile>();

        GameObject rightGo = new GameObject("RightTile");
        TerrainTile rightTile = rightGo.AddComponent<TerrainTile>();

        // Act
        leftTile.SetType(TerrainChunkType.SlopeDown, 0f);
        rightTile.SetType(TerrainChunkType.Flat, leftTile.RightTopYOffset);

        // Assert
        Assert.AreEqual(leftTile.RightTopYOffset, rightTile.LeftTopYOffset, 0.001f);
        Assert.AreEqual(rightTile.LeftTopYOffset, rightTile.RightTopYOffset, 0.001f);

        // Cleanup
        Object.DestroyImmediate(leftGo);
        Object.DestroyImmediate(rightGo);
    }

    [Test]
    public void 성공_접합부_CurveUp_다음_Flat_상단면_연속()
    {
        // Arrange
        GameObject leftGo = new GameObject("LeftTile");
        TerrainTile leftTile = leftGo.AddComponent<TerrainTile>();

        GameObject rightGo = new GameObject("RightTile");
        TerrainTile rightTile = rightGo.AddComponent<TerrainTile>();

        // Act
        leftTile.SetType(TerrainChunkType.CurveUp, 0f);
        rightTile.SetType(TerrainChunkType.Flat, leftTile.RightTopYOffset);

        // Assert
        Assert.AreEqual(leftTile.RightTopYOffset, rightTile.LeftTopYOffset, 0.001f);
        Assert.AreEqual(rightTile.LeftTopYOffset, rightTile.RightTopYOffset, 0.001f);

        // Cleanup
        Object.DestroyImmediate(leftGo);
        Object.DestroyImmediate(rightGo);
    }

    [Test]
    public void 성공_Normal티어_Curve타입포함()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTypeSequencer");
        TerrainTypeSequencer sequencer = go.AddComponent<TerrainTypeSequencer>();
        sequencer.SetDifficultyTier(DifficultyTier.Normal);

        bool sawCurve = false;

        // Act: 충분한 횟수 반복하여 곡선 타입 등장 확인
        for (int i = 0; i < 200; i++)
        {
            TerrainChunkType type = sequencer.GetNextType();
            if (type == TerrainChunkType.CurveUp || type == TerrainChunkType.CurveDown)
            {
                sawCurve = true;
                break;
            }
        }

        // Assert
        Assert.IsTrue(sawCurve, "Normal 티어에서는 200회 내에 곡선 타입이 등장해야 한다.");

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_CurveUp_다음_CurveDown_금지()
    {
        // Arrange
        GameObject go = new GameObject("TerrainTypeSequencer");
        TerrainTypeSequencer sequencer = go.AddComponent<TerrainTypeSequencer>();
        sequencer.SetDifficultyTier(DifficultyTier.Normal);

        // Act / Assert: CurveUp 직후 CurveDown이 반환되지 않아야 함
        for (int i = 0; i < 100; i++)
        {
            sequencer.SetLastType(TerrainChunkType.CurveUp);
            TerrainChunkType next = sequencer.GetNextType();
            Assert.AreNotEqual(TerrainChunkType.CurveDown, next,
                "CurveUp 직후 CurveDown이 반환되면 안 된다.");
            Assert.AreNotEqual(TerrainChunkType.SlopeDown, next,
                "CurveUp 직후 SlopeDown이 반환되면 안 된다.");
        }

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_ClampTerrain_CurveUp_최대범위초과시_Flat()
    {
        // Arrange
        GameObject scrollerGo = new GameObject("GroundScroller");
        GroundScroller scroller = scrollerGo.AddComponent<GroundScroller>();
        SetPrivateField(scroller, "minTerrainYOffset", -1.6f);
        SetPrivateField(scroller, "maxTerrainYOffset", 1.6f);

        // Act: leftTopYOffset = 1.6, CurveUp이면 rightTop = 1.6 + slopeHeightDelta(0.8) = 2.4 -> 클램프
        TerrainChunkType clampedType = InvokeClampTerrainType(
            scroller,
            TerrainChunkType.CurveUp,
            1.6f,
            0.8f
        );
        // leftTopYOffset = 0.0, CurveDown이면 rightTop = 0.0 - 0.8 = -0.8 -> 범위 내
        TerrainChunkType allowedType = InvokeClampTerrainType(
            scroller,
            TerrainChunkType.CurveDown,
            0.0f,
            0.8f
        );

        // Assert
        Assert.AreEqual(TerrainChunkType.Flat, clampedType,
            "범위 초과 CurveUp은 Flat으로 보정되어야 한다.");
        Assert.AreEqual(TerrainChunkType.CurveDown, allowedType,
            "범위 내 CurveDown은 그대로 반환되어야 한다.");

        // Cleanup
        Object.DestroyImmediate(scrollerGo);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        target.GetType()
            .GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(target, value);
    }

    private static TerrainChunkType InvokeClampTerrainType(
        GroundScroller scroller,
        TerrainChunkType type,
        float leftTopYOffset,
        float slopeHeightDelta
    )
    {
        return (TerrainChunkType)typeof(GroundScroller)
            .GetMethod("ClampTerrainTypeToHeightRange", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(scroller, new object[] { type, leftTopYOffset, slopeHeightDelta });
    }
}
