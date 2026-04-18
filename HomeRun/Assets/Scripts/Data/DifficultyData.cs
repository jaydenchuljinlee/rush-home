using UnityEngine;

/// <summary>
/// 난이도 단계별 속도 설정 데이터.
/// [CreateAssetMenu]로 에디터에서 에셋 인스턴스를 생성할 수 있다.
/// 런타임에 수정하지 않는 읽기 전용 데이터.
/// </summary>
[CreateAssetMenu(fileName = "DifficultyData", menuName = "HomeRun/Difficulty Data")]
public class DifficultyData : ScriptableObject
{
    [Header("난이도 단계별 스크롤 속도")]
    [Tooltip("Easy: 게임 시작 ~ 30초")]
    [SerializeField] private float easySpeed = 6f;

    [Tooltip("Normal: 30초 ~ 60초")]
    [SerializeField] private float normalSpeed = 8f;

    [Tooltip("Hard: 60초 ~ 90초")]
    [SerializeField] private float hardSpeed = 11f;

    [Tooltip("Extreme: 90초 이후")]
    [SerializeField] private float extremeSpeed = 15f;

    [Header("난이도 전환 시간 (초)")]
    [SerializeField] private float normalThreshold = 30f;
    [SerializeField] private float hardThreshold = 60f;
    [SerializeField] private float extremeThreshold = 90f;

    public float EasySpeed => easySpeed;
    public float NormalSpeed => normalSpeed;
    public float HardSpeed => hardSpeed;
    public float ExtremeSpeed => extremeSpeed;

    public float NormalThreshold => normalThreshold;
    public float HardThreshold => hardThreshold;
    public float ExtremeThreshold => extremeThreshold;

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
}
