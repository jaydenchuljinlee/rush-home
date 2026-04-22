using System.Collections;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Play Suite용 자동 시작 헬퍼. GameManager에 부착 시 Start()에서 무적 모드 + StartGame() 실행.
/// Suite 실행 후 remove_component로 제거한다.
/// </summary>
public class SuiteAutoStarter : MonoBehaviour
{
    private void Start()
    {
        // GameManager.Instance는 같은 Awake()에서 설정됨 (GameManager보다 나중에 Start() 실행)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
            Debug.Log("[Suite] StartGame() via SuiteAutoStarter");
        }
        else
        {
            Debug.LogError("[Suite] GameManager.Instance still null in SuiteAutoStarter.Start()");
        }

        // 무적 모드: 모든 OnEnable 구독이 완료된 후 1프레임 뒤에 OnPlayerHit 구독자 제거
        StartCoroutine(ApplyInvincibleModeNextFrame());
    }

    private IEnumerator ApplyInvincibleModeNextFrame()
    {
        yield return null; // 1프레임 대기 - 모든 OnEnable 구독 완료 후 적용

        var field = typeof(PlayerController).GetField("OnPlayerHit",
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (field != null)
        {
            field.SetValue(null, null);
            Debug.Log("[Suite] Invincible mode ON (applied after frame)");
        }
    }
}
