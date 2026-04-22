using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using System.Collections.Generic;

public class RunTests : ICallbacks
{
    private static RunTests _instance;
    private static List<string> _passed = new List<string>();
    private static List<string> _failed = new List<string>();
    private static List<string> _errors = new List<string>();

    public static void Execute()
    {
        _passed.Clear();
        _failed.Clear();
        _errors.Clear();

        _instance = new RunTests();
        var api = ScriptableObject.CreateInstance<TestRunnerApi>();
        api.RegisterCallbacks(_instance);

        var filter = new Filter
        {
            testMode = TestMode.EditMode,
            assemblyNames = new[] { "HomeRun.Tests.EditMode" }
        };
        api.Execute(new ExecutionSettings(filter));
    }

    public void RunStarted(ITestAdaptor testsToRun) { }

    public void RunFinished(ITestResultAdaptor result)
    {
        Debug.Log("=== EDIT MODE TEST RESULTS ===");
        Debug.Log($"PASS: {_passed.Count}");
        Debug.Log($"FAIL: {_failed.Count}");
        foreach (var f in _failed)
            Debug.LogError($"FAILED: {f}");
        foreach (var e in _errors)
            Debug.LogError($"ERROR: {e}");
        if (_failed.Count == 0 && _errors.Count == 0)
            Debug.Log("ALL_TESTS_PASSED");
        else
            Debug.Log("SOME_TESTS_FAILED");
    }

    public void TestStarted(ITestAdaptor test) { }

    public void TestFinished(ITestResultAdaptor result)
    {
        if (!result.Test.IsSuite)
        {
            if (result.TestStatus == TestStatus.Passed)
                _passed.Add(result.Test.FullName);
            else if (result.TestStatus == TestStatus.Failed)
                _failed.Add($"{result.Test.FullName}: {result.Message}");
            else if (result.TestStatus == TestStatus.Inconclusive || result.TestStatus == TestStatus.Skipped)
                _errors.Add($"{result.Test.FullName}: {result.TestStatus}");
        }
    }
}
