# Task: Add coding-agent output diagnostics

## Status

Done

## Scope

- Add quiet, normal, and verbose command-line output profiles.
- Preserve optional model thinking fragments behind `--show-thinking`.
- Include delegated tool name, success state, duration, validated input, and bounded output in verbose reports.
- Tighten delegated tool schema descriptions with defaults and constraints.

## Implementation

- `OllamaAgentCliOptions` supports `--verbosity`, `--show-thinking`, `AGENT_VERBOSITY`, and `AGENT_SHOW_THINKING`.
- `AgentRunner` preserves Thinking metadata when the concrete model client supports it.
- Delegated steps retain selection thinking and structured execution details.
- README usage and tool inventory were updated.

## Validation

- `dotnet test .\Ollama.CodingAgent.Tests\Ollama.CodingAgent.Tests.csproj --no-restore`
- 35 tests passed.
- A real local Ollama delegate smoke run completed successfully using fallback tool execution after model-selection timeouts.
