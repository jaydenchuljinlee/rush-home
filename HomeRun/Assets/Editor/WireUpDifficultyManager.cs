using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class WireUpDifficultyManager
{
    public static void Execute()
    {
        // Find DifficultyManager GameObject
        GameObject dmObj = GameObject.Find("DifficultyManager");
        if (dmObj == null)
        {
            Debug.LogError("[WireUpDifficultyManager] DifficultyManager GameObject not found in scene.");
            return;
        }

        // Add DifficultyManager component if missing
        DifficultyManager dm = dmObj.GetComponent<DifficultyManager>();
        if (dm == null)
        {
            dm = dmObj.AddComponent<DifficultyManager>();
            Debug.Log("[WireUpDifficultyManager] Added DifficultyManager component.");
        }

        // Load DifficultyData SO
        DifficultyData diffData = AssetDatabase.LoadAssetAtPath<DifficultyData>("Assets/Data/DifficultyData.asset");
        if (diffData == null)
        {
            Debug.LogError("[WireUpDifficultyManager] DifficultyData.asset not found.");
            return;
        }

        // Find GroundScroller GameObject
        GameObject groundScrollerObj = GameObject.Find("GroundScroller");
        if (groundScrollerObj == null)
        {
            Debug.LogError("[WireUpDifficultyManager] GroundScroller GameObject not found in scene.");
            return;
        }

        GroundScroller gs = groundScrollerObj.GetComponent<GroundScroller>();
        if (gs == null)
        {
            Debug.LogError("[WireUpDifficultyManager] GroundScroller component not found on GroundScroller GameObject.");
            return;
        }

        // Wire up via SerializedObject
        SerializedObject so = new SerializedObject(dm);
        so.FindProperty("difficultyData").objectReferenceValue = diffData;
        so.FindProperty("groundScroller").objectReferenceValue = gs;
        so.ApplyModifiedProperties();

        // Mark scene dirty
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[WireUpDifficultyManager] DifficultyManager wired up: difficultyData + groundScroller assigned.");
    }
}
