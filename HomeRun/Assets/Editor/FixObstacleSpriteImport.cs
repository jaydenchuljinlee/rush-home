using UnityEngine;
using UnityEditor;

public class FixObstacleSpriteImport
{
    public static void Execute()
    {
        FixImport("Assets/Art/Sprites/GroundObstacle.png");
        FixImport("Assets/Art/Sprites/AirObstacle.png");
        Debug.Log("[FixObstacleSpriteImport] 스프라이트 임포트 설정 완료");
    }

    private static void FixImport(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 32;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
        else
        {
            Debug.LogError($"[FixObstacleSpriteImport] TextureImporter not found: {path}");
        }
    }
}
