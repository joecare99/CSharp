# OpenAI/Ollama Coding Agent Delivery Plan

## Vision
Deliver a full-capability C# coding agent that can execute practical software-engineering workflows with local Ollama first and OpenAI-compatible providers second, without rewriting core orchestration.

## Delivery Strategy
- **Local-first baseline:** stabilize Ollama + `qwen2.5-coder:7b` before advanced features.
- **Provider-agnostic runtime:** isolate provider-specific request/response shaping.
- **Incremental capability growth:** foundation -> runtime -> tools+memory -> hardening.
- **Evidence-driven completion:** every increment includes scenario-level validation.

## Confirmed Constraints
- A dedicated coding-agent project will be used.
- Sprint 0 and Sprint 1 follow an Ollama-first rollout.
- Baseline model for early delivery is `qwen2.5-coder:7b`.
- Model selection becomes configurable after the initial baseline is stable.
- MVP must include code-read summary, targeted edit, and build/test execution flows.
- Runtime defaults for the local baseline:
  - timeout: **10+ minutes** per long model step
  - retries: **3**
  - max iterations: **80** hard cap

## Ordered Backlog Sequence
1. [PBI-14 Local Ollama Baseline and Agent Skeleton](../BacklogItems/PBI-14-LocalOllamaBaselineAndAgentSkeleton.md)
2. [PBI-15 Provider-Agnostic Agent Runtime](../BacklogItems/PBI-15-ProviderAgnosticAgentRuntime.md)
3. [PBI-16 Tool Execution and Memory Integration](../BacklogItems/PBI-16-ToolExecutionAndMemoryIntegration.md)
4. [PBI-17 Agent Evaluation Hardening and Readiness](../BacklogItems/PBI-17-AgentEvaluationHardeningAndReadiness.md)

## Concrete Project Mapping (Existing Code First)
| Sprint | Primary Existing Projects | Planned Reuse |
| --- | --- | --- |
| Sprint 0 | `Ollama.Protocol`, `Ollama.Client`, `Ollama.Samples.TagsCheck`, `Ollama.Samples.ChatCheck` | endpoint/model preflight, first prompt roundtrip, baseline smoke harness |
| Sprint 1 | `Ollama.Client`, `Ollama.Protocol`, `OpenAIPlayground`, `Ollama.Extensions.DependencyInjection` | provider adapter boundary, shared runtime envelope, DI-based composition |
| Sprint 2 | `Ollama.Tools`, `Ollama.Tools.Tests`, `Ollama.Samples.ToolUse` | tool-call loop, orchestration reuse, safety policy integration |
| Sprint 3 | `Ollama.Client.Tests`, `Ollama.Protocol.Tests`, `Ollama.Tools.Tests`, `Ollama.Wpf.TextAnalysis` | regression matrix, diagnostics hardening, scenario validation from existing hosts |

## Proposed New Project Scope
- Introduce one dedicated coding-agent host project from the beginning.
- Keep shared abstractions inside existing reusable projects instead of duplicating logic in hosts.
- Add one dedicated test project for new runtime-specific behavior if existing test projects become overloaded.

## Scrum-Style Increment Plan

### Sprint 0 - Environment and Baseline Validation
**Objective**
Confirm local runtime viability and create a minimal runnable C# agent skeleton.

**Expected Outcomes**
- Local Ollama endpoint is reachable from C# host.
- `qwen2.5-coder:7b` is available and can produce deterministic baseline responses.
- A thin command-line agent runner exists with configuration loading and logging basics.

**Primary Deliverables**
- Environment checklist and known-failures catalog.
- Baseline smoke test suite.
- Agent skeleton with provider contract stub.

### Sprint 1 - Provider-Agnostic Runtime
**Objective**
Implement the multi-turn agent loop with abstraction boundaries for OpenAI-compatible and Ollama providers.

**Expected Outcomes**
- Shared conversation model and agent-state machine.
- Planner-executor loop with iteration and stop conditions.
- Retry and cancellation boundaries.

