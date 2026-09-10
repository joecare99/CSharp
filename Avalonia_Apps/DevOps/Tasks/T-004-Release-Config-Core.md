# T-004 Release Config Core

## Parent
B-Config-Core | Previous: T-003 | Next: T-005

## Status
Done

## Request
Complete Config.Service component documentation, host registration guidance, and independent component validation.

## Planned tests
- Execute complete Config.Service.Tests suite on net8.0.
- Build Config.Service and tests on each available target framework.
- Verify registration guidance can be followed by a minimal host fixture.
- Check documented warning policy; only explicitly accepted warnings remain.

## Release evidence

- `Config.Service.Tests`: 57 passed, 0 failed, 0 skipped across `net8.0`,
  `net9.0`, and `net10.0` in Release configuration.
- `Config.Service` Release build passed on `net8.0`, `net9.0`, and `net10.0`.
- `Config.Service.Tests` Release build passed on `net8.0`, `net9.0`, and
  `net10.0`.
- Minimal host registration is covered by the DI regression tests for singleton
  resolution, vendor/application identity, provider ordering, localized
  descriptions, and invalid registration arguments.
- Production code produced no warnings. The test build reported only the
  existing nullable warnings in test fixtures and tests; these are accepted by
  the repository warning policy and do not affect the release component.
- No additional production change was required for release validation.

## Host registration guidance

1. Register the component with `services.AddConfigService(vendorName,
   applicationName)`.
2. Register each section with `AddConfigSection<TModel>(provider)` or provide
   a UI-localized description through the overload accepting `description`.
3. Resolve `ConfigService`, `IConfigStore`, or
   `IConfigSectionRegistry` from the host container.
4. Use `CONFIG_ROOT` only for controlled test or deployment scenarios; the
   default path remains `%LOCALAPPDATA%/<Vendor>/<Application>/config`.

## Gate and completion
Pass G-Component-Quality. Before Done: publish evidence in this task, English SVN commit and revision record, set B-Config-Core Done, T-005 In Progress, and update feature status.

## Gate decision

G-Component-Quality is approved. The shared configuration core is release-ready
for the current target frameworks and may be consumed by the next shared
component backlog item.

## SVN and status handover

- SVN revisions: 1839 for the release documentation and gate evidence, and
  1840 for the final status handover and T-005 activation.
- English commit message: `Release Config.Service core`.
- Current task: T-004 Release Config Core is Done.
- Next task: T-005 Define Code Location is In Progress.
- Backlog: B-Config-Core is Done; B-Code-Navigation is In Progress.
- Feature: F-Shared-CodeStudio-Components remains In Progress because other
  shared components are still pending.
