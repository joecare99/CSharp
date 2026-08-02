# AA98-T067 Persist Local Planning Document Edits

## Parent
- Feature: `../Features/AA98-F43-Repository-and-Planning-Workflows.md`
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`

## Goal
Persist explicit edits to local Markdown planning documents through the provider-neutral planning contract.

## Scope
- Reuse `IPlanningProvider.WriteAsync` and its existing write request and result models.
- Preserve existing Markdown document content while updating editable title and status metadata.
- Expose an explicit save command in the reusable planning explorer.
- Keep external provider synchronization out of scope.

## Delivered
- Extended `LocalPlanningProvider.WriteAsync` to preserve existing Markdown content when writing an existing planning document.
- Updated the top-level heading and status section from edited planning metadata while retaining other document sections.
- Added an explicit `SaveSelectedItemCommand` to `PlanningExplorerViewModel` and a corresponding Save button in the document detail area.
- Added regression tests for preserving custom Markdown and for the explorer's provider-neutral write request.

## Validation
- Targeted persistence test run completed successfully: 6 executions passed, 0 failed.
- Builds succeeded for `AA98_AvlnCodeStudio.Planning.Local` and `AA98_AvlnCodeStudio.Planning.UI`.

## Follow-up
- Persist parent or related-parent link edits only after the editing UI exposes those fields deliberately.
- Add conflict detection or reload-before-save behavior before connected provider synchronization is introduced.

## Status
- Completed
