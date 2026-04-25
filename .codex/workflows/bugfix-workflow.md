# Bugfix Workflow

Use this workflow when the user reports a bug or asks for a fix.

## 1. Branch and State Check

- Confirm the current branch.
- If already on `bugfix/*`, continue there.
- If on `feature/*`, a dedicated `bugfix/*` branch is preferred.
- If on `main`, warn and avoid starting direct bugfix work there unless the user explicitly wants it.
- If the worktree is dirty with unrelated files, stop before making the situation worse.

## 2. Reproduce and Record

- Reproduce first when feasible.
- Create or update a bug record in `.claude/bugs/{id-or-slug}.md` using `.codex/templates/bug-report.md`.
- Record:
  - symptom
  - environment
  - repro steps
  - observed logs
  - suspected root cause

## 3. Plan the Fix

- Create or update a targeted plan in `.claude/plans/bugfix-{slug}.md`.
- Keep the plan narrow to the reported defect.
- If the real fix requires feature-scope changes, escalate instead of quietly broadening the patch.

## 4. Implement and Validate

- Fix the root cause, not just the visible symptom.
- Run existing tests first when available.
- Add a focused regression test if the bug is testable and the project already supports that layer.
- If Coplay is available, do runtime verification after compile/test validation.

## 5. Report

- State the root cause clearly.
- List changed files.
- State regression coverage and remaining risks.
- If the branch is `bugfix/*`, remind the user that the normal next step is merging back into the owning `feature/*` branch.
