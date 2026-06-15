# AA28-T001 Create Avalonia DataGridExt Project Set

## Parent
- Backlog Item: `AA28 local workspace migration of MVVM_28_1_CTDataGridExt to Avalonia`

## Goal
Create a new Avalonia-based `AA28_DataGridExt` project set derived from the WPF sample `MVVM_28_1_CTDataGridExt_net` without modifying the original project.

## Scope
- Create a separate model project for the shared data types.
- Create a shared Avalonia application project for UI, services, view models, and views.
- Create dedicated desktop and browser host projects.
- Create a dedicated MSTest project covering migrated models, services, view models, converters, app startup, and basic Avalonia view instantiation.
- Keep the original WPF sample and its tests untouched.

## Assumptions
- The new shared application project can target the same modern .NET desktop/browser baselines already used by Avalonia samples in this workspace.
- Department display strings can be kept provider-neutral in the new model layer instead of depending on the WPF resource designer.
- Avalonia headless testing is sufficient for basic UI instantiation validation in the new test project.

## Open Questions
- Whether the migrated sample should keep localized resource files or use inline English labels for the first Avalonia slice.
- Whether the browser host needs additional styling or data-grid specific tuning beyond the repository baseline.

## Tasks
- [x] Analyze the WPF reference project and its tests.
- [x] Define and document the Avalonia target structure in DevOps.
- [x] Create the new model, app, desktop, browser, and test projects.
- [x] Migrate models, services, view models, converters, and startup wiring.
- [x] Implement Avalonia views for desktop and browser capable usage.
- [x] Add and adapt tests for the migrated project set.
- [x] Validate build and relevant tests.
- [x] Mark this task completed after validation.

## Notes
- The target architecture follows the repository preference for component-based composition with a separate model project and dedicated host projects.
- The implementation should support both Desktop and Browser hosts from the beginning.
- Follow-up refinement: the sample `IPersonService` and `PersonService` were moved from the shared UI project into `AA28_DataGridExt.Model` to keep the UI layer host-focused.
- Follow-up refinement: the shared Avalonia app now loads the DataGrid Fluent theme resource so rows render correctly at runtime.
- Validation result: targeted build for `AA28_DataGridExt.Tests` succeeded for `net8.0`, `net9.0`, and `net10.0` after the service relocation and DataGrid theme fix.
- Validation result: relevant `AA28_DataGridExt.Tests` execution completed with 48 successful tests, 0 failed, 0 skipped after the follow-up refinement.
- Workspace-wide build remains affected by unrelated external projects, including platform TFM configuration issues in `Avalonia_App01` mobile hosts and missing referenced projects in the original `CSharpBible` WPF sample area.
