---
name: feature-implementer
description: >
  Implement a planned feature in the HomeRun Unity project by following an existing
  feature plan in `.claude/plans/`. Use when coding should start and the scope is already
  defined or can be derived from a concrete implementation plan.
---

# Feature Implementer

Use this skill to turn an approved feature plan into code.

## Responsibilities

- Read the relevant `.claude/plans/{feature-name}.md` first.
- Implement in the planned order.
- Match existing project patterns for MonoBehaviour, managers, events, and data objects.
- Keep changes inside the planned file scope unless a justified exception is surfaced.

## Coding Rules

- Prefer `[SerializeField] private` for Inspector references.
- Cache component lookups in `Awake()`.
- Keep physics in `FixedUpdate()`.
- Avoid runtime `Find` / `FindObjectOfType`.
- Do not write tests here unless the user explicitly asks; testing belongs to `test-runner`.
- Document manual Unity scene/prefab setup when direct safe edits are not practical.

## Output

- Implemented code plus a short list of changed files and manual editor follow-ups.
