using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public enum GameState
{
    Ready,
    Playing,
    GameOver
}

/// <summary>
/// 게임 전체 상태를 관리하는 싱글톤 매니저.
/// 게임 시작, 게임오버, 재시작, 시간 측정을 담당.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.Ready;
    public float ElapsedTime { get; private set; }

    /// <summary>
    /// 게임이 Playing 상태인지 확인하는 편의 프로퍼티.
    /// Instance null 체크를 포함하므로 각 컴포넌트에서 별도 null 체크 불필요.
    /// </summary>
    public static bool IsPlaying => Instance != null && Instance.CurrentState == GameState.Playing;

    public static event System.Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerHit += HandlePlayerHit;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerHit -= HandlePlayerHit;
    }

    private void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            ElapsedTime += Time.deltaTime;
        }

        // Ready 상태에서 아무 입력이나 들어오면 게임 시작
        if (CurrentState == GameState.Ready)
        {
            bool anyKey = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool anyTouch = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
            if (anyKey || anyTouch)
            {
                StartGame();
            }
        }
    }

    public void StartGame()
    {
        ElapsedTime = 0f;
        ChangeState(GameState.Playing);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void HandlePlayerHit()
    {
        GameOver();
    }

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}
