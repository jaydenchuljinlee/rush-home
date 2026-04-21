using UnityEngine;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using System.Text;

public class RunDebugCheatPlayModeTests
{
    public static void Execute()
    {
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        api.RegisterCallbacks(new TestCallbacks());
        api.Execute(new ExecutionSettings(new Filter
        {
            testMode = TestMode.PlayMode,
            testNames = new string[]
            {
                "DebugCheatManagerPlayTest"
            }
        }));
    }

    private class TestCallbacks : ICallbacks
    {
        private int _passed = 0;
        private int _failed = 0;
        private StringBuilder _results = new StringBuilder();

        public void RunStarted(ITestAdaptor testsToRun) { }

        public void RunFinished(ITestResultAdaptor result)
        {
            _results.AppendLine($"\nTotal: {_passed + _failed}, Passed: {_passed}, Failed: {_failed}");
            Debug.Log($"[TestRunner] {_results}");
        }

        public void TestStarted(ITestAdaptor test) { }

        public void TestFinished(ITestResultAdaptor result)
        {
            if (result.Test.IsSuite) return;

            string status = result.TestStatus == TestStatus.Passed ? "PASS" : "FAIL";
            if (result.TestStatus == TestStatus.Passed) _passed++;
            else _failed++;

            string msg = string.IsNullOrEmpty(result.Message) ? "" : $" — {result.Message}";
            _results.AppendLine($"{status}: {result.Test.Name}{msg}");
        }
    }
}
