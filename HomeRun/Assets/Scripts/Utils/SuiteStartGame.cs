public class SuiteStartGame
{
    public static void Execute()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
            UnityEngine.Debug.Log("[Suite] GameManager.StartGame() 호출 완료 — Playing 상태로 전환");
        }
        else
        {
            UnityEngine.Debug.LogError("[Suite] GameManager.Instance가 null입니다.");
        }
    }
}
