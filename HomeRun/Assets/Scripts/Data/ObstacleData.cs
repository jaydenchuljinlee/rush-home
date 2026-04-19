using UnityEngine;

/// <summary>
/// 장애물 타입별 설정 데이터 ScriptableObject.
/// 런타임에 수정하지 않는 읽기 전용 데이터.
/// </summary>
[CreateAssetMenu(fileName = "ObstacleData", menuName = "HomeRun/Obstacle Data")]
public class ObstacleData : ScriptableObject
{
    [Header("장애물 타입")]
    [SerializeField] private ObstacleType obstacleType = ObstacleType.Ground;

    [Header("스폰 위치")]
    [Tooltip("지면 Y(0) 기준 스폰 Y 오프셋. 바닥 장애물: -1.5, 공중 장애물: 0.5 등")]
    [SerializeField] private float spawnYOffset = -1.5f;

    [Header("콜라이더 설정")]
    [SerializeField] private Vector2 colliderSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 colliderOffset = Vector2.zero;

    public ObstacleType ObstacleType => obstacleType;
    public float SpawnYOffset => spawnYOffset;
    public Vector2 ColliderSize => colliderSize;
    public Vector2 ColliderOffset => colliderOffset;
}
