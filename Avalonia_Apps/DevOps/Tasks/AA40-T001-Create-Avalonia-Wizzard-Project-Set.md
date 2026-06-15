# AA40-T001 Create Avalonia Wizzard Project Set

## Parent
- Direct workspace migration request for `MVVM_40_Wizzard_net`

## Goal
Create a new Avalonia-based `AA40_Wizzard` project set derived from the WPF sample `MVVM_40_Wizzard_net` without modifying the original project.

## Scope
- Create a separate model project for shared models and services.
- Create a shared Avalonia application project for view models, views, localization helpers, DI wiring, and startup logic.
- Create dedicated desktop and browser host projects.
- Create a dedicated MSTest project for migrated models, services, view models, startup, converters, and Avalonia views.
- Keep the original WPF sample and its tests untouched.

## Assumptions
- The new project set follows the existing repository pattern used by other Avalonia desktop/browser samples.
- Embedded Avalonia assets replace file system based WPF resource access so the migration works on Desktop and Browser.
- Headless Avalonia testing is sufficient for view instantiation and startup validation.

## Open Questions
- Whether a later refinement should restore richer formatted document rendering beyond the first Avalonia-compatible migration slice.

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
- The target architecture should follow the repository preference for a separate model/service component plus a shared UI component and dedicated hosts.
- The migrated project set should support Desktop and Browser from the beginning.
- Localization should move from static WPF XAML resource lookups to view-model driven Avalonia bindings so culture changes can refresh visible text without recreating the original WPF project.