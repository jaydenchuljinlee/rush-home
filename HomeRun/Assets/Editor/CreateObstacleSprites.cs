using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateObstacleSprites
{
    public static void Execute()
    {
        CreateSprite("Assets/Art/Sprites/GroundObstacle.png", 32, 32, new Color(0.6f, 0.35f, 0.15f)); // 갈색 상자
        CreateSprite("Assets/Art/Sprites/AirObstacle.png", 32, 16, new Color(0.8f, 0.2f, 0.2f));      // 빨간 장벽

        AssetDatabase.Refresh();
        Debug.Log("[CreateObstacleSprites] GroundObstacle, AirObstacle 스프라이트 생성 완료");
    }

    private static void CreateSprite(string path, int width, int height, Color color)
    {
        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 테두리 1px 어둡게
                bool border = x == 0 || x == width - 1 || y == 0 || y == height - 1;
                pixels[y * width + x] = border ? color * 0.6f : color;
                pixels[y * width + x].a = 1f;
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        tex.filterMode = FilterMode.Point;

        string dir = Path.GetDirectoryName(path);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);

        AssetDatabase.ImportAsset(path);
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 32;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }
}
