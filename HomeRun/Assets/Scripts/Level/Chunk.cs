using UnityEngine;

/// <summary>
/// 청크 프리팹에 부착하는 컴포넌트.
/// 지면 + 장애물 배치를 하나의 단위로 관리하고, 오브젝트 풀을 통해 재사용된다.
/// </summary>
public class Chunk : MonoBehaviour, IPoolable
{
    [SerializeField] private float _chunkWidth = 20f;

    private float _scrollSpeed;
    private System.Action<Chunk> _returnToPool;

    public float ChunkWidth => _chunkWidth;

    /// <summary>ChunkSpawner가 스폰 시 호출한다.</summary>
    public void Initialize(float scrollSpeed, System.Action<Chunk> returnToPool)
    {
        _scrollSpeed = scrollSpeed;
        _returnToPool = returnToPool;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        transform.position += Vector3.left * _scrollSpeed * Time.deltaTime;

        // 청크 오른쪽 끝이 화면 왼쪽 밖으로 나가면 회수
        if (transform.position.x + _chunkWidth < -_chunkWidth)
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
        // 하위 장애물도 활성화 (프리팹 구조에 따라 자동)
    }

    public void OnReturnToPool()
    {
        _returnToPool = null;
        _scrollSpeed = 0f;
    }
}
