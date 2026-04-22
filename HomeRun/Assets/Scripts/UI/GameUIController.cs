using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 게임 UI를 관리. 시작 안내, 경과 시간, 게임오버 패널을 표시.
/// Canvas 하위에 배치하고, Inspector에서 UI 요소를 할당할 것.
/// </summary>
public class GameUIController : MonoBehaviour
{
    [Header("In-Game")]
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("Ready")]
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private TextMeshProUGUI readyText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalTimeText;
    [SerializeField] private Button restartButton;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void Start()
    {
        // 현재 GameState에 맞게 UI 초기화 (Start 실행 순서에 의존하지 않음)
        if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Playing)
        {
            if (readyPanel != null) readyPanel.SetActive(false);
            if (timeText != null) timeText.gameObject.SetActive(true);
        }
        else
        {
            if (readyPanel != null) readyPanel.SetActive(true);
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
    }

    private void Update()
    {
        if (GameManager.Instance == null) return; // IsPlaying 외 CurrentState도 참조하므로 null 가드 유지

        if (GameManager.IsPlaying && timeText != null)
        {
            timeText.text = FormatTime(GameManager.Instance.ElapsedTime);
        }
    }

    private void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                if (readyPanel != null) readyPanel.SetActive(false);
                if (gameOverPanel != null) gameOverPanel.SetActive(false);
                if (timeText != null) timeText.gameObject.SetActive(true);
                break;

            case GameState.GameOver:
                if (gameOverPanel != null) gameOverPanel.SetActive(true);
                if (timeText != null) timeText.gameObject.SetActive(false);
                if (finalTimeText != null && GameManager.Instance != null)
                    finalTimeText.text = FormatTime(GameManager.Instance.ElapsedTime);
                break;
        }
    }

    private void OnRestartClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }

    public static string FormatTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        int milliseconds = (int)((time * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }
}
