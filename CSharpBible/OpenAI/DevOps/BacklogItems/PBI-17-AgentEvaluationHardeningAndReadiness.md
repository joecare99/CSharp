# Backlog Item: Add Evaluation, Hardening, and Delivery Readiness

## Feature Link
[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status
Done

## Description
As an engineer, I want an evaluation and hardening phase so that the coding agent can be trusted for repeated local engineering workflows with measurable quality and clear operational diagnostics.

## Acceptance Criteria
- Scenario-based evaluation matrix is defined and executable.
- Runtime diagnostics capture latency, failures, and step traces.
- Hardening backlog from evaluation results is tracked and prioritized.
- Local setup and operation are documented for repeatable onboarding.

## Primary Project Targets
- `Ollama.Protocol.Tests`
- `Ollama.Client.Tests`
- `Ollama.Tools.Tests`
- `Ollama.Samples.*` and `Ollama.Wpf.TextAnalysis` for scenario-based validation

## Tasks
- Define benchmark scenarios for coding-agent workflows.
- Add runtime diagnostics and structured logs.
- Execute hardening loop for highest-impact defects.
- Produce readiness checklist and usage documentation.
- Include source-citation and local wiki quality checks in readiness criteria.

## Test Tasks
- Add repeatable scenario-runner tests for baseline, tool, and memory flows.
- Add diagnostics contract tests to keep log schema stable.

## Dependencies
- Depends on: [PBI-16](./PBI-16-ToolExecutionAndMemoryIntegration.md)

## Open Questions
- Which minimum reliability threshold should block release (pass rate, failure budget, or both)?
- Should evaluation metrics be persisted per model or per provider+model combination?

## Completion Log
- 2026-08-13: Added the executable, versioned evaluation matrix with baseline, retry, tool-policy, tool-reinjection, memory, retention, and cancellation scenarios.
- 2026-08-13: Added provider-neutral runtime diagnostics with correlation IDs, completion latency, retry attempt data, and structured failure details.
- 2026-08-13: Added deterministic evaluation-runner threshold handling and exception-to-failed-outcome conversion.
- 2026-08-13: Added diagnostics and evaluation contract tests; `Ollama.CodingAgent.Tests` passes 45 tests and `Ollama.Tools.Tests` passes 103 tests.
