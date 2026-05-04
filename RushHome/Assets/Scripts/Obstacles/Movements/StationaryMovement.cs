using UnityEngine;

/// <summary>
/// 정지 — 제자리에 고정된 장애물
/// </summary>
public class StationaryMovement : MonoBehaviour, IObstacleMovement
{
    public void UpdateMovement(Transform obstacleTransform, Transform playerTransform, float deltaTime)
    {
        // 움직이지 않음
    }
}
