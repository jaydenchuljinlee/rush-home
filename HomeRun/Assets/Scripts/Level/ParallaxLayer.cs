using UnityEngine;

/// <summary>
/// 파랄랙스 배경의 단일 레이어를 담당.
/// 자식으로 2개의 SpriteRenderer 타일을 가지며, 화면 밖으로 나가면 오른쪽으로 재배치.
/// </summary>
public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.3f;
    [SerializeField] private float tileWidth = 20f;

    private Transform[] _tiles;
    private Camera _camera;

    public float SpeedMultiplier => speedMultiplier;

    /// <summary>레이어 초기화. ParallaxBackground가 Awake에서 호출.</summary>
    public void Init(Camera cam)
    {
        _camera = cam;

        int childCount = transform.childCount;
        _tiles = new Transform[childCount];
        for (int i = 0; i < childCount; i++)
        {
            _tiles[i] = transform.GetChild(i);
        }
    }

    /// <summary>매 프레임 호출. isPlaying이 false이면 이동하지 않음.</summary>
    public void Tick(float groundScrollSpeed, bool isPlaying)
    {
        if (!isPlaying || _tiles == null)
        {
            return;
        }

        float moveAmount = groundScrollSpeed * speedMultiplier * Time.deltaTime;
        float totalWidth = _tiles.Length * tileWidth;

        foreach (Transform tile in _tiles)
        {
            tile.position += Vector3.left * moveAmount;
        }

        foreach (Transform tile in _tiles)
        {
            if (ShouldRecycle(tile))
            {
                tile.position += new Vector3(totalWidth, 0f, 0f);
            }
        }
    }

    private bool ShouldRecycle(Transform tile)
    {
        float halfTileWidth = tileWidth * 0.5f;

        if (_camera == null || !_camera.orthographic)
        {
            return tile.position.x + halfTileWidth <= -halfTileWidth;
        }

        float screenHalfWidth = _camera.orthographicSize * _camera.aspect;
        float leftEdge = _camera.transform.position.x - screenHalfWidth;
        return tile.position.x + halfTileWidth <= leftEdge;
    }
}
