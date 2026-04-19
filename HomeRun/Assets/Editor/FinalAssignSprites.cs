using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class FinalAssignSprites
{
    public static void Execute()
    {
        AssetDatabase.ImportAsset("Assets/Art/Sprites/GroundObstacle.png", ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset("Assets/Art/Sprites/AirObstacle.png", ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        var groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/GroundObstacle.png");
        var airSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/AirObstacle.png");

        Debug.Log($"[Final] ground={groundSprite != null}, air={airSprite != null}");

        if (groundSprite == null || airSprite == null)
        {
            Debug.LogError("[Final] 스프라이트 로드 실패");
            return;
        }

        // 씬 오브젝트
        Assign(GameObject.Find("GroundObstacle"), groundSprite, new Vector2(1f, 1f));
        Assign(GameObject.Find("AirObstacle"), airSprite, new Vector2(1f, 0.5f));

        // 프리팹
        UpdatePrefab("Assets/Prefabs/Obstacles/GroundObstacle.prefab", groundSprite, new Vector2(1f, 1f));
        UpdatePrefab("Assets/Prefabs/Obstacles/AirObstacle.prefab", airSprite, new Vector2(1f, 0.5f));

        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[Final] 완료");
    }

    static void Assign(GameObject obj, Sprite sprite, Vector2 colSize)
    {
        if (obj == null) return;
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
        var col = obj.GetComponent<BoxCollider2D>();
        col.size = colSize;
        col.offset = Vector2.zero;
    }

    static void UpdatePrefab(string path, Sprite sprite, Vector2 colSize)
    {
        using (var scope = new PrefabUtility.EditPrefabContentsScope(path))
        {
            var root = scope.prefabContentsRoot;
            root.GetComponent<SpriteRenderer>().sprite = sprite;
            var col = root.GetComponent<BoxCollider2D>();
            col.size = colSize;
            col.offset = Vector2.zero;
        }
        Debug.Log($"[Final] Prefab: {path}");
    }
}
