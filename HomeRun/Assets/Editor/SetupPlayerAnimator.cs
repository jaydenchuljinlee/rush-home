using UnityEngine;
using UnityEditor;

public class SetupPlayerAnimator
{
    public static void Execute()
    {
        string prefabPath = "Assets/Prefabs/Player/Player.prefab";
        string controllerPath = "Assets/Animations/Player/PlayerAnimatorController.controller";

        // 프리팹 로드
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) { Debug.LogError("Player prefab not found: " + prefabPath); return; }

        var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
        if (controller == null) { Debug.LogError("Controller not found: " + controllerPath); return; }

        // 프리팹 편집 컨텍스트 열기
        using (var scope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var root = scope.prefabContentsRoot;

            // Animator 추가 또는 가져오기
            var animator = root.GetComponent<Animator>();
            if (animator == null)
                animator = root.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            Debug.Log("Animator controller assigned.");

            // PlayerAnimationController 추가
            var pac = root.GetComponent<PlayerAnimationController>();
            if (pac == null)
            {
                pac = root.AddComponent<PlayerAnimationController>();
                Debug.Log("PlayerAnimationController added to prefab.");
            }
            else
            {
                Debug.Log("PlayerAnimationController already exists on prefab.");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Prefab saved successfully.");
    }
}
