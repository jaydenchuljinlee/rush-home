using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 난이도에 따라 다양한 장애물을 스폰하는 스포너.
/// DifficultyData로 스폰 간격을 제어하고, ObstacleData 목록에서 현재 등장 가능한 장애물을 선택한다.
/// ObjectPool을 통해 Instantiate/Destroy 최소화.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [Header("장애물 데이터")]
    [SerializeField] private ObstacleData[] _obstacleDataList;

    [Header("참조")]
    [SerializeField] private DifficultyData _difficultyData;
    [SerializeField] private GroundScroller _groundScroller;

    [Header("스폰 설정")]
    [SerializeField] private float _spawnX = 12f;
    [SerializeField] private float _baseSpawnY = -1.5f;

    [Header("풀 설정")]
    [SerializeField] private int _poolInitialSize = 3;
    [SerializeField] private int _poolMaxSize = 10;

    private float _spawnTimer;

    // ObstacleData별 풀
    private Dictionary<ObstacleData, ObjectPool<Obstacle>> _pools;

    private void Awake()
    {
        _pools = new Dictionary<ObstacleData, ObjectPool<Obstacle>>();

        foreach (ObstacleData data in _obstacleDataList)
        {
            if (data == null || data.Prefab == null) continue;

            Obstacle prefabObstacle = data.Prefab.GetComponent<Obstacle>();
            if (prefabObstacle == null)
            {
                Debug.LogWarning($"[ObstacleSpawner] {data.name} 프리팹에 Obstacle 컴포넌트가 없습니다.");
                continue;
            }

            _pools[data] = new ObjectPool<Obstacle>(prefabObstacle, _poolInitialSize, _poolMaxSize, transform);
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Start()
    {
        ResetTimer();
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
        ObstacleData selectedData = SelectObstacleData();
        if (selectedData == null) return;

        if (!_pools.TryGetValue(selectedData, out ObjectPool<Obstacle> pool)) return;

        Obstacle obstacle = pool.Get();
        if (obstacle == null) return;

        float spawnY = _baseSpawnY + selectedData.SpawnYOffset;
        obstacle.transform.position = new Vector3(_spawnX, spawnY, 0f);

        float elapsed = GameManager.Instance != null ? GameManager.Instance.ElapsedTime : 0f;
        float scrollSpeed = _groundScroller != null
            ? _groundScroller.ScrollSpeed
            : (_difficultyData != null ? _difficultyData.GetScrollSpeed(elapsed) : 8f);

        obstacle.Initialize(scrollSpeed, selectedData.ObstacleType, (o) => ReturnObstacle(selectedData, o));
    }

    private void ReturnObstacle(ObstacleData data, Obstacle obstacle)
    {
        if (_pools.TryGetValue(data, out ObjectPool<Obstacle> pool))
            pool.Return(obstacle);
        else
            Destroy(obstacle.gameObject);
    }

    private void ResetTimer()
    {
        float elapsed = GameManager.Instance != null ? GameManager.Instance.ElapsedTime : 0f;

        if (_difficultyData != null)
            _spawnTimer = _difficultyData.GetRandomSpawnInterval(elapsed);
        else
            _spawnTimer = Random.Range(1.5f, 3f);
    }

    /// <summary>현재 경과 시간 기준으로 등장 가능한 ObstacleData 중 하나를 무작위 선택한다.</summary>
    private ObstacleData SelectObstacleData()
    {
        if (_obstacleDataList == null || _obstacleDataList.Length == 0) return null;

        float elapsed = GameManager.Instance != null ? GameManager.Instance.ElapsedTime : 0f;

        List<ObstacleData> candidates = new List<ObstacleData>();
        foreach (ObstacleData data in _obstacleDataList)
        {
            if (data != null && elapsed >= data.MinAppearTime)
                candidates.Add(data);
        }

        if (candidates.Count == 0) return _obstacleDataList[0];

        return candidates[Random.Range(0, candidates.Count)];
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing)
        {
            ResetTimer();
        }
    }
}
