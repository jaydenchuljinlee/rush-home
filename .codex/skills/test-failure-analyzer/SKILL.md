---
name: test-failure-analyzer
description: >
  Analyze failing Unity tests and classify the root cause, such as null setup issues,
  asmdef problems, scene setup gaps, or implementation drift. Use when tests fail and the
  next action depends on understanding whether the bug is in product code, test code, or environment.
---

# Test Failure Analyzer

Use this skill when a failing test needs diagnosis before deciding how to proceed.

## Responsibilities

- Read the stack trace and find the first meaningful project location.
- Inspect both the test and production files involved.
- Classify the failure cause.
- Recommend the next owner: implementation, test fix, or user/environment escalation.

## Common Categories

- `[NULL_REF]`
- `[PHYSICS]`
- `[ASMDEF]`
- `[SCENE_SETUP]`
- `[CODE_DRIFT]`
- `[IMPL_MISSING]`

## Output

- Failure summary, root cause classification, key file references, and recommended branch in the workflow.
