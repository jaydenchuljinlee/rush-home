using UnityEngine;
using UnityEditor;
using System.IO;

public class CreatePlayerSprite
{
    public static void Execute()
    {
        // Art/Sprites 폴더 생성
        string spritesDir = Application.dataPath + "/Art/Sprites";
        if (!Directory.Exists(Application.dataPath + "/Art"))
            Directory.CreateDirectory(Application.dataPath + "/Art");
        if (!Directory.Exists(spritesDir))
            Directory.CreateDirectory(spritesDir);

        int w = 64, h = 128;
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);

        Color transparent  = new Color(0, 0, 0, 0);
        Color skinColor    = new Color(1.0f, 0.85f, 0.70f, 1f);
        Color hairColor    = new Color(0.15f, 0.10f, 0.05f, 1f);
        Color shirtColor   = new Color(0.95f, 0.95f, 0.95f, 1f);
        Color tieColor     = new Color(0.7f, 0.1f, 0.1f, 1f);
        Color pantsColor   = new Color(0.15f, 0.15f, 0.25f, 1f);
        Color shoeColor    = new Color(0.1f, 0.08f, 0.05f, 1f);
        Color outlineColor = new Color(0.05f, 0.05f, 0.05f, 1f);

        // 전체 투명 초기화
        for (int x = 0; x < w; x++)
            for (int y = 0; y < h; y++)
                tex.SetPixel(x, y, transparent);

        // 신발 (y: 0~7)
        FillRect(tex, w, h, 16, 0, 47, 7, shoeColor);
        FillRect(tex, w, h, 31, 0, 32, 7, transparent);

        // 바지 (y: 8~52)
        FillRect(tex, w, h, 18, 8, 45, 52, pantsColor);
        FillRect(tex, w, h, 31, 8, 32, 45, outlineColor);

        // 셔츠 몸통 (y: 53~90)
        FillRect(tex, w, h, 14, 53, 49, 90, shirtColor);
        FillRect(tex, w, h, 29, 60, 34, 85, tieColor);

        // 팔 (y: 53~80)
        FillRect(tex, w, h, 7,  53, 13, 80, shirtColor);
        FillRect(tex, w, h, 50, 53, 56, 80, shirtColor);
        FillRect(tex, w, h, 7,  68, 13, 80, skinColor);
        FillRect(tex, w, h, 50, 68, 56, 80, skinColor);

        // 목 (y: 91~96)
        FillRect(tex, w, h, 27, 91, 36, 96, skinColor);

        // 머리 (y: 97~127)
        FillRect(tex, w, h, 19, 97, 44, 127, skinColor);
        FillRect(tex, w, h, 19, 115, 44, 127, hairColor);
        FillRect(tex, w, h, 23, 107, 26, 109, outlineColor);
        FillRect(tex, w, h, 37, 107, 40, 109, outlineColor);
        FillRect(tex, w, h, 27, 101, 36, 102, outlineColor);

        tex.Apply();

        byte[] pngData = tex.EncodeToPNG();
        string savePath = spritesDir + "/Player.png";
        File.WriteAllBytes(savePath, pngData);

        AssetDatabase.Refresh();

        string assetPath = "Assets/Art/Sprites/Player.png";
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 100;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            AssetDatabase.WriteImportSettingsIfDirty(assetPath);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

        Debug.Log("[CreatePlayerSprite] Player sprite created: " + savePath);
    }

    static void FillRect(Texture2D tex, int w, int h, int x0, int y0, int x1, int y1, Color c)
    {
        for (int x = x0; x <= x1; x++)
            for (int y = y0; y <= y1; y++)
                if (x >= 0 && x < w && y >= 0 && y < h)
                    tex.SetPixel(x, y, c);
    }
}
