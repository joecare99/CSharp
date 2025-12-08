# OtherPlugin

Sample plugin implementation consumed by AppWithPlugin and AppWithPluginWpf.

## Target Frameworks
net6.0; net7.0; net8.0; net9.0

## Purpose
Provide alternative behavior set vs HelloPlugin to demonstrate extensibility & resource localization.

## Features
- Localized resources (satellite DLLs: de, en, fr)
- Logging abstraction integration (compile-time only, runtime excluded)
- Implements contracts from PluginBase

## Packaging Notes
References PluginBase with Private=false / ExcludeAssets=runtime to avoid duplicate type loading in host (host supplies the runtime copy).

## Build
`dotnet build OtherPlugin/OtherPlugin.csproj`
