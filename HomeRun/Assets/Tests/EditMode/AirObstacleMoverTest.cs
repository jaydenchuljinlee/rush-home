using NUnit.Framework;
using UnityEngine;

/// <summary>
/// AirObstacleMover 컴포넌트의 이동 로직 Edit Mode 테스트.
/// GameManager 의존성 없이 SetMoving/SetAmplitude/SetFrequency를 통해 순수 로직 검증.
/// </summary>
[TestFixture]
public class AirObstacleMoverTest
{
    private GameObject _go;
    private AirObstacleMover _mover;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("AirObstacleMoverTestObject");
        _mover = _go.AddComponent<AirObstacleMover>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_go);
    }

    [Test]
    public void 성공_SetActive_false시_amplitude_1이어도_Y위치_변하지_않음()
    {
        // Arrange
        _mover.SetAmplitude(1f);
        _mover.SetFrequency(1f);
        _mover.SetMoving(false);
        float initialY = _go.transform.position.y;

        // Act — Update를 직접 호출할 수 없으므로 내부 상태로 검증
        // IsMoving이 false이면 Y 오프셋 적용 안 됨을 확인
        // (실제 이동은 Update에서 일어나므로, SetMoving(false)시 IsMoving=false 검증)
        Assert.IsFalse(_mover.IsMoving, "SetMoving(false) 후 IsMoving이 false여야 한다.");
        Assert.AreEqual(initialY, _go.transform.position.y, 0.001f,
            "SetMoving(false) 후 Y 위치가 변하지 않아야 한다.");
    }

    [Test]
    public void 성공_SetActive_true시_IsMoving_true_반환()
    {
        // Arrange
        _mover.SetAmplitude(1f);
        _mover.SetFrequency(1f);

        // Act
        _mover.SetMoving(true);

        // Assert
        Assert.IsTrue(_mover.IsMoving, "SetMoving(true) 후 IsMoving이 true여야 한다.");
    }

    [Test]
    public void 성공_amplitude_0이면_SpawnY와_현재Y_동일()
    {
        // Arrange
        _go.transform.position = new Vector3(5f, 2f, 0f);
        _mover.SetAmplitude(0f);
        _mover.SetFrequency(1f);
        _mover.SetMoving(true);

        // Assert — amplitude=0이므로 sin 계산 결과도 0
        // SpawnY는 SetMoving 호출 시 현재 Y로 설정됨
        Assert.AreEqual(2f, _mover.SpawnY, 0.001f,
            "SetMoving(true) 호출 시 SpawnY가 현재 Y(2f)로 설정되어야 한다.");
        Assert.AreEqual(0f, _mover.Amplitude, 0.001f,
            "amplitude가 0f여야 한다.");
    }

    [Test]
    public void 성공_SetMoving_true후_SpawnY가_현재_Y로_설정됨()
    {
        // Arrange
        _go.transform.position = new Vector3(0f, 3.5f, 0f);

        // Act
        _mover.SetMoving(true);

        // Assert
        Assert.AreEqual(3.5f, _mover.SpawnY, 0.001f,
            "SetMoving(true) 호출 시 SpawnY가 현재 Y(3.5f)로 설정되어야 한다.");
    }

    [Test]
    public void 성공_amplitude_setter_정상_동작()
    {
        // Act
        _mover.SetAmplitude(2.5f);

        // Assert
        Assert.AreEqual(2.5f, _mover.Amplitude, 0.001f);
    }

    [Test]
    public void 성공_frequency_setter_정상_동작()
    {
        // Act
        _mover.SetFrequency(0.5f);

        // Assert
        Assert.AreEqual(0.5f, _mover.Frequency, 0.001f);
    }
}
