using UnityEngine;

/// <summary>장애물 회피 방식 분류.</summary>
public enum ObstacleType
{
    /// <summary>바닥 장애물 — 점프로 회피.</summary>
    Jump,

    /// <summary>상단 장애물 — 슬라이딩으로 회피.</summary>
    Slide,

    /// <summary>복합 장애물 — 점프 또는 슬라이딩 조합으로 회피.</summary>
    Combo
}

/// <summary>
/// 장애물 프리팹과 속성을 묶어 관리하는 ScriptableObject.
/// </summary>
[CreateAssetMenu(fileName = "ObstacleData", menuName = "HomeRun/Obstacle Data")]
public class ObstacleData : ScriptableObject
{
    [Tooltip("장애물 프리팹")]
    [SerializeField] private GameObject _prefab;

    [Tooltip("장애물 유형 (점프/슬라이딩/복합)")]
    [SerializeField] private ObstacleType _obstacleType;

    [Tooltip("이 장애물이 등장 가능한 최소 난이도 시간(초)")]
    [SerializeField] private float _minAppearTime = 0f;

    [Tooltip("스폰 Y 오프셋 (지면 기준 높이 조정)")]
    [SerializeField] private float _spawnYOffset = 0f;

    public GameObject Prefab => _prefab;
    public ObstacleType ObstacleType => _obstacleType;
    public float MinAppearTime => _minAppearTime;
    public float SpawnYOffset => _spawnYOffset;
}
