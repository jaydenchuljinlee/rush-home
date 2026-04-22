using NUnit.Framework;
using UnityEngine;
using System.Reflection;

/// <summary>
/// GameManager.IsPlaying 프로퍼티 Edit Mode 테스트.
/// Instance null 체크 집중화 리팩토링(F-15) 검증.
/// </summary>
[TestFixture]
public class GameManagerIsPlayingTest
{
    private GameObject _gameManagerGo;

    [SetUp]
    public void SetUp()
    {
        ResetInstance();
    }

    [TearDown]
    public void TearDown()
    {
        if (_gameManagerGo != null)
            Object.DestroyImmediate(_gameManagerGo);
        ResetInstance();
    }

    /// <summary>
    /// 에디터 모드에서는 AddComponent 후 Awake가 자동 호출되지 않으므로
    /// 백킹 필드를 직접 설정하여 Instance를 초기화한다.
    /// </summary>
    private GameManager CreateGameManager(string name)
    {
        _gameManagerGo = new GameObject(name);
        var gm = _gameManagerGo.AddComponent<GameManager>();
        var field = typeof(GameManager).GetField("<Instance>k__BackingField",
            BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, gm);
        return gm;
    }

    private static void ResetInstance()
    {
        var field = typeof(GameManager).GetField("<Instance>k__BackingField",
            BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, null);
    }

    [Test]
    public void 성공_IsPlaying_Instance_null시_false()
    {
        // Instance가 없으면 false
        Assert.IsFalse(GameManager.IsPlaying);
    }

    [Test]
    public void 성공_IsPlaying_Ready상태_false()
    {
        // Arrange: 기본 상태 Ready
        CreateGameManager("TestGM_Ready");

        // Assert
        Assert.IsFalse(GameManager.IsPlaying);
    }

    [Test]
    public void 성공_IsPlaying_Playing상태_true()
    {
        // Arrange
        var gm = CreateGameManager("TestGM_Playing");

        // Act
        gm.StartGame();

        // Assert
        Assert.IsTrue(GameManager.IsPlaying);
    }

    [Test]
    public void 성공_IsPlaying_GameOver상태_false()
    {
        // Arrange
        var gm = CreateGameManager("TestGM_GameOver");
        gm.StartGame();

        // Act
        gm.GameOver();

        // Assert
        Assert.IsFalse(GameManager.IsPlaying);
    }
}
