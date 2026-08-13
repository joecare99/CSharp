# Backlog Item: Implement Provider-Agnostic Agent Runtime

## Feature Link
[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status
Draft

## Description
As an engineer, I want a provider-agnostic runtime with a deterministic multi-turn state machine so that the same C# agent orchestration can run against Ollama and OpenAI-compatible backends.

## Acceptance Criteria
- Shared internal message and turn model exists.
- Runtime loop supports planning, execution, and stop conditions.
- Provider adapters implement one shared contract with explicit capability flags.
- Cancellation, timeout, and retry behavior are test-covered.
- Runtime defaults are configurable, with baseline values:
  - timeout >= 10 minutes for long-running local model calls
  - retry count = 3
  - max iteration cap = 80

## Primary Project Targets
- `Ollama.Client` (shared model-scoped operations for Ollama provider path)
- `OpenAIPlayground` (OpenAI-compatible provider reference and migration seed)
- `Ollama.Extensions.DependencyInjection` (runtime registration model)

## Tasks
- Define runtime state machine and stop conditions.
- Implement provider contract and envelope normalization.
- Add Ollama and OpenAI-compatible adapters.
- Add unit tests for success, timeout, retry, and cancellation paths.

## Test Tasks
- Add transition-coverage tests for all state machine branches.
- Add adapter normalization tests with provider-specific fixture payloads.

## Dependencies
- Depends on: [PBI-14](./PBI-14-LocalOllamaBaselineAndAgentSkeleton.md)

## Open Questions
- Should streaming support be mandatory in sprint 1 or feature-flagged for sprint 2?
