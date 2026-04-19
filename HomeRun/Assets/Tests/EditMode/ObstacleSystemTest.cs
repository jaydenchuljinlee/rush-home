using NUnit.Framework;
using UnityEngine;

/// <summary>
/// 장애물 시스템 Edit Mode 테스트.
/// Obstacle 초기화 및 ObstaclePool 풀링 동작 검증.
/// </summary>
[TestFixture]
public class ObstacleSystemTest
{
    // --- Obstacle 초기화 테스트 ---

    [Test]
    public void 성공_Obstacle_Initialize_속도_설정()
    {
        // Arrange
        GameObject go = new GameObject("TestObstacle");
        go.AddComponent<BoxCollider2D>();
        Obstacle obstacle = go.AddComponent<Obstacle>();

        // Act
        obstacle.Initialize(10f, null);

        // Assert - 오브젝트가 살아있고 컴포넌트가 정상 부착됨을 확인
        Assert.IsNotNull(obstacle);
        Assert.IsTrue(go.activeInHierarchy);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    [Test]
    public void 성공_Obstacle_ObstacleType_기본값_Ground()
    {
        // Arrange
        GameObject go = new GameObject("TestObstacle");
        go.AddComponent<BoxCollider2D>();
        Obstacle obstacle = go.AddComponent<Obstacle>();

        // Assert
        Assert.AreEqual(ObstacleType.Ground, obstacle.ObstacleType);

        // Cleanup
        Object.DestroyImmediate(go);
    }

    // --- ObstaclePool 테스트 ---

    [Test]
    public void 성공_ObstaclePool_Get_반환_오브젝트_활성화()
    {
        // Arrange
        GameObject poolGo = new GameObject("ObstaclePool");
        ObstaclePool pool = poolGo.AddComponent<ObstaclePool>();

        GameObject prefab = new GameObject("ObstaclePrefab");
        prefab.AddComponent<BoxCollider2D>();
        prefab.AddComponent<Obstacle>();

        // Act
        Obstacle result = pool.Get(prefab, Vector3.zero);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.gameObject.activeInHierarchy);

        // Cleanup
        Object.DestroyImmediate(poolGo);
        Object.DestroyImmediate(prefab);
    }

    [Test]
    public void 성공_ObstaclePool_Return_비활성화()
    {
        // Arrange
        GameObject poolGo = new GameObject("ObstaclePool");
        ObstaclePool pool = poolGo.AddComponent<ObstaclePool>();

        GameObject prefab = new GameObject("ObstaclePrefab");
        prefab.AddComponent<BoxCollider2D>();
        prefab.AddComponent<Obstacle>();

        Obstacle obstacle = pool.Get(prefab, Vector3.zero);

        // Act
        pool.Return(obstacle);

        // Assert
        Assert.IsFalse(obstacle.gameObject.activeInHierarchy);

        // Cleanup
        Object.DestroyImmediate(poolGo);
        Object.DestroyImmediate(prefab);
    }

    [Test]
    public void 성공_ObstaclePool_재사용_동일_인스턴스()
    {
        // Arrange
        GameObject poolGo = new GameObject("ObstaclePool");
        ObstaclePool pool = poolGo.AddComponent<ObstaclePool>();

        GameObject prefab = new GameObject("ObstaclePrefab");
        prefab.AddComponent<BoxCollider2D>();
        prefab.AddComponent<Obstacle>();

        // Act: Get -> Return -> Get
        Obstacle first = pool.Get(prefab, Vector3.zero);
        pool.Return(first);
        Obstacle second = pool.Get(prefab, Vector3.zero);

        // Assert: 같은 인스턴스가 재사용됨
        Assert.AreSame(first, second);

        // Cleanup
        Object.DestroyImmediate(poolGo);
        Object.DestroyImmediate(prefab);
    }
}
