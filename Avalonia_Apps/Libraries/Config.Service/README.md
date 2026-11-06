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

## Layered values

The UI-neutral layer contracts represent explicit per-property overrides. Resolution precedence is:

1. Enforced system policy
2. User override
3. Group override
4. Ordinary system override
5. Product default

An explicit `null` override is distinct from a missing key. Only values in the system layer can be marked enforced. `ConfigLayerResolver` validates each selected value against the declared property type and reports its source in `ConfigValueResolution`. Enforced system values are **not trusted by default**; the caller must explicitly set `trustEnforcedSystemValues: true` only after independently establishing that the system store is protected and authenticated. Without that opt-in, an enforced marker is treated as an ordinary system override and can be superseded by user/group values.

`Config.UI.ConfigService.ConfigLayerPropertyContext` maps a trusted resolution to the shared `PropertyValueContext`, including source scope, origin, inheritance, and a read-only reason for enforced policies. Consumers should only pass a resolution marked `IsEnforced` when they actually enabled the trusted-policy opt-in above.

Use `snapshot.WithValue(key, value)` to create an explicit draft override and `snapshot.WithoutValue(key)` to remove it and reveal the next lower-precedence value. Both preserve the storage version observed at load, so a save continues to detect stale drafts.

`ConfigLayeredSettingsResolver` connects registered section defaults to explicitly supplied stores. Use `ResolveAsync` for one value or `ResolveSectionAsync` to resolve a complete section from one consistent set of loaded snapshots. The section overload accepts an optional draft snapshot, which replaces that layer's loaded snapshot for resolution:

```csharp
var resolver = new ConfigLayeredSettingsResolver(
    sectionRegistry,
    [userLayerStore, groupLayerStore, systemLayerStore],
    trustEnforcedSystemValues: false);
ConfigValueResolution endpoint = await resolver.ResolveAsync("Workbench", "AIEndpoint", cancellationToken);
IReadOnlyDictionary<string, ConfigValueResolution> settings =
    await resolver.ResolveSectionAsync("Workbench", cancellationToken: cancellationToken);
```

Only registered sections and public instance properties are accepted. Each call loads current store snapshots and surfaces storage errors; it does not cache or silently fall back from invalid layer data. A supplied draft must have a layer with a registered store.

`Config.UI.ConfigService.ConfigLayeredUiSection` adapts a registered section to the shared property-provider contract for one explicitly selected target layer. It shows effective values and per-property provenance, writes edits only to that target layer, uses the loaded storage version for stale-write detection, and maps trusted enforced policy to a read-only property. `ResetDraftAsync` removes editable visible section overrides from the draft to reveal lower layers without persisting; `ResetAsync` persists that removal. Both preserve hidden/legacy properties and trusted enforced policy overrides. Instantiate it only with an explicitly chosen layer store and provide an accurate storage location/permission decision at the host boundary. It does not choose system/group paths or establish ACLs and trust.

For a complete settings dialog bound to one scope, `ConfigLayeredUiSectionRegistry` projects every registered section through this adapter. Sections share one versioned layer draft so edits in different categories are saved together without overwriting one another or causing self-generated stale-write conflicts. Explicit Load/Discard reloads the shared snapshot and refreshes the other sections; the UI coalesces repeated discard requests by draft identity. The host chooses the target store/layer explicitly and may mark the entire registry read-only (for example, when a group/system store is not writable). It implements the existing `IConfigUiSectionRegistry`, so the shared Config.UI view model can consume it without a UI-specific layer dependency or changing the legacy registry composition.

`JsonConfigLayerStore` persists a versioned snapshot to an explicitly supplied path. Save returns the new snapshot; use it as the base for a subsequent edit so stale loads are rejected:

```csharp
IConfigLayerStore userStore = new JsonConfigLayerStore(ConfigLayer.User, userConfigPath);
ConfigLayerSnapshot userOverrides = await userStore.LoadAsync(cancellationToken);
ConfigLayerSnapshot updatedOverrides = await userStore.SaveAsync(userOverrides, cancellationToken);
```

The store fails on malformed or mismatched files instead of replacing them with defaults, writes a temporary file beside the target, rejects stale snapshots, and rejects paths traversing existing symbolic links/reparse points. The version check and write are serialized among store instances in the same process; a non-cooperating external process can still race the final check and replacement. It does not choose system/group locations, configure file permissions, resolve a group identity, or make a system file trustworthy.

`ConfigLayerStorePathResolver` can derive conventional user (`LocalApplicationData`) and system (`CommonApplicationData`) layer file paths. `CreateUserStore()` explicitly creates only the per-user layer directory; system and group paths are never created by this helper. A group file is unavailable unless an administrative group root is explicitly supplied, and its group ID must be a single safe path segment. Product and group path segments reject traversal and platform-independent path separators. The resolver does not inspect ACLs, verify group membership, or enable trusted enforced-policy resolution. The host must separately arrange/prove store permissions before trusting system policy or offering writes; path selection alone is not a security guarantee.

`ConfigLayerLegacyJsonMigrator` imports one legacy section into a user-layer store. It maps serialized names back to registered model property names, preserves unknown JSON properties as raw values, and never replaces an existing destination override. The source file is never edited or deleted. A completion marker in the destination prevents a later reset from re-importing the old value. Malformed input and stale destination snapshots fail explicitly. The migrator is not a general automatic migration policy; hosts decide which legacy sections and files to import.

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
