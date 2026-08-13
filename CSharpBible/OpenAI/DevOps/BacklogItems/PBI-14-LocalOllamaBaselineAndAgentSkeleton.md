# Backlog Item: Establish Local Ollama Baseline and Agent Skeleton

## Feature Link
[Feature: OpenAI/Ollama Coding Agent Platform](../Features/Feat-09-OpenAIOllamaCodingAgent.md)

## Status
Draft

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

## Test Tasks
- Add MSTest smoke checks for local connectivity and first response roundtrip.
- Add timeout/cancellation baseline tests for startup and first prompt execution.

## Dependencies
- None.

## Open Questions
- Should baseline smoke tests use strict output assertions or structural assertions only?
- Which default host/port configuration should be mandatory versus optional?
