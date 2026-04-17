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

    public float ScrollSpeed
    {
        get => scrollSpeed;
        set => scrollSpeed = value;
    }

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
            return;

        float moveAmount = scrollSpeed * Time.deltaTime;

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
