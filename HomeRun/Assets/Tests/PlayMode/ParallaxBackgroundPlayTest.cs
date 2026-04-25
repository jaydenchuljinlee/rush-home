using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// 파랄랙스 배경 Play Mode 테스트.
/// Playing 상태에서 레이어 이동 및 레이어 간 속도 차이를 검증.
/// </summary>
public class ParallaxBackgroundPlayTest
{
    private GameObject _gameManagerGo;

    [SetUp]
    public void SetUp()
    {
        // GameManager 생성 및 Playing 상태로 전환
        _gameManagerGo = new GameObject("GameManager");
        GameManager gm = _gameManagerGo.AddComponent<GameManager>();
        gm.StartGame();
    }

    [TearDown]
    public void TearDown()
    {
        if (_gameManagerGo != null) Object.Destroy(_gameManagerGo);
    }

    // -----------------------------------------------------------------------
    // 테스트 1: Playing 상태에서 레이어 타일이 왼쪽으로 이동
    // 2프레임 대기 후 Tick 호출 — Play Mode에서 Time.deltaTime이 유효한 값을 가짐
    // -----------------------------------------------------------------------
    [UnityTest]
    public IEnumerator 성공_플레이상태에서_레이어가_이동함()
    {
        // Arrange
        GameObject layerGo = new GameObject("Layer");
        ParallaxLayer layer = layerGo.AddComponent<ParallaxLayer>();
        SetField(layer, "speedMultiplier", 0.3f);
        SetField(layer, "tileWidth", 20f);

        GameObject tile0 = new GameObject("Tile0");
        GameObject tile1 = new GameObject("Tile1");
        tile0.transform.parent = layerGo.transform;
        tile1.transform.parent = layerGo.transform;
        tile0.transform.position = new Vector3(0f, 0f, 0f);
        tile1.transform.position = new Vector3(20f, 0f, 0f);

        layer.Init(null);

        // Act — 2프레임 대기로 Time.deltaTime이 0이 아닌 값을 가지게 한 후 Tick 호출
        yield return null;
        yield return null;

        float initialX = tile0.transform.position.x;
        layer.Tick(8f, true);

        // Assert — X 위치가 감소함 (Time.deltaTime > 0이므로 이동량 > 0)
        Assert.Less(tile0.transform.position.x, initialX,
            $"tile0.x({tile0.transform.position.x}) < initialX({initialX}) 이어야 합니다.");

        Object.Destroy(layerGo);
    }

    // -----------------------------------------------------------------------
    // 테스트 2: Near 레이어가 Far 레이어보다 더 빠르게 이동
    // -----------------------------------------------------------------------
    [UnityTest]
    public IEnumerator 성공_레이어별_속도차이_확인()
    {
        // Arrange
        GameObject farGo = new GameObject("FarLayer");
        GameObject nearGo = new GameObject("NearLayer");

        ParallaxLayer farLayer = farGo.AddComponent<ParallaxLayer>();
        ParallaxLayer nearLayer = nearGo.AddComponent<ParallaxLayer>();

        SetField(farLayer, "speedMultiplier", 0.3f);
        SetField(nearLayer, "speedMultiplier", 0.6f);
        SetField(farLayer, "tileWidth", 20f);
        SetField(nearLayer, "tileWidth", 20f);

        // 각 레이어에 타일 2개 추가
        CreateTiles(farGo, 20f);
        CreateTiles(nearGo, 20f);

        farLayer.Init(null);
        nearLayer.Init(null);

        float farInitialX = farGo.transform.GetChild(0).position.x;
        float nearInitialX = nearGo.transform.GetChild(0).position.x;

        // Act — 1프레임 대기 후 동일 groundScrollSpeed로 Tick 1회
        yield return null;
        float groundSpeed = 8f;
        farLayer.Tick(groundSpeed, true);
        nearLayer.Tick(groundSpeed, true);

        // Assert — Near가 Far보다 더 많이 이동 (더 작은 X 값)
        float farDelta = farInitialX - farGo.transform.GetChild(0).position.x;
        float nearDelta = nearInitialX - nearGo.transform.GetChild(0).position.x;

        Assert.Greater(nearDelta, farDelta,
            $"Near 이동량({nearDelta:F4}) > Far 이동량({farDelta:F4}) 이어야 합니다.");

        Object.Destroy(farGo);
        Object.Destroy(nearGo);
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------
    private static void CreateTiles(GameObject parent, float spacing)
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject tile = new GameObject($"Tile{i}");
            tile.transform.parent = parent.transform;
            tile.transform.position = new Vector3(i * spacing, 0f, 0f);
        }
    }

    private static void SetField(object target, string name, object value)
    {
        target.GetType()
            .GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(target, value);
    }
}
