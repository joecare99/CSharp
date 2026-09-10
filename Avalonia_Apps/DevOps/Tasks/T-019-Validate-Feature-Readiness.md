# T-019 Validate Feature Readiness

## Parent
B-Designer-Consumption | Previous: T-018 | Next: none

## Status
Planned

## Request
Validate the complete plan implementation evidence and close the feature only when every component task has tests, SVN commit, and status handover records.

## Planned tests
- Run all dedicated component test projects on net8.0.
- Build production/test projects for available target frameworks.
- Verify host DI integration: Config.Service/Config.UI, Explorer-to-Editor, Diagnostics-to-Navigation, Property Editor consumption.
- Inspect accepted warnings under repository policy.

## Gate and completion
Pass G-Release-Readiness. Before Done: record full test matrix, English SVN commit/revision, set B-Designer-Consumption, F-Shared-CodeStudio-Components, and E-Shared-CodeStudio-Components Done; no next task may be started until status evidence is complete.