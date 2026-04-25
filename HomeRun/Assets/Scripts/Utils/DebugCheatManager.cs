#if UNITY_EDITOR
using System;
using System.Reflection;
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
    private Delegate _savedDelegate = null;

    private static readonly FieldInfo OnPlayerHitField = typeof(PlayerController)
        .GetField("OnPlayerHit", BindingFlags.NonPublic | BindingFlags.Static)
        ?? typeof(PlayerController).GetField("OnPlayerHit", BindingFlags.Public | BindingFlags.Static);

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.f1Key.wasPressedThisFrame)
            ToggleInvincible();

        // 난이도 전환은 OnGUI 버튼으로 처리
    }

    private void SetDifficultyTier(DifficultyTier tier)
    {
        // Ready 상태면 게임 시작
        if (GameManager.Instance != null && !GameManager.IsPlaying)
            GameManager.Instance.StartGame();

        // DifficultyManager의 디버그 오버라이드로 매 프레임 덮어쓰기 방지
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetDebugTierOverride(tier);
            Debug.Log($"[DEBUG] Difficulty Override → {tier}");
        }
        else
        {
            Debug.LogWarning("[DebugCheatManager] DifficultyManager not found");
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
        if (OnPlayerHitField == null)
        {
            Debug.LogWarning("[DebugCheatManager] OnPlayerHit backing field를 찾을 수 없습니다.");
            return;
        }

        _savedDelegate = OnPlayerHitField.GetValue(null) as Delegate;
        OnPlayerHitField.SetValue(null, null);

        _isInvincible = true;
        Debug.Log("[DEBUG] INVINCIBLE: ON — 장애물 충돌 무시됨");
    }

    private void DisableInvincible()
    {
        if (OnPlayerHitField == null)
        {
            Debug.LogWarning("[DebugCheatManager] OnPlayerHit backing field를 찾을 수 없습니다.");
            return;
        }

        if (_savedDelegate != null)
        {
            OnPlayerHitField.SetValue(null, _savedDelegate);
            _savedDelegate = null;
        }
        else if (GameManager.Instance != null)
        {
            // 저장된 delegate가 없으면 GameManager.HandlePlayerHit을 직접 재등록
            var handlePlayerHit = typeof(GameManager)
                .GetMethod("HandlePlayerHit", BindingFlags.NonPublic | BindingFlags.Instance);
            if (handlePlayerHit != null)
            {
                var action = (Action)Delegate.CreateDelegate(typeof(Action), GameManager.Instance, handlePlayerHit);
                PlayerController.OnPlayerHit += action;
            }
        }

        _isInvincible = false;
        Debug.Log("[DEBUG] INVINCIBLE: OFF — 정상 충돌 처리 복원됨");
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
