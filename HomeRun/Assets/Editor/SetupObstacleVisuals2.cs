using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SetupObstacleVisuals2
{
    public static void Execute()
    {
        var groundSprite = LoadSprite("Assets/Art/Sprites/GroundObstacle.png");
        var airSprite = LoadSprite("Assets/Art/Sprites/AirObstacle.png");

        Debug.Log($"[Setup2] ground={groundSprite != null}, air={airSprite != null}");

        AssignToSceneObject("GroundObstacle", groundSprite, new Vector2(1f, 1f));
        AssignToSceneObject("AirObstacle", airSprite, new Vector2(1f, 0.5f));

        UpdatePrefab("Assets/Prefabs/Obstacles/GroundObstacle.prefab", groundSprite, new Vector2(1f, 1f));
        UpdatePrefab("Assets/Prefabs/Obstacles/AirObstacle.prefab", airSprite, new Vector2(1f, 0.5f));

        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[SetupObstacleVisuals2] 완료");
    }

    static Sprite LoadSprite(string path)
    {
        // 모든 서브에셋 탐색
        var assets = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (var a in assets)
        {
            Debug.Log($"  asset at {path}: {a.GetType().Name} - {a.name}");
            if (a is Sprite s) return s;
        }

        // 폴백: Texture2D에서 직접 Sprite 생성
        var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (tex != null)
        {
            Debug.Log($"  Texture2D found, creating Sprite from texture: {tex.width}x{tex.height}");
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0f), 32);
        }

        return null;
    }

    static void AssignToSceneObject(string name, Sprite sprite, Vector2 colSize)
    {
        var obj = GameObject.Find(name);
        if (obj == null) return;
        if (sprite != null) obj.GetComponent<SpriteRenderer>().sprite = sprite;
        var col = obj.GetComponent<BoxCollider2D>();
        col.size = colSize;
        col.offset = Vector2.zero;
    }

    static void UpdatePrefab(string path, Sprite sprite, Vector2 colSize)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) return;
        using (var scope = new PrefabUtility.EditPrefabContentsScope(path))
        {
            var root = scope.prefabContentsRoot;
            if (sprite != null) root.GetComponent<SpriteRenderer>().sprite = sprite;
            var col = root.GetComponent<BoxCollider2D>();
            col.size = colSize;
            col.offset = Vector2.zero;
        }
    }
}
