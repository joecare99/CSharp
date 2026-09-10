# B-Config-Core

## Parent
F-Shared-CodeStudio-Components

## Status
Done

## Request
Adopt `RnzTrauer/Config.Service` as the standalone, product-neutral configuration component for shared hosts. Preserve its `IConfigStore`, `IConfigSectionRegistry`, `IConfigSectionProvider`, `ConfigService`, JSON persistence, `%LOCALAPPDATA%/<Vendor>/<Application>/config` convention, and `CONFIG_ROOT` override.

## Acceptance criteria
- No RnzTrauer, Avalonia, ConsoleLib, or CodeStudio model types in the core.
- Hosts register vendor/application identity and sections through DI.
- Section metadata supports localization at the registering UI boundary.
- Defaults, load, save, reset, validation, and failure behavior are specified and tested.

## Tasks
T-001, T-002, T-003, T-004

## Gates
G-Architecture before T-002; G-Component-Quality after T-004.

## Completion evidence
Task test reports, SVN revisions, dependency audit, and next backlog status.