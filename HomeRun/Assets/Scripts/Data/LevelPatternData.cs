using UnityEngine;

/// <summary>
/// 장애물이 등장할 수 있는 최소 난이도 티어.
/// </summary>
public enum DifficultyTier
{
    Easy = 0,
    Normal = 1,
    Hard = 2,
    Extreme = 3
}

/// <summary>
/// 패턴 내 개별 장애물 항목.
/// xOffset: 패턴 기준 X 상대 오프셋 (0이면 기본 스폰 X에서 시작).
/// yOverride: -999f이면 Obstacle 타입 기본값(Ground/Air)을 사용, 그 외 직접 Y 좌표 지정.
/// </summary>
[System.Serializable]
public struct ObstacleEntry
{
    [Tooltip("사용할 장애물 프리팹")]
    public GameObject prefab;

    [Tooltip("패턴 기준 X 상대 오프셋. 0이면 기본 스폰 X 위치.")]
    public float xOffset;

    [Tooltip("Y 위치 직접 지정. -999f이면 ObstacleType(Ground/Air) 기본값 사용.")]
    public float yOverride;

    /// <summary>yOverride가 설정되지 않은 경우를 나타내는 마커 값.</summary>
    public const float UseDefaultY = -999f;

    /// <summary>이 항목이 기본 Y를 사용해야 하는지 여부.</summary>
    public bool UsesDefaultY => Mathf.Approximately(yOverride, UseDefaultY);
}

/// <summary>
/// 장애물 조합 패턴 ScriptableObject.
/// 하나의 패턴은 여러 장애물로 구성되며, 최소 난이도 티어를 지정한다.
/// DifficultyData를 통해 난이도별 패턴 풀을 구성하고 PatternSpawner에 전달된다.
/// </summary>
[CreateAssetMenu(fileName = "LevelPattern", menuName = "HomeRun/Level Pattern Data")]
public class LevelPatternData : ScriptableObject
{
    [Header("패턴 정보")]
    [Tooltip("패턴 식별 이름 (에디터 표시용)")]
    [SerializeField] private string patternName = "NewPattern";

    [Tooltip("이 패턴이 등장할 수 있는 최소 난이도 티어")]
    [SerializeField] private DifficultyTier minDifficulty = DifficultyTier.Easy;

    [Header("패턴 구성 장애물")]
    [Tooltip("패턴을 구성하는 장애물 목록. xOffset 순으로 정렬 권장.")]
    [SerializeField] private ObstacleEntry[] obstacles = new ObstacleEntry[0];

    public string PatternName => patternName;
    public DifficultyTier MinDifficulty => minDifficulty;
    public ObstacleEntry[] Obstacles => obstacles;
}
