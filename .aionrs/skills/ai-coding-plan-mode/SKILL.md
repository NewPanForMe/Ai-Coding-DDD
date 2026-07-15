---
name: ai-coding-plan-mode
description: >
  Plan-first execution mode for complex development tasks. Implements an "Explore → Plan → Approve → Execute" workflow
  that prevents blind modification by forcing AI to gather context, create a structured plan, get user approval, and only
  then execute with a validated approval token. Use this skill when: (1) user explicitly requests plan mode,
  (2) tasks involve multi-file code generation or refactoring, (3) operations have irreversible consequences (database
  changes, file deletions, configuration changes), (4) user wants to review changes before execution, (5) complex
  bug fixes requiring deep analysis, (6) architecture or design decisions that need user consensus.
---

# Plan Mode

## Overview

Plan Mode enforces a structured "plan first, execute later" workflow for AI-driven development tasks. It prevents premature or blind modifications by requiring:

1. **Exploration** — gather context via read-only operations
2. **Planning** — construct a detailed Markdown plan
3. **Approval** — user reviews and explicitly approves
4. **Execution** — modifications proceed only with a validated approval token

## Workflow

### Phase 1: Enter Plan Mode

Declare entry into plan mode. State the explicit constraint: **no write operations (file edits, command execution) are permitted until approval is granted.**

### Phase 2: Exploration (Read-Only)

Gather all necessary context before drafting a plan:

- Read relevant files with the `Read` tool
- Search code with `Grep`
- List directory structures with `Glob`
- Analyze dependencies and impact scope

**Constraint: Zero writes allowed in this phase.**

### Phase 3: Build the Plan

Construct a structured Markdown plan file. The plan must include:

```markdown
# Plan: [brief title]

**Created**: [timestamp]
**Scope**: [affected files, modules, or systems]
**Risk Level**: [low / medium / high]
**Rollback Strategy**: [how to undo if something goes wrong]

## Summary
[2-3 sentence description of what will be done]

## Steps

### Step 1: [action name]
- **Operation**: [read/edit/create/delete]
- **Target**: [file path or entity]
- **Description**: [what will be done]
- **Expected Result**: [what should happen after this step]

### Step 2: [action name]
...
```

### Phase 4: Submit for Approval

Present the complete plan to the user. The plan file should be saved to the working directory as `plan-<topic>.md`.

Wait for explicit user feedback:

- `approve` / `批准` / `执行` → proceed to Phase 5
- `reject` / `拒绝` / `修改` + feedback → return to Phase 3

**Do not proceed without explicit approval.**

### Phase 5: Execution

After approval, execute each step in order:

1. Work through steps sequentially
2. After each step, verify the result matches expectations
3. If a step fails, pause and ask the user before continuing
4. After all steps complete, provide a summary of changes

### Cancellation

User may cancel plan mode at any time by saying `cancel` / `取消`. Clean up temporary plan files and exit plan mode.

## State Persistence

### Plan File

Plans are saved as Markdown files at `plan-<topic>.md` in the current working directory. The file serves as:

- An audit trail of what was planned
- A reference for the execution phase
- A document for future review

### Approval Token

Approval is tracked conversationally (user confirmation). No file system token is required — the explicit user `approve` message is the gate.

## Constraint Rules

### Mandatory

- 【强制】No write operations (`Edit`, `Write`, `ExecCommand` with side effects) before explicit user approval
- 【强制】Every plan must include a rollback strategy
- 【强制】Do not skip steps in the workflow
- 【强制】If execution deviates from the plan, pause and re-submit for approval

### Recommended

- 【推荐】For plans with 5+ steps, batch steps into logical groups
- 【推荐】Include estimated impact (lines changed, files affected) in step descriptions
- 【推荐】Reference specific code snippets in the plan for clarity

## Tool Usage During Plan Mode

| Phase | Allowed Tools | Forbidden Tools |
|-------|---------------|-----------------|
| Exploration | `Read`, `Grep`, `Glob` | `Edit`, `Write`, `ExecCommand` |
| Planning | `Read`, `Grep`, `Glob`, `Write` (plan file only) | `Edit` (on non-plan files), `ExecCommand` |
| Execution | All tools | None (with approval) |

## References

- **Detailed design**: See [references/design.md](references/design.md) for the full architecture, token mechanism, and multi-role extensions
