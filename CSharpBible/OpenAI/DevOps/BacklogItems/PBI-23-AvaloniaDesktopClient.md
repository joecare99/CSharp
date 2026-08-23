# Backlog Item: Avalonia Coding-Agent Desktop Client

## Feature Link

[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status

Done

## Description

As an operator, I want an Avalonia-oriented desktop client over the shared coding-agent session so that I can manage an interactive conversation and review available operational state without duplicating runtime, Git, planning, or session business logic.

## Acceptance Criteria

- A .NET 8 Avalonia desktop adapter composes `AgentSessionViewModel` through DI and delegates conversation/session commands to it.
- The desktop provides editable Endpoint/model/workspace configuration, directory selection, recent configuration loading, transcript and prompt entry, status/error display, approval decisions, and cancellation where supported.
- Plan/tool/Git sections show only data exposed by the shared layer and explicit placeholders for deferred data.
- The desktop exposes the existing local CodeWikiVault Markdown import and workspace-local search capabilities.
- Avalonia package versions are centrally managed and a focused dedicated MSTest project validates the presentation adapter.

## Completion Log

- 2026-08-14: Added `Ollama.CodingAgent.Desktop` with an Avalonia `App`, DI-composed `MainWindow`, and small reusable approval, activity, and CodeWikiVault widgets.
- 2026-08-14: Kept shared session state in `Ollama.CodingAgent.Application`; the desktop only projects and delegates to `AgentSessionViewModel`.
- 2026-08-14: Added startup configuration, transcript/prompt, status/error, approvals, local wiki import/search, and explicit placeholders for unavailable plan/tool/Git data.
- 2026-08-14: Added focused presentation adapter tests and solution registration.
- 2026-08-18: Replaced the separate desktop activity list with one expandable transcript. Thinking and tool entries are now grouped into the main conversation and show an indeterminate progress indicator while live updates are received.
- 2026-08-18: Added an optional asynchronous streaming callback from the Ollama HTTP client through the agent runtime and session ViewModel, so the desktop can project thinking fragments before the model request completes.
- 2026-08-18: Added application-layer regression coverage for non-blocking submission and validated the Application and Desktop projects with successful builds.
- 2026-08-18: Replaced the read-only startup display with editable Endpoint/Model configuration. The Endpoint can be tested asynchronously through `/api/tags`; available models are presented in a ComboBox and changes are applied only to subsequent prompts, leaving an active prompt on its original runtime snapshot.
- 2026-08-18: Added a per-user MRU configuration store at `%APPDATA%\Ollama\CodingAgent\desktop-configurations.json`. Successful configurations are retained outside the workspace, and changing the Endpoint clears the current model list before loading models from the new Endpoint.
- 2026-08-18: Added regression coverage for configuration normalization and MRU persistence; all 12 `Ollama.CodingAgent.Desktop.Tests` tests passed and the Desktop project built successfully.
- 2026-08-18: Added workspace directory selection and consolidated Endpoint, model, and workspace into one apply operation. Recent entries now restore the complete configuration, and the current model is shown immediately at startup.
- 2026-08-18: Added `/config` and `:config` support to the Console REPL using `endpoint | model | workspace`; console prompts use a fresh validated runtime snapshot so configuration changes do not interrupt active requests. `Ollama.CodingAgent.Console.Tests` passed 26/26 and `Ollama.CodingAgent.Desktop.Tests` passed 12/12.
