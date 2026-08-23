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

## Interactive operator setup

Install .NET 8, Git, and Ollama, then make a local model available (the default is
`qwen2.5-coder:7b`). The interactive clients do not contact Ollama until an operator submits
a prompt. Select an existing local workspace explicitly; session snapshots are stored only at
`<workspace>\.agent\sessions\<session>.json` and can be resumed only with that same workspace
and session identifier.

```powershell
# Persistent terminal session
dotnet run --project .\Ollama.CodingAgent.Console\Ollama.CodingAgent.Console.csproj -- `
  --workspace C:\Work\MyRepository --session daily-work

# Avalonia desktop session with an explicit CodeWikiVault
dotnet run --project .\Ollama.CodingAgent.Desktop\Ollama.CodingAgent.Desktop.csproj -- `
  --workspace C:\Work\MyRepository --session daily-work `
  --code-wiki-vault C:\Projekte\CSharp\CodeWikiVault
```

Use `--endpoint` and `--model` in either client to override the local defaults. The terminal
offers `:reload` to resume the selected snapshot and `:transcript` to inspect it.

### Git safety and recovery

Git credentials must be configured by the operator in Git Credential Manager, SSH agent, or
another local Git-supported credential helper before a fetch, pull, or push. Do not put tokens
in a client argument, prompt, remote URL, or session file. The Git provider neither accepts nor
stores credentials; remote displays and Git failure diagnostics redact URL user information.

Every Git mutation is a mandatory approval boundary: inspect the exact preview, then use
`:approve <id>` or the desktop approval control. Rejecting an approval, or cancelling while it
is pending, performs no mutation. A failed non-fast-forward push leaves the remote unchanged:
fetch/pull or rebase with normal Git tooling, resolve and commit any conflicts, verify status,
then request a new approved push. Resolve conflicts or an in-progress merge/rebase before
requesting any mutation; the provider rejects those states.

### CodeWikiVault

The desktop's CodeWikiVault panel imports Markdown from `--code-wiki-vault` (or
`CODE_WIKI_VAULT`) into the workspace-local `.agent\local-wiki.json` store and searches that
local copy. Import is operator-initiated; it does not watch, edit, publish, or synchronize the
vault, and it does not make external web requests.

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

### LLM debug logging

The LLM traffic logger is enabled by default and cannot be disabled in the current command set.
It writes session-scoped diagnostic records to a central per-user location below
`%APPDATA%\Ollama\CodingAgent\` (one JSON-lines file per agent session), including outgoing
provider requests, incoming responses, and provider failures for Ollama and OpenAI-compatible
endpoints.
Credentials are always redacted before persistence. This includes authorization and bearer headers,
API keys, JSON credential fields, and user information embedded in URLs. Non-sensitive prompt and
response content remains available for diagnosis.

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
