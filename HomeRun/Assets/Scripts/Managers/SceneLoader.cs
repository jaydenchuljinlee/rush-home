using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환을 담당하는 유틸리티 클래스.
/// 씬 이름을 상수로 관리하여 하드코딩 문자열 오류를 방지한다.
/// </summary>
public static class SceneLoader
{
    public const string MainMenuScene = "MainMenu";
    public const string GameScene = "GameScene";

    public static void LoadMainMenu()
    {
        SceneManager.LoadScene(MainMenuScene);
    }

    public static void LoadGame()
    {
        SceneManager.LoadScene(GameScene);
    }
}
