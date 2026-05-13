# SH-T010303 - Reset UI State On New Run

## Work Item Type
Task

## Parent
`SH-BI0103 - Add Restart and New Run Flow`

## Goal
Ensure UI state is cleared or rebuilt when a new run starts.

## Scope
- Reset messages, inventory view, map buffers, minimap cache, and state summaries.
- Avoid stale references to old sessions.
- Preserve user settings where appropriate.

## Done
- New run UI does not display stale map, inventory, or terminal messages from prior runs.

## Status
Completed
