using UnityEngine;
using UnityEditor;

/// <summary>
/// 씬의 기존 ParallaxLayer 타일 SpriteRenderer를 "Background" Sorting Layer로 업데이트.
/// bugfix-parallax-behind-ground 수정 스크립트.
/// </summary>
public class FixParallaxSortingLayer
{
    [MenuItem("HomeRun/Fix Parallax Sorting Layer")]
    public static void Execute()
    {
        // Background Sorting Layer ID 확인
        int backgroundLayerID = SortingLayer.NameToID("Background");
        if (backgroundLayerID == 0 && SortingLayer.NameToID("Background") == SortingLayer.NameToID("Default"))
        {
            Debug.LogError("[FixParallax] 'Background' Sorting Layer가 존재하지 않습니다. TagManager.asset을 확인하세요.");
            return;
        }

        // (name, sortingOrder) 쌍 정의 — Sky=0, Far=1, Near=2
        var layerConfig = new (string name, int order)[]
        {
            ("ParallaxLayer_Sky",  0),
            ("ParallaxLayer_Far",  1),
            ("ParallaxLayer_Near", 2),
        };

        int fixedCount = 0;

        foreach (var cfg in layerConfig)
        {
            GameObject layerGo = GameObject.Find($"ParallaxBackground/{cfg.name}");
            if (layerGo == null)
            {
                Debug.LogWarning($"[FixParallax] {cfg.name} 오브젝트를 찾을 수 없습니다.");
                continue;
            }

            SpriteRenderer[] renderers = layerGo.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer sr in renderers)
            {
                Undo.RecordObject(sr, "Fix Parallax Sorting Layer");
                sr.sortingLayerName = "Background";
                sr.sortingOrder = cfg.order;
                EditorUtility.SetDirty(sr);
                fixedCount++;
                Debug.Log($"[FixParallax] {sr.gameObject.name}: sortingLayer=Background, sortingOrder={cfg.order}");
            }
        }

        if (fixedCount > 0)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

        Debug.Log($"[FixParallax] 완료. {fixedCount}개 SpriteRenderer 수정됨.");
    }
}
