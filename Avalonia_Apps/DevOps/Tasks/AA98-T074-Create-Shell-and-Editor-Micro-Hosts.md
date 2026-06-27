# AA98-T074 Create Shell and Editor Micro Hosts

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl048-Component-Micro-Host-Foundation.md`

## Goal
Create the first shell and editor micro hosts using the common micro-host pattern.

## Scope
- Add `AA98.Shell.Host` or equivalent host for shell composition validation.
- Add `AA98.Editor.Host` or equivalent host for editor workflow validation.
- Keep both hosts thin and focused.

## Execution Notes
1. Implement only after `AA98-T073` defines the pattern.
2. Reuse existing component code rather than duplicating it.
3. Keep user-facing text localizable where applicable.

## Acceptance Criteria
- Shell and editor hosts build and run their first smoke scenarios.
- Host projects do not contain component core logic.

## Validation
- Build host projects.
- Run shell/editor host smoke checks.

## Status
- Proposed
