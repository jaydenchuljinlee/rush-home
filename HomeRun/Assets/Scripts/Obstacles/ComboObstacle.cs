/// <summary>
/// 복합 장애물. 점프 + 슬라이딩을 조합하여 회피해야 하는 장애물.
/// ObstacleType.Combo로 분류된다.
/// 스폰 시 ObstacleSpawner 또는 ChunkSpawner가 Initialize를 호출한다.
/// </summary>
public class ComboObstacle : Obstacle
{
    // Obstacle 공통 동작으로 충분.
    // 필요 시 복합 패턴 전용 로직을 이곳에서 확장한다.
}
