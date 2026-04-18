using UnityEngine;
using UnityEditor;

public class FixPlayerSpriteImport
{
    public static void Execute()
    {
        string assetPath = "Assets/Art/Sprites/Player.png";
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError("[FixPlayerSpriteImport] Importer not found for: " + assetPath);
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;

        EditorUtility.SetDirty(importer);
        AssetDatabase.WriteImportSettingsIfDirty(assetPath);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);

        Debug.Log("[FixPlayerSpriteImport] Sprite import settings applied to: " + assetPath);
    }
}
