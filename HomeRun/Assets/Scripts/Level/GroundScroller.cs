using UnityEngine;

/// <summary>
/// 지면 타일을 왼쪽으로 스크롤하고, 화면 밖으로 나가면 오른쪽 끝으로 재배치.
/// 2개 이상의 지면 타일을 자식으로 갖는 부모 오브젝트에 부착.
/// </summary>
public class GroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 8f;
    [SerializeField] private float tileWidth = 20f;

    private Transform[] _tiles;
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
        _tiles = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            _tiles[i] = transform.GetChild(i);
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
        {
            _lastMoveAmount = 0f;
            return;
        }

        float moveAmount = scrollSpeed * Time.deltaTime;
        _lastMoveAmount = moveAmount;

        foreach (Transform tile in _tiles)
        {
            tile.position += Vector3.left * moveAmount;

            // 화면 왼쪽 밖으로 나가면 오른쪽 끝으로 재배치
            if (tile.position.x <= -tileWidth)
            {
                float rightmostX = GetRightmostX();
                tile.position = new Vector3(rightmostX + tileWidth, tile.position.y, tile.position.z);
            }
        }
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
}
