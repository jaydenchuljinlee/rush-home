using UnityEngine;

/// <summary>
/// 에디터 플레이 검증용 임시 MonoBehaviour.
/// Start()에서 GameManager.StartGame()을 호출하고 스스로 삭제된다.
/// </summary>
public class AnimVerifyStarter : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[AnimVerify] GameManager.Instance is null");
            Destroy(gameObject);
            return;
        }

        GameManager.Instance.StartGame();
        Debug.Log($"[AnimVerify] StartGame 호출 완료. State={GameManager.Instance.CurrentState}");
        Destroy(gameObject);
    }
}
