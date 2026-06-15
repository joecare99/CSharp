# AA19-T001 Complete Avalonia FilterLists Migration

## Status
Done

## Parent
- Follow-up completion task for `AA19_FilterLists` after the WPF-to-Avalonia migration

## Goal
Complete the Avalonia port of `AA19_FilterLists` so the main window exposes the same source-code tabs as the original WPF sample and the person view more closely matches the richer WPF interaction model.

## Scope
- Restore the main tab layout in `MainWindow.axaml` with the runnable sample tab and source-code tabs.
- Expose the current Avalonia view source and the backing ViewModel source through the main window ViewModel.
- Expand `PersonView.axaml` beyond the simplified filter list into a richer master-detail layout with list/table style navigation and editable person details.
- Keep the implementation MVVM-friendly and aligned with the existing Avalonia dependency-injection setup.
- Update or add regression tests for the adjusted ViewModel behavior.

## Assumptions
- The legacy WPF XAML files remain reference material only.
- `Properties/Resources.resx` remains the source for embedded source-code snippets shown in the UI.
- Adding Avalonia's DataGrid package is acceptable if needed to mirror the former table-oriented view.

## Open Questions
- Whether a later cleanup task should remove or archive obsolete WPF files after the Avalonia port has reached parity.

## Tasks
- [x] Analyze the current `AA19_FilterLists` Avalonia and WPF files.
- [x] Document the completion scope in DevOps.
- [x] Extend the main window ViewModel for source-tab content.
- [x] Restore the main Avalonia tab layout.
- [x] Expand the Avalonia person view interaction model.
- [x] Update or add regression tests.
- [x] Validate the project build and relevant tests.
- [x] Mark this task completed after validation.

## Validation
- `dotnet build C:\Projekte\CSharp\Avalonia_Apps\AA19_FilterLists\AA19_FilterLists\AA19_FilterLists.csproj --configuration Debug -f net10.0 -m:1` succeeded.
- `dotnet test C:\Projekte\CSharp\Avalonia_Apps\AA19_FilterLists\AA19_FilterListsTests\AA19_FilterListsTests.csproj --configuration Debug -f net10.0 --filter "FullyQualifiedName~MainWindowViewModelTests|FullyQualifiedName~PersonViewViewModelTests"` succeeded: 4 total, 4 passed, 0 failed, 0 skipped.
- A full multi-target build and the legacy `PersonTests` suite were not used as acceptance gates because the repository currently contains unrelated net8.0 PDB locking noise during parallel multi-target builds and pre-existing legacy `PersonTests` failures.

## Notes
- The WPF sample used the main window as a documentation shell around the person view; the Avalonia port now preserves that tutorial character.
- Existing unsupported older framework warnings elsewhere in the repository are not part of this task's acceptance scope.
