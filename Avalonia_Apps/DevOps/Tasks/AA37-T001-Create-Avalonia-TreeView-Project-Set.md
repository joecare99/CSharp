# AA37-T001 Create Avalonia TreeView Project Set

## Parent
- Direct workspace migration request for `MVVM_37_TreeView_net`

## Goal
Create a new Avalonia-based `AA37_TreeView` project set derived from the WPF sample `MVVM_37_TreeView_net` without modifying the original project.

## Scope
- Create a separate model project for shared models and services.
- Create a shared Avalonia application project for view models, views, DI wiring, and startup logic.
- Create dedicated desktop and browser host projects.
- Create a dedicated MSTest project for migrated models, services, view models, startup, and Avalonia views.
- Keep the original WPF sample and its tests untouched.

## Assumptions
- The new project set follows the existing repository pattern used by other Avalonia desktop/browser samples.
- Headless Avalonia testing is sufficient for view instantiation and startup validation.
- The TreeView selection handling may be adapted to Avalonia patterns while preserving the original user-facing behavior.

## Open Questions
- Whether a later refinement should add a browser-specific responsive layout beyond the first migration slice.

## Tasks
- [x] Analyze the WPF reference project and its tests.
- [x] Define and document the Avalonia target structure in DevOps.
- [x] Create the new model, app, desktop, browser, and test projects.
- [x] Migrate models, services, view models, and startup wiring.
- [x] Implement Avalonia views for desktop and browser capable usage.
- [x] Add and adapt tests for the migrated project set.
- [x] Validate build and relevant tests.
- [x] Mark this task completed after validation.

## Notes
- The target architecture follows the repository preference for a separate model/service component plus shared UI component and dedicated hosts.
- The implementation should support both Desktop and Browser from the beginning.
- The migrated project set was created as `AA37_TreeView.Model`, `AA37_TreeView`, `AA37_TreeView.Desktop`, `AA37_TreeView.Browser`, and `AA37_TreeView.Tests` without modifying the original WPF sample.
- Tree selection was adapted to Avalonia by binding `SelectedItem` to a dedicated selected node and mirroring the result into `SelectedBook`.
- Validation result: targeted builds succeeded for `AA37_TreeView.Tests` on `net8.0`, `net9.0`, and `net10.0`, plus dedicated builds succeeded for the Desktop and Browser hosts.
- Validation result: Test Explorer executed 57 tests with 57 successful, 0 failed, 0 skipped; `dotnet test` executed 72 tests across all configured test TFMs with 72 successful, 0 failed, 0 skipped.
- Workspace-wide build still remains affected by unrelated existing solution issues, including missing project metadata in `AA98_AvlnCodeStudio`, platform-version issues in `Avalonia_App01` mobile hosts, and unresolved external `CSharpBible` dependencies.
