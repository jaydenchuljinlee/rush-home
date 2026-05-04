using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject readyPanel;
    [SerializeField] GameObject gameOverPanel;

    [Header("Timer")]
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI finalTimeText;

    [Header("Buttons")]
    [SerializeField] Button restartButton;
    [SerializeField] Button mainMenuButton;

    void Start()
    {
        restartButton?.onClick.AddListener(() => GameManager.Instance.Restart());
        mainMenuButton?.onClick.AddListener(() => SceneLoader.LoadMainMenu());

        GameManager.Instance.OnGameStateChanged += HandleStateChanged;
        HandleStateChanged(GameManager.Instance.CurrentState);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= HandleStateChanged;
    }

    void Update()
    {
        var state = GameManager.Instance.CurrentState;
        if (state == GameManager.GameState.Playing || state == GameManager.GameState.Dying)
        {
            timerText.text = FormatTime(GameManager.Instance.ElapsedTime);
        }
    }

    void HandleStateChanged(GameManager.GameState state)
    {
        readyPanel.SetActive(state == GameManager.GameState.Ready);
        gameOverPanel.SetActive(state == GameManager.GameState.GameOver);
        timerText.gameObject.SetActive(state == GameManager.GameState.Playing || state == GameManager.GameState.Dying);

        if (state == GameManager.GameState.GameOver)
        {
            finalTimeText.text = FormatTime(GameManager.Instance.ElapsedTime);
        }
    }

    string FormatTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        int ms = (int)((time * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}.{ms:00}";
    }
}
