using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverAnimator : MonoBehaviour
{
    [SerializeField] CanvasGroup overlayGroup;
    [SerializeField] RectTransform titleRect;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] CanvasGroup contentGroup;

    float timer;
    bool isAnimating;

    const float OverlayFadeDuration = 0.3f;
    const float TitlePunchDuration = 0.5f;
    const float ContentFadeDelay = 0.6f;
    const float ContentFadeDuration = 0.3f;

    void OnEnable()
    {
        timer = 0f;
        isAnimating = true;

        if (overlayGroup != null) overlayGroup.alpha = 0f;
        if (titleRect != null) titleRect.localScale = Vector3.zero;
        if (contentGroup != null) contentGroup.alpha = 0f;
    }

    void Update()
    {
        if (!isAnimating) return;

        // Time.timeScale = 0이므로 unscaledDeltaTime 사용
        timer += Time.unscaledDeltaTime;

        // 1) 어두운 오버레이 페이드인
        if (overlayGroup != null)
        {
            float overlayT = Mathf.Clamp01(timer / OverlayFadeDuration);
            overlayGroup.alpha = overlayT * 0.7f;
        }

        // 2) GAME OVER 타이틀 — 펀치 스케일
        if (titleRect != null)
        {
            float titleT = Mathf.Clamp01(timer / TitlePunchDuration);
            float scale = EaseOutBack(titleT);
            titleRect.localScale = Vector3.one * scale;
        }

        // 3) 시간 + 버튼 페이드인
        if (contentGroup != null && timer > ContentFadeDelay)
        {
            float contentT = Mathf.Clamp01((timer - ContentFadeDelay) / ContentFadeDuration);
            contentGroup.alpha = contentT;
        }

        if (timer > ContentFadeDelay + ContentFadeDuration)
            isAnimating = false;
    }

    // 뒤로 갔다가 튕기는 이징
    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
