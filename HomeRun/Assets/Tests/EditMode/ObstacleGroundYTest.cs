using NUnit.Framework;
using UnityEngine;
using System.Reflection;

/// <summary>
/// bugfix-obstacle-y-on-slope: Ground 장애물 Y 위치가 경사 높이를 반영하는지 검증.
/// TerrainTile.GetGroundYAtLocalX()를 직접 단위 테스트하고,
/// Obstacle._groundYOffset 초기화 로직을 검증한다.
/// </summary>
[TestFixture]
public class ObstacleGroundYTest
{
    private const float TileWidth = 16f;
    private const float TileHeight = 2f;
    private const float SlopeHeightDelta = 0.8f;

    // TerrainTile 직접 테스트 (GroundScroller 없이)

    [Test]
    public void 성공_Flat타일_GetGroundYAtLocalX_중앙에서_0반환()
    {
        // Arrange
        var go = new GameObject("Tile");
        var tile = go.AddComponent<TerrainTile>();
        SetField(tile, "tileWidth", TileWidth);
        SetField(tile, "tileHeight", TileHeight);
        SetField(tile, "slopeHeightDelta", SlopeHeightDelta);
        tile.SetType(TerrainChunkType.Flat, 0f);

        // Act
        float y = tile.GetGroundYAtLocalX(0f);

        // Assert: Flat (leftTopYOffset=0, rightTopYOffset=0) → linearY=0
        Assert.AreEqual(0f, y, 0.001f,
            "Flat 타일 중앙(localX=0)에서 GetGroundYAtLocalX는 0이어야 한다.");

        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_SlopeUp타일_GetGroundYAtLocalX_오른쪽이_더_높음()
    {
        // Arrange
        var go = new GameObject("Tile");
        var tile = go.AddComponent<TerrainTile>();
        SetField(tile, "tileWidth", TileWidth);
        SetField(tile, "tileHeight", TileHeight);
        SetField(tile, "slopeHeightDelta", SlopeHeightDelta);
        tile.SetType(TerrainChunkType.SlopeUp, 0f);

        // Act
        float leftY = tile.GetGroundYAtLocalX(-6f);   // 왼쪽
        float rightY = tile.GetGroundYAtLocalX(6f);    // 오른쪽

        // Assert: SlopeUp → 오른쪽 offset이 더 크므로 rightY > leftY
        Assert.Greater(rightY, leftY,
            "SlopeUp 타일에서 오른쪽 GetGroundYAtLocalX가 왼쪽보다 높아야 한다.");

        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_SlopeUp타일_GetGroundYAtLocalX_오른쪽끝이_slopeHeightDelta()
    {
        // Arrange: leftTopYOffset=0, rightTopYOffset=SlopeHeightDelta
        var go = new GameObject("Tile");
        var tile = go.AddComponent<TerrainTile>();
        SetField(tile, "tileWidth", TileWidth);
        SetField(tile, "tileHeight", TileHeight);
        SetField(tile, "slopeHeightDelta", SlopeHeightDelta);
        tile.SetType(TerrainChunkType.SlopeUp, 0f);

        float halfWidth = TileWidth * 0.5f; // =8

        // Act: 오른쪽 끝 localX=halfWidth → t=1 → linearY=rightTopYOffset=SlopeHeightDelta
        float rightEndY = tile.GetGroundYAtLocalX(halfWidth);

        Assert.AreEqual(SlopeHeightDelta, rightEndY, 0.001f,
            "SlopeUp 타일 오른쪽 끝(localX=halfWidth)에서 GetGroundYAtLocalX는 slopeHeightDelta와 같아야 한다.");

        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_SlopeDown타일_GetGroundYAtLocalX_왼쪽이_더_높음()
    {
        // Arrange: leftTopYOffset=0.8 (이미 높은 곳에서 내려옴)
        var go = new GameObject("Tile");
        var tile = go.AddComponent<TerrainTile>();
        SetField(tile, "tileWidth", TileWidth);
        SetField(tile, "tileHeight", TileHeight);
        SetField(tile, "slopeHeightDelta", SlopeHeightDelta);
        tile.SetType(TerrainChunkType.SlopeDown, SlopeHeightDelta);

        // Act
        float leftY = tile.GetGroundYAtLocalX(-6f);
        float rightY = tile.GetGroundYAtLocalX(6f);

        // Assert: SlopeDown → 왼쪽이 더 높음
        Assert.Greater(leftY, rightY,
            "SlopeDown 타일에서 왼쪽 GetGroundYAtLocalX가 오른쪽보다 높아야 한다.");

        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_Obstacle_groundYOffset_초기화_로직_검증()
    {
        // Arrange: Obstacle과 _groundScroller를 reflection으로 주입
        var obstacleGo = new GameObject("Obstacle");
        obstacleGo.AddComponent<BoxCollider2D>();
        var obstacle = obstacleGo.AddComponent<Obstacle>();

        // _groundScroller를 null로 두면 Initialize()에서 groundYOffset 계산 생략됨
        // (FindFirstObjectByType이 null 반환 → if문 skip)
        // obstacleType은 기본 Ground이므로 _groundScroller가 있어야 동작함.
        // 여기서는 _groundScroller=null 환경에서 _groundYOffset이 0(기본값) 유지되는지 검증.

        obstacleGo.transform.position = new Vector3(5f, 1.5f, 0f);
        obstacle.Initialize(8f, null);

        float groundYOffset = (float)typeof(Obstacle)
            .GetField("_groundYOffset", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(obstacle);

        // _groundScroller=null → groundYOffset이 변경되지 않아 기본값 0 유지
        Assert.AreEqual(0f, groundYOffset, 0.001f,
            "_groundScroller가 null이면 _groundYOffset은 0(기본값)이어야 한다.");

        Object.DestroyImmediate(obstacleGo);
    }

    [Test]
    public void 성공_Obstacle_groundYOffset_필드_존재()
    {
        // _groundYOffset 필드가 Obstacle에 추가되었는지 확인 (버그 수정 검증)
        var field = typeof(Obstacle).GetField(
            "_groundYOffset",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        Assert.IsNotNull(field,
            "Obstacle 클래스에 _groundYOffset 필드가 존재해야 한다 (경사 추적 버그 수정).");
    }

    private static void SetField(object target, string name, object value)
    {
        var field = target.GetType().GetField(
            name,
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public
        );
        if (field != null)
            field.SetValue(target, value);
    }
}
