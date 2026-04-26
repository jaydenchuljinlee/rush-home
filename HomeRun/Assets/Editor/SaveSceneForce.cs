using UnityEditor;
using UnityEditor.SceneManagement;

public class SaveSceneForce
{
    public static void Execute()
    {
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
    }
}
