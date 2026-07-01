# AA98-T060 Implement Markdown Planning Reader

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl043-DevOps-Planning-Local-Model-Baseline.md`

## Goal
Implement a reader that turns local DevOps markdown files into structured planning items.

## Scope
- Read files from `DevOps/Epics`, `DevOps/Features`, `DevOps/BacklogItems`, and `DevOps/Tasks`.
- Extract stable ID, title, parent references, status, and source path.
- Report parsing diagnostics without stopping the full read when one file is invalid.

## Execution Notes
1. Keep the reader independent from Azure DevOps and GitHub.
2. Use deterministic parsing rules based on current planning conventions.
3. Avoid using the `DevOps` directory for runtime state.

## Acceptance Criteria
- Current repository planning files can be loaded as structured items.
- Duplicate IDs and missing expected sections can be reported.

## Delivered
- Added provider-neutral planning read request and result contracts plus a local planning reader service interface in `AA98_AvlnCodeStudio.Base`.
- Implemented `MarkdownPlanningReader` to scan the local `DevOps` hierarchy, derive item kinds from folder structure, parse IDs and titles from markdown headings, resolve parent references, normalize statuses, and keep per-file diagnostics instead of aborting the full read.
- Added deterministic fixture-based tests that validate successful hierarchy loading, parent-child link resolution, duplicate-ID reporting, and missing-parent diagnostics.

## Validation
- Add targeted reader tests or execute them in `AA98-T061`.
- `dotnet test AA98_AvlnCodeStudio/AA98_AvlnCodeStudio.Tests/AA98_AvlnCodeStudio.Tests.csproj --no-restore --filter "FullyQualifiedName~MarkdownPlanningReaderTests"`

## Status
- Completed
