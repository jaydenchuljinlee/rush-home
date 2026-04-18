using UnityEngine;

/// <summary>
/// 장애물 공통 동작. 왼쪽으로 이동하며, 화면 밖으로 나가면 풀로 반환된다.
/// IPoolable 구현으로 ObjectPool과 함께 동작한다.
/// Collider2D를 Trigger로 설정하고 Tag를 "Obstacle"로 지정할 것.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour, IPoolable
{
    [SerializeField] private float _destroyX = -15f;

    private float _scrollSpeed;
    private System.Action<Obstacle> _returnToPool;

    /// <summary>장애물 유형 (Inspector에서 설정 또는 Initialize로 주입).</summary>
    public ObstacleType ObstacleType { get; private set; }

    /// <summary>
    /// 스폰 시 초기화. PoolManager 또는 ChunkSpawner에서 호출한다.
    /// </summary>
    /// <param name="scrollSpeed">이동 속도</param>
    /// <param name="obstacleType">장애물 유형</param>
    /// <param name="returnToPool">풀 반환 콜백 (null이면 Destroy)</param>
    public void Initialize(float scrollSpeed, ObstacleType obstacleType = ObstacleType.Jump,
        System.Action<Obstacle> returnToPool = null)
    {
        _scrollSpeed = scrollSpeed;
        ObstacleType = obstacleType;
        _returnToPool = returnToPool;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        transform.position += Vector3.left * _scrollSpeed * Time.deltaTime;

        if (transform.position.x < _destroyX)
        {
            Recycle();
        }
    }

    private void Recycle()
    {
        if (_returnToPool != null)
            _returnToPool(this);
        else
            Destroy(gameObject);
    }

    // ---- IPoolable ----

    public void OnSpawnFromPool()
    {
        // 풀에서 꺼낼 때 필요한 리셋 로직 (현재 없음 — 서브클래스에서 확장)
    }

    public void OnReturnToPool()
    {
        _returnToPool = null;
        _scrollSpeed = 0f;
    }
}
