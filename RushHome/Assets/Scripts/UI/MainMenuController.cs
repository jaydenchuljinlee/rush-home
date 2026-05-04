using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Button startButton;

    void Start()
    {
        startButton?.onClick.AddListener(() => SceneLoader.LoadGame());
    }
}
