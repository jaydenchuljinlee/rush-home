---
name: test-runner
description: >
  Write or run Edit Mode and Play Mode tests for the HomeRun Unity project, based on an
  existing plan or a concrete verification target. Use when validation needs executable
  tests or when existing tests must be checked for regressions.
---

# Test Runner

Use this skill for automated verification.

## Responsibilities

- Read the relevant test plan first.
- Prefer existing test patterns in `HomeRun/Assets/Tests/`.
- Distinguish Edit Mode from Play Mode responsibilities.
- Check compile health before reporting test results.

## Rules

- Use AAA structure.
- Existing Korean naming style for test methods is acceptable.
- Use `[UnityTest]` for coroutine/runtime tests.
- If asmdef wiring is missing or broken, diagnose it explicitly.
- If failure appears to come from product code rather than test code, report that clearly instead of masking it.

## Output

- Test files added or updated, executed scope, and PASS/FAIL summary with the likely failure class when relevant.
