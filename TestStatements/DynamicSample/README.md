# DynamicSample

Console sample demonstrating runtime code generation / dynamic behaviors. Two project flavors exist in this directory:

- DynamicSample_net.csproj (net6.0; net7.0; net8.0)
- DynamicSample.csproj (net462; net472; net48; net481)

## Purpose
Show cross-generation differences between classic .NET Framework and modern .NET runtimes for dynamic APIs.

## Possible Demonstrated Concepts
- Reflection.Emit or Roslyn compilation (Microsoft.CodeAnalysis appears elsewhere in solution)
- Dynamic loading of assemblies
- Use of `dynamic` keyword vs statically typed invocation

## Build
Modern: `dotnet build DynamicSample/DynamicSample_net.csproj`
Legacy: `dotnet build DynamicSample/DynamicSample.csproj`

## Testing
Consumed by both modern and legacy test projects to validate identical semantics.

## Notes
Maintains separate TFMs instead of single multi-target file to keep sample clarity and dependency isolation.
