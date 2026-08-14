# Task: OpenAI/Ollama Coding Agent Sprint 1 Runtime

## Parent
- Backlog Item: [PBI-15 Provider-Agnostic Agent Runtime](../BacklogItems/PBI-15-ProviderAgnosticAgentRuntime.md)

## Goal
Implement the provider-agnostic multi-turn runtime and normalize provider-specific behaviors behind one orchestration contract.

## Scope
- create runtime state machine and turn orchestration
- add provider adapter contract with capability metadata
- implement Ollama and OpenAI-compatible adapters
- add timeout/cancellation/retry behavior with test coverage

## Existing Project Integration
- Keep Ollama adapter path on `Ollama.Client` + `Ollama.Protocol` contracts.
- Use `OpenAIPlayground` as the starting host for OpenAI-compatible adapter extraction.
- Reuse DI patterns from `Ollama.Extensions.DependencyInjection`.
- Reuse the dedicated `Ollama.CodingAgent` host and its runtime defaults as the primary runtime baseline.

## Recommended Implementation Order
1. Define runtime state machine and result envelope.
2. Implement provider contract and adapter registration.
3. Add Ollama adapter first, then OpenAI-compatible adapter.
4. Add failure-handling policy (timeouts, retries, cancellation).
5. Add unit tests for transition and error behavior.

## Subtasks
1. Define canonical message and turn models.
2. Implement planner-executor loop with max-iteration policy.
3. Add provider capability negotiation (streaming/tool-call support flags).
4. Implement adapter normalization for provider response differences.
5. Add retry/cancellation policy and diagnostics hooks.
6. Add MSTest suites for state transitions and error paths.
7. Apply and test baseline runtime defaults:
   - timeout >= 10 minutes
   - retries = 3
   - max iterations = 80

## Assumptions
- provider APIs differ in details but can map to one internal envelope
- runtime loop should not depend on UI concerns
- first increment can use deterministic test doubles for transport behavior
- default limits are tuned for slower local model execution and can be overridden by configuration

## Exit Criteria
- one orchestration path can run with both providers
- runtime failure behavior is explicit and tested
- no provider-specific logic leaks into core runtime services

## Status
Done

## Status Log
- 2026-08-13: Implemented first provider-agnostic runtime skeleton with configurable timeout/retry/iteration defaults in dedicated `Ollama.CodingAgent` project.
- 2026-08-13: Runtime diagnostics, optional thinking capture, and live delegated execution are validated.
- 2026-08-13: Added `IAgentProviderClient`, `AgentProviderCapabilities`, the Ollama capability implementation, and the provider-specific `OpenAI.CodingAgent` adapter.
- 2026-08-13: Added OpenAI-compatible adapter normalization tests; all 41 coding-agent tests pass.
