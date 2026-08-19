# OpenAI/Ollama Coding Agent Delivery Plan

## Vision
Deliver a full-capability C# coding agent that can execute practical software-engineering workflows with local Ollama first and OpenAI-compatible providers second, without rewriting core orchestration.

## Delivery Strategy
- **Local-first baseline:** stabilize Ollama + `qwen2.5-coder:7b` before advanced features.
- **Provider-agnostic runtime:** isolate provider-specific request/response shaping.
- **Diagnostic transparency:** enable redacted, session-scoped LLM traffic logging by default after
  the provider boundary is stable; reserve `--debug-log` for the later runtime configuration slice.
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
3. [PBI-25 LLM Debug Traffic Logging](../BacklogItems/PBI-25-LlmDebugTrafficLogging.md)
4. [PBI-16 Tool Execution and Memory Integration](../BacklogItems/PBI-16-ToolExecutionAndMemoryIntegration.md)
5. [PBI-17 Agent Evaluation Hardening and Readiness](../BacklogItems/PBI-17-AgentEvaluationHardeningAndReadiness.md)
6. [PBI-18 Web Knowledge and Local LLM Wiki](../BacklogItems/PBI-18-WebKnowledgeAndLocalLlmWiki.md)
7. [PBI-19 Goal-Focused Task Planning and Execution](../BacklogItems/PBI-19-GoalFocusedTaskPlanningAndExecution.md)
8. [PBI-20 Shared Interactive Agent Application Layer](../BacklogItems/PBI-20-SharedInteractiveAgentApplicationLayer.md)
9. [PBI-21 Approval-Gated Git Workspace Provider](../BacklogItems/PBI-21-ApprovalGatedGitWorkspaceProvider.md)
10. [PBI-22 Persistent Coding-Agent Terminal Client](../BacklogItems/PBI-22-PersistentTerminalClient.md)
11. [PBI-23 Avalonia Coding-Agent Desktop Client](../BacklogItems/PBI-23-AvaloniaDesktopClient.md)
12. [PBI-24 Interactive Coding-Agent Operational Readiness](../BacklogItems/PBI-24-InteractiveAgentOperationalReadiness.md)

## Concrete Project Mapping (Existing Code First)
| Sprint | Primary Existing Projects | Planned Reuse |
| --- | --- | --- |
| Sprint 0 | `Ollama.Protocol`, `Ollama.Client`, `Ollama.Samples.TagsCheck`, `Ollama.Samples.ChatCheck` | endpoint/model preflight, first prompt roundtrip, baseline smoke harness |
| Sprint 1 | `Ollama.Client`, `Ollama.Protocol`, `OpenAIPlayground`, `Ollama.Extensions.DependencyInjection` | provider adapter boundary, shared runtime envelope, DI-based composition |
| Sprint 2 | `Ollama.Tools`, `Ollama.Tools.Tests`, `Ollama.Samples.ToolUse`, `Ollama.CodingAgent.Tests` | tool-call loop, orchestration reuse, goal-focused subtask execution |
| Sprint 3 | `Ollama.Client.Tests`, `Ollama.Protocol.Tests`, `Ollama.Tools.Tests`, `Ollama.Wpf.TextAnalysis`, `McpTools` | regression matrix, diagnostics hardening, source-aware knowledge and local wiki scenario validation |

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

### Planning Wave 1 - Goal-Focused Subtask Control
**Objective**
Add explicit planning state and drift-safe subtask execution so complex tasks remain aligned with the main goal.

**Expected Outcomes**
- Goal contract retained during the full run.
- Subtask decomposition with dependencies and checkpoints.
- Drift detection with targeted replanning behavior.

**Primary Deliverables**
- Plan-state model with checkpoint snapshots.
- Planner/executor loop extension for subtasks.
- Drift and resume regression tests.

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

### Knowledge Wave 1 - Trusted Web Sources and Local Wiki
**Objective**
Enable bounded external knowledge lookup and local wiki accumulation for reusable coding guidance.

**Expected Outcomes**
- Trusted domain allowlist for external sources (Wikipedia, Rosetta Code, Microsoft Learn).
- Citation envelope for responses that use external knowledge.
- Local LLM wiki write/read/search flow with curation policy.

