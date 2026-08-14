# Project: OpenAI/Ollama Coding Agent

## Status
Draft

## Purpose
Define and implement a reusable C# coding-agent runtime that can operate with local Ollama and OpenAI-compatible providers while reusing existing OpenAI solution components.

## Planned Composition from Existing Projects
- **Transport and protocol:** `Ollama.Protocol`
- **Model-scoped client abstraction:** `Ollama.Client`
- **Tool orchestration:** `Ollama.Tools`
- **Dependency injection host composition:** `Ollama.Extensions.DependencyInjection`
- **OpenAI-compatible reference flow:** `OpenAIPlayground`
- **Scenario and smoke references:** `Ollama.Samples.*`
- **Dedicated live-response host checks:** `Ollama.CodingAgent.HostCheck`
- **External source and connector bridge (optional):** `McpTools`

## Confirmed Implementation Decision
- The coding agent will be implemented as a **dedicated project**.
- The project should use `Ollama.Client` as its primary local-model access path.
- Compatibility with the `OpenAIPlayground` flow should be preserved where practical.

## Confirmed Scope Decisions (2026-08-13)
- Sprint 0/1 are **Ollama-first**; OpenAI-compatible adapter work follows later.
- Initial tool safety profile is **controlled write + execute** with strict allowlist and path boundaries.
- First memory scope is **session-only** (no mandatory long-term persistence in the first increment).
- MVP success scenarios are mandatory:
  1. read and summarize code,
  2. perform a targeted file change,
  3. run build/tests.
- Model baseline is `qwen2.5-coder:7b` first, then configurable model substitution in later increments.

## Integration Boundaries
- Keep provider-specific details in adapters, not in the core runtime loop.
- Keep host-specific UI concerns outside shared runtime and tool assemblies.
- Keep safety and diagnostics contracts reusable across console and WPF hosts.
- Keep external knowledge usage bounded by source allowlist and citation requirements.
- Keep local wiki entries curated summaries instead of raw unfiltered web mirrors.
- Keep plan-state and subtask execution separated from single tool implementations to avoid goal drift coupling.

## Related Planning Items
- [Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)
- [Roadmap: OpenAI/Ollama Coding Agent Delivery Plan](../Roadmaps/OpenAIOllamaCodingAgent-DeliveryPlan.Info.md)
- [PBI-14 Local Ollama Baseline and Agent Skeleton](../BacklogItems/PBI-14-LocalOllamaBaselineAndAgentSkeleton.md)
- [PBI-15 Provider-Agnostic Agent Runtime](../BacklogItems/PBI-15-ProviderAgnosticAgentRuntime.md)
- [PBI-16 Tool Execution and Memory Integration](../BacklogItems/PBI-16-ToolExecutionAndMemoryIntegration.md)
- [PBI-17 Agent Evaluation Hardening and Readiness](../BacklogItems/PBI-17-AgentEvaluationHardeningAndReadiness.md)
- [PBI-18 Web Knowledge and Local LLM Wiki](../BacklogItems/PBI-18-WebKnowledgeAndLocalLlmWiki.md)
- [PBI-19 Goal-Focused Task Planning and Execution](../BacklogItems/PBI-19-GoalFocusedTaskPlanningAndExecution.md)
