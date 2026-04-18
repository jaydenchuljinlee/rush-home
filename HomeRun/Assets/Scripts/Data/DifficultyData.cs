using UnityEngine;

/// <summary>
/// 난이도 단계 정의. 시간 구간별로 스크롤 속도와 장애물 밀도를 설정한다.
/// </summary>
[System.Serializable]
public class DifficultyStage
{
    [Tooltip("이 단계가 시작되는 경과 시간(초)")]
    public float startTime;

    [Tooltip("스크롤 속도 (단위/초)")]
    public float scrollSpeed;

    [Tooltip("장애물 스폰 최소 간격(초)")]
    public float spawnIntervalMin;

    [Tooltip("장애물 스폰 최대 간격(초)")]
    public float spawnIntervalMax;

    [Tooltip("청크에서 사용 가능한 장애물 최대 개수")]
    public int maxObstaclesPerChunk;
}

/// <summary>
/// 게임 전체 난이도 곡선을 정의하는 ScriptableObject.
/// 시간 경과에 따라 단계가 올라간다.
/// </summary>
[CreateAssetMenu(fileName = "DifficultyData", menuName = "HomeRun/Difficulty Data")]
public class DifficultyData : ScriptableObject
{
    [SerializeField] private DifficultyStage[] _stages = new DifficultyStage[]
    {
        // 0~30초: 쉬움
        new DifficultyStage { startTime = 0f,   scrollSpeed = 6f,  spawnIntervalMin = 2.5f, spawnIntervalMax = 4.0f, maxObstaclesPerChunk = 2 },
        // 30~60초: 보통
        new DifficultyStage { startTime = 30f,  scrollSpeed = 8f,  spawnIntervalMin = 1.8f, spawnIntervalMax = 3.0f, maxObstaclesPerChunk = 3 },
        // 60~120초: 어려움
        new DifficultyStage { startTime = 60f,  scrollSpeed = 10f, spawnIntervalMin = 1.2f, spawnIntervalMax = 2.0f, maxObstaclesPerChunk = 4 },
        // 120초~: 지옥
        new DifficultyStage { startTime = 120f, scrollSpeed = 13f, spawnIntervalMin = 0.8f, spawnIntervalMax = 1.4f, maxObstaclesPerChunk = 5 },
    };

    /// <summary>경과 시간에 맞는 난이도 단계를 반환한다.</summary>
    public DifficultyStage GetStage(float elapsedTime)
    {
        DifficultyStage result = _stages[0];
        foreach (DifficultyStage stage in _stages)
        {
            if (elapsedTime >= stage.startTime)
                result = stage;
        }
        return result;
    }

    /// <summary>현재 스크롤 속도를 반환한다.</summary>
    public float GetScrollSpeed(float elapsedTime) => GetStage(elapsedTime).scrollSpeed;

    /// <summary>현재 스폰 간격 범위에서 무작위 값을 반환한다.</summary>
    public float GetRandomSpawnInterval(float elapsedTime)
    {
        DifficultyStage stage = GetStage(elapsedTime);
        return Random.Range(stage.spawnIntervalMin, stage.spawnIntervalMax);
    }

    /// <summary>현재 청크당 최대 장애물 수를 반환한다.</summary>
    public int GetMaxObstaclesPerChunk(float elapsedTime) => GetStage(elapsedTime).maxObstaclesPerChunk;

    /// <summary>단계 배열을 읽기 전용으로 노출 (테스트용).</summary>
    public DifficultyStage[] Stages => _stages;
}
