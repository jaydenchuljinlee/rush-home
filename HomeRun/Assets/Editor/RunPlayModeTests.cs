using UnityEngine;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;

/// <summary>
/// Play Mode 테스트를 에디터에서 실행하는 헬퍼 스크립트.
/// </summary>
public class RunPlayModeTests
{
    public static void Execute()
    {
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter()
        {
            testMode = TestMode.PlayMode,
            testNames = new string[] { "ObstaclePlayTest" }
        };

        api.RegisterCallbacks(new PlayModeCallbacks());
        api.Execute(new ExecutionSettings(filter));
    }

    private class PlayModeCallbacks : ICallbacks
    {
        public void RunStarted(ITestAdaptor testsToRun)
        {
            Debug.Log("[PlayModeTest] 테스트 시작");
        }

        public void RunFinished(ITestResultAdaptor result)
        {
            Debug.Log($"[PlayModeTest] 완료 - Passed: {result.PassCount}, Failed: {result.FailCount}, Skipped: {result.SkipCount}");
        }

        public void TestStarted(ITestAdaptor test) { }

        public void TestFinished(ITestResultAdaptor result)
        {
            string status = result.TestStatus == TestStatus.Passed ? "PASS" : "FAIL";
            string msg = string.IsNullOrEmpty(result.Message) ? "" : $" - {result.Message}";
            Debug.Log($"[PlayModeTest] {status}: {result.Name}{msg}");
        }
    }
}
