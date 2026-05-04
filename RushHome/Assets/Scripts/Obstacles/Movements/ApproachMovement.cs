using UnityEngine;

/// <summary>
/// 접근 — 플레이어를 향해 다가오는 장애물
/// </summary>
public class ApproachMovement : MonoBehaviour, IObstacleMovement
{
    [SerializeField] float approachSpeed = 10f;

    public void UpdateMovement(Transform obstacleTransform, Transform playerTransform, float deltaTime)
    {
        // Z축으로 플레이어 방향 접근 (뒤쪽으로)
        Vector3 pos = obstacleTransform.position;
        pos.z -= approachSpeed * deltaTime;
        obstacleTransform.position = pos;
    }
}
