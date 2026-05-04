using System;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyData", menuName = "RushHome/DifficultyData")]
public class DifficultyData : ScriptableObject
{
    public enum DifficultyTier { Easy, Normal, Hard, Extreme }

    [Serializable]
    public struct TierSettings
    {
        public DifficultyTier tier;
        public float startTime;
        public float scrollSpeed;
        public float spawnIntervalMin;
        public float spawnIntervalMax;
    }

    public TierSettings[] tiers = new TierSettings[]
    {
        new() { tier = DifficultyTier.Easy,    startTime = 0f,   scrollSpeed = 8f,  spawnIntervalMin = 1.5f, spawnIntervalMax = 3.0f },
        new() { tier = DifficultyTier.Normal,  startTime = 30f,  scrollSpeed = 12f, spawnIntervalMin = 1.2f, spawnIntervalMax = 2.5f },
        new() { tier = DifficultyTier.Hard,    startTime = 60f,  scrollSpeed = 16f, spawnIntervalMin = 0.9f, spawnIntervalMax = 2.0f },
        new() { tier = DifficultyTier.Extreme, startTime = 120f, scrollSpeed = 22f, spawnIntervalMin = 0.6f, spawnIntervalMax = 1.5f },
    };

    public TierSettings GetSettingsForTime(float time)
    {
        TierSettings result = tiers[0];
        for (int i = tiers.Length - 1; i >= 0; i--)
        {
            if (time >= tiers[i].startTime)
            {
                result = tiers[i];
                break;
            }
        }
        return result;
    }

    public float GetSpeedForTime(float time)
    {
        // 구간 내 선형 보간으로 부드러운 속도 증가
        for (int i = tiers.Length - 1; i >= 0; i--)
        {
            if (time >= tiers[i].startTime)
            {
                if (i < tiers.Length - 1)
                {
                    float t = (time - tiers[i].startTime) / (tiers[i + 1].startTime - tiers[i].startTime);
                    return Mathf.Lerp(tiers[i].scrollSpeed, tiers[i + 1].scrollSpeed, t);
                }
                return tiers[i].scrollSpeed;
            }
        }
        return tiers[0].scrollSpeed;
    }
}
