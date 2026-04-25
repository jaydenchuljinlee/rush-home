using NUnit.Framework;
using UnityEngine;
using System.Reflection;

/// <summary>
/// PlayerAnimationController 색상 Placeholder 로직 Edit Mode 테스트.
/// F-08 캐릭터 애니메이션 구현 검증.
/// </summary>
[TestFixture]
public class PlayerAnimationControllerTest
{
    private GameObject _playerGo;
    private PlayerAnimationController _animController;
    private SpriteRenderer _spriteRenderer;
    private PlayerController _playerController;
    private GameManager _gameManager;
    private GameObject _gameManagerGo;

    [SetUp]
    public void SetUp()
    {
        // GameManager 생성 및 Instance 설정
        _gameManagerGo = new GameObject("TestGameManager");
        _gameManager = _gameManagerGo.AddComponent<GameManager>();
        SetStaticField(typeof(GameManager), "<Instance>k__BackingField", _gameManager);

        // Player 오브젝트 구성
        _playerGo = new GameObject("TestPlayer");
        _playerGo.AddComponent<BoxCollider2D>();
        _playerGo.AddComponent<Rigidbody2D>();
        _spriteRenderer = _playerGo.AddComponent<SpriteRenderer>();
        _playerController = _playerGo.AddComponent<PlayerController>();

        // PlayerAnimationController 에는 Animator가 필요함 ([RequireComponent])
        _playerGo.AddComponent<Animator>();
        _animController = _playerGo.AddComponent<PlayerAnimationController>();

        // Awake가 Edit Mode에서 자동 호출되지 않으므로 필드 직접 주입
        InjectAnimControllerFields();
        InjectPlayerControllerFields();
    }

    [TearDown]
    public void TearDown()
    {
        if (_playerGo != null) Object.DestroyImmediate(_playerGo);
        if (_gameManagerGo != null) Object.DestroyImmediate(_gameManagerGo);
        SetStaticField(typeof(GameManager), "<Instance>k__BackingField", null);
    }

    // ----------------------------------------------------------------
    // 색상 Placeholder 검증
    // ----------------------------------------------------------------

    [Test]
    public void 성공_Ready상태에서_색상이_흰색()
    {
        // Arrange: GameState = Ready (기본값), isHit = false
        SetPrivateField(_animController, "_isHit", false);

        // Act
        InvokePrivateMethod(_animController, "UpdatePlaceholderColor");

        // Assert
        Assert.AreEqual(Color.white, _spriteRenderer.color,
            "Ready 상태에서 플레이어 색상은 흰색이어야 한다");
    }

    [Test]
    public void 성공_Playing_지면_비슬라이딩_색상이_초록()
    {
        // Arrange: GameState = Playing, isGrounded = true, isSliding = false
        _gameManager.StartGame();
        SetPrivateField(_playerController, "_isGrounded", true);
        SetPrivateField(_playerController, "_isSliding", false);
        SetPrivateField(_animController, "_isHit", false);

        // Act
        InvokePrivateMethod(_animController, "UpdatePlaceholderColor");

        // Assert
        Color expected = new Color(0.2f, 0.8f, 0.2f, 1f);
        AssertColorApprox(expected, _spriteRenderer.color, "Playing+지면 상태에서 색상은 초록이어야 한다");
    }

    [Test]
    public void 성공_Playing_공중_색상이_파랑()
    {
        // Arrange: GameState = Playing, isGrounded = false
        _gameManager.StartGame();
        SetPrivateField(_playerController, "_isGrounded", false);
        SetPrivateField(_playerController, "_isSliding", false);
        SetPrivateField(_animController, "_isHit", false);

        // Act
        InvokePrivateMethod(_animController, "UpdatePlaceholderColor");

        // Assert
        Color expected = new Color(0.2f, 0.4f, 1f, 1f);
        AssertColorApprox(expected, _spriteRenderer.color, "공중(점프) 상태에서 색상은 파랑이어야 한다");
    }

    [Test]
    public void 성공_Playing_슬라이딩_색상이_노랑()
    {
        // Arrange: GameState = Playing, isSliding = true
        _gameManager.StartGame();
        SetPrivateField(_playerController, "_isGrounded", true);
        SetPrivateField(_playerController, "_isSliding", true);
        SetPrivateField(_animController, "_isHit", false);

        // Act
        InvokePrivateMethod(_animController, "UpdatePlaceholderColor");

        // Assert
        Color expected = new Color(1f, 0.9f, 0f, 1f);
        AssertColorApprox(expected, _spriteRenderer.color, "슬라이딩 상태에서 색상은 노랑이어야 한다");
    }

