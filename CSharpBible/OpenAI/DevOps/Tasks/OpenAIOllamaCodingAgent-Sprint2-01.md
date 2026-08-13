# Task: OpenAI/Ollama Coding Agent Sprint 2 Tools and Memory

## Parent
- Backlog Item: [PBI-16 Tool Execution and Memory Integration](../BacklogItems/PBI-16-ToolExecutionAndMemoryIntegration.md)

## Goal
Enable practical coding-task execution through safe tools and memory-backed context continuity.

## Scope
- implement tool registry and allowlist policy
- add tool-call lifecycle integration into runtime loop
- implement session memory write/read path
- add safety and correctness tests for tool and memory behavior

## Existing Project Integration
- Extend `Ollama.Tools` around `OllamaToolOrchestrator` and `OllamaToolLoopRunner`.
- Reuse `Ollama.Samples.ToolUse` as the reference end-to-end flow.
- Keep memory-related contracts decoupled so they can be reused by non-console hosts.

## Recommended Implementation Order
1. Define tool contract and policy constraints.
2. Implement tool execution adapters with bounded filesystem/process access.
3. Integrate tool-call cycle into runtime orchestration.
4. Implement session memory storage and retrieval strategy.
5. Add integration tests for multi-turn tool and memory scenarios.

## Subtasks
1. Implement allowlist policy for tool categories and paths.
2. Add explicit error contracts for denied and failed tool calls.
3. Integrate tool results back into conversation state.
4. Add session context store schema and retention limits.
5. Add retrieval heuristics for relevant context selection.
6. Add MSTest/NSubstitute coverage for policy enforcement and failure paths.

## Assumptions
- safety defaults should deny unknown tools and out-of-scope paths
- memory should remain scoped and explicit rather than unbounded recall
- first version can prioritize deterministic local persistence over distributed storage

## Exit Criteria
- agent can execute at least one multi-turn coding flow with tools
- memory-assisted follow-up turns recover prior relevant context
- denied/failed tool behavior is explicit and test-covered

## Status
In Progress

## Status Log
- 2026-08-13: Added first delegated coding-task mode in `Ollama.CodingAgent` with safe workspace tools (`list_workspace_files`, `read_workspace_file`, `run_dotnet_build`) to transfer selected subtasks to the agent.
- 2026-08-13: Verified delegated live run against local `qwen2.5-coder:7b`; the agent selected `list_workspace_files`, executed it successfully, and produced a coding next-step summary based on tool output.
- 2026-08-13: Extended delegation from single-step to bounded multi-step execution (up to 3 delegated tool steps) with step history folded into final agent synthesis.
