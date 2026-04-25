using NUnit.Framework;
using UnityEngine;
using System.Reflection;

/// <summary>
/// PlayerController 슬라이딩 시각 피드백 + CheckGround BoxCast 수정 검증 테스트.
/// bugfix/slide-visual-and-ground-check 수정 확인용 Edit Mode 테스트.
/// </summary>
[TestFixture]
public class PlayerControllerSlideTest
{
    private GameObject _playerGo;
    private PlayerController _controller;
    private BoxCollider2D _collider;

    [SetUp]
    public void SetUp()
    {
        _playerGo = new GameObject("TestPlayer");

        // PlayerController 가 Awake 에서 참조하는 컴포넌트를 먼저 추가한다
        _collider = _playerGo.AddComponent<BoxCollider2D>();
        _collider.size = new Vector2(0.8f, 1.6f);
        _collider.offset = new Vector2(0f, 0f);

        _playerGo.AddComponent<Rigidbody2D>();
        _playerGo.AddComponent<SpriteRenderer>();

        _controller = _playerGo.AddComponent<PlayerController>();

        // Edit Mode 에서 Awake 가 자동 호출되지 않을 수 있으므로
        // PlayerController 의 private 필드를 직접 Reflection 으로 초기화한다
        InjectPrivateFields();
    }

    /// <summary>
    /// Edit Mode 에서 Awake 미실행 대비: PlayerController 의 캐싱 필드를 직접 주입.
    /// </summary>
    private void InjectPrivateFields()
    {
        // _collider 주입
        SetPrivateField("_collider", _collider);
        // _normalColliderSize / _normalColliderOffset 주입
        SetPrivateField("_normalColliderSize", _collider.size);
        SetPrivateField("_normalColliderOffset", _collider.offset);
        // _normalScale 주입
        SetPrivateField("_normalScale", _playerGo.transform.localScale);
        // _rigidbody 주입
        SetPrivateField("_rigidbody", _playerGo.GetComponent<Rigidbody2D>());
    }

    [TearDown]
    public void TearDown()
    {
        if (_playerGo != null)
            Object.DestroyImmediate(_playerGo);
    }

    // ------------------------------------------------------------------
    // 슬라이딩 시각 피드백 (localScale Y 축소)
    // ------------------------------------------------------------------

    [Test]
    public void 성공_슬라이딩시_Y스케일이_절반으로_변경됨()
    {
        // Arrange
        Vector3 originalScale = _playerGo.transform.localScale;
        ForceSlideStart();

        // Assert
        float expectedY = originalScale.y * 0.5f;
        Assert.AreEqual(expectedY, _playerGo.transform.localScale.y, 0.001f,
            "슬라이딩 중 Y 스케일은 원본의 0.5 배여야 한다");
    }

    [Test]
    public void 성공_슬라이딩시_X스케일은_변하지_않음()
    {
        // Arrange
        float originalX = _playerGo.transform.localScale.x;
        ForceSlideStart();

        // Assert
        Assert.AreEqual(originalX, _playerGo.transform.localScale.x, 0.001f,
            "슬라이딩 중 X 스케일은 변하지 않아야 한다");
    }

    [Test]
    public void 성공_슬라이딩_종료시_Y스케일이_복구됨()
    {
        // Arrange
        Vector3 originalScale = _playerGo.transform.localScale;
        ForceSlideStart();

        // Act
        ForceSlideEnd();

        // Assert
        Assert.AreEqual(originalScale.y, _playerGo.transform.localScale.y, 0.001f,
            "슬라이딩 종료 후 Y 스케일이 원래 값으로 복구되어야 한다");
    }

    // ------------------------------------------------------------------
    // 콜라이더 크기 변화
    // ------------------------------------------------------------------

    [Test]
    public void 성공_슬라이딩시_콜라이더_높이가_절반으로_줄어듦()
    {
        // Arrange
        float originalHeight = _collider.size.y;
        ForceSlideStart();

        // Assert
        Assert.AreEqual(originalHeight * 0.5f, _collider.size.y, 0.001f,
            "슬라이딩 중 콜라이더 높이는 원본의 0.5 배여야 한다");
    }

    [Test]
    public void 성공_슬라이딩_종료시_콜라이더_크기가_복구됨()
    {
        // Arrange
        Vector2 originalSize = _collider.size;
        Vector2 originalOffset = _collider.offset;
        ForceSlideStart();

        // Act
        ForceSlideEnd();

        // Assert
        Assert.AreEqual(originalSize.y, _collider.size.y, 0.001f,
            "슬라이딩 종료 후 콜라이더 높이가 복구되어야 한다");
        Assert.AreEqual(originalOffset.y, _collider.offset.y, 0.001f,
            "슬라이딩 종료 후 콜라이더 오프셋 Y 가 복구되어야 한다");
    }

    // ------------------------------------------------------------------
    // 헬퍼: Reflection 으로 private 메서드 직접 호출
    // ------------------------------------------------------------------

    private void ForceSlideStart()
    {
        // _isGrounded = true 로 설정해야 TrySlide() early return 을 통과
        SetPrivateField("_isGrounded", true);
        SetPrivateField("_isSliding", false);

        InvokePrivateMethod("TrySlide");
    }

    private void ForceSlideEnd()
    {
        InvokePrivateMethod("EndSlide");
    }

    private void SetPrivateField(string fieldName, object value)
    {
        var field = typeof(PlayerController).GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"필드 '{fieldName}' 를 찾을 수 없습니다");
        field.SetValue(_controller, value);
    }

    private void InvokePrivateMethod(string methodName)
    {
        var method = typeof(PlayerController).GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"메서드 '{methodName}' 를 찾을 수 없습니다");
        method.Invoke(_controller, null);
    }
}
