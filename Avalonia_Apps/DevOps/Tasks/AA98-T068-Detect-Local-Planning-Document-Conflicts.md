# AA98-T068 Detect Local Planning Document Conflicts

## Parent
- Feature: `../Features/AA98-F43-Repository-and-Planning-Workflows.md`
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`

## Goal
Prevent local Markdown planning edits from silently overwriting changes made after a document was loaded.

## Scope
- Extend the provider-neutral write request with optional loaded-document snapshots.
- Compare existing local Markdown files against their loaded snapshot before writing.
- Return a diagnostic and leave a conflicting document unchanged.
- Pass the selected document snapshot from the planning explorer save command.

## Delivered
- Added `ExpectedDocumentTexts` to `PlanningWriteRequest`, keyed by source path.
- Added `PLW002` conflict detection in `LocalPlanningProvider.WriteAsync` for externally changed local documents.
- Updated `PlanningExplorerViewModel` to retain loaded document text and provide it to the write request.
- Added provider and explorer regression tests for conflict protection and snapshot propagation.
- Added an explicit reload command that reloads the latest provider state and discards unsaved local explorer changes.

## Follow-up
- Keep merge and connected-provider synchronization behavior out of this local safety slice.

## Status
- Completed
