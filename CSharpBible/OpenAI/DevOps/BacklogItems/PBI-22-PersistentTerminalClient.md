# Backlog Item: Persistent Coding-Agent Terminal Client

## Feature Link

[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status

Done

## Description

As an operator, I want a persistent terminal client for the shared coding-agent application session so I can submit multiple prompts, resume transcript state, inspect status, and resolve explicit operation approvals without a desktop UI.

## Acceptance Criteria

- A .NET 8 executable terminal adapter uses `AgentSessionViewModel` and persists sessions by workspace and session identifier.
- Endpoint, model, workspace, and session have validated command-line configuration.
- The REPL supports prompts, visible transcript/status, reload/clear, cancellation where supported, and explicit approval/rejection commands.
- ConsoleLib is referenced directly and its generic console input/output abstraction is used without introducing a missing widget backend.
- Pending shared approval requests are visible even though Git tool-registry integration remains deferred.
- Parser and projection behavior has focused MSTest coverage.

## Completion Log

- 2026-08-14: Added `Ollama.CodingAgent.Console`, a thin System.Console REPL over the shared application ViewModel.
- 2026-08-14: Reused ConsoleLib's generic `IConsole` ecosystem through a concrete System.Console adapter; no ConsoleLib widget backend was fabricated.
- 2026-08-14: Wired existing runtime, application, and Git DI registrations while retaining the intentionally deferred Git tool-registry integration.
- 2026-08-14: Added persistent session commands and focused console-client tests.
