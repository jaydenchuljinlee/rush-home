using UnityEngine;

/// <summary>
/// 차단 효과 — 물리적으로 막기만 하고 게임오버나 감속 없음
/// </summary>
public class BlockEffect : MonoBehaviour, IObstacleEffect
{
    public void ApplyEffect(PlayerController player)
    {
        // 물리 충돌로 자연스럽게 막힘, 추가 효과 없음
    }
}
