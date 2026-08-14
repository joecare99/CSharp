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
- `--verbosity <quiet|normal|verbose>`
- `--show-thinking`
- `--preflight`
- `--baseline-smoke`
- `--prompt <text>`
- `--delegate`
- `--workspace-root <path>`

Environment variables are also supported:

- `OLLAMA_ENDPOINT`
- `OLLAMA_MODEL`
- `AGENT_TIMEOUT_MINUTES`
- `AGENT_RETRY_COUNT`
- `AGENT_MAX_ITERATIONS`
- `AGENT_VERBOSITY`
- `AGENT_SHOW_THINKING=true`

## Delegated coding-task mode

Use `--delegate` to let the agent run up to 3 safe delegated coding subtasks before returning a final summary.
If model-based tool selection times out or fails, the agent now executes a deterministic fallback tool call (`list_workspace_files`, or `run_dotnet_build` / `run_dotnet_test` based on prompt intent).
Delegated runs include a lightweight planning layer (goal contract, planned subtasks, and drift detection).

The current delegated tools are:
- `list_workspace_files`
- `read_workspace_file`
- `write_workspace_file`
- `run_dotnet_build`
- `run_dotnet_test`
- `web_lookup`
- `local_wiki_search`
- `local_wiki_write`

All delegated paths are restricted to `--workspace-root` (default: current directory).
Verbose delegated output includes the tool name, success status, elapsed time, validated JSON input,
and a bounded result preview. Model thinking is hidden by default and can be enabled explicitly with
`--show-thinking`.

## Local Ollama baseline

Run the C# endpoint/model readiness check:

```powershell
dotnet run --project .\Ollama.CodingAgent\Ollama.CodingAgent.csproj -- --preflight
```

Run the readiness check followed by one bounded chat roundtrip:

```powershell
dotnet run --project .\Ollama.CodingAgent\Ollama.CodingAgent.csproj -- --baseline-smoke --prompt "Reply with one short sentence."
```

The baseline uses `OLLAMA_ENDPOINT` and `OLLAMA_MODEL` when present. If the endpoint cannot be
reached, start Ollama and verify `http://localhost:11434/`. If the model is missing, install it
with `ollama pull qwen2.5-coder:7b` or pass a different model using `--model`. Slow local models
may require increasing `--timeout-minutes`; cancellation is propagated instead of being hidden.
