# AA40-T002 Fix Wizzard Document Rendering and Selection Refresh

## Parent
- Follow-up correction for `AA40_Wizzard` after initial Avalonia migration

## Goal
Restore semantically correct document resources, render the selection description with formatting, fix missing image display, and stabilize redraw behavior when the selected entry changes.

## Scope
- Rename document assets away from `.txt` back to proper document-oriented `.xaml` files.
- Keep document XAML as data resources rather than Avalonia UI markup.
- Improve the content service so it can provide formatted document content and reliable image loading.
- Stabilize the page-1 selection/view refresh behavior.
- Adjust tests and validate the corrected project set.

## Assumptions
- WPF FlowDocument XAML is still the source format for the descriptive content.
- A read-only formatted viewer is sufficient for this wizard scenario.
- The current redraw issue is caused by unstable binding state rather than host-level rendering issues.

## Open Questions
- Whether a later slice should move the document rendering into a reusable shared viewer component.

## Tasks
- [x] Analyze the current AA40_Wizzard implementation and the available Flow document helper project.
- [x] Document the correction scope in DevOps.
- [x] Restore semantic document asset naming and build actions.
- [x] Implement formatted document rendering and reliable image loading.
- [x] Fix page selection refresh behavior.
- [x] Update or add regression tests.
- [x] Validate build and relevant tests.
- [x] Mark this task completed after validation.

## Notes
- The original migrated `.txt` extension should be removed because the assets remain FlowDocument XAML content.
- `Document.Flow` may help as a parsing/reference layer, but the final rendering path must stay practical for Avalonia Desktop and Browser.