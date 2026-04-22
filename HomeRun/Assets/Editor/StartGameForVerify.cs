using UnityEngine;

public class StartGameForVerify
{
    public static void Execute()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
            Debug.Log("[PlayVerify] GameManager.StartGame() 호출 완료. 현재 상태: " + GameManager.Instance.CurrentState);
        }
        else
        {
            Debug.LogError("[PlayVerify] GameManager.Instance가 null입니다.");
        }
    }
}
