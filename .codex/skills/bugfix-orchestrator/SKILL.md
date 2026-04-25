---
name: bugfix-orchestrator
description: >
  Coordinate the full bugfix pipeline by chaining diagnosis, implementation, regression
  checks, and runtime verification. Use when the user wants a defect fixed end to end
  rather than only analyzed or partially patched.
---

# Bugfix Orchestrator

Use this skill for end-to-end bug repair.

## Pipeline

1. `bug-diagnostician`
2. `feature-implementer` for the actual patch
3. `test-runner` for regression or targeted verification
4. `play-verifier` when runtime confirmation is needed

## Rules

- Keep the fix inside the diagnosed bug scope.
- If reproduction is weak or impossible, state the confidence level clearly.
- If the real solution expands into feature work, escalate instead of quietly broadening the change.
- Prefer a narrow regression test when the project supports it.

## Output

- Root cause, changed files, verification results, and next branch-management action.
