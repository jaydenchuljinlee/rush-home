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
/// Collider2D를 Trigger로 설정하고, Tag를 "Obstacle"로 지정할 것.
/// </summary>
public class Obstacle : MonoBehaviour, IPoolable
{
    [SerializeField] private ObstacleType obstacleType = ObstacleType.Ground;

    [Tooltip("이 값보다 X가 작아지면 풀로 반환 (화면 왼쪽 밖)")]
    [SerializeField] private float destroyX = -15f;

    private float _scrollSpeed;
    private ObstaclePool _pool;

    public ObstacleType ObstacleType => obstacleType;

    /// <summary>
    /// 스폰 시 초기화. ObstacleSpawner에서 호출.
    /// </summary>
    public void Initialize(float scrollSpeed, ObstaclePool pool = null)
    {
        _scrollSpeed = scrollSpeed;
        _pool = pool;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        transform.position += Vector3.left * _scrollSpeed * Time.deltaTime;

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
