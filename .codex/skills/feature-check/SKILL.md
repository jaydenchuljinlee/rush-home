---
name: feature-check
description: >
  Inspect the current branch, worktree, commit state, and roadmap status, then suggest the
  correct cleanup path for feature or bugfix work. Use when the user asks what should be
  committed, merged, pushed, or prepared for PR.
---

# Feature Check

Use this skill to assess branch readiness and next git actions.

## Responsibilities

- Inspect current branch and worktree state.
- Classify the branch as `main`, `feature/*`, or `bugfix/*`.
- Summarize uncommitted changes and recent commits.
- Cross-check progress tracking when `docs/PROGRESS.md` is relevant.

## Decision Rules

- `bugfix/*` should normally merge back into its parent `feature/*`.
- `feature/*` should only move toward PR when the feature is actually complete.
- `main` should not be the default place for ongoing implementation.
- Never commit, push, merge, or delete branches without user approval.

## Output

- Current branch summary, change classification, and recommended next actions.
