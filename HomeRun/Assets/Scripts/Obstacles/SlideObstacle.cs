/// <summary>
/// 슬라이딩 장애물. 플레이어가 슬라이딩으로 통과해야 하는 상단 배치 장애물.
/// ObstacleType.Slide로 분류된다.
/// 스폰 시 ObstacleSpawner 또는 ChunkSpawner가 Initialize를 호출한다.
/// </summary>
public class SlideObstacle : Obstacle
{
    // Obstacle 공통 동작으로 충분.
    // 필요 시 높이 조정 로직 등을 이곳에서 확장한다.
}
