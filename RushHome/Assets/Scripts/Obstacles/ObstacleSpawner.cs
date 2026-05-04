using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    [SerializeField] GameObject[] groundObstacles;
    [SerializeField] GameObject[] airObstacles;
    [SerializeField] GameObject[] laneObstacles;

    [Header("Spawn Settings")]
    [SerializeField] float spawnDistance = 60f;
    [SerializeField] float laneWidth = 2.5f;

    float nextSpawnZ;
    Transform playerTransform;

    void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerController>().transform;
        nextSpawnZ = playerTransform.position.z + spawnDistance * 0.5f;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.Playing) return;

        while (playerTransform.position.z + spawnDistance > nextSpawnZ)
        {
            SpawnObstacle();
            float interval = Random.Range(
                DifficultyManager.Instance.SpawnIntervalMin,
                DifficultyManager.Instance.SpawnIntervalMax
            );
            nextSpawnZ += DifficultyManager.Instance.CurrentSpeed * interval;
        }
    }

    void SpawnObstacle()
    {
        float roll = Random.value;
        GameObject prefab;
        Vector3 position;

        if (roll < 0.4f)
        {
            // 지면 장애물 (점프)
            prefab = groundObstacles[Random.Range(0, groundObstacles.Length)];
            int lane = Random.Range(-1, 2);
            position = new Vector3(lane * laneWidth, 0f, nextSpawnZ);
        }
        else if (roll < 0.65f)
        {
            // 공중 장애물 (슬라이드)
            prefab = airObstacles[Random.Range(0, airObstacles.Length)];
            int lane = Random.Range(-1, 2);
            position = new Vector3(lane * laneWidth, 1.5f, nextSpawnZ);
        }
        else
        {
            // 레인 차단 장애물 (레인 전환)
            prefab = laneObstacles[Random.Range(0, laneObstacles.Length)];
            int lane = Random.Range(-1, 2);
            position = new Vector3(lane * laneWidth, 0f, nextSpawnZ);
        }

        GameObject obj = ObstaclePool.Instance.Get(prefab);
        obj.transform.position = position;
        obj.transform.rotation = Quaternion.identity;
    }
}
