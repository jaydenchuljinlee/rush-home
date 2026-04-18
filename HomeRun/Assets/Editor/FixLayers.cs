#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class FixLayers
{
    public static void Execute()
    {
        // Layer 8 = Ground, Layer 9 = Player
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset"));
        SerializedProperty layers = tagManager.FindProperty("layers");

        // Layer 8 -> Ground
        if (layers.arraySize > 8)
        {
            layers.GetArrayElementAtIndex(8).stringValue = "Ground";
        }

        // Layer 9 -> Player
        if (layers.arraySize > 9)
        {
            layers.GetArrayElementAtIndex(9).stringValue = "Player";
        }

        tagManager.ApplyModifiedProperties();
        Debug.Log("[FixLayers] Layer 8=Ground, Layer 9=Player 설정 완료");
    }
}
#endif
