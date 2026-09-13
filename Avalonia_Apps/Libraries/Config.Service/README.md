# Config.Service

## Purpose

`Config.Service` is a generic, DI-friendly configuration component for application settings. It is intentionally independent from any specific product such as RnzTrauer and supports multiple vendor/application combinations.

## Key traits

- Vendor/application-based storage root instead of product-specific naming.
- Provider-based section registration through `IConfigSectionProvider`.
- JSON persistence beneath `%LOCALAPPDATA%\<Vendor>\<Application>\config\` or a custom `CONFIG_ROOT` override.
- Value lookups for strings and enums through `ConfigService`.
- Section validation and registry-based discovery.
- UI/localization-friendly description handling: keep the explanatory text in the UI registration layer rather than inside the provider implementation.

## Basic setup

```csharp
services.AddConfigService("Contoso", "DemoApp")
    .AddConfigSection<TestConfig>(new DemoConfigProvider());

var configService = new ConfigService(store, registry, "Contoso.DemoApp", "Contoso", "DemoApp");
var server = await configService.GetStringValueAsync<TestConfig>("General", nameof(TestConfig.Server), "localhost");
var mode = await configService.GetEnumValueAsync<TestConfig, DemoMode>("General", nameof(TestConfig.Mode), DemoMode.Automatic);
```

## Provider contract

A configuration provider supplies a stable section key, display name, order, and model factory. The description is treated as app/UI metadata and should come from the registering UI layer for localization.

```csharp
public sealed class DemoConfigProvider : IConfigSectionProvider
{
    public string Name => "General";
    public string DisplayName => "General settings";
    public string? Description => null;
    public int Order => 0;
    public Type ModelType => typeof(TestConfig);
    public TestConfig CreateModel() => new();
    object IConfigSectionProvider.CreateModel() => CreateModel();
}
```

## Lookup and validation

The service can resolve structured values without a direct section-specific implementation:

```csharp
var server = await configService.GetStringValueAsync<TestConfig>("General", nameof(TestConfig.Server), "localhost");
var mode = await configService.GetEnumValueAsync<TestConfig, DemoMode>("General", nameof(TestConfig.Mode), DemoMode.Automatic);
var isValid = await configService.ValidateSectionAsync<TestConfig>("General");
```

## Storage location

By default, the JSON files are stored under:

`%LOCALAPPDATA%\<Vendor>\<Application>\config\`

This can be overridden via the environment variable `CONFIG_ROOT`.

## Notes

- Description text should be supplied by the UI/app layer that registers a configuration section so it can be localized and changed without altering the core provider contract.
- The section registry now supports `TryGetSection` lookups and sorted access by section order.
- The registry removal behavior is also fixed to report real removal success.

## Status

The `Config.Service` component is now generic and does not depend on RnzTrauer-specific defaults.