**Primary Deliverables**
- Web lookup tool contract and first source connector.
- Local wiki storage contract and retrieval ranking behavior.
- Host-check scenario for lookup + local wiki writeback.

### Sprint 5 - Approval-Gated Git Workspaces
**Objective**
Give interactive coding-agent clients safe, provider-specific access to local Git workspaces without coupling to migration providers.

**Expected Outcomes**
- Read-only repository discovery, status, bounded diffs, branches, and credential-sanitized remotes.
- Typed stage, unstage, branch, commit, fetch, pull, and push operations.
- One exact approval preview before every mutation, including remote operations.

**Primary Deliverables**
- `Ollama.CodingAgent.Git` provider and dedicated tests.
- Shared `IAgentApprovalService` integration without a circular tool-registry dependency.

### Sprint 6 - Persistent Terminal Client
**Objective**
Provide a small, durable operator interface over the shared application session without coupling application logic to a terminal framework.

**Expected Outcomes**
- Multiple prompts, transcript reload, and session clearing from one process.
- Explicit status and approval decision commands.
- ConsoleLib generic console abstraction reuse with a System.Console REPL fallback until a separate widget backend is available.

**Primary Deliverables**
- `Ollama.CodingAgent.Console` and focused terminal parser/projection tests.
- Persistent session configuration for endpoint, model, workspace, and session identifier.

### Sprint 7 - Avalonia Desktop Client
**Objective**
Provide a focused desktop operator adapter while retaining the shared application layer as the single owner of interactive session business logic.

**Expected Outcomes**
- An Avalonia `net8.0` client can submit prompts, show the persisted transcript, and surface session state.
- Operators can resolve currently exposed approvals and use the existing local CodeWikiVault import/search capability.
- Deferred plan, tool, and Git state remains visibly identified rather than duplicated or inferred by the desktop.

**Primary Deliverables**
- `Ollama.CodingAgent.Desktop`, reusable desktop widgets, and a focused MSTest project.
- Central Avalonia package registration and solution integration.

### Sprint 8 - Interactive Operational Readiness
**Objective**
Make the delivered interactive session, Git, terminal, and desktop boundaries repeatable for a
local operator without requiring a live model or external Git host during validation.

**Expected Outcomes**
- Session resume is workspace-bound and local snapshot replacement does not leave a partial file.
- Rejection and cancellation preserve the no-mutation approval boundary.
- A local bare-repository divergence returns an actionable redacted failure.
- Operator setup, recovery, and CodeWikiVault limitations are explicit.

**Primary Deliverables**
- PBI-24 readiness tests and concise operator guidance.
- Local-only readiness/evaluation matrix and exact command evidence.

## Cross-Cutting Tracks
- **Security and safety:** command constraints, path boundaries, prompt-injection resilience.
- **Diagnostics:** structured logs, correlation IDs, step traces.
- **I18N readiness:** keep user-visible strings localizable.
- **Testability:** provider mocks and deterministic scenario seeds.
- **Knowledge governance:** domain allowlists, citation format, and local wiki curation workflow.

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
| Web knowledge + local wiki | allowlist/citation and wiki write/read tests | lookup + writeback + retrieval scenario |
| Goal-focused planning | decomposition/dependency and drift tests | multi-subtask end-to-end plan execution |
| Operational diagnostics | log schema assertions | failure replay with trace IDs |
| Interactive readiness | approval, resume, redaction, and local-bare divergence tests | repeat the focused PBI-24 test commands without Ollama or external Git |

## Existing Test Assets to Reuse
- `Ollama.Protocol.Tests` for streaming and parser behaviors.
- `Ollama.Client.Tests` for client validation and adapter behavior.
- `Ollama.Tools.Tests` for orchestration and tool-loop behavior.
- Existing sample apps (`Ollama.Samples.*`) as executable smoke-check baselines.

## Regular Live Host Checks
- Keep at least one tiny live-check host app in the solution (`Ollama.CodingAgent.HostCheck`).
- Run the host check regularly in addition to unit tests to validate real Ollama behavior.
- Use the baseline model `qwen2.5-coder:7b` for the first live-check pass before testing alternative models.
- Add focused host checks for:
  - planning behavior and malformed planning input handling,
  - internet-source retrieval and malformed source/output handling,
  - local knowledge-base write/search and malformed data handling.

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
