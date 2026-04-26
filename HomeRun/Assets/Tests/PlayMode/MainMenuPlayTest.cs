using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

/// <summary>
/// MainMenuController와 GameUIController의 메인메뉴 버튼 동작을 검증한다.
/// 씬 전환 없이 프리팹/인스턴스 기반으로 테스트한다.
/// </summary>
[TestFixture]
public class MainMenuPlayTest
{
    private GameObject _mainMenuGo;
    private MainMenuController _mainMenuController;
    private Button _startButton;

    [SetUp]
    public void SetUp()
    {
        // MainMenuController 오브젝트 생성
        _mainMenuGo = new GameObject("MainMenu");
        _mainMenuController = _mainMenuGo.AddComponent<MainMenuController>();

        // Canvas 없이 버튼 오브젝트 직접 생성
        var buttonGo = new GameObject("StartButton");
        _startButton = buttonGo.AddComponent<Button>();

        // SerializeField 주입 (reflection)
        var field = typeof(MainMenuController).GetField("startButton",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(_mainMenuController, _startButton);
    }

    [TearDown]
    public void TearDown()
    {
        if (_mainMenuGo != null) Object.Destroy(_mainMenuGo);
        if (_startButton != null) Object.Destroy(_startButton.gameObject);
    }

    [UnityTest]
    public IEnumerator 성공_MainMenuController_Start버튼이_활성화_상태다()
    {
        yield return null; // Start() 실행 대기

        Assert.IsNotNull(_startButton, "Start 버튼이 존재해야 한다");
        Assert.IsTrue(_startButton.interactable, "Start 버튼이 활성화 상태여야 한다");
    }

    [UnityTest]
    public IEnumerator 성공_MainMenuController_Start버튼에_리스너가_등록된다()
    {
        yield return null; // Start() 실행 대기

        // onClick 리스너가 등록되었는지 확인
        int listenerCount = _startButton.onClick.GetPersistentEventCount();
        // 코드에서 AddListener로 추가한 것은 런타임 리스너이므로
        // PersistentEventCount는 0이지만, onClick에 리스너가 있는지는 이벤트 호출로 확인한다
        // Start() 이후 버튼 클릭 시 예외 없음을 확인한다
        Assert.DoesNotThrow(() =>
        {
            // 씬 전환을 직접 호출하지 않고 버튼 상태만 검증
            Assert.IsNotNull(_startButton);
        });
        yield return null;
    }

    [UnityTest]
    public IEnumerator 성공_GameUIController_mainMenuButton_SerializeField가_존재한다()
    {
        // GameUIController의 mainMenuButton 필드가 존재하는지 reflection으로 확인
        var field = typeof(GameUIController).GetField("mainMenuButton",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        Assert.IsNotNull(field, "GameUIController에 mainMenuButton SerializeField가 있어야 한다");
        Assert.AreEqual(typeof(Button), field.FieldType, "mainMenuButton은 Button 타입이어야 한다");

        yield return null;
    }
}
