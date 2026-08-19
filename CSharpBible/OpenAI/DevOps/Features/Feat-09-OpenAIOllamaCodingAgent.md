# Feature: OpenAI/Ollama Coding Agent Platform

## Epic Link
[Epic: OpenAI/Ollama Platform and Shared Client Ecosystem](../Epics/Epic-OpenAI-Ollama.md)

## Status

In Progress

## Completion Boundary

The local interactive-client readiness slice is complete through PBI-24: shared session
persistence/resume, approval-gated local Git operations, terminal and desktop adapters, and their
operator safety documentation have deterministic local validation. Feature 09 remains in progress
because PBI-15 through PBI-19 retain the provider-parity, memory reinjection, evaluation,
knowledge-quality, and persisted planning-checkpoint work described below. PBI-24 does not claim
live Ollama, external Git-host, or autonomous end-to-end readiness.

## Description
Build a full-capability coding agent in C# that can run against local Ollama and OpenAI-compatible endpoints, starting with a stable local baseline (`qwen2.5-coder:7b`) and expanding into orchestration, memory, tool execution, and quality gates.

## Goals
- Provide a reusable agent host abstraction that is provider-agnostic.
- Validate local Ollama reliability first using `qwen2.5-coder:7b`.
- Support multi-turn planning and execution with bounded tool access.
- Add memory and session context support with explicit policies.
- Add trusted external knowledge retrieval (for example Wikipedia, Rosetta Code, Microsoft Learn) with citations.
- Add local LLM wiki build/read workflows for reusable coding knowledge.
- Add evaluation and hardening so the agent can be used repeatedly for real engineering tasks.

## Non-Goals (for this feature slice)
- Building a production cloud control-plane.
- Replacing all existing sample apps immediately.
- Solving broad autonomous orchestration for unrelated domains in the first increment.

## Architecture Intent
1. **Provider layer:** OpenAI-compatible and Ollama providers behind one contract.
2. **Agent runtime layer:** prompt policy, planning loop, step execution, stop conditions.
3. **Planning layer:** goal contract, subtask decomposition, dependency ordering, drift detection.
4. **Tooling layer:** safe command execution, repository interaction, and bounded file operations.
5. **Memory layer:** session transcript, short-term state, optional long-term retrieval.
6. **Diagnostics layer:** provider-neutral, redacted LLM request/response logging with session-scoped storage.
7. **Evaluation layer:** scenario tests, quality metrics, and regression checks.

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
- Sprint 4 shared application task: [OpenAIOllamaCodingAgent-Sprint4-01-SharedApplication](../Tasks/OpenAIOllamaCodingAgent-Sprint4-01-SharedApplication.md)
- Sprint 5 Git workspace task: [OpenAIOllamaCodingAgent-Sprint5-01-GitWorkspace](../Tasks/OpenAIOllamaCodingAgent-Sprint5-01-GitWorkspace.md)
- Sprint 6 persistent terminal task: [OpenAIOllamaCodingAgent-Sprint6-01-PersistentTerminal](../Tasks/OpenAIOllamaCodingAgent-Sprint6-01-PersistentTerminal.md)
- Sprint 7 Avalonia desktop task: [OpenAIOllamaCodingAgent-Sprint7-01-AvaloniaDesktop](../Tasks/OpenAIOllamaCodingAgent-Sprint7-01-AvaloniaDesktop.md)
- Sprint 8 operational readiness task: [OpenAIOllamaCodingAgent-Sprint8-01-OperationalReadiness](../Tasks/OpenAIOllamaCodingAgent-Sprint8-01-OperationalReadiness.md)

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
- [PBI-18: Add Web Knowledge Retrieval and Local LLM Wiki](../BacklogItems/PBI-18-WebKnowledgeAndLocalLlmWiki.md)
- [PBI-19: Goal-Focused Task Planning and Execution](../BacklogItems/PBI-19-GoalFocusedTaskPlanningAndExecution.md)
- [PBI-20: Shared Interactive Agent Application Layer](../BacklogItems/PBI-20-SharedInteractiveAgentApplicationLayer.md)
- [PBI-21: Approval-Gated Git Workspace Provider](../BacklogItems/PBI-21-ApprovalGatedGitWorkspaceProvider.md)
- [PBI-22: Persistent Coding-Agent Terminal Client](../BacklogItems/PBI-22-PersistentTerminalClient.md)
- [PBI-23: Avalonia Coding-Agent Desktop Client](../BacklogItems/PBI-23-AvaloniaDesktopClient.md)
- [PBI-24: Interactive Coding-Agent Operational Readiness](../BacklogItems/PBI-24-InteractiveAgentOperationalReadiness.md)
- [PBI-25: LLM Debug Traffic Logging](../BacklogItems/PBI-25-LlmDebugTrafficLogging.md)

## Open Questions
- Should the first agent host live in `OpenAIPlayground` or a dedicated `Agent` project?
- Which tool categories are mandatory in wave 1 versus wave 2?
- How strict should the first safety policy be for local command execution?
