#if UNITY_EDITOR
using System;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class DebugCheatManagerEditTest
{
    [Test]
    public void 성공_DebugCheatManager_클래스_존재()
    {
        // Arrange & Act — 전체 어셈블리에서 탐색
        Type type = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = asm.GetType("DebugCheatManager");
            if (type != null) break;
        }

        // Assert
        Assert.IsNotNull(type, "DebugCheatManager 클래스가 존재해야 합니다.");
    }

    [Test]
    public void 성공_OnPlayerHit_BackingField_리플렉션_조회()
    {
        // Arrange
        var playerControllerType = typeof(PlayerController);

        // Act — NonPublic Static 먼저, 없으면 Public Static
        FieldInfo field = playerControllerType.GetField(
            "OnPlayerHit",
            BindingFlags.NonPublic | BindingFlags.Static
        ) ?? playerControllerType.GetField(
            "OnPlayerHit",
            BindingFlags.Public | BindingFlags.Static
        );

        // Assert
        Assert.IsNotNull(field, "PlayerController.OnPlayerHit backing field를 리플렉션으로 조회할 수 있어야 합니다.");
    }
}
#endif
