using UnityEngine;

/// <summary>
/// 거리 기반 슬롯 시스템으로 장애물을 스폰하는 스포너.
/// 지면이 일정 거리 스크롤할 때마다 하나의 슬롯을 생성하고,
/// 그 슬롯에 Ground 또는 Air 중 하나를 배치한다.
/// 슬롯 간 최소 X 간격이 보장되어 겹침이 발생하지 않는다.
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [Header("장애물 프리팹 목록 (Ground, Air 등 혼합 가능)")]
    [SerializeField] private GameObject[] obstaclePrefabs;

    [Header("참조")]
    [SerializeField] private GroundScroller groundScroller;
    [SerializeField] private ObstaclePool obstaclePool;

    [Header("슬롯 설정")]
    [Tooltip("슬롯 간 최소 X 거리 (지면 스크롤 기준)")]
    [SerializeField] private float slotSpacingMin = 5f;
    [Tooltip("슬롯 간 최대 X 거리")]
    [SerializeField] private float slotSpacingMax = 10f;
    [SerializeField] private float spawnX = 12f;

    [Header("점프 클리어런스")]
    [Tooltip("플레이어 점프 체공 시간 (초). jumpForce=9, gravityScale=3 기준 약 0.61초")]
    [SerializeField] private float jumpDuration = 0.61f;
    [Tooltip("장애물 폭 (스케일 반영). 0.7 기준")]
    [SerializeField] private float obstacleWidth = 0.7f;
    [Tooltip("점프 클리어런스 여유 비율 (1.0 = 여유 없음, 1.3 = 30% 여유)")]
    [SerializeField] private float clearanceMargin = 1.3f;

    [Header("Y 위치 설정")]
    [Tooltip("바닥 장애물 스폰 Y (지면 상단 = 0)")]
    [SerializeField] private float groundObstacleY = 0f;
    [Tooltip("공중 장애물 스폰 Y (플레이어 머리 위)")]
    [SerializeField] private float airObstacleY = 1.5f;

    /// <summary>마지막 스폰 이후 누적 이동 거리</summary>
    private float _distanceSinceLastSpawn;
    /// <summary>다음 스폰까지 필요한 거리</summary>
    private float _nextSlotDistance;

    private void Start()
    {
        ResetSlotDistance();
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

        // 지면이 이동한 거리를 누적
        float moveAmount = groundScroller != null ? groundScroller.LastMoveAmount : 8f * Time.deltaTime;
        _distanceSinceLastSpawn += moveAmount;

        if (_distanceSinceLastSpawn >= _nextSlotDistance)
        {
            SpawnObstacle();
            _distanceSinceLastSpawn = 0f;
            ResetSlotDistance();
        }
    }

    private void SpawnObstacle()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        // 랜덤으로 프리팹 선택 (Ground 또는 Air)
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
            GameObject go = Instantiate(prefab, spawnPos, Quaternion.identity);
            obstacle = go.GetComponent<Obstacle>();
        }

        if (obstacle != null)
        {
            float speed = groundScroller != null ? groundScroller.ScrollSpeed : 8f;
            obstacle.Initialize(speed, obstaclePool);
        }
    }

    /// <summary>
    /// 난이도 매니저에서 호출. 시간 간격(초)을 거리 간격으로 변환하여 적용.
    /// 최소 간격은 플레이어 점프 클리어런스 이상으로 보장한다.
    /// </summary>
    public void SetSpawnInterval(float minTime, float maxTime)
    {
        float speed = groundScroller != null ? groundScroller.ScrollSpeed : 8f;

        // 점프 클리어런스: 점프 중 이동 거리 + 장애물 폭 + 여유
        float minJumpClearance = (jumpDuration * speed + obstacleWidth) * clearanceMargin;

        slotSpacingMin = Mathf.Max(minTime * speed, minJumpClearance);
        slotSpacingMax = Mathf.Max(maxTime * speed, minJumpClearance + 2f);

        if (_nextSlotDistance > slotSpacingMax)
        {
            _nextSlotDistance = slotSpacingMax;
        }
    }

    private void ResetSlotDistance()
    {
        _nextSlotDistance = Random.Range(slotSpacingMin, slotSpacingMax);
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.Playing)
        {
            _distanceSinceLastSpawn = 0f;
            ResetSlotDistance();
        }
    }
}
