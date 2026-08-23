# Backlog Item: Shared Interactive Agent Application Layer

## Feature Link

[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status

Done

## Description

As an operator, I want UI-neutral interactive session state and explicit approval workflows so terminal and desktop clients can share one agent experience without duplicating runtime or safety logic.

## Acceptance Criteria

- Shared CommunityToolkit.Mvvm session state runs, cancels, saves, and reloads agent conversations without a UI dependency.
- State-changing operations can wait for an explicit structured approval decision.
- Agent runtime/provider/tool DI composition is reusable by future client hosts.
- Session snapshots are scoped to an explicit workspace and session identity.

## Primary Project Targets

- `Ollama.CodingAgent.Application`
- `Ollama.CodingAgent.Application.Tests`
- `Ollama.CodingAgent`

## Completion Log

- 2026-08-14: Added UI-neutral conversation, session snapshot/store, session runner abstraction, approval queue, and shared `AgentSessionViewModel`.
- 2026-08-14: Added a JSON session store at an explicit caller-selected path; it validates session identity and workspace identity when reloaded.
- 2026-08-14: Extracted Ollama runtime/provider/tool registration into `AddOllamaCodingAgent` for reuse by interactive hosts.
- 2026-08-14: Added isolated MSTest coverage for approval decisions, cancellation rejection, session submission/persistence, wrong-session rejection, and JSON round-trip.
- 2026-08-14: Validated 5 application-layer tests and 54 existing coding-agent regression tests.
