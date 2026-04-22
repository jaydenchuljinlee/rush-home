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
        // 무적 모드: OnPlayerHit 이벤트 구독자 제거
        var field = typeof(PlayerController).GetField("OnPlayerHit",
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        if (field != null)
        {
            field.SetValue(null, null);
            Debug.Log("[Suite] Invincible mode ON");
        }

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
    }
}
