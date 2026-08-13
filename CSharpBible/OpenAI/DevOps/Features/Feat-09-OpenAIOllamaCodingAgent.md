# Feature: OpenAI/Ollama Coding Agent Platform

## Epic Link
[Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](../Epics/Epic-OpenAI-Ollama.md)

## Status
Draft

## Description
Build a full-capability coding agent in C# that can run against local Ollama and OpenAI-compatible endpoints, starting with a stable local baseline (`qwen2.5-coder:7b`) and expanding into orchestration, memory, tool execution, and quality gates.

## Goals
- Provide a reusable agent host abstraction that is provider-agnostic.
- Validate local Ollama reliability first using `qwen2.5-coder:7b`.
- Support multi-turn planning and execution with bounded tool access.
- Add memory and session context support with explicit policies.
- Add evaluation and hardening so the agent can be used repeatedly for real engineering tasks.

## Non-Goals (for this feature slice)
- Building a production cloud control-plane.
- Replacing all existing sample apps immediately.
- Solving broad autonomous orchestration for unrelated domains in the first increment.

## Architecture Intent
1. **Provider layer:** OpenAI-compatible and Ollama providers behind one contract.
2. **Agent runtime layer:** prompt policy, planning loop, step execution, stop conditions.
3. **Tooling layer:** safe command execution, repository interaction, and bounded file operations.
4. **Memory layer:** session transcript, short-term state, optional long-term retrieval.
5. **Evaluation layer:** scenario tests, quality metrics, and regression checks.

## Existing Code and Project Reuse Plan
- `Ollama.Protocol` for low-level `/api/tags`, `/api/chat`, `/api/generate`, and `/api/embed` transport.
- `Ollama.Client` for model-scoped chat/generate/embed abstractions and input validation.
- `Ollama.Tools` for tool orchestration components (`OllamaToolOrchestrator`, `OllamaToolLoopRunner`).
- `Ollama.Extensions.DependencyInjection` for host composition and service registration.
- `Ollama.Samples.*` projects as behavior references and smoke-scenario seeds.
- `OpenAIPlayground` as the initial OpenAI-compatible provider reference host.
- `McpTools` as an optional future bridge for external tool exposure after core runtime stability.

## Acceptance Criteria
- A C# agent host can run at least one end-to-end coding scenario with local Ollama (`qwen2.5-coder:7b`).
- The same orchestration path can be configured for an OpenAI-compatible provider without code duplication.
- Tool execution and memory integration are test-covered and bounded by explicit policy.
- A roadmap and sprint-ready implementation tasks exist in `DevOps`.

## Scrum Breakdown
- Sprint 0 baseline task: [OpenAIOllamaCodingAgent-Sprint0-01](../Tasks/OpenAIOllamaCodingAgent-Sprint0-01.md)
- Sprint 1 runtime task: [OpenAIOllamaCodingAgent-Sprint1-01](../Tasks/OpenAIOllamaCodingAgent-Sprint1-01.md)
- Sprint 2 tools and memory task: [OpenAIOllamaCodingAgent-Sprint2-01](../Tasks/OpenAIOllamaCodingAgent-Sprint2-01.md)
- Sprint 3 hardening task: [OpenAIOllamaCodingAgent-Sprint3-01](../Tasks/OpenAIOllamaCodingAgent-Sprint3-01.md)

## Definition of Done for the Feature
- All linked PBIs have implementation evidence and updated status.
- Targeted tests for runtime, provider adapters, tools, and memory are green.
- At least one representative coding scenario is reproducible with local Ollama and documented run steps.
- Known limitations and deferred work are explicitly listed in DevOps artifacts.

## Linked Backlog Items
- [PBI-14: Establish Local Ollama Baseline and Agent Skeleton](../BacklogItems/PBI-14-LocalOllamaBaselineAndAgentSkeleton.md)
- [PBI-15: Implement Provider-Agnostic Agent Runtime](../BacklogItems/PBI-15-ProviderAgnosticAgentRuntime.md)
- [PBI-16: Add Tool Execution and Memory Integration](../BacklogItems/PBI-16-ToolExecutionAndMemoryIntegration.md)
- [PBI-17: Add Evaluation, Hardening, and Delivery Readiness](../BacklogItems/PBI-17-AgentEvaluationHardeningAndReadiness.md)

## Open Questions
- Should the first agent host live in `OpenAIPlayground` or a dedicated `Agent` project?
- Which tool categories are mandatory in wave 1 versus wave 2?
- How strict should the first safety policy be for local command execution?
