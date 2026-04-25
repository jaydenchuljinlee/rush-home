using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using System.Collections.Generic;

public class RunEditModeTests : ICallbacks
{
    private static RunEditModeTests _instance;
    private static List<string> _results = new List<string>();
    private static int _passed = 0;
    private static int _failed = 0;

    public static void Execute()
    {
        _instance = new RunEditModeTests();
        _results.Clear();
        _passed = 0;
        _failed = 0;

        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        api.RegisterCallbacks(_instance);

        var filter = new Filter()
        {
            testMode = TestMode.EditMode
        };

        api.Execute(new ExecutionSettings(filter));
    }

    public void RunStarted(ITestAdaptor testsToRun) { }

    public void RunFinished(ITestResultAdaptor result)
    {
        Debug.Log($"[TestRunner] 완료 - PASS: {_passed}, FAIL: {_failed}");
        foreach (var r in _results)
        {
            Debug.Log(r);
        }
    }

    public void TestStarted(ITestAdaptor test) { }

    public void TestFinished(ITestResultAdaptor result)
    {
        if (result.HasChildren) return;

        string status = result.TestStatus == TestStatus.Passed ? "PASS" : "FAIL";
        string msg = $"[{status}] {result.Test.Name}";
        if (result.TestStatus != TestStatus.Passed && !string.IsNullOrEmpty(result.Message))
        {
            msg += $"\n  >> {result.Message}";
        }
        _results.Add(msg);

        if (result.TestStatus == TestStatus.Passed) _passed++;
        else _failed++;
    }
}
