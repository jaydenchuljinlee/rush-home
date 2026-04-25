# Feature Workflow

Use this workflow when the user asks to implement or extend a game feature.

## 1. Preflight

- Check current branch and worktree state.
- If there are unrelated uncommitted changes, stop and ask the user how to proceed.
- If currently on `bugfix/*`, finish or merge that work before starting a new feature.
- Review `docs/PROGRESS.md` if the request maps to a tracked feature milestone.

## 2. Scope and Plan

- Read the relevant product spec and nearby runtime code first.
- Create or update a plan in `.claude/plans/{feature-name}.md` using `.codex/templates/feature-plan.md`.
- Explicitly list:
  - files to create
  - files to modify
  - Unity editor/manual setup
  - edit/play test strategy
- If the requested scope conflicts with the current architecture, escalate instead of silently cutting scope.

## 3. Implement

- Follow the planned file list.
- Match existing naming, singleton, and event patterns.
- Keep binary scene/prefab edits minimal and safe; document any manual editor work.
- Avoid opportunistic refactors unless they are required for the feature.

## 4. Verify

- First confirm compile health.
- Then run or describe the smallest useful automated tests.
- Then perform editor/play verification when Coplay is available.
- Capture evidence for user-visible runtime behavior when possible.

## 5. Closeout

- Summarize code changes, manual Unity steps, and verification results.
- If the feature should be part of ongoing play regression, add or update an entry in `.claude/play-suite.md` using `.codex/templates/play-suite-entry.md`.

## Unity Batch Examples

```bash
Unity -batchmode -runTests -testPlatform EditMode -projectPath HomeRun -testResults results.xml
Unity -batchmode -runTests -testPlatform PlayMode -projectPath HomeRun -testResults results.xml
Unity -batchmode -quit -projectPath HomeRun -executeMethod BuildScript.BuildAndroid -logFile build.log
```
