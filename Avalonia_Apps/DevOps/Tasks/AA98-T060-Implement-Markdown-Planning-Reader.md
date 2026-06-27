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

## Validation
- Add targeted reader tests or execute them in `AA98-T061`.

## Status
- Proposed
