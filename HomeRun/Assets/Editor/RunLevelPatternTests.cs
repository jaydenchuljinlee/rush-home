using UnityEngine;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using System.Collections.Generic;
using System.Text;

public class RunLevelPatternTests
{
    public static void Execute()
    {
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        var filter = new Filter()
        {
            testMode = TestMode.EditMode,
            assemblyNames = new string[] { "HomeRun.Tests.EditMode" }
        };

        api.RegisterCallbacks(new TestCallbacks());
        api.Execute(new ExecutionSettings(filter));
    }

    private class TestCallbacks : ICallbacks
    {
        private readonly List<string> _passed = new List<string>();
        private readonly List<string> _failed = new List<string>();

        public void RunStarted(ITestAdaptor testsToRun) { }

        public void RunFinished(ITestResultAdaptor result)
        {
            var sb = new StringBuilder();
            foreach (var p in _passed) sb.AppendLine($"PASS: {p}");
            foreach (var f in _failed) sb.AppendLine($"FAIL: {f}");
            sb.AppendLine($"\nTotal: {_passed.Count + _failed.Count}, Passed: {_passed.Count}, Failed: {_failed.Count}");

            Debug.Log($"[TestRunner] {sb}");
        }

        public void TestStarted(ITestAdaptor test) { }

        public void TestFinished(ITestResultAdaptor result)
        {
            if (result.Test.IsSuite) return;

            if (result.TestStatus == TestStatus.Passed)
                _passed.Add(result.Test.Name);
            else
                _failed.Add($"{result.Test.Name} — {result.Message}");
        }
    }
}
