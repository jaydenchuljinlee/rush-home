---
name: feature-planner
description: >
  Analyze a requested game feature, inspect the existing Unity codebase, and produce a
  concrete implementation plan in `.claude/plans/`. Use when the user asks for design,
  architecture, implementation planning, or when a feature should be scoped before coding.
---

# Feature Planner

Use this skill before implementing a non-trivial feature.

## Responsibilities

- Read the product spec and nearby runtime code first.
- Identify affected scripts, prefabs, scenes, and manager/event flows.
- Keep planned changes explicit and bounded.
- Save or update the plan in `.claude/plans/{feature-name}.md`.

## Planning Rules

- Follow `.codex/templates/feature-plan.md`.
- Include exact files to create and modify.
- Include key method signatures.
- Include Unity editor setup that cannot be safely automated.
- Prefer minimal impact to existing systems.
- If the request conflicts with architecture or needs scope reduction, escalate instead of silently trimming.

## Output

- A concrete implementation plan that an implementer can execute directly.
