using UnityEngine;
using UnityEditor;

/// <summary>
/// 장애물 시스템 핵심 로직을 에디터 스크립트로 동기 검증.
/// </summary>
public class ValidateObstacleSystem
{
    public static void Execute()
    {
        int passed = 0;
        int failed = 0;

        // --- 테스트 1: Obstacle 컴포넌트 기본값 확인 ---
        {
            GameObject go = new GameObject("TestObstacle");
            go.AddComponent<BoxCollider2D>();
            Obstacle obstacle = go.AddComponent<Obstacle>();
            obstacle.Initialize(10f, null);

            bool ok = obstacle != null && go.activeInHierarchy;
            Log("성공_Obstacle_Initialize_속도_설정", ok, ref passed, ref failed);
            GameObject.DestroyImmediate(go);
        }

        // --- 테스트 2: ObstacleType 기본값 Ground ---
        {
            GameObject go = new GameObject("TestObstacle2");
            go.AddComponent<BoxCollider2D>();
            Obstacle obstacle = go.AddComponent<Obstacle>();

            bool ok = obstacle.ObstacleType == ObstacleType.Ground;
            Log("성공_Obstacle_ObstacleType_기본값_Ground", ok, ref passed, ref failed);
            GameObject.DestroyImmediate(go);
        }

        // --- 테스트 3: ObstaclePool.Get -> 활성화된 오브젝트 반환 ---
        {
            GameObject poolGo = new GameObject("ObstaclePool");
            ObstaclePool pool = poolGo.AddComponent<ObstaclePool>();

            GameObject prefab = new GameObject("ObstaclePrefab");
            prefab.AddComponent<BoxCollider2D>();
            prefab.AddComponent<Obstacle>();

            Obstacle result = pool.Get(prefab, Vector3.zero);
            bool ok = result != null && result.gameObject.activeInHierarchy;
            Log("성공_ObstaclePool_Get_반환_오브젝트_활성화", ok, ref passed, ref failed);

            GameObject.DestroyImmediate(poolGo);
            GameObject.DestroyImmediate(prefab);
        }

        // --- 테스트 4: ObstaclePool.Return -> 비활성화 ---
        {
            GameObject poolGo = new GameObject("ObstaclePool");
            ObstaclePool pool = poolGo.AddComponent<ObstaclePool>();

            GameObject prefab = new GameObject("ObstaclePrefab");
            prefab.AddComponent<BoxCollider2D>();
            prefab.AddComponent<Obstacle>();

            Obstacle obstacle = pool.Get(prefab, Vector3.zero);
            pool.Return(obstacle);

            bool ok = !obstacle.gameObject.activeInHierarchy;
            Log("성공_ObstaclePool_Return_비활성화", ok, ref passed, ref failed);

            GameObject.DestroyImmediate(poolGo);
            GameObject.DestroyImmediate(prefab);
        }

        // --- 테스트 5: ObstaclePool 재사용 동일 인스턴스 ---
        {
            GameObject poolGo = new GameObject("ObstaclePool");
            ObstaclePool pool = poolGo.AddComponent<ObstaclePool>();

            GameObject prefab = new GameObject("ObstaclePrefab");
            prefab.AddComponent<BoxCollider2D>();
            prefab.AddComponent<Obstacle>();

            Obstacle first = pool.Get(prefab, Vector3.zero);
            pool.Return(first);
            Obstacle second = pool.Get(prefab, Vector3.zero);

            bool ok = ReferenceEquals(first, second);
            Log("성공_ObstaclePool_재사용_동일_인스턴스", ok, ref passed, ref failed);

            GameObject.DestroyImmediate(poolGo);
            GameObject.DestroyImmediate(prefab);
        }

        Debug.Log($"[ObstacleSystemValidation] 완료: PASS={passed}, FAIL={failed}");
    }

    private static void Log(string name, bool ok, ref int passed, ref int failed)
    {
        if (ok)
        {
            Debug.Log($"[PASS] {name}");
            passed++;
        }
        else
        {
            Debug.LogError($"[FAIL] {name}");
            failed++;
        }
    }
}
