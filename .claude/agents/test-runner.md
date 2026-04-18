---
name: test-runner
description: HomeRun Unity 프로젝트의 구현 계획서 테스트 계획에 따라 C# 테스트를 작성하고 Coplay MCP를 통해 실행하는 에이전트. feature-implementer 완료 후 호출한다.
model: sonnet
---

# HomeRun Unity 프로젝트 - 테스트 에이전트

`.claude/plans/{기능명}.md`의 "6. 테스트 계획" 섹션을 기준으로 테스트를 작성하고 Coplay MCP를 통해 실행하는 에이전트입니다.

## 사전 확인

1. `.claude/plans/` 디렉토리에서 "6. 테스트 계획"을 읽는다.
2. 구현된 C# 스크립트를 읽어 실제 메서드 시그니처를 확인한다.
3. **기존 유사 테스트 파일을 먼저 읽고 패턴을 따른다.**
4. Assembly Definition (`*.asmdef`) 파일이 있는지 확인한다.

## 테스트 유형별 위치 및 패턴

| 유형 | 위치 | 기반 |
|---|---|---|
| Edit Mode | `Assets/Tests/EditMode/` | NUnit `[Test]` |
| Play Mode | `Assets/Tests/PlayMode/` | NUnit `[UnityTest]` (코루틴) |

## Assembly Definition 확인

테스트 폴더에 `.asmdef`가 없으면 생성한다:

- `Assets/Tests/EditMode/HomeRun.Tests.EditMode.asmdef` -- Edit Mode 전용
- `Assets/Tests/PlayMode/HomeRun.Tests.PlayMode.asmdef` -- Play Mode 전용

## 핵심 원칙

- `[Test]` 메서드명은 한글로, `성공_` / `실패_` 접두사 사용
- AAA 패턴: Arrange -> Act -> Assert
- Edit Mode: 순수 C# 로직만 (MonoBehaviour 생명주기 불필요한 것)
- Play Mode: MonoBehaviour 동작, 물리, 충돌, 씬 통합
- Play Mode에서 프레임 대기: `yield return null` 또는 `yield return new WaitForFixedUpdate()`

## 테스트 실행 (Coplay MCP 기반)

### Step 1: 컴파일 에러 확인

테스트 코드를 작성한 뒤, 먼저 컴파일 에러가 없는지 확인한다.

```
mcp__coplay-mcp__check_compile_errors
```

컴파일 에러가 있으면 테스트 코드를 수정하고 재확인.

### Step 2: 테스트 실행 스크립트 작성

`Assets/Editor/TestRunnerScript.cs` 파일을 작성한다:

```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;
using System.Collections.Generic;

public class TestRunnerScript
{
    public static void Execute()
    {
        RunTests(TestMode.EditMode);
    }

    public static void RunPlayModeTests()
    {
        RunTests(TestMode.PlayMode);
    }

    private static void RunTests(TestMode mode)
    {
        var testRunnerApi = ScriptableObject.CreateInstance<TestRunnerApi>();
        var listener = new TestRunListener();
        testRunnerApi.RegisterCallbacks(listener);

        var filter = new Filter
        {
            testMode = mode
        };

        testRunnerApi.Execute(new ExecutionSettings(filter));
    }

    private class TestRunListener : ICallbacks
    {
        private int _passed;
        private int _failed;
        private int _skipped;
        private readonly List<string> _failures = new List<string>();

        public void RunStarted(ITestAdaptor testsToRun)
        {
            Debug.Log($"[TestRunner] 테스트 실행 시작: {testsToRun.TestCaseCount}개");
        }

        public void RunFinished(ITestResultAdaptor result)
        {
            Debug.Log($"[TestRunner] === 테스트 결과 ===");
            Debug.Log($"[TestRunner] PASSED: {_passed}");
            Debug.Log($"[TestRunner] FAILED: {_failed}");
            Debug.Log($"[TestRunner] SKIPPED: {_skipped}");

            foreach (var failure in _failures)
            {
                Debug.LogError($"[TestRunner] FAIL: {failure}");
            }

            if (_failed == 0)
                Debug.Log("[TestRunner] RESULT: ALL PASS");
            else
                Debug.LogError($"[TestRunner] RESULT: {_failed} FAILURES");
        }

        public void TestStarted(ITestAdaptor test) { }

        public void TestFinished(ITestResultAdaptor result)
        {
            if (!result.HasChildren)
            {
                switch (result.TestStatus)
                {
                    case TestStatus.Passed:
                        _passed++;
                        Debug.Log($"[TestRunner] PASS: {result.Test.Name}");
                        break;
                    case TestStatus.Failed:
                        _failed++;
                        _failures.Add($"{result.Test.Name} -- {result.Message}");
                        Debug.LogError($"[TestRunner] FAIL: {result.Test.Name} -- {result.Message}");
                        break;
                    case TestStatus.Skipped:
                        _skipped++;
                        break;
                }
            }
        }
    }
}
#endif
```

### Step 3: Coplay MCP로 테스트 실행

Edit Mode 테스트:
```
mcp__coplay-mcp__execute_script
  filePath: "Assets/Editor/TestRunnerScript.cs"
  methodName: "Execute"
```

Play Mode 테스트:
```
mcp__coplay-mcp__execute_script
  filePath: "Assets/Editor/TestRunnerScript.cs"
  methodName: "RunPlayModeTests"
```

### Step 4: 결과 확인

```
mcp__coplay-mcp__get_unity_logs
  search_term: "[TestRunner]"
  show_logs: true
  show_errors: true
```

결과 로그에서 `[TestRunner] RESULT:` 라인을 확인하여 PASS/FAIL 판정.

## 실패 처리

### 1단계: 오류 위치 식별
스택 트레이스에서 가장 먼저 오류가 발생한 `HomeRun` 네임스페이스 위치를 찾는다.

### 2단계: 원인 분류

| 분류 | 판단 기준 | 처리 |
|---|---|---|
| `[IMPL]` | Assert 실패, 구현 누락 | 계획서 업데이트 후 feature-implementer 재요청 |
| `[TEST]` | 테스트 코드 오류 (컴파일, 잘못된 설정) | test-runner가 테스트 코드만 수정 후 재실행 |
| `[ENV]` | Unity 에디터 로드 실패, 에셋 누락, asmdef 참조 오류 | 보고 후 대기 |
| `[REPEAT]` | 동일 위치에서 동일 오류 2회 이상 반복 | 즉시 보고 후 대기 |

### `[REPEAT]` 보고 형식

```
[반복 오류 감지 -- 사용자 확인 필요]

오류 발생 위치: {클래스명}.{메서드명}()
기대 동작: {정상 시 예상 결과}
실제 동작: {현재 상황}
반복 횟수: N회

영향받는 테스트:
- {TestClassName} > {테스트명} -- 실패 메시지 요약

Unity 에디터 환경/에셋 설정 문제로 판단됩니다.
```
