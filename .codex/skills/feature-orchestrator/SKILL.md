---
name: feature-orchestrator
description: >
  Coordinate the full feature delivery pipeline by chaining planning, implementation,
  testing, validation, play verification, and play-suite registration. Use when the user
  wants an end-to-end feature workflow rather than a single isolated step.
---

# Feature Orchestrator

Use this skill when a feature should move from request to verified implementation.

## Pipeline

1. `feature-planner`
2. `feature-implementer`
3. `test-runner`
4. `code-validator`
5. `play-verifier`
6. update `.claude/play-suite.md` when the feature belongs in regression coverage

## Rules

- Do not silently drop requested scope.
- If tests fail because implementation is incomplete, send the work back to implementation.
- If validation reveals scope drift or architectural issues, correct those before runtime verification.
- If runtime verification fails due to editor/setup limitations, surface that explicitly.

## Output

- End-to-end delivery summary: code changes, tests, play verification, and remaining manual setup.
