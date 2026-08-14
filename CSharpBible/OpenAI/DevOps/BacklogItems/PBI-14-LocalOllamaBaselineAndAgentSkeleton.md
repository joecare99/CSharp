# Backlog Item: Establish Local Ollama Baseline and Agent Skeleton

## Feature Link
[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status
Done

## Description
As an engineer, I want a verified local Ollama baseline with `qwen2.5-coder:7b` and a minimal C# agent skeleton so that later runtime and tooling work starts from a stable executable foundation.

## Acceptance Criteria
- Local Ollama connectivity is validated from C#.
- `qwen2.5-coder:7b` request/response smoke checks exist and are repeatable.
- A minimal agent skeleton can send and receive one conversation turn.
- Baseline setup and troubleshooting are documented in `DevOps`.

## Primary Project Targets
- `Ollama.Protocol` (tags endpoint and connectivity flow)
- `Ollama.Client` (chat baseline call path)
- `Ollama.Samples.TagsCheck` and `Ollama.Samples.ChatCheck` (smoke baseline harness)

## Tasks
- Define the local preflight checklist (endpoint, model presence, timeout policy).
- Create baseline smoke tests for model availability and simple completion.
- Implement initial agent skeleton and configuration model.
- Document common baseline failure modes and recovery steps.

## Implementation Plan
1. Add a repeatable C# preflight command that checks endpoint reachability and verifies the configured model is listed.
2. Add a bounded baseline smoke path for one chat roundtrip using the configured endpoint and model.
3. Add structural assertions for connectivity, model presence, non-empty response, timeout, and cancellation behavior.
4. Document normal operation, missing-model recovery, endpoint overrides, and slow-local-model troubleshooting.
5. Run the plan through a copied CodingAgent binary so the live agent can build and test its own project without locking its active output.

## Delegated Execution Contract
- Workspace root: repository root.
- Primary model: `qwen2.5-coder:7b`.
- Default endpoint: `http://localhost:11434/`.
- No destructive commands or broad file rewrites.
- Completion evidence must include targeted tests and one real local Ollama smoke run.

## Test Tasks
- Add MSTest smoke checks for local connectivity and first response roundtrip.
- Add timeout/cancellation baseline tests for startup and first prompt execution.

## Dependencies
- None.

## Open Questions
- Baseline smoke checks use structural assertions so model wording can vary safely.
- The default endpoint/model remain optional overrides through `OLLAMA_ENDPOINT`, `OLLAMA_MODEL`, `--endpoint`, and `--model`.

## Status Log
- 2026-08-13: Added `--preflight` for endpoint reachability and configured-model verification.
- 2026-08-13: Added `--baseline-smoke` for a preflight plus one bounded non-empty chat response.
- 2026-08-13: Added focused baseline service tests covering model absence, empty responses, endpoint failures, and cancellation.
- 2026-08-13: Added baseline operation and troubleshooting documentation to the coding-agent README.
- 2026-08-13: Live preflight passed against `qwen2.5-coder:7b`; live baseline smoke returned a non-empty response.
- 2026-08-13: Targeted coding-agent test suite passed 39/39.
