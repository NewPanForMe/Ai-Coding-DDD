# Plan Mode: Detailed Architecture Design

> Source: Based on the "Skills-based Plan Mode Implementation" design document.

## 1. Core Philosophy

- **Goal**: Force AI to plan before executing, preventing blind modifications and improving task controllability.
- **Constraint**: Tools only support Skills (function calls); no system-level permission control; no global state.
- **Strategy**: Decompose "plan mode" into a set of semantically clear, single-responsibility Skills. Use prompt engineering to enforce the "Explore → Plan → Approve → Execute" fixed workflow. Use temporary files or parameter passing for state and permission tokens.

---

## 2. Skill System Design

### 2.1 Atomic Skills (Read/Write Separation)

| Category | Skill | Function | Notes |
|----------|-------|----------|-------|
| Read-Only | `read_file` | Read file contents | |
| Read-Only | `list_directory` | List directory | |
| Read-Only | `search_code` | Search code | |
| Read-Only | `get_file_tree` | Get project tree | |
| Write | `write_file` | Write/overwrite file | Requires `plan_approval_token` |
| Write | `run_command` | Execute shell command | Requires `plan_approval_token` |

### 2.2 Process Management Skills (State & Lifecycle)

| Skill | Function | Output |
|-------|----------|--------|
| `start_plan_session` | Declare entry into plan mode, initialize draft | Session ID (or empty) |
| `append_plan_item` | Append an execution step to plan draft | Current draft summary |
| `finalize_plan` | Complete plan, generate Markdown file, wait approval | Plan file path or full content |
| `approve_plan` | User approves, return execution token | Token and execution instructions |
| `reject_plan` | User rejects, clear draft | Re-planning prompt |

### 2.3 State Persistence

- Plan content saved as temporary file (e.g., `plan-<topic>.md`)
- After approval, compute file hash or generate random token as required parameter for write skills

---

## 3. Workflow & Prompt Constraints

Enforce the following behavior sequence in system prompt:

1. **Call `start_plan_session`** (pass overall goal)
2. **Read-only exploration**: Use `read_file`, `search_code`, `get_file_tree` to collect information
3. **Step-by-step planning**: Multiple calls to `append_plan_item` to add specific steps
4. **Submit for approval**: Call `finalize_plan` to generate plan, return to user
5. **Wait for user feedback**: User inputs "approve" or "reject"
6. **Execution phase**
   - If approved: Call `approve_plan` to get token
   - Carry token when calling write skills (`write_file`, `run_command`) to complete steps
   - Update progress after each step (optional)
7. **If rejected**: Call `reject_plan` to return to planning phase, re-adjust

> Prompt must explicitly prohibit skipping steps and inform AI that write skills will fail without a token.

---

## 4. Key Implementation Details

### Permission Token

- `approve_plan` returns a token bound to the plan file content (e.g., HMAC signature)
- Write skills validate token validity to prevent forgery

### Error Handling

- Write skills that receive no token or an invalid token return clear error messages and guide AI to complete approval

### Observability

- All skills log invocation records for debugging workflow violations

### Multi-Role Extension (Optional)

- Can split into Planner (read-only + plan) and Executor (write) skill sets for clearer responsibility separation

---

## 5. Solution Analysis

### Advantages

- **Lightweight**: No modification to tool infrastructure needed; achievable purely through Skill design
- **High controllability**: Enforced workflow prevents AI from taking shortcuts, reduces accidental modifications
- **Auditable**: Plan files saved as Markdown for easy manual review and version tracking
- **Rollback-friendly**: Complete plan exists before execution; if errors occur during execution, can quickly restore to plan state

### Limitations

- **Prompt dependency**: If user modifies system prompt, workflow may be bypassed (but write token mechanism can still block unauthorized operations)
- **State maintenance cost**: Need to manage temporary files; multi-session concurrency requires file isolation
- **Frequent user interaction**: Every plan requires manual approval; unsuitable for highly automated scenarios

### Suitable Scenarios

- Code generation, refactoring, bug fixing, and other development tasks requiring explicit plans
- Multi-person collaboration environments requiring consensus before modifications
- Educational or beginner scenarios, helping users understand task decomposition and planning

### Improvement Directions

- Support plan template reuse to reduce repetitive planning
- Add automated testing steps to verify plan feasibility before approval
- Integrate with external knowledge bases to improve planning quality

---

## 6. Mapping to AionUi Tools

This design maps to AionUi's actual tool set:

| Design Skill | AionUi Equivalent | Notes |
|-------------|-------------------|-------|
| `read_file` | `Read` | Built-in |
| `list_directory` | `Glob` | Built-in |
| `search_code` | `Grep` | Built-in |
| `get_file_tree` | `Glob` with `**/*` | Built-in |
| `write_file` | `Write` / `Edit` | Built-in; gated by plan mode |
| `run_command` | `ExecCommand` | Built-in; gated by plan mode |
| `start_plan_session` | EnterPlanMode | Deferred tool in AionUi |
| `finalize_plan` | Plan file creation + user confirmation | Manual in this skill |
| `approve_plan` | User message "approve" | Conversational gate |
| `reject_plan` | User message "reject" | Conversational gate |

The AionUi `EnterPlanMode` / `ExitPlanMode` deferred tools provide native plan mode support. This skill augments and standardizes the behavior within that mode.
