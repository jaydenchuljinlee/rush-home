using UnityEngine;

/// <summary>
/// 장애물 조합 패턴(LevelPatternData) 기반 스포너.
/// DifficultyManager가 SetAvailablePatterns()로 현재 난이도의 패턴 풀을 전달한다.
/// ObstacleSpawner와 공존 가능. 씬에서 둘 중 하나만 활성화하여 사용한다.
/// </summary>
public class PatternSpawner : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private GroundScroller groundScroller;
    [SerializeField] private ObstaclePool obstaclePool;

    [Header("스폰 설정")]
    [SerializeField] private float spawnIntervalMin = 1.5f;
    [SerializeField] private float spawnIntervalMax = 3f;
    [SerializeField] private float spawnX = 12f;

    [Header("Y 위치 기본값")]
    [Tooltip("Ground 장애물 스폰 Y (지면 상단 = 0)")]
    [SerializeField] private float groundObstacleY = 0f;
    [Tooltip("Air 장애물 스폰 Y (플레이어 머리 위)")]
    [SerializeField] private float airObstacleY = 1.5f;

    private LevelPatternData[] _availablePatterns = new LevelPatternData[0];
    private float _spawnTimer;

    private void Start()
    {
        ResetTimer();
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
        if (!GameManager.IsPlaying) return;

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            SpawnPattern();
            ResetTimer();
        }
    }

    /// <summary>
    /// DifficultyManager에서 현재 난이도에 맞는 패턴 풀을 전달한다.
    /// null이 오면 빈 배열로 처리한다.
    /// </summary>
    public void SetAvailablePatterns(LevelPatternData[] patterns)
    {
        _availablePatterns = patterns ?? new LevelPatternData[0];
    }

    /// <summary>
    /// 난이도 매니저에서 호출. 스폰 간격 범위를 갱신한다.
    /// </summary>
    public void SetSpawnInterval(float min, float max)
    {
        spawnIntervalMin = min;
        spawnIntervalMax = max;

        if (_spawnTimer > spawnIntervalMax)
        {
            _spawnTimer = spawnIntervalMax;
        }
    }

    private void SpawnPattern()
    {
        if (_availablePatterns == null || _availablePatterns.Length == 0) return;

        // 유효한 패턴만 필터링
        int validCount = 0;
        for (int i = 0; i < _availablePatterns.Length; i++)
        {
            if (_availablePatterns[i] != null && _availablePatterns[i].Obstacles.Length > 0)
                validCount++;
        }
        if (validCount == 0) return;

        // 랜덤 패턴 선택
        LevelPatternData pattern = null;
        int attempts = 0;
        while (pattern == null && attempts < _availablePatterns.Length * 2)
        {
            LevelPatternData candidate = _availablePatterns[Random.Range(0, _availablePatterns.Length)];
            if (candidate != null && candidate.Obstacles.Length > 0)
                pattern = candidate;
            attempts++;
        }
        if (pattern == null) return;

        float speed = groundScroller != null ? groundScroller.ScrollSpeed : 8f;

        foreach (ObstacleEntry entry in pattern.Obstacles)
        {
            if (entry.prefab == null) continue;

            // Y 위치 결정
            float spawnY;
            if (!entry.UsesDefaultY)
            {
                spawnY = entry.yOverride;
            }
            else
            {
                Obstacle prefabObstacle = entry.prefab.GetComponent<Obstacle>();
                spawnY = (prefabObstacle != null && prefabObstacle.ObstacleType == ObstacleType.Air)
                    ? airObstacleY
                    : groundObstacleY;
            }

            Vector3 spawnPos = new Vector3(spawnX + entry.xOffset, spawnY, 0f);

            Obstacle obstacle;
            if (obstaclePool != null)
            {
                obstacle = obstaclePool.Get(entry.prefab, spawnPos);
            }
            else
            {
                GameObject go = Instantiate(entry.prefab, spawnPos, Quaternion.identity);
                obstacle = go.GetComponent<Obstacle>();
            }

            if (obstacle != null)
            {
                obstacle.Initialize(speed, obstaclePool);
            }
        }
    }

    private void ResetTimer()
    {
        _spawnTimer = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing)
        {
            ResetTimer();
        }
    }
}
