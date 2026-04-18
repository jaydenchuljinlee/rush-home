using UnityEditor;
using UnityEngine;

/// <summary>
/// F-01 씬 기초 세팅 자동화 에디터 스크립트.
/// Unity 에디터 시작 시 DifficultyData.asset이 없으면 자동 생성한다.
/// </summary>
[InitializeOnLoad]
public static class F01SceneSetupEditor
{
    static F01SceneSetupEditor()
    {
        EditorApplication.delayCall += EnsureDifficultyDataAsset;
    }

    [MenuItem("HomeRun/Setup/Create DifficultyData Asset")]
    public static void EnsureDifficultyDataAsset()
    {
        const string assetPath = "Assets/Data/DifficultyData.asset";

        // 이미 존재하면 스킵
        if (AssetDatabase.LoadAssetAtPath<DifficultyData>(assetPath) != null)
            return;

        // Assets/Data 폴더 확인
        if (!AssetDatabase.IsValidFolder("Assets/Data"))
        {
            AssetDatabase.CreateFolder("Assets", "Data");
        }

        DifficultyData asset = ScriptableObject.CreateInstance<DifficultyData>();
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("[F01Setup] DifficultyData.asset 생성 완료: " + assetPath);
    }

    [MenuItem("HomeRun/Setup/Assign DifficultyData to DifficultyManager in GameScene")]
    public static void AssignDifficultyDataToScene()
    {
        const string assetPath = "Assets/Data/DifficultyData.asset";
        DifficultyData data = AssetDatabase.LoadAssetAtPath<DifficultyData>(assetPath);

        if (data == null)
        {
            Debug.LogWarning("[F01Setup] DifficultyData.asset이 없습니다. 먼저 Create DifficultyData Asset을 실행하세요.");
            return;
        }

        DifficultyManager manager = Object.FindAnyObjectByType<DifficultyManager>();
        if (manager == null)
        {
            Debug.LogWarning("[F01Setup] 씬에서 DifficultyManager를 찾을 수 없습니다. GameScene이 열려 있는지 확인하세요.");
            return;
        }

        SerializedObject so = new SerializedObject(manager);
        SerializedProperty prop = so.FindProperty("difficultyData");
        prop.objectReferenceValue = data;
        so.ApplyModifiedProperties();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
        Debug.Log("[F01Setup] DifficultyManager에 DifficultyData 할당 완료.");
    }
}
