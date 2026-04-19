using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// GameUIController Play Mode 테스트.
/// GameManager 상태 변경에 따른 UI 패널 전환을 검증한다.
/// </summary>
public class GameUIPlayTest
{
    private GameObject _gameManagerGo;
    private GameObject _canvasGo;
    private GameUIController _uiController;
    private GameObject _readyPanel;
    private GameObject _gameOverPanel;

    [SetUp]
    public void SetUp()
    {
        // GameManager 생성
        _gameManagerGo = new GameObject("GameManager");
        _gameManagerGo.AddComponent<GameManager>();

        // Canvas + GameUIController 생성
        _canvasGo = new GameObject("Canvas");
        _uiController = _canvasGo.AddComponent<GameUIController>();

        // ReadyPanel 생성
        _readyPanel = new GameObject("ReadyPanel");
        _readyPanel.transform.SetParent(_canvasGo.transform);
        _readyPanel.SetActive(true);

        // GameOverPanel 생성
        _gameOverPanel = new GameObject("GameOverPanel");
        _gameOverPanel.transform.SetParent(_canvasGo.transform);
        _gameOverPanel.SetActive(false);

        // SerializeField 주입 (Reflection)
        var readyPanelField = typeof(GameUIController).GetField("readyPanel",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        readyPanelField?.SetValue(_uiController, _readyPanel);

        var gameOverPanelField = typeof(GameUIController).GetField("gameOverPanel",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        gameOverPanelField?.SetValue(_uiController, _gameOverPanel);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_gameManagerGo);
        Object.DestroyImmediate(_canvasGo);
    }

    [UnityTest]
    public IEnumerator 성공_게임시작시_ReadyPanel_비활성화()
    {
        // Arrange: ReadyPanel 활성화 상태 확인
        Assert.IsTrue(_readyPanel.activeSelf, "시작 전 ReadyPanel은 활성화 상태여야 합니다.");

        // Act
        GameManager.Instance.StartGame();
        yield return null;

        // Assert
        Assert.IsFalse(_readyPanel.activeSelf, "게임 시작 후 ReadyPanel은 비활성화되어야 합니다.");
    }

    [UnityTest]
    public IEnumerator 성공_게임오버시_GameOverPanel_활성화()
    {
        // Arrange: Playing 상태로 전환
        GameManager.Instance.StartGame();
        yield return null;

        // Act
        GameManager.Instance.GameOver();
        yield return null;

        // Assert
        Assert.IsTrue(_gameOverPanel.activeSelf, "게임오버 후 GameOverPanel은 활성화되어야 합니다.");
    }
}
