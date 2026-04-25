using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class ParallaxBackgroundTest
{
    // -----------------------------------------------------------------------
    // Helper: private 필드 설정
    // -----------------------------------------------------------------------
    private static void SetField(object target, string name, object value)
    {
        target.GetType()
            .GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(target, value);
    }

    // -----------------------------------------------------------------------
    // ShouldRecycle 직접 호출 헬퍼
    // -----------------------------------------------------------------------
    private static bool InvokeShouldRecycle(ParallaxLayer layer, Transform tile)
    {
        return (bool)typeof(ParallaxLayer)
            .GetMethod("ShouldRecycle", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(layer, new object[] { tile });
    }

    // -----------------------------------------------------------------------
    // 테스트 1: 속도 배율이 SpeedMultiplier 프로퍼티에 올바르게 반영되는지 확인
    // -----------------------------------------------------------------------
    [Test]
    public void 성공_파랄랙스레이어_속도배율_프로퍼티_반환()
    {
        // Arrange
        GameObject go = new GameObject("Layer");
        ParallaxLayer layer = go.AddComponent<ParallaxLayer>();
        SetField(layer, "speedMultiplier", 0.3f);

        // Act
        float result = layer.SpeedMultiplier;

        // Assert
        Assert.AreEqual(0.3f, result, 0.0001f);

        Object.DestroyImmediate(go);
    }

    // -----------------------------------------------------------------------
    // 테스트 2: isPlaying=false이면 타일 위치가 변하지 않음
    // -----------------------------------------------------------------------
    [Test]
    public void 성공_게임오버시_스크롤정지()
    {
        // Arrange
        GameObject layerGo = new GameObject("Layer");
        ParallaxLayer layer = layerGo.AddComponent<ParallaxLayer>();
        SetField(layer, "speedMultiplier", 0.5f);
        SetField(layer, "tileWidth", 20f);

        // 타일 2개 자식 생성
        GameObject tile0 = new GameObject("Tile0");
        GameObject tile1 = new GameObject("Tile1");
        tile0.transform.parent = layerGo.transform;
        tile1.transform.parent = layerGo.transform;
        tile0.transform.position = new Vector3(0f, 0f, 0f);
        tile1.transform.position = new Vector3(20f, 0f, 0f);

        // 카메라 없이 Init (null camera)
        layer.Init(null);

        Vector3 pos0Before = tile0.transform.position;
        Vector3 pos1Before = tile1.transform.position;

        // Act — isPlaying = false
        layer.Tick(8f, false);

        // Assert — 위치 변화 없음
        Assert.AreEqual(pos0Before, tile0.transform.position);
        Assert.AreEqual(pos1Before, tile1.transform.position);

        Object.DestroyImmediate(layerGo);
    }

    // -----------------------------------------------------------------------
    // 테스트 3: 타일이 화면 왼쪽 밖으로 나가면 ShouldRecycle = true
    // -----------------------------------------------------------------------
    [Test]
    public void 성공_타일_화면밖_재배치_판정()
    {
        // Arrange
        GameObject cameraGo = new GameObject("Camera");
        Camera cam = cameraGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.aspect = 1.6f;
        cameraGo.transform.position = new Vector3(0f, 0f, -10f);

        GameObject layerGo = new GameObject("Layer");
        ParallaxLayer layer = layerGo.AddComponent<ParallaxLayer>();
        SetField(layer, "tileWidth", 20f);

        GameObject tile0 = new GameObject("Tile0");
        GameObject tile1 = new GameObject("Tile1");
        tile0.transform.parent = layerGo.transform;
        tile1.transform.parent = layerGo.transform;

        layer.Init(cam);

        // 카메라 왼쪽 경계 = -8f (5 * 1.6 = 8)
        // tileWidth/2 = 10f
        // ShouldRecycle: tile.x + 10 <= -8 → tile.x <= -18

        // 재배치 불필요 (-10f + 10 = 0 > -8)
        tile0.transform.position = new Vector3(-10f, 0f, 0f);
        Assert.IsFalse(InvokeShouldRecycle(layer, tile0.transform));

        // 재배치 필요 (-18.1f + 10 = -8.1 <= -8)
        tile0.transform.position = new Vector3(-18.1f, 0f, 0f);
        Assert.IsTrue(InvokeShouldRecycle(layer, tile0.transform));

        Object.DestroyImmediate(layerGo);
        Object.DestroyImmediate(cameraGo);
    }

    // -----------------------------------------------------------------------
    // 테스트 4: Near 레이어가 Far 레이어보다 SpeedMultiplier가 큼을 검증
    // -----------------------------------------------------------------------
    [Test]
    public void 성공_Near레이어_Far레이어보다_속도배율_큼()
    {
        // Arrange
        GameObject farGo = new GameObject("Far");
        GameObject nearGo = new GameObject("Near");
        ParallaxLayer farLayer = farGo.AddComponent<ParallaxLayer>();
        ParallaxLayer nearLayer = nearGo.AddComponent<ParallaxLayer>();

        SetField(farLayer, "speedMultiplier", 0.3f);
        SetField(nearLayer, "speedMultiplier", 0.6f);

        // Assert
        Assert.Less(farLayer.SpeedMultiplier, nearLayer.SpeedMultiplier);

        Object.DestroyImmediate(farGo);
        Object.DestroyImmediate(nearGo);
    }
}
