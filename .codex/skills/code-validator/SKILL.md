---
name: code-validator
description: >
  Review implemented HomeRun Unity code for convention compliance, side effects, scope
  drift, and common performance problems. Use after implementation or tests when the user
  wants a validation pass before wrapping up.
---

# Code Validator

Use this skill after implementation is functionally complete.

## Responsibilities

- Validate only the intended change scope first.
- Compare actual diffs against the planned file list when a plan exists.
- Check naming, architecture, runtime anti-patterns, side effects, and unnecessary edits.

## Checks

- Naming conventions
- Component and manager architecture
- Performance anti-patterns in hot paths
- Scope drift and unintended API changes
- Noise changes such as unused usings or gratuitous formatting

## Output

- PASS/FAIL validation summary with `[CRITICAL]` and `[WARNING]` style findings when needed.
