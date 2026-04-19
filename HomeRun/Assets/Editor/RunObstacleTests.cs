using UnityEngine;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using System.Collections.Generic;

public class RunObstacleTests
{
    private static List<string> _results = new List<string>();
    private static int _passed = 0;
    private static int _failed = 0;

    public static void Execute()
    {
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter()
        {
            testMode = TestMode.EditMode,
            testNames = new string[]
            {
                "ObstacleSystemTest"
            }
        };

        api.RegisterCallbacks(new TestCallbacks());
        api.Execute(new ExecutionSettings(filter));
    }

    private class TestCallbacks : ICallbacks
    {
        public void RunStarted(ITestAdaptor testsToRun) { }

        public void RunFinished(ITestResultAdaptor result)
        {
            UnityEngine.Debug.Log($"[TestRunner] Finished - Passed: {result.PassCount}, Failed: {result.FailCount}, Skipped: {result.SkipCount}");
        }

        public void TestStarted(ITestAdaptor test) { }

        public void TestFinished(ITestResultAdaptor result)
        {
            string status = result.TestStatus == TestStatus.Passed ? "PASS" : "FAIL";
            UnityEngine.Debug.Log($"[TestRunner] {status}: {result.Name} - {result.Message}");
        }
    }
}