**Primary Deliverables**
- Runtime orchestration components.
- Provider adapters with common response envelopes.
- Unit tests for loop transitions and error handling.

### Sprint 2 - Tooling and Memory
**Objective**
Add safe tool execution and structured memory integration for practical coding workflows.

**Expected Outcomes**
- Tool registry and policy layer.
- Controlled file/system command capabilities.
- Session memory store with retrieval boundaries.

**Primary Deliverables**
- Tool-call cycle support (request -> execute -> reinject).
- Session context persistence model.
- Tests for tool errors, denied operations, and memory recall.

### Sprint 3 - Evaluation and Hardening
**Objective**
Make the agent robust for repeated usage with measurable quality and operational diagnostics.

**Expected Outcomes**
- Scenario-based evaluation suite.
- Latency, failure-rate, and token-usage diagnostics.
- Packaging and usage documentation for repeatable local setup.

**Primary Deliverables**
- Regression scenario matrix.
- Hardening fixes and operational guardrails.
- "Definition of Ready/Done" checklist for further features.

## Cross-Cutting Tracks
- **Security and safety:** command constraints, path boundaries, prompt-injection resilience.
- **Diagnostics:** structured logs, correlation IDs, step traces.
- **I18N readiness:** keep user-visible strings localizable.
- **Testability:** provider mocks and deterministic scenario seeds.

## Definition of Ready (DoR) per Sprint Slice
- Parent backlog item is approved and linked.
- Scope, assumptions, and exit criteria are explicit in the sprint task file.
- Test approach is identified (unit/integration/smoke).
- External prerequisites are listed (for example local Ollama and model availability).

## Definition of Done (DoD) per Sprint Slice
- Scope items are implemented and reflected in the related task status.
- Targeted tests pass for the changed behavior.
- Updated documentation exists for setup, operation, and known constraints.
- Follow-up risks or open questions are tracked in DevOps artifacts.

## Verification Matrix
| Area | Baseline Verification | Regression Verification |
| --- | --- | --- |
| Local Ollama connectivity | preflight check and one prompt roundtrip | repeated preflight in CI/local script |
| Runtime loop | deterministic transition tests | multi-turn scenario tests |
| Tool execution | policy allow/deny tests | end-to-end tool-call scenario |
| Memory integration | persistence and retrieval tests | follow-up turn recall scenarios |
| Operational diagnostics | log schema assertions | failure replay with trace IDs |

## Existing Test Assets to Reuse
- `Ollama.Protocol.Tests` for streaming and parser behaviors.
- `Ollama.Client.Tests` for client validation and adapter behavior.
- `Ollama.Tools.Tests` for orchestration and tool-loop behavior.
- Existing sample apps (`Ollama.Samples.*`) as executable smoke-check baselines.

## Regular Live Host Checks
- Keep at least one tiny live-check host app in the solution (`Ollama.CodingAgent.HostCheck`).
- Run the host check regularly in addition to unit tests to validate real Ollama behavior.
- Use the baseline model `qwen2.5-coder:7b` for the first live-check pass before testing alternative models.

## Risk Register
1. Model output inconsistency for tool-call formats.
2. Drift between Ollama and OpenAI-compatible response semantics.
3. Unbounded command execution risk without strict policy.
4. Memory growth and irrelevant retrieval degrading answer quality.
5. Flaky local environment assumptions across machines.

## Mitigation Plan
1. Canonical internal message schema with strict parser validation.
2. Adapter normalization with explicit unsupported-capability handling.
3. Tool policy allowlist + path scoping + timeout defaults.
4. Scoped memory windows and deterministic retrieval strategy.
5. Automated preflight checks and local environment diagnostics.

## Exit Conditions for "Full-Capability Baseline"
- End-to-end task completion for representative coding scenarios.
- Green targeted test suite covering runtime, tools, and memory contracts.
- Reproducible local setup with `qwen2.5-coder:7b`.
- Operational diagnostics sufficient for troubleshooting failed runs.
- DevOps statuses for the sprint tasks and parent PBIs are updated to reflect completion state.
