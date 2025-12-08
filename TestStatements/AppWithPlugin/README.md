# AppWithPlugin

Console (or hybrid) host application supporting runtime plugin loading.

## Target Frameworks
net481; net6.0; net7.0; net8.0; net9.0

## Core Features
- Dynamic assembly loading (System.Runtime.Loader)
- Dependency injection integration (Microsoft.Extensions.DependencyInjection)
- Optional secrets/config (UserSecretsId present)
- Logging abstraction consumption
- Plugin discovery (copies OtherPlugin artifact + satellite resources into PlugIns folder)

## Packages
CommunityToolkit.Mvvm, DI, Logging.Abstractions, Configuration.UserSecrets, System.Private.Uri, System.Runtime.Loader

## Plugin Deployment
Content items copy plugin DLL + localized resources (de/en/fr) to output, simulating a simple plugin drop model.

## Run
`dotnet run --project AppWithPlugin/AppWithPlugin.csproj`

## Extension Ideas
- Add plugin manifest validation
- Hot-reload detection via FileSystemWatcher
- Structured logging of plugin lifecycle events
