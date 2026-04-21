using UnityEngine;

/// <summary>
/// 장애물 타입 분류.
/// Ground: 바닥 장애물 (점프로 피함)
/// Air: 공중 장애물 (슬라이딩으로 피함)
/// </summary>
public enum ObstacleType
{
    Ground,
    Air
}

/// <summary>
/// 장애물 컴포넌트. 왼쪽으로 이동하며, 화면 밖으로 나가면 오브젝트 풀로 반환.
/// GroundScroller의 현재 속도와 동기화하여 지면과 같은 속도로 이동한다.
/// Collider2D를 Trigger로 설정하고, Tag를 "Obstacle"로 지정할 것.
/// </summary>
public class Obstacle : MonoBehaviour, IPoolable
{
    [SerializeField] private ObstacleType obstacleType = ObstacleType.Ground;

    [Tooltip("이 값보다 X가 작아지면 풀로 반환 (화면 왼쪽 밖)")]
    [SerializeField] private float destroyX = -15f;

    private GroundScroller _groundScroller;
    private ObstaclePool _pool;

    public ObstacleType ObstacleType => obstacleType;

    /// <summary>
    /// 스폰 시 초기화. ObstacleSpawner에서 호출.
    /// </summary>
    public void Initialize(float scrollSpeed, ObstaclePool pool = null)
    {
        _pool = pool;
        // GroundScroller 캐싱 (한 번만)
        if (_groundScroller == null)
            _groundScroller = Object.FindFirstObjectByType<GroundScroller>();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        // GroundScroller의 현재 속도와 동기화
        float speed = _groundScroller != null ? _groundScroller.ScrollSpeed : 8f;
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < destroyX)
        {
            ReturnOrDestroy();
        }
    }

    private void ReturnOrDestroy()
    {
        if (_pool != null)
        {
            _pool.Return(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- IPoolable 구현 ---

    public void OnSpawnFromPool()
    {
        gameObject.SetActive(true);
    }

    public void OnReturnToPool()
    {
        gameObject.SetActive(false);
    }
}
