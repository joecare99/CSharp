# AA19-T002 Add Multi-Host Support and Person Data Service

## Status
Done

## Parent
- Follow-up architecture task for `AA19_FilterLists`

## Goal
Refactor `AA19_FilterLists` so the Avalonia application can run through dedicated Desktop and Browser hosts and source its person data from an explicit model/data service instead of handling the list directly in the ViewModel.

## Scope
- Convert the current `AA19_FilterLists` project into a shared Avalonia application core.
- Add a reusable host-neutral main view for desktop and browser lifetimes.
- Add dedicated `AA19_FilterLists.Desktop` and `AA19_FilterLists.Browser` host projects.
- Introduce an explicit person data service abstraction and implementation in the core project.
- Update ViewModels, dependency injection, and tests to use the new data service.
- Validate relevant builds and focused tests for the changed architecture.

## Assumptions
- Browser support is the preferred path for remote-display-style usage.
- The existing WPF files remain reference material only.
- A filtered projection for UI display may remain ViewModel-owned, while the authoritative person collection must come from the data service.

## Open Questions
- Whether a later task should add a dedicated remote session host beyond the browser host.

## Tasks
- [x] Analyze the current AA19 structure and reference host patterns.
- [x] Document the architecture scope in DevOps.
- [x] Refactor the core project for host-neutral lifetimes.
- [x] Introduce the explicit person data service.
- [x] Add desktop and browser host projects.
- [x] Update or add regression tests.
- [x] Add the new projects to the solution.
- [x] Validate builds and relevant tests.
- [x] Mark this task completed after validation.

## Validation
- `dotnet sln C:\Projekte\CSharp\Avalonia_Apps\Avalonia_Apps.slnx add C:\Projekte\CSharp\Avalonia_Apps\AA19_FilterLists\AA19_FilterLists.Desktop\AA19_FilterLists.Desktop.csproj C:\Projekte\CSharp\Avalonia_Apps\AA19_FilterLists\AA19_FilterLists.Browser\AA19_FilterLists.Browser.csproj` succeeded.
- `dotnet build C:\Projekte\CSharp\Avalonia_Apps\AA19_FilterLists\AA19_FilterLists\AA19_FilterLists.csproj --configuration Debug -f net10.0 -m:1` succeeded.
- `dotnet build C:\Projekte\CSharp\Avalonia_Apps\AA19_FilterLists\AA19_FilterLists.Desktop\AA19_FilterLists.Desktop.csproj --configuration Debug -f net10.0 -m:1` succeeded.
- `dotnet build C:\Projekte\CSharp\Avalonia_Apps\AA19_FilterLists\AA19_FilterLists.Browser\AA19_FilterLists.Browser.csproj --configuration Debug -m:1` succeeded.
- `dotnet test C:\Projekte\CSharp\Avalonia_Apps\AA19_FilterLists\AA19_FilterListsTests\AA19_FilterListsTests.csproj --configuration Debug -f net10.0 -m:1 /p:BuildInParallel=false --filter "FullyQualifiedName~MainWindowViewModelTests|FullyQualifiedName~PersonViewViewModelTests|FullyQualifiedName~PersonDataServiceTests"` succeeded: 7 total, 7 passed, 0 failed, 0 skipped.
- The focused test run still reports pre-existing warnings in the legacy `PersonTests` file; those warnings were not changed by this task.

## Notes
- The browser host enables the same app logic for local browser execution and browser-based remote display scenarios.
- Desktop-specific bootstrapping no longer remains in the shared core project.
- The former application-settings persistence was removed from the shared core because the new multi-host/browser-ready data service must not depend on `System.Configuration`.
