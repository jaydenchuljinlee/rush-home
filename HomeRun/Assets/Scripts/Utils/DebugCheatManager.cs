#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 에디터 전용 디버그 치트 매니저.
/// F1 키로 무적 모드를 ON/OFF 토글한다.
/// 무적 ON 시 PlayerController.OnPlayerHit 이벤트 backing field를 null로 설정해
/// 장애물 충돌 시 게임오버가 발생하지 않는다.
/// 빌드에는 포함되지 않는다 (#if UNITY_EDITOR).
/// </summary>
public class DebugCheatManager : MonoBehaviour
{
    private bool _isInvincible = false;

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.f1Key.wasPressedThisFrame)
            ToggleInvincible();

        // 난이도 전환은 OnGUI 버튼으로 처리
    }

    private void SetDifficultyTier(DifficultyTier tier)
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.CurrentState == GameState.GameOver)
            {
                // 게임오버 상태: 플레이어/장애물을 초기 위치로 리셋 후 재시작
                GameManager.Instance.SoftRestart();
            }
            else if (!GameManager.IsPlaying)
            {
                // Ready 상태: 일반 시작
                GameManager.Instance.StartGame();
            }
        }

        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetDebugTierOverride(tier);
            Debug.Log($"[DEBUG] Difficulty Override → {tier}");
        }
    }

    private void ToggleInvincible()
    {
        if (_isInvincible)
        {
            DisableInvincible();
        }
        else
        {
            EnableInvincible();
        }
    }

    private void EnableInvincible()
    {
        PlayerController.DebugInvincible = true;
        _isInvincible = true;
        Debug.Log("[DEBUG] INVINCIBLE: ON");
    }

    private void DisableInvincible()
    {
        PlayerController.DebugInvincible = false;
        _isInvincible = false;
        Debug.Log("[DEBUG] INVINCIBLE: OFF");
    }

    private void OnGUI()
    {
        var btnStyle = new GUIStyle(GUI.skin.button) { fontSize = 14 };
        float y = 40f;

        if (_isInvincible)
        {
            var labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold };
            labelStyle.normal.textColor = Color.red;
            GUI.Label(new Rect(10, 10, 300, 30), "[DEBUG] INVINCIBLE: ON", labelStyle);
        }

        // 난이도 전환 버튼 (우측 상단)
        float btnW = 80f, btnH = 30f, margin = 5f;
        float rightX = Screen.width - btnW - 10f;

        if (GUI.Button(new Rect(rightX, y, btnW, btnH), "Normal", btnStyle))
            SetDifficultyTier(DifficultyTier.Normal);
        y += btnH + margin;

        if (GUI.Button(new Rect(rightX, y, btnW, btnH), "Hard", btnStyle))
            SetDifficultyTier(DifficultyTier.Hard);
        y += btnH + margin;

        if (GUI.Button(new Rect(rightX, y, btnW, btnH), "Extreme", btnStyle))
            SetDifficultyTier(DifficultyTier.Extreme);
    }
}
#endif
