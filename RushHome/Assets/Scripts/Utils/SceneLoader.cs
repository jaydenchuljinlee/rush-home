using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public static void LoadMainMenu() => SceneManager.LoadScene("MainMenu");
    public static void LoadGame() => SceneManager.LoadScene("GameScene");
}
