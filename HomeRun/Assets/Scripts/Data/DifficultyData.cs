using UnityEngine;

/// <summary>
/// 난이도 단계별 속도 및 장애물 스폰 간격 설정 데이터.
/// [CreateAssetMenu]로 에디터에서 에셋 인스턴스를 생성할 수 있다.
/// 런타임에 수정하지 않는 읽기 전용 데이터.
///
/// 난이도 단계:
/// Easy    : 0 ~ 30초   (속도 6,  스폰 1.5 ~ 3.0초)
/// Normal  : 30 ~ 60초  (속도 8,  스폰 1.2 ~ 2.5초)
/// Hard    : 60 ~ 120초 (속도 11, 스폰 0.9 ~ 2.0초)
/// Extreme : 120초 이상 (속도 15, 스폰 0.6 ~ 1.5초)
/// </summary>
[CreateAssetMenu(fileName = "DifficultyData", menuName = "HomeRun/Difficulty Data")]
public class DifficultyData : ScriptableObject
{
    [Header("난이도 단계별 스크롤 속도")]
    [Tooltip("Easy: 게임 시작 ~ 30초")]
    [SerializeField] private float easySpeed = 6f;

    [Tooltip("Normal: 30초 ~ 60초")]
    [SerializeField] private float normalSpeed = 8f;

    [Tooltip("Hard: 60초 ~ 120초")]
    [SerializeField] private float hardSpeed = 11f;

    [Tooltip("Extreme: 120초 이후")]
    [SerializeField] private float extremeSpeed = 15f;

    [Header("난이도 전환 시간 (초)")]
    [SerializeField] private float normalThreshold = 30f;
    [SerializeField] private float hardThreshold = 60f;
    [SerializeField] private float extremeThreshold = 120f;

    [Header("단계별 장애물 스폰 간격 (초)")]
    [SerializeField] private float easySpawnMin = 1.5f;
    [SerializeField] private float easySpawnMax = 3.0f;

    [SerializeField] private float normalSpawnMin = 1.2f;
    [SerializeField] private float normalSpawnMax = 2.5f;

    [SerializeField] private float hardSpawnMin = 0.9f;
    [SerializeField] private float hardSpawnMax = 2.0f;

    [SerializeField] private float extremeSpawnMin = 0.6f;
    [SerializeField] private float extremeSpawnMax = 1.5f;

    // --- 속도 프로퍼티 ---

    public float EasySpeed => easySpeed;
    public float NormalSpeed => normalSpeed;
    public float HardSpeed => hardSpeed;
    public float ExtremeSpeed => extremeSpeed;

    public float NormalThreshold => normalThreshold;
    public float HardThreshold => hardThreshold;
    public float ExtremeThreshold => extremeThreshold;

    // --- 스폰 간격 프로퍼티 ---

    public float EasySpawnMin => easySpawnMin;
    public float EasySpawnMax => easySpawnMax;
    public float NormalSpawnMin => normalSpawnMin;
    public float NormalSpawnMax => normalSpawnMax;
    public float HardSpawnMin => hardSpawnMin;
    public float HardSpawnMax => hardSpawnMax;
    public float ExtremeSpawnMin => extremeSpawnMin;
    public float ExtremeSpawnMax => extremeSpawnMax;

    /// <summary>
    /// 경과 시간에 따른 스크롤 속도를 반환한다.
    /// </summary>
    public float GetSpeedForTime(float elapsedTime)
    {
        if (elapsedTime >= extremeThreshold) return extremeSpeed;
        if (elapsedTime >= hardThreshold) return hardSpeed;
        if (elapsedTime >= normalThreshold) return normalSpeed;
        return easySpeed;
    }

    /// <summary>
    /// 경과 시간에 따른 장애물 스폰 간격 (min, max)을 반환한다.
    /// </summary>
    public (float min, float max) GetSpawnIntervalForTime(float elapsedTime)
    {
        if (elapsedTime >= extremeThreshold) return (extremeSpawnMin, extremeSpawnMax);
        if (elapsedTime >= hardThreshold) return (hardSpawnMin, hardSpawnMax);
        if (elapsedTime >= normalThreshold) return (normalSpawnMin, normalSpawnMax);
        return (easySpawnMin, easySpawnMax);
    }
}
