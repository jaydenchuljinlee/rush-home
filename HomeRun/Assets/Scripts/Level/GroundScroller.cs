using UnityEngine;

/// <summary>
/// 지면 타일을 왼쪽으로 스크롤하고, 화면 밖으로 나가면 오른쪽 끝으로 재배치.
/// 2개 이상의 지면 타일을 자식으로 갖는 부모 오브젝트에 부착.
/// </summary>
public class GroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 8f;
    [SerializeField] private float tileWidth = 16f;
    [SerializeField] private Camera viewCamera;
    [SerializeField] private float recycleScreenMargin = 0.5f;
    [SerializeField] private float minTerrainYOffset = 0f;
    [SerializeField] private float maxTerrainYOffset = 1.6f;
    [SerializeField] private TerrainTypeSequencer terrainSequencer;

    private Transform[] _tiles;
    private TerrainTile[] _terrainTiles;
    private float _lastMoveAmount;

    public float ScrollSpeed
    {
        get => scrollSpeed;
        set => scrollSpeed = value;
    }

    /// <summary>
    /// 이번 프레임에 지면이 이동한 거리. Ground 장애물이 동기화에 사용.
    /// </summary>
    public float LastMoveAmount => _lastMoveAmount;

    private void Awake()
    {
        if (viewCamera == null)
        {
            viewCamera = Camera.main;
        }

        _tiles = new Transform[transform.childCount];
        _terrainTiles = new TerrainTile[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            _tiles[i] = transform.GetChild(i);
            _terrainTiles[i] = _tiles[i].GetComponent<TerrainTile>();
        }

        InitializeTerrainTiles();
    }

    private void Update()
    {
        if (!GameManager.IsPlaying)
        {
            _lastMoveAmount = 0f;
            return;
        }

        float moveAmount = scrollSpeed * Time.deltaTime;
        _lastMoveAmount = moveAmount;

        foreach (Transform tile in _tiles)
        {
            tile.position += Vector3.left * moveAmount;
        }

        foreach (Transform tile in _tiles)
        {
            if (ShouldRecycle(tile))
            {
                float rightmostX = GetRightmostX();
                tile.position = new Vector3(rightmostX + tileWidth, tile.position.y, tile.position.z);
                ApplyNextTerrainType(tile);
            }
        }
    }

    private bool ShouldRecycle(Transform tile)
    {
        float halfTileWidth = tileWidth * 0.5f;

        if (viewCamera == null || !viewCamera.orthographic)
        {
            return tile.position.x + halfTileWidth <= -halfTileWidth;
        }

        float screenHalfWidth = viewCamera.orthographicSize * viewCamera.aspect;
        float leftVisibleX = viewCamera.transform.position.x - screenHalfWidth;
        return tile.position.x + halfTileWidth <= leftVisibleX - recycleScreenMargin;
    }

    private float GetRightmostX()
    {
        float max = float.MinValue;
        foreach (Transform tile in _tiles)
        {
            if (tile.position.x > max)
                max = tile.position.x;
        }
        return max;
    }

    public float GetGroundY(float worldX)
    {
        float halfWidth = tileWidth * 0.5f;

        // 1순위: worldX가 타일 X 범위 안에 포함된 타일을 우선 선택
        for (int i = 0; i < _tiles.Length; i++)
        {
            Transform tile = _tiles[i];
            TerrainTile terrainTile = _terrainTiles[i];
            if (terrainTile == null || !terrainTile.HasGround)
            {
                continue;
            }

            float tileLeft = tile.position.x - halfWidth;
            float tileRight = tile.position.x + halfWidth;
            if (worldX >= tileLeft && worldX <= tileRight)
            {
                float localX = terrainTile.transform.InverseTransformPoint(new Vector3(worldX, 0f, 0f)).x;
                return terrainTile.transform.position.y + terrainTile.GetGroundYAtLocalX(localX);
            }
        }

        // 2순위: 범위 내 타일이 없으면 가장 가까운 타일로 폴백
        TerrainTile nearestGroundTile = null;
        float nearestGroundDistance = float.MaxValue;
        Transform nearestTile = null;
        float nearestTileDistance = float.MaxValue;

        for (int i = 0; i < _tiles.Length; i++)
        {
            Transform tile = _tiles[i];
            float distance = Mathf.Abs(tile.position.x - worldX);

            if (distance < nearestTileDistance)
            {
                nearestTile = tile;
                nearestTileDistance = distance;
            }

            TerrainTile terrainTile = _terrainTiles[i];
            if (terrainTile == null || !terrainTile.HasGround)
            {
                continue;
            }

            if (distance < nearestGroundDistance)
            {
                nearestGroundTile = terrainTile;
                nearestGroundDistance = distance;
            }
        }

        if (nearestGroundTile != null)
        {
            float localX = nearestGroundTile.transform.InverseTransformPoint(new Vector3(worldX, 0f, 0f)).x;
            return nearestGroundTile.transform.position.y + nearestGroundTile.GetGroundYAtLocalX(localX);
        }

        return nearestTile != null ? nearestTile.position.y : 0f;
    }

    private void ApplyNextTerrainType(Transform tile)
    {
        int index = System.Array.IndexOf(_tiles, tile);
        if (index < 0)
        {
            return;
        }

        TerrainTile terrainTile = _terrainTiles[index];
        if (terrainTile == null)
        {
            return;
        }

        TerrainChunkType nextType = terrainSequencer != null
            ? terrainSequencer.GetNextType()
            : TerrainChunkType.Flat;

        float leftTopYOffset = GetRightmostTerrainTile(index)?.RightTopYOffset ?? 0f;
        nextType = ClampTerrainTypeToHeightRange(nextType, leftTopYOffset, terrainTile.SlopeHeightDelta);
        terrainSequencer?.SetLastType(nextType);
        terrainTile.SetType(nextType, leftTopYOffset);
    }

    private TerrainChunkType ClampTerrainTypeToHeightRange(
        TerrainChunkType type,
        float leftTopYOffset,
        float slopeHeightDelta
    )
    {
        float rightTopYOffset = type switch
        {
            TerrainChunkType.SlopeUp => leftTopYOffset + slopeHeightDelta,
            TerrainChunkType.SlopeDown => leftTopYOffset - slopeHeightDelta,
            TerrainChunkType.CurveUp => leftTopYOffset + slopeHeightDelta,
            TerrainChunkType.CurveDown => leftTopYOffset - slopeHeightDelta,
            _ => leftTopYOffset
        };

        return rightTopYOffset < minTerrainYOffset || rightTopYOffset > maxTerrainYOffset
            ? TerrainChunkType.Flat
            : type;
    }

    private void InitializeTerrainTiles()
    {
        TerrainTile previousTile = null;

        for (int i = 0; i < _terrainTiles.Length; i++)
        {
            TerrainTile terrainTile = _terrainTiles[i];
            if (terrainTile == null)
            {
                continue;
            }

            TerrainChunkType type = i == 0 || terrainSequencer == null
                ? TerrainChunkType.Flat
                : terrainSequencer.GetNextType();
            float leftTopYOffset = previousTile != null ? previousTile.RightTopYOffset : 0f;
            terrainTile.SetType(type, leftTopYOffset);
            previousTile = terrainTile;
        }
    }

    private TerrainTile GetRightmostTerrainTile(int excludedIndex)
    {
        TerrainTile rightmostTerrainTile = null;
        float rightmostX = float.MinValue;

        for (int i = 0; i < _tiles.Length; i++)
        {
            if (i == excludedIndex || _terrainTiles[i] == null)
            {
                continue;
            }

            if (_tiles[i].position.x > rightmostX)
            {
                rightmostX = _tiles[i].position.x;
                rightmostTerrainTile = _terrainTiles[i];
            }
        }

        return rightmostTerrainTile;
    }
}
