using UnityEngine;

/// <summary>
/// 장애물 충돌 시 플레이어에게 적용할 효과를 정의한다.
/// 치명적(GameOver), 감속, 넉백 등 다양한 효과를 구현할 수 있다.
/// </summary>
public interface IObstacleEffect
{
    void ApplyEffect(PlayerController player);
}
