using NUnit.Framework;

/// <summary>
/// SceneLoader 씬 이름 상수를 검증한다.
/// </summary>
[TestFixture]
public class SceneLoaderTest
{
    [Test]
    public void 성공_MainMenuScene_상수값이_MainMenu_문자열이다()
    {
        Assert.AreEqual("MainMenu", SceneLoader.MainMenuScene);
    }

    [Test]
    public void 성공_GameScene_상수값이_GameScene_문자열이다()
    {
        Assert.AreEqual("GameScene", SceneLoader.GameScene);
    }

    [Test]
    public void 성공_MainMenuScene_상수가_빈값이_아니다()
    {
        Assert.IsFalse(string.IsNullOrEmpty(SceneLoader.MainMenuScene));
    }

    [Test]
    public void 성공_GameScene_상수가_빈값이_아니다()
    {
        Assert.IsFalse(string.IsNullOrEmpty(SceneLoader.GameScene));
    }
}