    [Test]
    public void 성공_Hit상태_색상이_빨강()
    {
        // Arrange: _isHit = true
        _gameManager.StartGame();
        SetPrivateField(_animController, "_isHit", true);
        SetPrivateField(_animController, "_hitTimer", 0.3f);

        // Act
        InvokePrivateMethod(_animController, "UpdatePlaceholderColor");

        // Assert
        Color expected = new Color(1f, 0.2f, 0.2f, 1f);
        AssertColorApprox(expected, _spriteRenderer.color, "피격 상태에서 색상은 빨강이어야 한다");
    }

    [Test]
    public void 성공_Hit타이머_만료후_isHit_false로_전환()
    {
        // Arrange
        SetPrivateField(_animController, "_isHit", true);
        SetPrivateField(_animController, "_hitTimer", 0.01f);

        // Act: deltaTime을 직접 줄 수 없으므로 타이머를 0 이하로 설정 후 UpdateHitTimer 호출
        SetPrivateField(_animController, "_hitTimer", -0.1f);
        InvokePrivateMethod(_animController, "UpdateHitTimer");

        // Assert
        bool isHit = (bool)GetPrivateField(_animController, "_isHit");
        Assert.IsFalse(isHit, "Hit 타이머가 만료되면 _isHit은 false가 되어야 한다");
    }

    [Test]
    public void 성공_GameStateReady전환시_색상_흰색으로_리셋()
    {
        // Arrange: 먼저 Hit 상태로 만들고
        SetPrivateField(_animController, "_isHit", true);
        _spriteRenderer.color = Color.red;

        // Act: Ready 상태 전환 이벤트 시뮬레이션
        InvokePrivateMethodWithArg(_animController, "HandleGameStateChanged", GameState.Ready);

        // Assert
        Assert.AreEqual(Color.white, _spriteRenderer.color,
            "Ready 상태로 전환 시 색상이 흰색으로 리셋되어야 한다");
        bool isHit = (bool)GetPrivateField(_animController, "_isHit");
        Assert.IsFalse(isHit, "Ready 상태 전환 시 _isHit이 false로 초기화되어야 한다");
    }

    // ----------------------------------------------------------------
    // 헬퍼
    // ----------------------------------------------------------------

    private void InjectAnimControllerFields()
    {
        SetPrivateField(_animController, "_animator", _playerGo.GetComponent<Animator>());
        SetPrivateField(_animController, "_spriteRenderer", _spriteRenderer);
        SetPrivateField(_animController, "_playerController", _playerController);
    }

    private void InjectPlayerControllerFields()
    {
        var col = _playerGo.GetComponent<BoxCollider2D>();
        SetPrivateField(_playerController, "_collider", col);
        SetPrivateField(_playerController, "_normalColliderSize", col.size);
        SetPrivateField(_playerController, "_normalColliderOffset", col.offset);
        SetPrivateField(_playerController, "_normalScale", _playerGo.transform.localScale);
        SetPrivateField(_playerController, "_rigidbody", _playerGo.GetComponent<Rigidbody2D>());
        SetPrivateField(_playerController, "_spriteRenderer", _spriteRenderer);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"인스턴스 필드 '{fieldName}'을 찾을 수 없습니다 (타입: {target.GetType().Name})");
        field.SetValue(target, value);
    }

    private static object GetPrivateField(object target, string fieldName)
    {
        var field = target.GetType().GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"인스턴스 필드 '{fieldName}'을 찾을 수 없습니다");
        return field.GetValue(target);
    }

    private static void SetStaticField(System.Type type, string fieldName, object value)
    {
        var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static);
        field?.SetValue(null, value);
    }

    private static void InvokePrivateMethod(object target, string methodName)
    {
        var method = target.GetType().GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"메서드 '{methodName}'을 찾을 수 없습니다");
        method.Invoke(target, null);
    }

    private static void InvokePrivateMethodWithArg(object target, string methodName, object arg)
    {
        var method = target.GetType().GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"메서드 '{methodName}'을 찾을 수 없습니다");
        method.Invoke(target, new[] { arg });
    }

    private static void AssertColorApprox(Color expected, Color actual, string message, float tolerance = 0.01f)
    {
        Assert.AreEqual(expected.r, actual.r, tolerance, $"{message} (R 채널)");
        Assert.AreEqual(expected.g, actual.g, tolerance, $"{message} (G 채널)");
        Assert.AreEqual(expected.b, actual.b, tolerance, $"{message} (B 채널)");
        Assert.AreEqual(expected.a, actual.a, tolerance, $"{message} (A 채널)");
    }
}
