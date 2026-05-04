using UnityEngine;

/// <summary>
/// 치명적 효과 — 충돌 시 게임오버
/// </summary>
public class LethalEffect : MonoBehaviour, IObstacleEffect
{
    public void ApplyEffect(PlayerController player)
    {
        GameManager.Instance.PlayerHit();
        player.Die();
    }
}
