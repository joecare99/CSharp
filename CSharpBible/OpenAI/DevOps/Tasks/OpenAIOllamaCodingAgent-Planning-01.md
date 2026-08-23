# Task: OpenAI/Ollama Coding Agent Planning Loop Wave 01

## Parent
- Backlog Item: [PBI-19 Goal-Focused Task Planning and Execution](../BacklogItems/PBI-19-GoalFocusedTaskPlanningAndExecution.md)

## Goal
Add an explicit planning loop so the agent can keep the main objective in focus while executing complex multi-step coding work.

## Scope
- add goal contract and plan-state model to runtime
- decompose prompts into bounded subtasks with dependency metadata
- run subtasks with goal-alignment checks and drift detection
- expose concise plan status summaries and resume points

## Existing Project Integration
- Extend `Ollama.CodingAgent` runtime with planning and checkpoint components.
- Reuse delegated tools from current sprint-2 scope for subtask execution.
- Keep planning diagnostics compatible with sprint-3 evaluation outputs.

## Recommended Implementation Order
1. Add goal contract and plan-state schema.
2. Add decomposition policy for subtasks and dependencies.
3. Add subtask execution controller with drift checks.
4. Add checkpoint snapshots and resume support.
5. Add focused planner and drift tests.

## Subtasks
1. Implement `GoalContract` and `PlanState` models.
2. Implement `SubtaskPlanner` with deterministic decomposition rules.
3. Implement `PlanExecutionController` with goal-check hooks.
4. Implement drift signal model and recovery actions.
5. Implement plan summary renderer and resume logic.

## Assumptions
- explicit plan state reduces objective drift on long-running delegated flows
- drift checks should be lightweight and deterministic for local execution
- the first wave can use heuristic decomposition before model-based planning upgrades

## Exit Criteria
- complex task prompt can be decomposed into at least 3 ordered subtasks
- execution can report progress and detect out-of-goal behavior
- resume path can continue from a persisted checkpoint
- tests validate decomposition, drift checks, and resume consistency

## Status
Done

## Status Log
- 2026-08-13: Implemented first planning-layer code artifacts in `Ollama.CodingAgent` (goal contract, plan state, subtask planner, drift analyzer, and plan rendering) and integrated them into delegated task execution flow.
- 2026-08-13: Verified delegated live run output now includes explicit goal contract and subtask status rendering to keep long-running tasks aligned with the primary objective.
- 2026-08-13: Added `Ollama.CodingAgent.HostCheck.Planning` for real planning-loop checks and malformed planning-input validation.
- 2026-08-13: Executed `Ollama.CodingAgent.HostCheck.Planning`; normal plan rendering and malformed-input checks both behaved as expected.
- 2026-08-13: Added dependency-aware ready-subtask resolution and integrated it into delegated execution.
- 2026-08-13: Added `PlanStateStore` JSON checkpoint save/load support for resumable plans.
- 2026-08-13: Added dependency and checkpoint round-trip coverage; 54 coding-agent tests pass.
