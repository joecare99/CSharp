# HelloPlugin

Baseline plugin implementation for the modular host applications.

## Target Frameworks
net6.0; net7.0; net8.0; net9.0

## Features
- Implements PluginBase contracts
- Optional resource-driven greetings (Resources.resx present)
- Logging abstraction (compile-time reference, runtime provided by host)

## Dynamic Loading
Marked with EnableDynamicLoading to facilitate trimming-friendly plugin scenarios.

## Build
`dotnet build HelloPlugin/HelloPlugin.csproj`
