---
name: git-pull-request
description: >
  Draft and create a GitHub pull request for this repository by analyzing branch state,
  commit history, changed files, and the local PR template. Use when the user asks to
  create, draft, or prepare a PR. Always preserve the repository PR template structure,
  require user confirmation before creation, and never auto-check test checkboxes.
---

# Git Pull Request

Use this skill when the user wants a PR draft or an actual GitHub PR.

## Workflow

1. Check branch, remote, and push state.
2. Refuse to continue on shared branches such as `main`, `master`, `develop`, or `release`.
3. Ask for or infer the target branch conservatively.
4. Read `.github/PULL_REQUEST_TEMPLATE.md` and preserve its structure exactly.
5. Analyze:
   - `git log origin/{target}..HEAD --oneline`
   - `git diff origin/{target}..HEAD --name-status`
   - `git diff origin/{target}..HEAD`
6. Draft the PR title and body.
7. Show the draft to the user for confirmation or edits.
8. Only after approval, run `gh pr create`.

## Drafting Rules

- Keep the PR body aligned to the repository template.
- Do not remove empty sections.
- Do not rewrite section headings or helper text.
- Fill only the content areas that are meant to be filled.
- Never auto-check test checkboxes.

## Safety Rules

- If commits are not pushed, stop and tell the user push is required first.
- If `gh` is missing or unauthenticated, stop and report it.
- If the diff is unusually large, tell the user the draft may need manual tightening.
- If merge-conflict checking is requested, use a non-destructive check and abort cleanly afterward.
