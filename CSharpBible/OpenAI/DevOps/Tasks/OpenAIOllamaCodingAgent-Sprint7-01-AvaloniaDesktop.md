# Task: Avalonia Coding-Agent Desktop Client

## Parent Backlog Item

[PBI-23: Avalonia Coding-Agent Desktop Client](../BacklogItems/PBI-23-AvaloniaDesktopClient.md)

## Status

Done

## Delivered Work

- Created the `net8.0` Avalonia desktop adapter and registered it with central package management.
- Composed existing runtime and application services through DI and bound the desktop adapter to the shared `AgentSessionViewModel`.
- Added reusable approval, activity, and CodeWikiVault widgets while preserving application-layer UI independence.
- Added focused MSTest coverage for desktop option parsing and approval command delegation.

## Validation

- `dotnet test .\Ollama.CodingAgent.Desktop.Tests\Ollama.CodingAgent.Desktop.Tests.csproj`
- `dotnet build .\Ollama.CodingAgent.Desktop\Ollama.CodingAgent.Desktop.csproj`

## Coverage follow-up

The direct Microsoft.Testing.Platform Cobertura run on 2026-08-15 passed all 9
desktop tests. The desktop assembly reports 73/74 branches (98.648649%);
`App.axaml` remains at line 1 branch coverage 50% (1/2), while
the generated `!XamlIlPopulate` method and the other desktop classes are fully
covered. Avalonia generates this branch while applying the resource dictionary:
it tests whether the `Styles` collection is an `Avalonia.StyledElement` before
assigning a name scope. `Avalonia.Styling.Styles` cannot be an
`Avalonia.StyledElement` because those are unrelated runtime types, so the
second branch is unreachable for this `Application` resource initialization.
No exclusion or production/test code change was applied.
