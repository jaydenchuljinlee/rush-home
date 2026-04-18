using NUnit.Framework;
using UnityEngine;

/// <summary>
/// DifficultyData ScriptableObject의 기본값 및 속도 계산 로직 검증.
/// Edit Mode 테스트 - Unity 에디터 실행 불필요.
/// </summary>
[TestFixture]
public class DifficultyDataTest
{
    private DifficultyData _data;

    [SetUp]
    public void SetUp()
    {
        _data = ScriptableObject.CreateInstance<DifficultyData>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_data);
    }

    [Test]
    public void 성공_0초에서_Easy속도_반환()
    {
        // Arrange & Act
        float speed = _data.GetSpeedForTime(0f);

        // Assert
        Assert.AreEqual(_data.EasySpeed, speed, "0초에서는 EasySpeed를 반환해야 한다.");
    }

    [Test]
    public void 성공_30초에서_Normal속도_반환()
    {
        // Arrange & Act
        float speed = _data.GetSpeedForTime(30f);

        // Assert
        Assert.AreEqual(_data.NormalSpeed, speed, "30초(NormalThreshold)에서는 NormalSpeed를 반환해야 한다.");
    }

    [Test]
    public void 성공_60초에서_Hard속도_반환()
    {
        // Arrange & Act
        float speed = _data.GetSpeedForTime(60f);

        // Assert
        Assert.AreEqual(_data.HardSpeed, speed, "60초(HardThreshold)에서는 HardSpeed를 반환해야 한다.");
    }

    [Test]
    public void 성공_90초에서_Extreme속도_반환()
    {
        // Arrange & Act
        float speed = _data.GetSpeedForTime(90f);

        // Assert
        Assert.AreEqual(_data.ExtremeSpeed, speed, "90초(ExtremeThreshold)에서는 ExtremeSpeed를 반환해야 한다.");
    }

    [Test]
    public void 성공_기본값_Easy속도가_0보다_큼()
    {
        Assert.Greater(_data.EasySpeed, 0f, "EasySpeed는 0보다 커야 한다.");
    }

    [Test]
    public void 성공_속도_단계적_증가_Easy_Normal_Hard_Extreme()
    {
        // Arrange & Act & Assert
        Assert.Less(_data.EasySpeed, _data.NormalSpeed, "EasySpeed < NormalSpeed");
        Assert.Less(_data.NormalSpeed, _data.HardSpeed, "NormalSpeed < HardSpeed");
        Assert.Less(_data.HardSpeed, _data.ExtremeSpeed, "HardSpeed < ExtremeSpeed");
    }

    [Test]
    public void 성공_임계값_이전_시간은_이전_단계_속도_반환()
    {
        // 29초는 Easy 구간
        float speedAt29 = _data.GetSpeedForTime(29f);
        Assert.AreEqual(_data.EasySpeed, speedAt29, "29초는 Easy 속도여야 한다.");
    }

    [Test]
    public void 성공_매우_큰_시간에서_Extreme속도_반환()
    {
        float speed = _data.GetSpeedForTime(9999f);
        Assert.AreEqual(_data.ExtremeSpeed, speed, "매우 긴 시간에서는 ExtremeSpeed를 반환해야 한다.");
    }
}
