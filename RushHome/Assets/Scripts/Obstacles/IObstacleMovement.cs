using UnityEngine;

/// <summary>
/// 장애물의 이동 패턴을 정의한다.
/// 정지, 플레이어 접근, 좌우 왕복 등 다양한 패턴을 구현할 수 있다.
/// </summary>
public interface IObstacleMovement
{
    void UpdateMovement(Transform obstacleTransform, Transform playerTransform, float deltaTime);
}
