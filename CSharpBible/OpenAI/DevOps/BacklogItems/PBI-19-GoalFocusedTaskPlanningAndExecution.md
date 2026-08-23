# Backlog Item: Goal-Focused Task Planning and Execution

## Feature Link
[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status
Done

## Description
As an engineer, I want explicit goal-aware planning and subtask orchestration so that the coding agent can solve complex tasks without losing the global objective while still executing targeted side tasks.

## Acceptance Criteria
- The agent stores a primary goal contract for each run.
- Complex prompts are decomposed into explicit subtasks with dependencies.
- Each subtask is evaluated against the primary goal before and after execution.
- Drift detection flags when subtask activity diverges from the main objective.
- Plan state can be summarized and resumed.

## Primary Project Targets
- `Ollama.CodingAgent` (planner, plan state, drift checks)
- `Ollama.Tools` (task-execution tools reused by planned subtasks)
- `Ollama.CodingAgent.Tests` (planner and drift-safety tests)

## Tasks
- Define goal contract and plan-state model.
- Implement subtask decomposition and dependency tracking.
- Implement plan loop checkpoints and drift-detection rules.
- Add resume/snapshot support for partial progress.

## Test Tasks
- Add tests for decomposition quality and dependency ordering.
- Add tests for drift detection and recovery behavior.
- Add tests for plan resume consistency.

## Dependencies
- Depends on: [PBI-15](./PBI-15-ProviderAgnosticAgentRuntime.md)
- Depends on: [PBI-16](./PBI-16-ToolExecutionAndMemoryIntegration.md)

## Open Questions
- Should plan state be persisted only per session or across sessions by default?
- Which drift threshold should force replanning versus continue execution?

## Completion Log

- 2026-08-13: Added explicit subtask dependency metadata and ready-subtask resolution to the planning model.
- 2026-08-13: Integrated dependency-aware selection into delegated execution so blocked subtasks are not executed prematurely.
- 2026-08-13: Added JSON plan checkpoints with goal, criteria, dependencies, and subtask statuses for resume support.
- 2026-08-13: Added dependency-ordering and checkpoint round-trip tests; `Ollama.CodingAgent.Tests` passes 54 tests.
- 2026-08-13: Planning host check confirms goal rendering and malformed-input safeguards.
