# Task: OpenAI/Ollama Coding Agent Sprint 0 Baseline

## Parent
- Backlog Item: [PBI-14 Local Ollama Baseline and Agent Skeleton](../BacklogItems/PBI-14-LocalOllamaBaselineAndAgentSkeleton.md)

## Goal
Create a stable local-first baseline with Ollama and `qwen2.5-coder:7b`, then produce a minimal runnable C# agent shell.

## Scope
- verify local Ollama endpoint connectivity and model readiness
- run deterministic smoke prompts against `qwen2.5-coder:7b`
- implement minimal one-turn agent shell with configuration loading
- record baseline failure diagnostics and recovery notes

## Existing Project Integration
- Reuse `Ollama.Protocol.GetTagsAsync` for model visibility checks.
- Reuse `Ollama.Client.OllamaChatClient` for first chat roundtrip.
- Use `Ollama.Samples.TagsCheck` and `Ollama.Samples.ChatCheck` as baseline executable references.
- Add `Ollama.CodingAgent.HostCheck` as a dedicated live-response smoke host.

## Recommended Implementation Order
1. Add preflight checks (endpoint reachability, model exists, timeout defaults).
2. Run baseline prompt checks and capture expected/observed behaviors.
3. Implement the C# one-turn agent shell.
4. Add targeted tests for baseline paths.
5. Document setup and known local environment issues.

## Subtasks
1. Define baseline command set and endpoint assumptions.
2. Implement model-availability check in C#.
3. Add smoke scenario: prompt -> response -> basic output validation.
4. Implement initial `AgentRunner` skeleton and configuration binding.
5. Add focused MSTest coverage for happy-path and timeout behavior.
6. Document troubleshooting in `DevOps/Projects` or `DevOps/Tasks`.

## Assumptions
- local Ollama is available on the default host/port unless overridden in config
- the model `qwen2.5-coder:7b` can be pulled locally
- baseline output format may vary, so assertions should focus on structural validity

## Exit Criteria
- local baseline checks are repeatable
- minimal agent shell runs and returns a model response
- targeted tests pass
- troubleshooting notes exist for common startup failures

## Status
Done

## Status Log
- 2026-08-13: Started dedicated `Ollama.CodingAgent` host implementation and added first `Ollama.CodingAgent.HostCheck` live-check app for real Ollama response validation.
- 2026-08-13: Real delegated runs against local `qwen2.5-coder:7b` completed and provided the live baseline evidence used for the final implementation.
- 2026-08-13: Completed repeatable C# preflight and baseline smoke commands, focused failure/cancellation tests, and troubleshooting documentation.
- 2026-08-13: Live `--preflight` and `--baseline-smoke` runs passed against local `qwen2.5-coder:7b`; targeted suite passed 39/39.
