#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class FixPlayerGroundLayer
{
    public static void Execute()
    {
        var player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("[FixPlayer] Player 오브젝트를 찾을 수 없습니다.");
            return;
        }

        var controller = player.GetComponent<PlayerController>();
        if (controller == null)
        {
            Debug.LogError("[FixPlayer] PlayerController 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        // groundLayer를 Layer 8 (Ground)의 비트마스크로 설정
        SerializedObject so = new SerializedObject(controller);
        SerializedProperty groundLayerProp = so.FindProperty("groundLayer");

        // LayerMask는 비트마스크: Layer 8 = 1 << 8 = 256
        groundLayerProp.intValue = 1 << 8;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(player.scene);
        EditorSceneManager.SaveScene(player.scene);

        Debug.Log($"[FixPlayer] groundLayer를 Ground(Layer 8, mask={1 << 8})로 설정 완료");
    }
}
#endif
