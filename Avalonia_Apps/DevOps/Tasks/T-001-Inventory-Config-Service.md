# T-001 Inventory Config.Service

## Parent
B-Config-Core | Previous: none | Next: T-002

## Status
Done

## Request
Inspect `Config.Service` and `Config.Service.Tests`; record public contracts, JSON store behavior, DI registration, framework targets, product dependencies, and current tests. Produce a migration decision with no code transfer yet.

## Planned tests
- Run existing Config.Service MSTest suite on net8.0.
- Static dependency audit proves no RnzTrauer, Avalonia, ConsoleLib, or CodeStudio model reference.
- Record baseline for default root and CONFIG_ROOT behavior.

## Inventory evidence

### Project and framework

- `Config.Service.csproj` targets `net8.0` and conditionally adds `net9.0`,
  `net10.0`, and `net11.0` when the corresponding SDK is available.
- Nullable reference types are enabled and implicit usings are disabled.
- The only direct package reference is
  `Microsoft.Extensions.DependencyInjection.Abstractions`.
- `Config.Service.Tests.csproj` is a dedicated non-packable MSTest project with
  a project reference to the service and package references for the test SDK,
  MSTest, and NSubstitute.

### Public contracts and implementation

- `IConfigStore` defines asynchronous load, save, and reset operations by
  stable section key.
- `IConfigSectionProvider` supplies stable name, display metadata, ordering,
  model type, and a default model factory. It has no UI or operating-system
  dependency; descriptions can be supplied by the registration/UI boundary.
- `IConfigSectionRegistry` provides sorted section access, case-insensitive
  lookup/removal, registration, and change notifications.
- `ConfigService` is a disposable facade that composes a base key with section
  keys and exposes model, string, enum, and validation lookups.
- `ConfigSectionRegistration` preserves provider metadata while allowing a
  localized description override.
- `ConfigModelInspector`, `ConfigPropertyInfo`, and `ConfigPropertyKind`
  provide the metadata needed by a future shared configuration UI. Sensitive
  and ignored properties are represented through the existing attributes.

### Persistence and DI behavior

- `JsonConfigStore` stores one JSON document per section under
  `%LOCALAPPDATA%\\<Vendor>\\<Application>\\config` by default.
- When `CONFIG_ROOT` is non-empty, it is used as the root and the same vendor,
  application, and `config` segments are appended.
- JSON uses indented output and camel-case property naming. Missing files,
  null deserialization results, and `JsonException` return the supplied
  fallback value. Save creates the directory through the store constructor.
- Reset currently writes an empty file and suppresses all write exceptions;
  the next load therefore returns the fallback after the empty JSON document is
  treated as invalid. This behavior must be specified and secured by T-003.
- `AddConfigService` registers one registry, store, and facade as singletons.
  Section registration adds the provider and a `ConfigSectionRegistration` to
  the service collection and keeps localized descriptions outside the core.

### Existing test baseline and gaps

The net8.0 suite contains eight passing tests covering JSON roundtrip and
fallback behavior, multiple keys, sensitive-value roundtrip, registry events
and lookup, UI-layer descriptions, and string/enum/validation lookups.

The baseline does not explicitly test `CONFIG_ROOT`, default local-app-data
path construction, reset semantics, invalid JSON, inaccessible directories,
DI singleton composition, or invalid registration inputs. These cases are
accepted as follow-up coverage for T-002 and T-003 rather than changed in this
inventory task.

### Dependency audit and migration decision

The static source/project audit found no reference to Avalonia, ConsoleLib,
CodeStudio models, Planning, WinForms, WPF, or a product-specific project.
The only product-name occurrence is an illustrative XML documentation example
for a stable section key. The component is approved for shared adoption
without code transfer; hosts should reference it through its existing public
contracts and DI extensions. No production code migration is required for
T-001.

## Execution evidence

- `dotnet restore Config.Service.Tests.csproj --force`: passed.
- `dotnet test Config.Service.Tests.csproj --framework net8.0`: passed,
  8 tests passed, 0 failed, 0 skipped.
- The build reported five existing nullable warnings in test fixture code;
  none originate in the production component.
- Initial `--no-restore` execution failed with `NETSDK1064` because
  `MSTest.Analyzers 4.3.3` was missing locally; forced restore resolved it.

## Gate and completion
G-Architecture review must approve the audit before T-002. Before Done: document results, run planned tests, commit all changes in English through SVN, record revision/message/results, set this task Done, set T-002 In Progress, and update B-Config-Core and F-Shared-CodeStudio-Components status.

## Gate decision

Approved for T-002. The architecture boundary is product-neutral and DI-based;
the identified persistence and DI test gaps are explicitly assigned to the
following tasks.

## SVN and status handover

- SVN revisions: 1831 for the completed T-001 evidence and status handover,
  1832 for recording the first revision, and 1833 for finalizing this evidence.
- Commit messages: `Complete Config Service inventory`, `Record Config Service
  inventory revision`, and `Finalize Config Service evidence`.
- Next task: T-002 remains the next implementation task and is set to In
  Progress after the SVN commit is recorded.
