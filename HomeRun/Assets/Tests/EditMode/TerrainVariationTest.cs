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

            if (type == TerrainChunkType.SlopeUp || type == TerrainChunkType.SlopeDown)
            {
                sawSlope = true;
            }
        }

        // Assert
        Assert.IsTrue(sawSlope, "Normal 티어에서는 경사 타입이 등장해야 한다.");

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
}
