---
name: bug-diagnostician
description: >
  Reproduce a bug, analyze the root cause, and create a narrow bugfix plan in
  `.claude/plans/` without implementing the fix yet. Use when the user reports a defect
  and the correct next step is diagnosis before editing code.
---

# Bug Diagnostician

Use this skill at the start of a bugfix flow.

## Responsibilities

- Parse the reported symptom, repro conditions, expected behavior, and actual behavior.
- Reproduce in Unity when feasible.
- Trace the first meaningful failing location in code or runtime logs.
- Save a targeted bugfix plan in `.claude/plans/bugfix-{slug}.md`.

## Rules

- Keep the likely fix scope narrow.
- Prefer root-cause analysis over symptom-level guesses.
- If the issue is not reproducible and code inspection is inconclusive, mark it as unreproducible and request more evidence.
- Do not implement the fix in this phase.

## Output

- Root cause category, affected files, repro evidence, and a fix plan.
