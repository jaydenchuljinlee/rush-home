using UnityEngine;

/// <summary>
/// 일정 간격으로 장애물을 랜덤 스폰하는 스포너.
/// 바닥 장애물(Ground)과 공중 장애물(Air) 프리팹을 모두 지원하며
/// ObstaclePool을 통해 오브젝트를 재사용한다.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [Header("장애물 프리팹 목록 (Ground, Air 등 혼합 가능)")]
    [SerializeField] private GameObject[] obstaclePrefabs;

    [Header("참조")]
    [SerializeField] private GroundScroller groundScroller;
    [SerializeField] private ObstaclePool obstaclePool;

    [Header("스폰 설정")]
    [SerializeField] private float spawnIntervalMin = 1.5f;
    [SerializeField] private float spawnIntervalMax = 3f;
    [SerializeField] private float spawnX = 12f;

    [Header("Y 위치 설정")]
    [Tooltip("바닥 장애물 스폰 Y (지면 상단 = 0)")]
    [SerializeField] private float groundObstacleY = 0f;
    [Tooltip("공중 장애물 스폰 Y (플레이어 머리 위)")]
    [SerializeField] private float airObstacleY = 1.5f;

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
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
            return;

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            SpawnObstacle();
            ResetTimer();
        }
    }

    private void SpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        // 랜덤으로 프리팹 선택
        int index = Random.Range(0, obstaclePrefabs.Length);
        GameObject prefab = obstaclePrefabs[index];
        if (prefab == null) return;

        // 장애물 타입에 따라 Y 결정
        Obstacle prefabObstacle = prefab.GetComponent<Obstacle>();
        float spawnY = groundObstacleY;
        if (prefabObstacle != null && prefabObstacle.ObstacleType == ObstacleType.Air)
        {
            spawnY = airObstacleY;
        }

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);

        Obstacle obstacle;
        if (obstaclePool != null)
        {
            obstacle = obstaclePool.Get(prefab, spawnPos);
        }
        else
        {
            // 풀 없을 경우 Instantiate 폴백
            GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
            obstacle = go.GetComponent<Obstacle>();
        }

        if (obstacle != null)
        {
            float speed = groundScroller != null ? groundScroller.ScrollSpeed : 8f;
            obstacle.Initialize(speed, obstaclePool);
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
