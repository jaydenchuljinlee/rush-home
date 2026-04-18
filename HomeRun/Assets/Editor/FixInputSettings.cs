#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class FixInputSettings
{
    public static void Execute()
    {
        // Active Input Handling을 "Both"로 설정 (Old + New 모두 사용 가능)
        PlayerSettings.SetPropertyString("activeInputHandler", "2", BuildTargetGroup.Unknown);
        Debug.Log("[FixInputSettings] Active Input Handling을 'Both'로 변경했습니다. 에디터 재시작이 필요할 수 있습니다.");
    }
}
#endif
