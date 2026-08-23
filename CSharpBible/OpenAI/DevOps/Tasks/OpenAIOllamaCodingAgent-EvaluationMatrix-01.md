# Evaluation Matrix: OpenAI/Ollama Coding Agent

## Version

`1.0` - 2026-08-13

## Execution

The matrix is executable through `AgentEvaluationRunner`. Each scenario must
return a structured pass/fail result and a human-readable detail. The default
readiness threshold for deterministic regression runs is 100%; model-dependent
live runs may use a documented threshold and must retain all failed outcomes.

## Scenarios

| ID | Scenario | Required outcome |
| --- | --- | --- |
| `baseline-chat` | Provider returns one non-empty response | Response is normalized and the run completes |
| `retry-recovery` | First completion fails transiently | Retry succeeds within configured retry budget |
| `tool-denial` | Tool is not in the execution allowlist | Invocation fails explicitly and the tool is not executed |
| `tool-reinject` | Tool call is followed by a final model response | Tool result is present in the next model turn |
| `memory-recall` | Context is stored and queried in one session | Relevant entry is returned and other sessions are excluded |
| `memory-retention` | Session exceeds configured retention | Oldest entries are trimmed deterministically |
| `cancellation` | Caller cancellation is signaled | Cancellation propagates without a success-shaped result |

## Readiness gates

- All deterministic regression scenarios pass.
- Runtime diagnostics contain a correlation ID, completion duration, and
  failure details for retry/error paths.
- Tool policy denies unknown or non-allowlisted operations.
- Memory retrieval remains session-scoped and bounded.
- Local onboarding includes endpoint, model, preflight, smoke, and
  troubleshooting commands.

## Known limits

Live model quality is evaluated separately from deterministic contract tests.
Provider/model combinations must be recorded with the evaluation report because
latency and tool-selection reliability vary by backend.
