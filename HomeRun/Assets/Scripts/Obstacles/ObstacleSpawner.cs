using UnityEngine;

/// <summary>
/// 일정 간격으로 장애물을 스폰하는 스포너.
/// GroundScroller의 속도를 참조하여 장애물 이동 속도를 맞춤.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GroundScroller groundScroller;
    [SerializeField] private float spawnIntervalMin = 1.5f;
    [SerializeField] private float spawnIntervalMax = 3f;
    [SerializeField] private float spawnX = 12f;
    [SerializeField] private float spawnY = -1.5f;

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
        if (obstaclePrefab == null) return;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);

        Obstacle obstacleComponent = obstacle.GetComponent<Obstacle>();
        if (obstacleComponent != null)
        {
            obstacleComponent.Initialize(groundScroller != null ? groundScroller.ScrollSpeed : 8f);
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
