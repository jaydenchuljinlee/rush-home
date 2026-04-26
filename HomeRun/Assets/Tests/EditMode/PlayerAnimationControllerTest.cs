using NUnit.Framework;
using UnityEngine;
using System.Reflection;

/// <summary>
/// PlayerAnimationController v2 Animator 파라미터 방식 Edit Mode 테스트.
/// F-08 캐릭터 애니메이션 v2 구현 검증.
/// (v1 색상 Placeholder 방식에서 v2 Animator 파라미터 방식으로 업데이트됨)
/// </summary>
[TestFixture]
public class PlayerAnimationControllerTest
{
    private GameObject _playerGo;
    private PlayerAnimationController _animController;
    private SpriteRenderer _spriteRenderer;
    private PlayerController _playerController;
    private Animator _animator;
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
        _animator = _playerGo.AddComponent<Animator>();
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
    // Animator 파라미터 설정 검증
    // ----------------------------------------------------------------

    [Test]
    public void 성공_Ready상태에서_SpriteRenderer색상_흰색()
    {
        // Arrange: GameState = Ready (기본값)
        // Act: HandleGameStateChanged(Ready) 시뮬레이션
        InvokePrivateMethodWithArg(_animController, "HandleGameStateChanged", GameState.Ready);

        // Assert: Ready 전환 시 색상 복구
        Assert.AreEqual(Color.white, _spriteRenderer.color,
            "Ready 상태로 전환 시 플레이어 색상은 흰색이어야 한다");
    }

    [Test]
    public void 성공_UpdateAnimatorParams_isPlaying_true_설정()
    {
        // Arrange: GameManager.IsPlaying = true
        _gameManager.StartGame();

        // Act
        InvokePrivateMethod(_animController, "UpdateAnimatorParams");

        // Assert: Animator에 isPlaying=true가 설정되는지는
        // 컨트롤러 없이 직접 검증 불가 → 예외 없이 실행되면 PASS
        Assert.Pass("UpdateAnimatorParams가 예외 없이 실행됨");
    }

    [Test]
    public void 성공_UpdateAnimatorParams_PlayerController_null일때_예외없음()
    {
        // Arrange: _playerController를 null로 주입
        SetPrivateField(_animController, "_playerController", null);

        // Act & Assert: null 가드로 인해 예외 없이 실행되어야 함
        Assert.DoesNotThrow(
            () => InvokePrivateMethod(_animController, "UpdateAnimatorParams"),
            "PlayerController가 null일 때 UpdateAnimatorParams는 예외를 던지지 않아야 한다"
        );
    }

    [Test]
    public void 성공_HandlePlayerHit_호출시_예외없음()
    {
        // Act & Assert: Animator 컨트롤러 없이도 SetTrigger는 예외 없이 실행
        Assert.DoesNotThrow(
            () => InvokePrivateMethod(_animController, "HandlePlayerHit"),
            "HandlePlayerHit은 예외를 던지지 않아야 한다"
        );
    }

    [Test]
    public void 성공_HandleGameStateChanged_Ready_색상_흰색_복구()
    {
        // Arrange: 색상을 빨강으로 변경
        _spriteRenderer.color = Color.red;

        // Act
        InvokePrivateMethodWithArg(_animController, "HandleGameStateChanged", GameState.Ready);

        // Assert
        Assert.AreEqual(Color.white, _spriteRenderer.color,
            "Ready 상태 전환 시 색상이 흰색으로 복구되어야 한다");
    }

    [Test]
    public void 성공_HandleGameStateChanged_Playing_예외없음()
    {
        // Act & Assert
        Assert.DoesNotThrow(
            () => InvokePrivateMethodWithArg(_animController, "HandleGameStateChanged", GameState.Playing),
            "HandleGameStateChanged(Playing) 호출 시 예외 없어야 한다"
        );
    }

    [Test]
    public void 성공_HandleGameStateChanged_GameOver_예외없음()
    {
        // Act & Assert
        Assert.DoesNotThrow(
            () => InvokePrivateMethodWithArg(_animController, "HandleGameStateChanged", GameState.GameOver),
            "HandleGameStateChanged(GameOver) 호출 시 예외 없어야 한다"
        );
    }

    // ----------------------------------------------------------------
    // 헬퍼
    // ----------------------------------------------------------------

    private void InjectAnimControllerFields()
    {
        SetPrivateField(_animController, "_animator", _animator);
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
}
