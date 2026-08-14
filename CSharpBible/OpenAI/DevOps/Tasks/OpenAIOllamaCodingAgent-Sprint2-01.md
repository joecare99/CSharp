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
- prepare extension seams for trusted web-knowledge and local wiki tools

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
Done

## Status Log
- 2026-08-13: Added first delegated coding-task mode in `Ollama.CodingAgent` with safe workspace tools (`list_workspace_files`, `read_workspace_file`, `run_dotnet_build`) to transfer selected subtasks to the agent.
- 2026-08-13: Verified delegated live run against local `qwen2.5-coder:7b`; the agent selected `list_workspace_files`, executed it successfully, and produced a coding next-step summary based on tool output.
- 2026-08-13: Extended delegation from single-step to bounded multi-step execution (up to 3 delegated tool steps) with step history folded into final agent synthesis.
- 2026-08-13: Hardened delegated mode against slow-model stalls by adding timeout-safe fallback behavior for tool selection and final summary synthesis; short-timeout live run now exits deterministically with a safe actionable fallback summary.
- 2026-08-13: Expanded delegated coding toolset with controlled write support (`write_workspace_file`) and test execution support (`run_dotnet_test`) to cover end-to-end coding loops beyond read/build only.
- 2026-08-13: Added deterministic fallback tool planning when model-based tool selection fails (intent-based fallback to `list_workspace_files`, `run_dotnet_build`, or `run_dotnet_test`) so delegated execution can still make concrete progress.
- 2026-08-13: Hardened tool-input parsing to be case-insensitive and validated fallback build execution in delegated mode (`run_dotnet_build` executed successfully after selection timeout).
- 2026-08-13: Fixed delegated `run_dotnet_test` execution for the repository's Microsoft.Testing.Platform setup by removing incompatible `--nologo`, using a workspace-relative project path, and adding `--no-restore`; live delegation then executed 35 tests successfully three times.
- 2026-08-13: Added explicit allowlist-based tool execution policy with denied-operation results.
- 2026-08-13: Added bounded request-execute-reinject tool cycles with final-response and iteration-limit handling.
- 2026-08-13: Added durable, session-scoped JSON memory with bounded retention and relevance retrieval.
- 2026-08-13: Added tool-policy, lifecycle, memory persistence, retrieval, and retention coverage; 103 tool tests pass.
