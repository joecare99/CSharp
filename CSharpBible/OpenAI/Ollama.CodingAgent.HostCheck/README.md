# Ollama.CodingAgent.HostCheck

Small live-check host application to validate real Ollama responses in addition to unit tests.

## Purpose

- Execute a tiny scenario set against a real local Ollama model.
- Validate end-to-end behavior through `Ollama.CodingAgent` runtime classes.
- Provide a repeatable smoke-check command for local development.

## Run default scenarios

```powershell
dotnet run --project .\Ollama.CodingAgent.HostCheck\Ollama.CodingAgent.HostCheck.csproj
```

## Run a single custom scenario

```powershell
dotnet run --project .\Ollama.CodingAgent.HostCheck\Ollama.CodingAgent.HostCheck.csproj -- --prompt "Explain one safe C# refactoring for nullability warnings."
```

## Options

- `--endpoint <url>`
- `--model <name>`
- `--prompt <text>`
- `--delegate`
- `--workspace-root <path>`
