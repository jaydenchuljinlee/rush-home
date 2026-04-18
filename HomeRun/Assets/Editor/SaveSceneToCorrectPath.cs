using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SaveSceneToCorrectPath
{
    public static void Execute()
    {
        string correctPath = "Assets/Scenes/GameScene.unity";
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        // Save to correct path
        bool saved = EditorSceneManager.SaveScene(scene, correctPath);
        if (saved)
        {
            Debug.Log($"[SaveSceneToCorrectPath] Scene saved to: {correctPath}");
        }
        else
        {
            Debug.LogError($"[SaveSceneToCorrectPath] Failed to save scene to: {correctPath}");
        }
    }
}
