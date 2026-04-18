using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateGroundTileSprite
{
    public static void Execute()
    {
        int width = 512;
        int height = 64;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Base concrete color
        Color baseColor = new Color(0.55f, 0.55f, 0.55f, 1f);
        Color lineColor = new Color(0.35f, 0.35f, 0.35f, 1f);
        Color darkEdge = new Color(0.30f, 0.30f, 0.30f, 1f);
        Color lightTop = new Color(0.72f, 0.72f, 0.72f, 1f);

        // Fill base
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, y, baseColor);
            }
        }

        // Top highlight strip (top 3 pixels)
        for (int x = 0; x < width; x++)
        {
            tex.SetPixel(x, height - 1, lightTop);
            tex.SetPixel(x, height - 2, lightTop);
            tex.SetPixel(x, height - 3, new Color(0.65f, 0.65f, 0.65f, 1f));
        }

        // Bottom dark edge (bottom 3 pixels)
        for (int x = 0; x < width; x++)
        {
            tex.SetPixel(x, 0, darkEdge);
            tex.SetPixel(x, 1, darkEdge);
            tex.SetPixel(x, 2, new Color(0.42f, 0.42f, 0.42f, 1f));
        }

        // Vertical seam lines every 128 pixels (tile block divisions)
        int[] seamPositions = { 128, 256, 384 };
        foreach (int sx in seamPositions)
        {
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(sx, y, lineColor);
                if (sx + 1 < width)
                    tex.SetPixel(sx + 1, y, new Color(0.62f, 0.62f, 0.62f, 1f));
            }
        }

        // Horizontal mid-line (subtle crack)
        int midY = height / 2;
        for (int x = 0; x < width; x++)
        {
            // Only draw crack within each block (not on seams)
            bool onSeam = false;
            foreach (int sx in seamPositions)
                if (x >= sx - 1 && x <= sx + 2) { onSeam = true; break; }
            if (!onSeam)
                tex.SetPixel(x, midY, new Color(0.48f, 0.48f, 0.48f, 1f));
        }

        tex.Apply();

        string savePath = "Assets/Art/Sprites/GroundTile.png";
        byte[] pngData = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Art/Sprites/GroundTile.png", pngData);

        AssetDatabase.ImportAsset(savePath);
        TextureImporter importer = AssetImporter.GetAtPath(savePath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePivot = new Vector2(0.5f, 0.5f);
            importer.spritePixelsPerUnit = 32f;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        Debug.Log($"[CreateGroundTileSprite] GroundTile.png created at {savePath}");
        Object.DestroyImmediate(tex);
    }
}
