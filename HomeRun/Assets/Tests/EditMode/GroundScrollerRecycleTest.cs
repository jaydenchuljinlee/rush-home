using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class GroundScrollerRecycleTest
{
    [Test]
    public void 성공_타일이_카메라왼쪽밖으로_완전히나가야_재활용()
    {
        // Arrange
        GameObject cameraGo = new GameObject("Main Camera");
        Camera camera = cameraGo.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.aspect = 1.6f;
        cameraGo.transform.position = new Vector3(0f, 1f, -10f);

        GameObject scrollerGo = new GameObject("GroundScroller");
        GroundScroller scroller = scrollerGo.AddComponent<GroundScroller>();
        SetPrivateField(scroller, "viewCamera", camera);
        SetPrivateField(scroller, "tileWidth", 16f);
        SetPrivateField(scroller, "recycleScreenMargin", 1f);

        GameObject tileGo = new GameObject("GroundTile");
        tileGo.transform.parent = scrollerGo.transform;

        // Act / Assert
        tileGo.transform.position = new Vector3(-8f, 0f, 0f);
        Assert.IsFalse(InvokeShouldRecycle(scroller, tileGo.transform));

        tileGo.transform.position = new Vector3(-17.1f, 0f, 0f);
        Assert.IsTrue(InvokeShouldRecycle(scroller, tileGo.transform));

        // Cleanup
        Object.DestroyImmediate(scrollerGo);
        Object.DestroyImmediate(cameraGo);
    }

    [Test]
    public void 성공_경사누적높이가_범위를넘으면_Flat으로보정()
    {
        // Arrange
        GameObject scrollerGo = new GameObject("GroundScroller");
        GroundScroller scroller = scrollerGo.AddComponent<GroundScroller>();
        SetPrivateField(scroller, "minTerrainYOffset", -1.6f);
        SetPrivateField(scroller, "maxTerrainYOffset", 1.6f);

        // Act
        TerrainChunkType clampedType = InvokeClampTerrainType(
            scroller,
            TerrainChunkType.SlopeUp,
            1.6f,
            0.8f
        );
        TerrainChunkType allowedType = InvokeClampTerrainType(
            scroller,
            TerrainChunkType.SlopeDown,
            1.6f,
            0.8f
        );

        // Assert
        Assert.AreEqual(TerrainChunkType.Flat, clampedType);
        Assert.AreEqual(TerrainChunkType.SlopeDown, allowedType);

        // Cleanup
        Object.DestroyImmediate(scrollerGo);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        target.GetType()
            .GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(target, value);
    }

    private static bool InvokeShouldRecycle(GroundScroller scroller, Transform tile)
    {
        return (bool)typeof(GroundScroller)
            .GetMethod("ShouldRecycle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(scroller, new object[] { tile });
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
