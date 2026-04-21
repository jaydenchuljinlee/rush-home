using UnityEngine;

/// <summary>
/// 난이도에 따라 지형 청크를 스폰하는 스포너.
/// DifficultyManager가 SetDifficultyTier()로 현재 티어를 전달하면
/// 해당 티어에서 허용된 청크 타입 풀에서 랜덤 선택 후 스폰한다.
///
/// 티어별 허용 청크 타입:
/// Easy    : Flat
/// Normal  : Flat, SlopeUp, SlopeDown
/// Hard    : Flat, SlopeUp, SlopeDown, Gap
/// Extreme : Flat, SlopeUp, SlopeDown, Gap (Gap 확률 증가)
/// </summary>
public class TerrainChunkSpawner : MonoBehaviour
{
    [Header("청크 프리팹 (타입별)")]
    [SerializeField] private GameObject flatChunkPrefab;
    [SerializeField] private GameObject slopeUpChunkPrefab;
    [SerializeField] private GameObject slopeDownChunkPrefab;
    [SerializeField] private GameObject gapChunkPrefab;

    [Header("참조")]
    [SerializeField] private GroundScroller groundScroller;

    [Header("스폰 설정")]
    [Tooltip("새 청크가 생성될 X 위치")]
    [SerializeField] private float spawnX = 15f;
    [Tooltip("청크 스폰 간격 (이전 청크 끝에서 다음 청크 시작까지의 거리)")]
    [SerializeField] private float spawnDistanceThreshold = 8f;

    [Header("Gap 등장 확률 (0~1)")]
    [Tooltip("Hard 티어에서 Gap 청크가 선택될 확률")]
    [SerializeField] private float hardGapChance = 0.15f;
    [Tooltip("Extreme 티어에서 Gap 청크가 선택될 확률")]
    [SerializeField] private float extremeGapChance = 0.25f;

    private DifficultyTier _currentTier = DifficultyTier.Easy;
    private float _nextSpawnX;

    private void Start()
    {
        _nextSpawnX = spawnX;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        // 카메라 기준 스폰 X에 가까워지면 다음 청크 스폰
        float scrollSpeed = groundScroller != null ? groundScroller.ScrollSpeed : 8f;
        _nextSpawnX -= scrollSpeed * Time.deltaTime;

        if (_nextSpawnX <= spawnX - spawnDistanceThreshold)
        {
            SpawnNextChunk();
        }
    }

    /// <summary>
    /// DifficultyManager가 호출. 현재 난이도 티어를 갱신한다.
    /// </summary>
    public void SetDifficultyTier(DifficultyTier tier)
    {
        _currentTier = tier;
    }

    public DifficultyTier CurrentTier => _currentTier;

    /// <summary>
    /// TerrainChunk가 화면 밖으로 나갔을 때 콜백.
    /// </summary>
    public void OnChunkExited(TerrainChunk chunk)
    {
        // 현재는 별도 처리 없음. 향후 풀링 확장 지점.
    }

    private void SpawnNextChunk()
    {
        GameObject prefab = SelectChunkPrefab();
        if (prefab == null) return;

        Vector3 spawnPos = new Vector3(spawnX, 0f, 0f);
        GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
        TerrainChunk chunk = go.GetComponent<TerrainChunk>();

        float speed = groundScroller != null ? groundScroller.ScrollSpeed : 8f;
        if (chunk != null)
        {
            chunk.Initialize(speed, this);
            _nextSpawnX = spawnX; // 리셋 후 다시 대기
        }
    }

    private GameObject SelectChunkPrefab()
    {
        switch (_currentTier)
        {
            case DifficultyTier.Easy:
                return flatChunkPrefab;

            case DifficultyTier.Normal:
            {
                // Flat, SlopeUp, SlopeDown 중 선택
                int roll = Random.Range(0, 3);
                return roll switch
                {
                    0 => flatChunkPrefab,
                    1 => slopeUpChunkPrefab != null ? slopeUpChunkPrefab : flatChunkPrefab,
                    _ => slopeDownChunkPrefab != null ? slopeDownChunkPrefab : flatChunkPrefab,
                };
            }

            case DifficultyTier.Hard:
            {
                float r = Random.value;
                if (r < hardGapChance && gapChunkPrefab != null)
                    return gapChunkPrefab;

                int roll = Random.Range(0, 3);
                return roll switch
                {
                    0 => flatChunkPrefab,
                    1 => slopeUpChunkPrefab != null ? slopeUpChunkPrefab : flatChunkPrefab,
                    _ => slopeDownChunkPrefab != null ? slopeDownChunkPrefab : flatChunkPrefab,
                };
            }

            case DifficultyTier.Extreme:
            {
                float r = Random.value;
                if (r < extremeGapChance && gapChunkPrefab != null)
                    return gapChunkPrefab;

                int roll = Random.Range(0, 3);
                return roll switch
                {
                    0 => flatChunkPrefab,
                    1 => slopeUpChunkPrefab != null ? slopeUpChunkPrefab : flatChunkPrefab,
                    _ => slopeDownChunkPrefab != null ? slopeDownChunkPrefab : flatChunkPrefab,
                };
            }

            default:
                return flatChunkPrefab;
        }
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing)
        {
            _nextSpawnX = spawnX;
        }
    }
}
