# AA98-Bl009 File-Type Identification and Behavior Selection

## Parent
- Feature: `DevOps/Features/AA98-F08-File-Type-Aware-Editor-Behavior.md`
- Epic: `DevOps/Epics/AA98-E02-Editor-Framework.md`
- Vision: `DevOps/Vision.md`

## Scope
Define the first file-type-aware behavior baseline for `AA98_AvlnCodeStudio` so the editor can identify document types and select suitable behavior without overcomplicating the shared editor core.

## Goals
- Define the initial rules for identifying document types from file context.
- Introduce a baseline selection model for file-type-specific behavior.
- Separate shared editor responsibilities from specialized behavior for supported document types.
- Prepare extension seams for later per-type editor services beyond syntax highlighting.

## Assumptions
- Lightweight identification and selection rules are sufficient for the first type-aware baseline.
- The shared editor core should stay stable while type-specific behavior grows incrementally.
- Rich language-service integration and preview-oriented behaviors belong to later increments.

## Open Questions
- Should markdown preview remain outside this first type-aware baseline?
- How early should file-type-specific commands be separated from shared editor commands?

## Status
- Proposed
