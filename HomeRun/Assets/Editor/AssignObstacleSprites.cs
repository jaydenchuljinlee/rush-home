using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class AssignObstacleSprites
{
    public static void Execute()
    {
        // 스프라이트 로드
        var groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/GroundObstacle.png");
        var airSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/AirObstacle.png");

        if (groundSprite == null || airSprite == null)
        {
            Debug.LogError("[AssignObstacleSprites] 스프라이트 로드 실패. 임포트 상태 확인 필요.");
            Debug.Log($"  groundSprite: {(groundSprite != null ? "OK" : "NULL")}");
            Debug.Log($"  airSprite: {(airSprite != null ? "OK" : "NULL")}");
            return;
        }

        // 씬에서 오브젝트 찾기
        var groundObj = GameObject.Find("GroundObstacle");
        var airObj = GameObject.Find("AirObstacle");

        if (groundObj != null)
        {
            var sr = groundObj.GetComponent<SpriteRenderer>();
            sr.sprite = groundSprite;
            var col = groundObj.GetComponent<BoxCollider2D>();
            col.size = new Vector2(1f, 1f);
            col.offset = Vector2.zero;
            Debug.Log("[AssignObstacleSprites] GroundObstacle: sprite + collider 설정 완료");
        }

        if (airObj != null)
        {
            var sr = airObj.GetComponent<SpriteRenderer>();
            sr.sprite = airSprite;
            var col = airObj.GetComponent<BoxCollider2D>();
            col.size = new Vector2(1f, 0.5f);
            col.offset = Vector2.zero;
            Debug.Log("[AssignObstacleSprites] AirObstacle: sprite + collider 설정 완료");
        }

        // 프리팹도 업데이트
        UpdatePrefab("Assets/Prefabs/Obstacles/GroundObstacle.prefab", groundSprite, new Vector2(1f, 1f));
        UpdatePrefab("Assets/Prefabs/Obstacles/AirObstacle.prefab", airSprite, new Vector2(1f, 0.5f));

        // 씬 저장
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[AssignObstacleSprites] 완료 — 씬 저장됨");
    }

    private static void UpdatePrefab(string path, Sprite sprite, Vector2 colliderSize)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) return;

        var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        var sr = instance.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
        var col = instance.GetComponent<BoxCollider2D>();
        col.size = colliderSize;
        col.offset = Vector2.zero;

        PrefabUtility.SaveAsPrefabAsset(instance, path);
        Object.DestroyImmediate(instance);
        Debug.Log($"[AssignObstacleSprites] 프리팹 업데이트: {path}");
    }
}
