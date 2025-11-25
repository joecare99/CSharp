# AsyncExampleWPF

Multi-targeted WPF sample application (net462; net472; net48; net481) demonstrating asynchronous programming patterns in a desktop UI context.

## Goals
- Showcase async/await integration with WPF message loop
- Illustrate responsiveness improvements vs synchronous blocking
- Provide a baseline for comparing legacy framework behaviors

## Target Frameworks
Classic .NET Framework TFMs only; no .NET (Core) targets—useful to contrast with newer projects in the solution.

## Features (expected in code-behind / ViewModels)
- Async event handlers (e.g., button click triggering background operations)
- UI thread marshaling via SynchronizationContext / Dispatcher
- Potential cancellation token usage

## Build
`dotnet build AsyncExampleWPF/AsyncExampleWPF.csproj`

## Extensibility
Project can be upgraded to multi-target .NET 6+ WPF by adding windows TFMs (e.g., net8.0-windows) if desired.
