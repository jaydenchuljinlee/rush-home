# Codex Migration Notes

This directory is the Codex-facing replacement for the old `.claude/` operating structure.

## Mapping

- `.claude/rules/*` -> `AGENTS.md`
- `.claude/commands/*` -> `.codex/workflows/*`
- `.claude/agents/*` -> `.codex/skills/*` role skills plus workflow phases
- `.claude/skills/git-pull-request` -> `.codex/skills/git-pull-request`
- `.claude/plans/*`, `.claude/bugs/*`, `.claude/play-suite.md` -> retained as project records

## Design Choice

Claude's setup depended on explicit command files plus named sub-agents.
Codex already has strong built-in execution, planning, editing, and delegation primitives, so the migration collapses that structure into:

- a small root instruction file
- repo-local configuration
- a few workflow documents
- lightweight templates
- one reusable PR skill

This reduces duplication while preserving the original operating intent.

## Configuration

- `AGENTS.md` contains durable repository instructions that should load for every Codex session.
- `.codex/config.toml` contains shared repository defaults such as sandbox, approval, and MCP setup.
- `~/.codex/config.toml` should keep personal preferences, credentials, and repository trust decisions.
- The Coplay MCP server is configured for Unity editor workflows; add more MCP servers only when they remove a real repeated manual step.

## Role Skills

The repository now includes Codex-facing role modules that mirror the old Claude agent split:

- `feature-planner`
- `feature-implementer`
- `test-runner`
- `code-validator`
- `play-verifier`
- `bug-diagnostician`
- `test-failure-analyzer`
- `feature-check`
- `feature-orchestrator`
- `bugfix-orchestrator`

OpenAI's current Codex guidance describes shared team skills under `.agents/skills`.
This repository currently keeps skills under `.codex/skills` because that is the active local runtime convention for this project.
Do not duplicate the skill tree during normal feature work; migrate deliberately if the team standard changes.
