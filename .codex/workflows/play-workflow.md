# Play Workflow

Use this workflow when the user asks to smoke-test, observe, reproduce, or verify runtime behavior in Unity.

## Modes

- `check`: quick compile plus short runtime smoke test
- `watch`: observe runtime over time
- `test`: verify a specific feature or object
- `repro`: reproduce a reported issue and gather evidence
- `suite`: execute the project play checklist in `.claude/play-suite.md`

## Common Sequence

1. Check compile errors.
2. Check Unity editor state.
3. Stop an existing Play Mode session if needed.
4. Start play.
5. Gather logs plus scene/UI evidence.
6. Stop play unless the user explicitly wants a free-running session.

## Guardrails

- Do not auto-fix compile errors inside a pure play-check request without telling the user.
- Prefer evidence collection over speculation.
- In `suite` mode, continue through all checks even if some fail.
- When suite failures reveal a real defect, log it in `.claude/bugs/`.

## Suite Source

The suite source of truth remains `.claude/play-suite.md`.
Use `.codex/templates/play-suite-entry.md` when adding or updating entries.
