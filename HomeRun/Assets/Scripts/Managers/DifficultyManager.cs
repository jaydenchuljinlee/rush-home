using UnityEngine;

/// <summary>
/// 경과 시간에 따라 난이도(스크롤 속도 + 장애물 스폰 빈도)를 조절하는 싱글톤 매니저.
/// DifficultyData ScriptableObject에서 단계별 값을 읽어 GroundScroller, ObstacleSpawner에 적용.
///
/// 난이도 단계:
/// Easy    : 0 ~ 30초   (속도 6,  스폰 1.5 ~ 3.0초)
/// Normal  : 30 ~ 60초  (속도 8,  스폰 1.2 ~ 2.5초)
/// Hard    : 60 ~ 120초 (속도 11, 스폰 0.9 ~ 2.0초)
/// Extreme : 120초 이상 (속도 15, 스폰 0.6 ~ 1.5초)
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [SerializeField] private DifficultyData difficultyData;
    [SerializeField] private GroundScroller groundScroller;
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private PatternSpawner patternSpawner;
    [SerializeField] private TerrainChunkSpawner terrainChunkSpawner;
    [SerializeField] private TerrainTypeSequencer terrainTypeSequencer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
        if (difficultyData == null) return;

        float elapsed = GameManager.Instance.ElapsedTime;

        // 스크롤 속도 적용
        float targetSpeed = difficultyData.GetSpeedForTime(elapsed);
        if (groundScroller != null)
        {
            groundScroller.ScrollSpeed = targetSpeed;
        }

        // 장애물 스폰 간격 적용
        var (spawnMin, spawnMax) = difficultyData.GetSpawnIntervalForTime(elapsed);
        if (obstacleSpawner != null)
        {
            obstacleSpawner.SetSpawnInterval(spawnMin, spawnMax);
        }

        // 패턴 스포너: 현재 난이도에 맞는 패턴 풀 전달
        if (patternSpawner != null)
        {
            patternSpawner.SetSpawnInterval(spawnMin, spawnMax);
            LevelPatternData[] patterns = difficultyData.GetPatternsForTime(elapsed);
            patternSpawner.SetAvailablePatterns(patterns);
        }

        // 지형 청크 스포너: 현재 티어 전달
        if (terrainChunkSpawner != null)
        {
            DifficultyTier tier = difficultyData.GetTierForTime(elapsed);
            terrainChunkSpawner.SetDifficultyTier(tier);
        }

        if (terrainTypeSequencer != null)
        {
            DifficultyTier tier = difficultyData.GetTierForTime(elapsed);
            terrainTypeSequencer.SetDifficultyTier(tier);
        }
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing && difficultyData != null)
        {
            // 게임 시작 시 Easy 단계로 초기화
            if (groundScroller != null)
            {
                groundScroller.ScrollSpeed = difficultyData.EasySpeed;
            }

            if (obstacleSpawner != null)
            {
                obstacleSpawner.SetSpawnInterval(
                    difficultyData.EasySpawnMin,
                    difficultyData.EasySpawnMax
                );
            }

            if (patternSpawner != null)
            {
                patternSpawner.SetSpawnInterval(
                    difficultyData.EasySpawnMin,
                    difficultyData.EasySpawnMax
                );
                patternSpawner.SetAvailablePatterns(difficultyData.EasyPatterns);
            }

            if (terrainChunkSpawner != null)
            {
                terrainChunkSpawner.SetDifficultyTier(DifficultyTier.Easy);
            }

            if (terrainTypeSequencer != null)
            {
                terrainTypeSequencer.SetDifficultyTier(DifficultyTier.Easy);
            }
        }
    }

    /// <summary>
    /// 현재 경과 시간에 해당하는 목표 속도를 반환한다. (테스트 및 외부 접근용)
    /// </summary>
    public float GetCurrentTargetSpeed()
    {
        if (difficultyData == null || GameManager.Instance == null) return 0f;
        return difficultyData.GetSpeedForTime(GameManager.Instance.ElapsedTime);
    }
}
