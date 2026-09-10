# G-Architecture

## Status
Passed

## Required evidence
- Component and dedicated test-project boundaries.
- Dependency diagram and reference audit.
- Public contracts use interfaces/capabilities and DI.
- Base contracts have no product, UI, OS, Model, Planning, or ConsoleLib dependency unless expressly assigned.
- UI text is localizable at UI/registration boundaries.

## Decision
Approved for T-001, T-002, and T-005 preparation. The Config.Service audit found a
standalone net8.0 component with DI abstractions only and no product, UI,
ConsoleLib, CodeStudio, Planning, WinForms, or WPF references. The remaining
persistence and registration edge cases are assigned to T-002 and T-003. The
T-005 Code.Navigation audit confirms the same boundary for the source-location
contract and navigator capability.

T-020 confirms that `AppKomponentBaseLib` remains the shared base for component
contracts, while `Diagnostics.Navigation` references only the shared diagnostic
model and neutral `Code.Navigation` types. No UI or editor dependency was added.

- Reviewer: implementation agent
- Date: 2026-08-26
- Affected tasks: T-001 and T-005
- Next task status: T-006 In Progress after the required SVN commit
