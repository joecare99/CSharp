# Status Review: OpenAI/Ollama Coding Agent

## Review Date

2026-08-14

## Current Status

| Planning item | Status | Evidence | Remaining gap |
| --- | --- | --- | --- |
| Feature 09 | In Progress | PBI-20 through PBI-24 interactive readiness is complete | PBI-15 through PBI-19 remain for provider parity, memory, evaluation, knowledge quality, and planning checkpoints |
| PBI-14 baseline | Done | C# preflight, baseline smoke command, 39 focused tests, and live `qwen2.5-coder:7b` checks pass | None for this backlog item |
| PBI-15 runtime | In Progress | Provider-agnostic message loop, retries, timeout, iteration cap, and tests exist | OpenAI-compatible adapter parity |
| PBI-16 tools/memory | In Progress | Safe delegated tools, fallback execution, and test coverage exist | Session-memory integration and reinjection flow |
| PBI-17 hardening | In Progress | Verbosity, thinking capture, structured diagnostics, and live regression runs exist | Scenario matrix, thresholds, and readiness checklist |
| PBI-18 knowledge | In Progress | Web allowlist, citation envelope, local wiki store, and host checks exist | Combined lookup/writeback quality scenario |
| PBI-19 planning | In Progress | Goal contract, decomposition, drift checks, rendering, and planning host check exist | Persisted checkpoints and resume support |
| PBI-20 shared application | Done | UI-neutral session, approval queue, DI composition, and JSON snapshots exist | Runtime conversation reinjection remains in PBI-16 scope |
| PBI-21 Git provider | Done | Approval-gated typed Git operations and credential-sanitized remotes exist | Git tool-registry/UI exposure remains deferred |
| PBI-22 terminal client | Done | Persistent REPL, session commands, and approval controls exist | Uses only exposed shared application state |
| PBI-23 desktop client | Done | Avalonia session/approval/wiki adapter exists | Plan, tool, and Git state remain explicit placeholders |
| PBI-24 operational readiness | Done | Local-only approval, resume, divergence, and redaction hardening is complete | No live Ollama, external Git-host, or autonomous end-to-end claim |

## PBI-24 Readiness and Evaluation Matrix

| Readiness area | Deterministic evidence | Result | Operational boundary |
| --- | --- | --- | --- |
| Approval rejection | Rejected stage request stays out of Git index | Pass | Only an explicit approval permits a mutation |
| Pending-approval cancellation | Cancellation removes request and leaves file unstaged | Pass | Cancellation wins before apply |
| Session persistence/resume | Persist one session, instantiate a new view model, reload same workspace | Pass | Different session/workspace snapshots are rejected |
| Git remote divergence | Local bare remote rejects a non-fast-forward push | Pass | Remote remains unchanged; operator fetches/pulls or rebases, resolves, then retries |
| Credential handling | Remote and diagnostic URL user information is redacted | Pass | Credentials are not Git-provider inputs, results, or diagnostics |
| Interactive client composition | Console/desktop focused tests and builds | Pass | No client test initiates Ollama or network activity |

## PBI-24 Exact Validation

All commands ran from `C:\Projekte\CSharp\CSharpBible\OpenAI` with existing restored packages:

| Command | Observed result |
| --- | --- |
| `dotnet test .\Ollama.CodingAgent.Application.Tests\Ollama.CodingAgent.Application.Tests.csproj --no-restore` | 6 passed, 0 failed, 0 skipped |
| `dotnet test .\Ollama.CodingAgent.Git.Tests\Ollama.CodingAgent.Git.Tests.csproj --no-restore` | 15 passed, 0 failed, 0 skipped |
| `dotnet test .\Ollama.CodingAgent.Console.Tests\Ollama.CodingAgent.Console.Tests.csproj --no-restore` | 12 passed, 0 failed, 0 skipped |
| `dotnet test .\Ollama.CodingAgent.Desktop.Tests\Ollama.CodingAgent.Desktop.Tests.csproj --no-restore` | 2 passed, 0 failed, 0 skipped |
| `dotnet build .\Ollama.CodingAgent.Console\Ollama.CodingAgent.Console.csproj --no-restore` | succeeded: 0 warnings, 0 errors |
| `dotnet build .\Ollama.CodingAgent.Desktop\Ollama.CodingAgent.Desktop.csproj --no-restore` | succeeded: 0 warnings, 0 errors |

No command in this readiness evaluation starts Ollama or contacts an external Git remote.

## Real-Condition Agent Tests

The local `Ollama.CodingAgent` was run with `qwen2.5-coder:7b` in delegate mode against the workspace. The delegated `run_dotnet_test` task executed the targeted project three times:

- `Ollama.CodingAgent.Tests`: 35 passed, 0 failed, 0 skipped per run.
- Model tool selection timed out under the short smoke-test budget; deterministic fallback selected and executed the test tool.
- The test tool was corrected for this repository's Microsoft.Testing.Platform setup by avoiding `--nologo`, using a workspace-relative project path, and passing `--no-restore`.
- A second delegated `run_dotnet_build` task built `OpenAI.slnx` successfully three times under the same local Ollama run.

## Recommended Next Task

Implement session-memory persistence and conversation reinjection for PBI-16, then add a
deterministic multi-turn test before starting the full PBI-17 scenario matrix.
