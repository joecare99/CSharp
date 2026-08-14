# Backlog Item: Add Tool Execution and Memory Integration

## Feature Link
[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status
Done

## Description
As an engineer, I want safe tool execution and structured memory support so that the agent can solve realistic coding tasks across multiple turns without losing context or exceeding safety boundaries.

## Acceptance Criteria
- Tool registry and policy enforcement are implemented.
- Tool-call lifecycle (`request -> execute -> reinject`) works in the runtime loop.
- Session memory can persist and retrieve relevant context for later turns.
- Tests cover denied operations, tool failures, and memory retrieval correctness.

## Primary Project Targets
- `Ollama.Tools` (tool orchestration, loop runner, registry)
- `Ollama.Tools.Tests` (tool/memory safety and behavior verification)
- `McpTools` (optional integration surface for future external tool adapters)

## Tasks
- Define tool contract and allowlist-driven execution policy.
- Implement tool bridge for command and file operations with guardrails.
- Implement session-memory schema and retrieval strategy.
- Add integration tests for tool cycle and memory-assisted turns.
- Define extension points for external knowledge connectors and local wiki tools.

## Test Tasks
- Add allow/deny policy tests for path and command boundaries.
- Add memory retrieval precision tests for multi-turn follow-up prompts.

## Dependencies
- Depends on: [PBI-15](./PBI-15-ProviderAgnosticAgentRuntime.md)

## Open Questions
- Should command execution start read-only and require explicit opt-in for write operations?
- Which retention defaults should be used for session-memory trimming?
- Should local wiki ingestion happen only via explicit agent tools or also from post-run summarization hooks?

## Completion Log
- 2026-08-13: Added explicit tool execution policy contracts and an allowlist implementation; unknown tools remain denied and registered tools can be restricted by host policy.
- 2026-08-13: Added bounded multi-turn tool execution with request, execution, result reinjection, final-response detection, and iteration-limit handling.
- 2026-08-13: Added `JsonSessionMemoryStore` with session scoping, durable local persistence, bounded retention, and token-overlap relevance retrieval.
- 2026-08-13: Added policy, tool-loop, persistence, retrieval, and retention tests; `Ollama.Tools.Tests` passes 103 tests.
