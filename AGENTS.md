# HomeRun Codex Guide

This repository has been reorganized for Codex-first usage.
The old `.claude/` tree remains as historical reference, but Codex should use the files in this guide and `.codex/`.

## Project Scope

- Engine: Unity 6 LTS (2D URP)
- Language: C#
- Genre: 2D side-view runner
- Platforms: iOS / Android
- Backend: PlayFab
- Main Unity project path: `HomeRun/`

Codex should treat `HomeRun/` as the Unity project root when looking for `Assets/`, `Packages/`, and `ProjectSettings/`.

## Operating Model

- Prefer natural-language requests over slash commands.
- For non-trivial work, prompts should make the goal, relevant context, constraints, and done criteria explicit.
- Plan before implementing when a task is ambiguous, multi-step, or likely to touch multiple Unity systems.
- Use `.codex/workflows/` for multi-step execution.
- Use `.codex/templates/` when creating plans, reports, or suite entries.
- Reuse `.claude/bugs/`, `.claude/plans/`, and `.claude/play-suite.md` as working records unless the user asks to migrate the historical data too.
- Keep `AGENTS.md` concise and durable. Put task-specific detail in `.codex/workflows/`, `.codex/templates/`, or a plan file.

## Core Engineering Rules

- One component should have one responsibility.
- Follow existing project patterns before introducing new ones.
- Use `[SerializeField] private` for Inspector references.
- Cache `GetComponent` in `Awake()`.
- Keep physics in `FixedUpdate()`, input in `Update()`.
- Avoid runtime `Find` / `FindObjectOfType`.
- Prefer events for cross-system coordination.
- Do not edit Unity scene/prefab binaries blindly; explain required editor-side setup when direct safe editing is not possible.
- Do not shrink requested scope without explicit user approval.

## Naming

- C# classes: `PascalCase`
- MonoBehaviours: role-driven names like `GroundScroller`
- Managers: `{Domain}Manager`
- ScriptableObjects: `{Name}Data`
- Interfaces: `I{Name}`
- Private fields: `_camelCase`
- Events: `OnPascalCase`

## Testing

- Edit Mode tests: `HomeRun/Assets/Tests/EditMode/`
- Play Mode tests: `HomeRun/Assets/Tests/PlayMode/`
- Test names may follow the existing Korean style: `성공_...`, `실패_...`
- Use AAA structure.
- Use `[UnityTest]` for coroutine-based runtime checks.

Detailed guidance:
- `.codex/workflows/feature-workflow.md`
- `.codex/workflows/bugfix-workflow.md`
- `.codex/workflows/play-workflow.md`

## Coplay / Unity Guardrails

- After creating a GameObject, verify required components are actually attached.
- `save_scene` must use `Scenes/GameScene`-style paths, not bare scene names.
- If the project mixes old `UnityEngine.Input` usage with the new Input System, verify `Active Input Handling` is compatible.
- Do not call `save_scene` during Play Mode.
- Inspector-serialized values override code defaults; update scene objects and prefab assets together when required.
- `execute_script` is unreliable for game static access across assemblies; prefer scene components for runtime-triggered behavior.
- If a prefab is instantiated at runtime, scene-only component changes are insufficient; update the prefab asset too.

## Build / Validation

- Compile or test before claiming completion when the environment allows it.
- For documentation/configuration-only work, validate the edited files directly and review the diff instead of running Unity tests.
- If Unity or Coplay is unavailable, state the limitation explicitly.
- For build and test commands, prefer the Unity batch examples documented in `.codex/workflows/feature-workflow.md`.
- Completion means the requested behavior or configuration is changed, relevant checks have run or been explicitly skipped with reason, and risky assumptions are called out.

## Review Loop

- Review the diff before final handoff.
- Check for scope drift, accidental Unity asset churn, and unrelated formatting changes.
- Add or update focused tests when product code behavior changes and the project has an appropriate test layer.
- Do not mark PR template test checkboxes unless that exact validation actually ran.

## Codex Configuration

- Keep personal defaults, credentials, and trust decisions in `~/.codex/config.toml`.
- Keep shared repo behavior in `.codex/config.toml`.
- Prefer tight sandboxing and approval defaults; loosen only for trusted repository workflows.
- Use MCP only when it removes a real manual loop. This project uses Coplay MCP for Unity editor context and runtime verification.
- Repeated workflows should live in `.codex/skills/` or `.codex/workflows/` before becoming scheduled automation.

## Session Hygiene

- Keep one Codex thread per coherent unit of work.
- Use compacting or a fresh thread when context gets stale or the task meaningfully branches.
- Use subagents only for bounded parallel work with a clear output.

## Git Flow

- `feature/*` branches are the normal unit of delivery.
- `bugfix/*` branches should merge back into their parent `feature/*` branch, not directly to `main`.
- Do not commit, push, merge, or open PRs without explicit user approval.
- Avoid `git add -A`; stage files deliberately.

## Available Codex Resources

- Feature delivery: `.codex/workflows/feature-workflow.md`
- Bug fixing: `.codex/workflows/bugfix-workflow.md`
- Play verification: `.codex/workflows/play-workflow.md`
- Feature planning template: `.codex/templates/feature-plan.md`
- Bug report template: `.codex/templates/bug-report.md`
- Play suite entry template: `.codex/templates/play-suite-entry.md`
- PR skill: `.codex/skills/git-pull-request/SKILL.md`
- Feature planner skill: `.codex/skills/feature-planner/SKILL.md`
- Feature implementer skill: `.codex/skills/feature-implementer/SKILL.md`
- Test runner skill: `.codex/skills/test-runner/SKILL.md`
- Code validator skill: `.codex/skills/code-validator/SKILL.md`
- Play verifier skill: `.codex/skills/play-verifier/SKILL.md`
- Bug diagnostician skill: `.codex/skills/bug-diagnostician/SKILL.md`
- Test failure analyzer skill: `.codex/skills/test-failure-analyzer/SKILL.md`
- Feature check skill: `.codex/skills/feature-check/SKILL.md`
- Feature orchestrator skill: `.codex/skills/feature-orchestrator/SKILL.md`
- Bugfix orchestrator skill: `.codex/skills/bugfix-orchestrator/SKILL.md`
