# Backlog Item: LLM Debug Traffic Logging

## Feature Link

[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status

Done

## Description

As an engineer, I want the coding agent to record the complete diagnostic traffic exchanged with an LLM so that provider requests, responses, and failures can be investigated without changing the agent workflow.

## Scope

- Use `BaseLib.Models.Interfaces.ILog` from `Avln_BaseLib` as the logging abstraction.
- Log outgoing provider requests, incoming provider responses, and failed provider calls for Ollama and OpenAI-compatible adapters.
- Enable logging by default for the initial implementation.
- Store one session-scoped log beneath `<workspace>\\.agent\\logs`.
- Redact credentials before any data is persisted, including authorization headers, bearer tokens, API keys, and URL user information.
- Preserve non-sensitive prompt and response content for debugging.
- Keep the later `--debug-log` switch as a planned configuration refinement; it is not part of this initial slice.

## Non-Goals

- No provider-specific logging contract.
- No credential persistence or unredacted diagnostic fallback.
- No change to request retry, timeout, cancellation, or response handling semantics.
- No production telemetry or remote log shipping.

## Acceptance Criteria

- A provider-neutral logger can be supplied through dependency injection using `ILog`.
- Every Ollama and OpenAI-compatible request/response exchange is represented in the session log.
- Provider failures are logged with redacted diagnostic details and do not get swallowed by logging.
- Logs are created below the selected workspace's `.agent\\logs` directory and are isolated by session.
- Authorization headers, bearer tokens, API keys, and URL credentials never appear in persisted logs.
- Tests prove default activation, location, request/response/error capture, redaction, and unchanged transport outcomes.
- Documentation describes the default behavior and records `--debug-log` as the later opt-in/opt-out control.

## Primary Project Targets

- `Avln_BaseLib` (`BaseLib.Models.Interfaces.ILog`)
- `Ollama.Client` and `Ollama.Protocol`
- `OpenAI.CodingAgent`
- `Ollama.CodingAgent.Application`
- `Ollama.CodingAgent.Console`

## Dependencies

- Depends on: [PBI-15 Provider-Agnostic Agent Runtime](./PBI-15-ProviderAgnosticAgentRuntime.md)
- Uses the session and workspace boundaries established by [PBI-20 Shared Interactive Agent Application Layer](./PBI-20-SharedInteractiveAgentApplicationLayer.md)
- Must preserve the credential-safety guarantees completed by [PBI-24 Interactive Coding-Agent Operational Readiness](./PBI-24-InteractiveAgentOperationalReadiness.md)

## Open Questions

- Should the future `--debug-log` switch control only file persistence, or also console visibility?
- Should log rotation be introduced when long-running sessions produce large request/response files?
- Should structured JSON Lines be the final format, or should a human-readable session transcript remain available?

## Next Refinement Steps

1. Define the redaction rules and stable log record format.
2. Identify the shared provider boundary for request/response instrumentation.
3. Add implementation and dedicated test tasks.
4. Add operator documentation and validate representative provider fixtures.

## Completion Log

- 2026-08-15: Added the default-on session-scoped JSON Lines logger with the existing `ILog` contract.
- 2026-08-15: Added credential redaction for authorization headers, bearer tokens, API keys, JSON credential fields, and URL user information.
- 2026-08-15: Instrumented Ollama and OpenAI-compatible request, response, and failure paths without changing provider outcomes.
- 2026-08-15: Added deterministic logger/provider tests; the focused test class passes 2/2 tests.
- 2026-08-18: Moved the default Ollama coding-agent log location from the workspace to `%APPDATA%\Ollama\CodingAgent\Logs`; vendor and application names are configurable and file names include the UTC session-start timestamp.
- 2026-08-18: Added an end-to-end prompt integration test using the real Ollama HTTP client path and a test HTTP component; the test validates the transmitted model, messages, prompt, and delegated tool definitions.
