using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SuiteSetup
{
    public static void Execute()
    {
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        UnityEngine.Debug.Log("[Suite] Scene saved");
    }
}
