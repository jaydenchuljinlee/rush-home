using UnityEngine;

/// <summary>
/// 넉백 효과 — 충돌 시 플레이어를 뒤로 밀어낸다
/// </summary>
public class KnockbackEffect : MonoBehaviour, IObstacleEffect
{
    [SerializeField] float knockbackForce = 5f;

    public void ApplyEffect(PlayerController player)
    {
        player.ApplyKnockback(knockbackForce);
    }
}
