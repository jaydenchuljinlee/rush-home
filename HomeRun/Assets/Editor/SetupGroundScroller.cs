using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SetupGroundScroller
{
    public static void Execute()
    {
        // Load the ground tile sprite
        string spritePath = "Assets/Art/Sprites/GroundTile.png";
        Sprite groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (groundSprite == null)
        {
            Debug.LogError("[SetupGroundScroller] GroundTile sprite not found at: " + spritePath);
            return;
        }

        // Sprite world width in Unity units: pixels / pixelsPerUnit
        // GroundTile.png is 512px wide, pixelsPerUnit = 32 -> 16 units wide
        float tileWorldWidth = groundSprite.texture.width / groundSprite.pixelsPerUnit;
        Debug.Log($"[SetupGroundScroller] Sprite world width: {tileWorldWidth} units");

        // Create parent GroundScroller GameObject
        GameObject scrollerObj = new GameObject("GroundScroller");

        // Add GroundScroller component
        var scrollerScript = scrollerObj.AddComponent<GroundScroller>();

        // Set tileWidth via SerializedObject to respect [SerializeField]
        SerializedObject so = new SerializedObject(scrollerScript);
        so.FindProperty("tileWidth").floatValue = tileWorldWidth;
        so.FindProperty("scrollSpeed").floatValue = 8f;
        so.ApplyModifiedProperties();

        // Ground Y position: place ground so top surface is at y=0
        // Sprite pivot is center (0.5, 0.5), height = 64px / 32ppu = 2 units
        float tileWorldHeight = groundSprite.texture.height / groundSprite.pixelsPerUnit;
        float groundY = -tileWorldHeight / 2f; // top edge at y=0

        // Create 3 ground tiles as children
        for (int i = 0; i < 3; i++)
        {
            GameObject tile = new GameObject($"GroundTile_{i}");
            tile.transform.SetParent(scrollerObj.transform);

            // Position tiles side by side starting at x=0
            float xPos = i * tileWorldWidth;
            tile.transform.position = new Vector3(xPos, groundY, 0f);

            // SpriteRenderer
            SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
            sr.sprite = groundSprite;
            sr.sortingOrder = 0;

            // BoxCollider2D (non-trigger)
            BoxCollider2D col = tile.AddComponent<BoxCollider2D>();
            col.isTrigger = false;

            // Set layer to Ground (layer 8)
            tile.layer = 8;
        }

        // Mark scene dirty and save
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log($"[SetupGroundScroller] GroundScroller created with 3 tiles. tileWidth={tileWorldWidth}, groundY={groundY}");
    }
}
