# AppWithPluginTest

Test project validating the plugin-enabled host application (AppWithPlugin).

## Target Frameworks
net6.0; net7.0; net8.0; net9.0

## Focus Areas
- Plugin discovery & loading correctness
- Dependency injection behavior
- Logging or configuration handshake
- Edge cases: missing plugin, version mismatch

## Tooling
- MSTest + NSubstitute (mocking service dependencies)

## Build & Test
`dotnet test AppWithPluginTest/AppWithPluginTest.csproj`

## Design Notes
Shared helper (ClassHelper.cs) linked from external library for reuse.
