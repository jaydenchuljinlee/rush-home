using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class SetupObstacleVisuals
{
    public static void Execute()
    {
        // Step 1: 텍스처 생성
        CreateTexture("Assets/Art/Sprites/GroundObstacle.png", 32, 32, new Color(0.6f, 0.35f, 0.15f));
        CreateTexture("Assets/Art/Sprites/AirObstacle.png", 32, 16, new Color(0.8f, 0.2f, 0.2f));

        // Step 2: 임포트 설정
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        SetSpriteImport("Assets/Art/Sprites/GroundObstacle.png");
        SetSpriteImport("Assets/Art/Sprites/AirObstacle.png");

        // Step 3: 스프라이트 로드
        var groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/GroundObstacle.png");
        var airSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/AirObstacle.png");

        Debug.Log($"[Setup] ground={groundSprite != null}, air={airSprite != null}");

        // Step 4: 씬 오브젝트에 할당
        AssignToSceneObject("GroundObstacle", groundSprite, new Vector2(1f, 1f));
        AssignToSceneObject("AirObstacle", airSprite, new Vector2(1f, 0.5f));

        // Step 5: 프리팹 업데이트
        UpdatePrefab("Assets/Prefabs/Obstacles/GroundObstacle.prefab", groundSprite, new Vector2(1f, 1f));
        UpdatePrefab("Assets/Prefabs/Obstacles/AirObstacle.prefab", airSprite, new Vector2(1f, 0.5f));

        // Step 6: 씬 저장
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("[SetupObstacleVisuals] 완료");
    }

    static void CreateTexture(string path, int w, int h, Color color)
    {
        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            bool border = x == 0 || x == w - 1 || y == 0 || y == h - 1;
            var c = border ? color * 0.6f : color;
            c.a = 1f;
            tex.SetPixel(x, y, c);
        }
        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
    }

    static void SetSpriteImport(string path)
    {
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) { Debug.LogError($"No importer: {path}"); return; }
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 32;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }

    static void AssignToSceneObject(string name, Sprite sprite, Vector2 colSize)
    {
        var obj = GameObject.Find(name);
        if (obj == null) { Debug.LogWarning($"Not found: {name}"); return; }
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
        Debug.Log($"[Setup] Prefab updated: {path}");
    }
}
