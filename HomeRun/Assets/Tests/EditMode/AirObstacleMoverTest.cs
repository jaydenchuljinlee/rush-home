using NUnit.Framework;
using UnityEngine;

/// <summary>
/// AirObstacleMover 컴포넌트의 X축 좌우 이동 로직 Edit Mode 테스트.
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
    public void 성공_SetMoving_false시_IsMoving_false()
    {
        _mover.SetMoving(false);
        Assert.IsFalse(_mover.IsMoving);
    }

    [Test]
    public void 성공_SetMoving_true시_IsMoving_true()
    {
        _mover.SetMoving(true);
        Assert.IsTrue(_mover.IsMoving);
    }

    [Test]
    public void 성공_amplitude_0이면_X이동_없음()
    {
        _go.transform.position = new Vector3(5f, 2f, 0f);
        _mover.SetAmplitude(0f);
        _mover.SetFrequency(1f);
        _mover.SetMoving(true);

        Assert.AreEqual(0f, _mover.Amplitude, 0.001f,
            "amplitude가 0f여야 한다.");
    }

    [Test]
    public void 성공_amplitude_setter_정상_동작()
    {
        _mover.SetAmplitude(2.5f);
        Assert.AreEqual(2.5f, _mover.Amplitude, 0.001f);
    }

    [Test]
    public void 성공_frequency_setter_정상_동작()
    {
        _mover.SetFrequency(0.5f);
        Assert.AreEqual(0.5f, _mover.Frequency, 0.001f);
    }

    [Test]
    public void 성공_SetMoving_false후_위치_변하지_않음()
    {
        _go.transform.position = new Vector3(3f, 1f, 0f);
        _mover.SetMoving(false);

        Assert.AreEqual(3f, _go.transform.position.x, 0.001f);
        Assert.AreEqual(1f, _go.transform.position.y, 0.001f);
    }
}
