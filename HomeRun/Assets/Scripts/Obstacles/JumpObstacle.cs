/// <summary>
/// 점프 장애물. 플레이어가 점프로 회피해야 하는 바닥 배치 장애물.
/// ObstacleType.Jump로 분류된다.
/// 스폰 시 ObstacleSpawner 또는 ChunkSpawner가 Initialize를 호출한다.
/// </summary>
public class JumpObstacle : Obstacle
{
    // Obstacle 공통 동작으로 충분.
    // 필요 시 애니메이션, 특수 효과 등을 이곳에서 확장한다.
}
