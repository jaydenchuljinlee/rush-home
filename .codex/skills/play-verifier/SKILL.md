---
name: play-verifier
description: >
  Verify a feature or fix by running the game in Unity, checking logs, and collecting
  runtime evidence. Use when actual in-editor play validation is needed after code changes.
---

# Play Verifier

Use this skill for runtime confirmation in Unity.

## Responsibilities

- Confirm compile health before play.
- Check scene/object setup relevant to the target feature.
- Run the game, inspect logs, and capture evidence.
- Retry only when the problem is a narrow setup issue that can be safely corrected.

## Guardrails

- Stop immediately on compile failures.
- Distinguish input/config/setup issues from code issues.
- Capture screenshots or runtime state when possible.
- If Coplay or Unity is unavailable, state that rather than pretending verification happened.

## Output

- Runtime PASS/FAIL summary, key logs, and captured evidence references.
