# Task T-Terminal-001 - Create terminal project slice and solution wiring

## Status

Done

## Goal

Create the initial terminal library project set under `..\Libraries` and wire the projects into `Libraries.slnx`.

## Scope

- Create `Terminal.Core`
- Create `Terminal.Backends.Windows`
- Create `Terminal.Backends.Posix`
- Create `Terminal.Avalonia`
- Create `Terminal.Core.Tests`
- Add the new projects to `..\Libraries\Libraries.slnx`

## Out of Scope

- Terminal runtime behavior
- Backend implementation details
- Avalonia control rendering details

## Done Criteria

- All planned projects exist in the workspace
- Project references are wired for the first vertical slice
- `Libraries.slnx` includes the new projects
