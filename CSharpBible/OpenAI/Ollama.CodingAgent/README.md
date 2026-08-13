# Ollama.CodingAgent

Dedicated C# coding-agent host project for local Ollama-first execution.

## Current baseline

- Primary model baseline: `qwen2.5-coder:7b`
- Runtime defaults:
  - timeout: 12 minutes per model step
  - retries: 3
  - max iterations: 80

## Run

```powershell
dotnet run --project .\Ollama.CodingAgent\Ollama.CodingAgent.csproj -- "Summarize this repository architecture."
```

## Options

- `--endpoint <url>`
- `--model <name>`
- `--timeout-minutes <number>`
- `--retries <number>`
- `--max-iterations <number>`
- `--prompt <text>`
- `--delegate`
- `--workspace-root <path>`

Environment variables are also supported:

- `OLLAMA_ENDPOINT`
- `OLLAMA_MODEL`
- `AGENT_TIMEOUT_MINUTES`
- `AGENT_RETRY_COUNT`
- `AGENT_MAX_ITERATIONS`

## Delegated coding-task mode

Use `--delegate` to let the agent run one safe delegated coding subtask through the tool loop before returning a final summary.
Use `--delegate` to let the agent run up to 3 safe delegated coding subtasks before returning a final summary.

The current delegated tools are:
- `list_workspace_files`
- `read_workspace_file`
- `run_dotnet_build`

All delegated paths are restricted to `--workspace-root` (default: current directory).
