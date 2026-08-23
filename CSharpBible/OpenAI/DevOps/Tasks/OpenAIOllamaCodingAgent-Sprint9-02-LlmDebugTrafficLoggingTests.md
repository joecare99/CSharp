# Task: Test LLM Debug Traffic Logging

## Parent Backlog Item

[PBI-25: LLM Debug Traffic Logging](../BacklogItems/PBI-25-LlmDebugTrafficLogging.md)

## Status

Done

## Goal

Provide deterministic MSTest coverage for the initial default-on LLM traffic logging behavior without requiring a live Ollama service or an external OpenAI endpoint.

## Test Scope

- Verify that logging is enabled by default for a configured session.
- Verify that log files are created below the selected workspace's `.agent\\logs` directory and remain isolated by session.
- Verify that outgoing Ollama and OpenAI-compatible request records are written.
- Verify that incoming response records are written, including non-empty and structurally valid payloads.
- Verify that provider exceptions and failed responses are logged without replacing the original failure behavior.
- Verify redaction of `Authorization` headers, bearer tokens, API keys, JSON credential fields, and URL user information.
- Verify that non-sensitive prompt and response content remains available for diagnosis.
- Verify that logger failures do not alter successful provider results according to the selected failure policy.
- Verify that the later `--debug-log` option is not accidentally required by the initial default-on implementation.

## Test Design

- Use MSTest `[TestMethod]` tests and `NSubstitute` for provider, logger, and transport substitutes where appropriate.
- Use temporary workspace and session identities for file assertions.
- Use deterministic Ollama and OpenAI-compatible HTTP fixtures; do not call live services.
- Read persisted files and assert that known secret values are absent.
- Assert exact provider response and exception outcomes in instrumentation tests.
- Keep tests independent and clean up temporary files after each test.

## Acceptance Evidence

- Focused logging tests pass with zero failures and no skipped tests.
- Existing provider adapter, application, console, and desktop regression suites remain green.
- A fixture-based persisted log sample demonstrates request, response, and error records.
- Redaction assertions cover both headers and payload/URL locations.
- Test output documents the commands and project paths used for validation.

## Completion Log

- 2026-08-15: Added `LlmTrafficLoggingTests` with deterministic file-redaction and OpenAI request/response/failure coverage.
- 2026-08-15: Focused test class passed 2 tests with zero failures and zero skips.

## Dependencies

- Depends on implementation task [OpenAIOllamaCodingAgent-Sprint9-01-LlmDebugTrafficLogging](./OpenAIOllamaCodingAgent-Sprint9-01-LlmDebugTrafficLogging.md).
- Reuses the provider adapter tests established for PBI-15.
