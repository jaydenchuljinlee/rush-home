#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DebugCheatManagerPlayTest
{
    private static readonly FieldInfo OnPlayerHitField =
        typeof(PlayerController).GetField("OnPlayerHit", BindingFlags.NonPublic | BindingFlags.Static)
        ?? typeof(PlayerController).GetField("OnPlayerHit", BindingFlags.Public | BindingFlags.Static);

    // 테스트 후 정적 이벤트 정리
    [TearDown]
    public void TearDown()
    {
        OnPlayerHitField?.SetValue(null, null);
    }

    private static GameObject CreatePlayerObject()
    {
        // PlayerController.Awake()가 Rigidbody2D, BoxCollider2D를 GetComponent로 캐싱하므로
        // AddComponent<PlayerController> 전에 필수 컴포넌트를 먼저 추가해야 한다
        var pcObj = new GameObject("Player");
        pcObj.AddComponent<Rigidbody2D>();
        pcObj.AddComponent<BoxCollider2D>();
        pcObj.AddComponent<PlayerController>();
        return pcObj;
    }

    [UnityTest]
    public IEnumerator 성공_무적ON시_OnPlayerHit_구독자_제거됨()
    {
        // Arrange
        var gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();
        yield return null; // OnEnable 실행 대기 (HandlePlayerHit 구독)

        var pcObj = CreatePlayerObject();
        var debugObj = new GameObject("DebugCheatManager");
        var cheatMgr = debugObj.AddComponent<DebugCheatManager>();
        yield return null;

        // 구독자가 있는지 확인
        var beforeDelegate = OnPlayerHitField?.GetValue(null) as Delegate;
        Assert.IsNotNull(beforeDelegate, "무적 ON 전 OnPlayerHit 구독자가 있어야 합니다.");

        // Act — EnableInvincible 직접 호출 (리플렉션)
        var enableMethod = typeof(DebugCheatManager)
            .GetMethod("EnableInvincible", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(enableMethod, "EnableInvincible 메서드가 존재해야 합니다.");
        enableMethod.Invoke(cheatMgr, null);
        yield return null;

        // Assert
        var afterDelegate = OnPlayerHitField?.GetValue(null) as Delegate;
        Assert.IsNull(afterDelegate, "무적 ON 후 OnPlayerHit 구독자가 null이어야 합니다.");

        // Cleanup
        UnityEngine.Object.Destroy(gmObj);
        UnityEngine.Object.Destroy(pcObj);
        UnityEngine.Object.Destroy(debugObj);
        yield return null;
    }

    [UnityTest]
    public IEnumerator 성공_무적OFF시_OnPlayerHit_구독자_복원됨()
    {
        // Arrange
        var gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();
        yield return null;

        var pcObj = CreatePlayerObject();
        var debugObj = new GameObject("DebugCheatManager");
        var cheatMgr = debugObj.AddComponent<DebugCheatManager>();
        yield return null;

        var enableMethod = typeof(DebugCheatManager)
            .GetMethod("EnableInvincible", BindingFlags.NonPublic | BindingFlags.Instance);
        var disableMethod = typeof(DebugCheatManager)
            .GetMethod("DisableInvincible", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(enableMethod, "EnableInvincible 메서드가 존재해야 합니다.");
        Assert.IsNotNull(disableMethod, "DisableInvincible 메서드가 존재해야 합니다.");

        // Act — 무적 ON 후 OFF
        enableMethod.Invoke(cheatMgr, null);
        yield return null;
        disableMethod.Invoke(cheatMgr, null);
        yield return null;

        // Assert
        var afterDelegate = OnPlayerHitField?.GetValue(null) as Delegate;
        Assert.IsNotNull(afterDelegate, "무적 OFF 후 OnPlayerHit 구독자가 복원되어야 합니다.");

        // Cleanup
        UnityEngine.Object.Destroy(gmObj);
        UnityEngine.Object.Destroy(pcObj);
        UnityEngine.Object.Destroy(debugObj);
        yield return null;
    }
}
#endif
