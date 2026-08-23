# Task: Implement LLM Debug Traffic Logging

## Parent Backlog Item

[PBI-25: LLM Debug Traffic Logging](../BacklogItems/PBI-25-LlmDebugTrafficLogging.md)

## Status

Done

## Goal

Add provider-neutral diagnostic logging for all LLM traffic while preserving the existing provider, retry, timeout, cancellation, and error semantics.

## Scope

- Add or reuse an `ILog` implementation that writes session-scoped records below `<workspace>\\.agent\\logs`.
- Register the logger through the existing Microsoft.Extensions.DependencyInjection composition.
- Define a stable record format containing timestamp, session identity, provider, direction, operation, and redacted payload.
- Instrument the shared provider boundary so Ollama and OpenAI-compatible request and response payloads are logged consistently.
- Log exceptions and failed HTTP/provider results after redaction.
- Ensure the logging path cannot make a successful LLM operation fail; logging failures must follow an explicit, testable policy.
- Create the session log directory as needed without writing outside the selected workspace.
- Keep the initial behavior enabled by default and leave the future `--debug-log` option as a documented configuration seam.

## Redaction Rules

- Redact `Authorization` headers and bearer tokens.
- Redact API keys and known credential fields in JSON payloads.
- Redact user information and embedded credentials in URLs.
- Apply redaction before the `ILog` call and before file persistence.
- Never provide an unredacted fallback path for diagnostics.

## Implementation Order

1. Identify the provider-neutral request/response boundary used by the Ollama and OpenAI-compatible adapters.
2. Define the log record and redaction components as independently testable services.
3. Implement the file-backed `ILog` adapter with workspace/session path validation.
4. Register the services through the shared application/provider composition.
5. Instrument request, response, cancellation, and exception paths.
6. Add operator documentation for the default behavior, storage path, and credential handling.

## Technical Constraints

- Keep provider-specific transport types out of the shared logging contract.
- Use explicit `using` directives; do not add global or implicit usings.
- Follow the existing nullable and one-type-per-file conventions.
- Comments and XML documentation must be in English where documentation is needed.
- Do not change the existing `ILog` interface unless a concrete compatibility requirement is demonstrated.

## Validation

- Run focused logger and provider-adapter tests from the dedicated test task.
- Run the existing application, console, and provider regression tests.
- Verify a representative log using both provider fixture paths and confirm that no credential values are persisted.

## Completion Log

- 2026-08-15: Implemented the linked `ILog` file adapter, provider-neutral traffic contract, redaction, DI registration, and Ollama/OpenAI instrumentation.
