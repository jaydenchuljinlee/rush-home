using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 메인메뉴 씬의 UI를 관리한다.
/// Start 버튼 클릭 시 GameScene으로 전환한다.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked()
    {
        SceneLoader.LoadGame();
    }
}
