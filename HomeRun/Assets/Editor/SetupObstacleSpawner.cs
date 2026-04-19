using UnityEngine;
using UnityEditor;

/// <summary>
/// ObstacleSpawner에 프리팹 배열과 참조를 설정하는 에디터 스크립트.
/// </summary>
public class SetupObstacleSpawner
{
    public static void Execute()
    {
        // ObstacleSpawner 찾기
        GameObject spawnerGo = GameObject.Find("ObstacleSpawner");
        if (spawnerGo == null)
        {
            Debug.LogError("[SetupObstacleSpawner] ObstacleSpawner 오브젝트를 찾을 수 없습니다.");
            return;
        }

        ObstacleSpawner spawner = spawnerGo.GetComponent<ObstacleSpawner>();
        if (spawner == null)
        {
            Debug.LogError("[SetupObstacleSpawner] ObstacleSpawner 컴포넌트가 없습니다.");
            return;
        }

        // 프리팹 로드
        GameObject groundPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Obstacles/GroundObstacle.prefab");
        GameObject airPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Obstacles/AirObstacle.prefab");

        if (groundPrefab == null || airPrefab == null)
        {
            Debug.LogError($"[SetupObstacleSpawner] 프리팹 로드 실패. Ground={groundPrefab}, Air={airPrefab}");
            return;
        }

        // GroundScroller 참조
        GameObject groundScrollerGo = GameObject.Find("GroundScroller");
        GroundScroller groundScroller = groundScrollerGo != null ? groundScrollerGo.GetComponent<GroundScroller>() : null;

        // ObstaclePool 참조
        GameObject poolGo = GameObject.Find("ObstaclePool");
        ObstaclePool pool = poolGo != null ? poolGo.GetComponent<ObstaclePool>() : null;

        // SerializedObject로 배열 설정
        SerializedObject so = new SerializedObject(spawner);

        SerializedProperty prefabsArray = so.FindProperty("obstaclePrefabs");
        prefabsArray.arraySize = 2;
        prefabsArray.GetArrayElementAtIndex(0).objectReferenceValue = groundPrefab;
        prefabsArray.GetArrayElementAtIndex(1).objectReferenceValue = airPrefab;

        SerializedProperty groundScrollerProp = so.FindProperty("groundScroller");
        groundScrollerProp.objectReferenceValue = groundScroller;

        SerializedProperty poolProp = so.FindProperty("obstaclePool");
        poolProp.objectReferenceValue = pool;

        so.ApplyModifiedProperties();

        Debug.Log($"[SetupObstacleSpawner] 설정 완료: GroundPrefab={groundPrefab.name}, AirPrefab={airPrefab.name}, GroundScroller={groundScroller}, Pool={pool}");
    }
}
