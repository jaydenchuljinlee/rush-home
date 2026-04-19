using NUnit.Framework;

/// <summary>
/// GameUIController Edit Mode 테스트.
/// FormatTime 유틸리티 메서드의 출력 형식을 검증한다.
/// </summary>
[TestFixture]
public class GameUIControllerTest
{
    [Test]
    public void 성공_FormatTime_0초_반환형식_검증()
    {
        // Arrange & Act
        string result = GameUIController.FormatTime(0f);

        // Assert
        Assert.AreEqual("00:00.00", result);
    }

    [Test]
    public void 성공_FormatTime_90초_반환형식_검증()
    {
        // Arrange & Act
        string result = GameUIController.FormatTime(90f);

        // Assert
        Assert.AreEqual("01:30.00", result);
    }

    [Test]
    public void 성공_FormatTime_소수점_반환형식_검증()
    {
        // Arrange & Act
        string result = GameUIController.FormatTime(1.5f);

        // Assert
        Assert.AreEqual("00:01.50", result);
    }

    [Test]
    public void 성공_FormatTime_59초59_경계값_검증()
    {
        // Arrange & Act
        string result = GameUIController.FormatTime(59.99f);

        // Assert: 59초 99밀리초
        Assert.AreEqual("00:59.99", result);
    }
}
