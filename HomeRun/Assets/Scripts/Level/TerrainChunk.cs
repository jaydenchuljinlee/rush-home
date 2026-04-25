using UnityEngine;

/// <summary>
/// 지형 청크 타입.
/// Flat: 평지 (기본)
/// SlopeUp: 오르막 직선 경사
/// SlopeDown: 내리막 직선 경사
/// CurveUp: 오르막 곡선 (중간 볼록)
/// CurveDown: 내리막 곡선 (중간 오목)
/// Gap: 틈새 (점프 필요)
/// </summary>
public enum TerrainChunkType
{
    Flat,
    SlopeUp,
    SlopeDown,
    CurveUp,
    CurveDown,
    Gap
}

/// <summary>
/// 지형 청크 컴포넌트. TerrainChunkSpawner가 스폰하는 청크 오브젝트에 부착.
/// 타입에 따라 초기 회전/크기 변형을 적용하며, 왼쪽으로 스크롤 후 화면 밖으로 나가면 반환된다.
/// </summary>
public class TerrainChunk : MonoBehaviour
{
    [Header("청크 설정")]
    [SerializeField] private TerrainChunkType chunkType = TerrainChunkType.Flat;
    [SerializeField] private float chunkWidth = 10f;

    [Header("경사 설정")]
    [Tooltip("SlopeUp/SlopeDown 타입 시 Z 회전각 (도)")]
    [SerializeField] private float slopeAngle = 15f;

    [Header("갭 설정")]
    [Tooltip("Gap 타입 시 청크가 스폰되지 않는 빈 구간 폭. 이 값만큼 다음 청크가 뒤로 밀린다.")]
    [SerializeField] private float gapWidth = 4f;

    [Tooltip("화면 왼쪽 밖으로 나가면 반환 (TerrainChunkSpawner 콜백)")]
    [SerializeField] private float destroyX = -20f;

    private float _scrollSpeed;
    private TerrainChunkSpawner _spawner;

    public TerrainChunkType ChunkType => chunkType;
    public float ChunkWidth => chunkType == TerrainChunkType.Gap ? gapWidth : chunkWidth;

    /// <summary>
    /// TerrainChunkSpawner가 스폰 시 호출하는 초기화 메서드.
    /// </summary>
    public void Initialize(float scrollSpeed, TerrainChunkSpawner spawner)
    {
        _scrollSpeed = scrollSpeed;
        _spawner = spawner;

        ApplyVisualTransform();
    }

    private void ApplyVisualTransform()
    {
        switch (chunkType)
        {
            case TerrainChunkType.SlopeUp:
                transform.localRotation = Quaternion.Euler(0f, 0f, slopeAngle);
                break;
            case TerrainChunkType.SlopeDown:
                transform.localRotation = Quaternion.Euler(0f, 0f, -slopeAngle);
                break;
            case TerrainChunkType.Gap:
                // Gap은 비어있는 구간이므로 오브젝트를 비활성화 (Spawner가 폭만 예약)
                // 렌더러가 있으면 숨긴다
                Renderer rend = GetComponent<Renderer>();
                if (rend != null) rend.enabled = false;
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
                break;
            default:
                transform.localRotation = Quaternion.identity;
                break;
        }
    }

    private void Update()
    {
        if (!GameManager.IsPlaying) return;

        transform.position += Vector3.left * _scrollSpeed * Time.deltaTime;

        if (transform.position.x < destroyX)
        {
            ReturnToSpawner();
        }
    }

    private void ReturnToSpawner()
    {
        _spawner?.OnChunkExited(this);
        Destroy(gameObject);
    }
}
