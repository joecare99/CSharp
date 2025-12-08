# AppWithPluginWpf

WPF host application variant supporting the same plugin model as the console host.

## Target Frameworks
net481; net6.0-windows; net7.0-windows; net8.0-windows; net9.0-windows

## Features
- MVVM (CommunityToolkit.Mvvm)
- Dynamic plugin discovery + resource loading
- Logging + DI integration
- Potential XAML behaviors (Microsoft.Xaml.Behaviors.Wpf) for interaction triggers

## Plugin Deployment
Copies OtherPlugin binaries + localized satellite assemblies into PlugIns directory at build.

## Run
`dotnet run --project AppWithPluginWpf/AppWithPluginWpf.csproj`
