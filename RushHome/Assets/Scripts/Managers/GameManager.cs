using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Ready, Playing, Dying, GameOver }

    public event Action<GameState> OnGameStateChanged;
    public event Action OnPlayerHit;

    public GameState CurrentState { get; private set; } = GameState.Ready;
    public float ElapsedTime { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            ElapsedTime += Time.deltaTime;
        }

        // Ready 상태에서 이동 키를 누르면 게임 시작
        if (CurrentState == GameState.Ready &&
            (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) ||
             Input.GetKeyDown(KeyCode.Space)))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        ElapsedTime = 0f;
        SetState(GameState.Playing);
    }

    public void GameOver()
    {
        SetState(GameState.GameOver);
        Time.timeScale = 0f;
    }

    public void PlayerHit()
    {
        if (CurrentState != GameState.Playing) return;
        OnPlayerHit?.Invoke();
        SetState(GameState.Dying);
    }

    public void PlayerLanded()
    {
        if (CurrentState != GameState.Dying) return;
        GameOver();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    void SetState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}
