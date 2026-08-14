# Task: OpenAI/Ollama Coding Agent Sprint 3 Evaluation and Hardening

## Parent
- Backlog Item: [PBI-17 Agent Evaluation Hardening and Readiness](../BacklogItems/PBI-17-AgentEvaluationHardeningAndReadiness.md)

## Goal
Validate quality, harden defects, and produce repeatable local delivery readiness for the coding agent.

## Scope
- define and execute scenario-based evaluation matrix
- add operational diagnostics and run-level observability
- harden high-priority failure modes
- finalize onboarding and operational guidance
- validate knowledge-source citations and local wiki quality behavior

## Existing Project Integration
- Build regression suites on `Ollama.Protocol.Tests`, `Ollama.Client.Tests`, and `Ollama.Tools.Tests`.
- Reuse `Ollama.Samples.*` and `Ollama.Wpf.TextAnalysis` scenarios as operational validation inputs.
- Keep diagnostics contracts reusable across console and WPF hosts.

## Recommended Implementation Order
1. Define scenario matrix and pass/fail thresholds.
2. Add diagnostics capture for turn timing, failures, and tool outcomes.
3. Run evaluations and prioritize defects.
4. Implement hardening fixes for top-severity issues.
5. Publish readiness checklist and usage guidance.

## Subtasks
1. Define scenario sets (baseline chat, tool-call loop, memory recall, error recovery).
2. Implement structured runtime logs with correlation IDs.
3. Add regression suite entry points for repeated local runs.
4. Execute hardening cycle and track issue closure.
5. Document readiness, known limits, and support playbooks.

## Assumptions
- quality should be measured per scenario, not by one aggregate metric only
- diagnostics must remain lightweight enough for local developer use
- hardening priority follows impact on task completion reliability

## Exit Criteria
- evaluation matrix is executable and versioned in `DevOps`
- top-priority reliability defects are addressed
- local onboarding and runbook steps are complete and reproducible

## Status
Done

## Status Log
- 2026-08-13: Added output profiles, optional thinking output, structured delegated tool diagnostics, and tightened tool schemas.
- 2026-08-13: `Ollama.CodingAgent.Tests` passed 35/35; live delegated execution repeated the same test task successfully three times.
- 2026-08-13: Added the versioned executable evaluation matrix and documented deterministic readiness gates and live-run limits.
- 2026-08-13: Added structured runtime diagnostics with correlation IDs, timing, retry attempts, and failure details.
- 2026-08-13: Added evaluation threshold handling and regression coverage; 45 coding-agent tests and 103 tool tests pass.
