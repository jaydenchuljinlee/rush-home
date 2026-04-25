using UnityEngine;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;

/// <summary>
/// 파랄랙스 배경 Play Mode 테스트를 에디터에서 실행하는 헬퍼.
/// </summary>
public class RunParallaxPlayTests
{
    public static void Execute()
    {
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter()
        {
            testMode = TestMode.PlayMode,
            testNames = new string[]
            {
                "ParallaxBackgroundPlayTest.성공_플레이상태에서_레이어가_이동함",
                "ParallaxBackgroundPlayTest.성공_레이어별_속도차이_확인"
            }
        };

        api.RegisterCallbacks(new ParallaxPlayCallbacks());
        api.Execute(new ExecutionSettings(filter));
    }

    private class ParallaxPlayCallbacks : ICallbacks
    {
        public void RunStarted(ITestAdaptor testsToRun) { }

        public void RunFinished(ITestResultAdaptor result)
        {
            Debug.Log($"[ParallaxPlayTest] 완료 - PASS: {result.PassCount}, FAIL: {result.FailCount}");
        }

        public void TestStarted(ITestAdaptor test) { }

        public void TestFinished(ITestResultAdaptor result)
        {
            if (result.HasChildren) return;
            string status = result.TestStatus == TestStatus.Passed ? "PASS" : "FAIL";
            string msg = string.IsNullOrEmpty(result.Message) ? "" : $" - {result.Message}";
            Debug.Log($"[ParallaxPlayTest] {status}: {result.Test.Name}{msg}");
        }
    }
}
