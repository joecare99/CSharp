# AA05-T001 Split CommandParCalc Into Core Desktop and Browser

## Parent
- Backlog Item: `AA05 local workspace restructuring for Avalonia host separation`

## Goal
Split `AA05_CommandParCalc` into a shared Avalonia application project plus dedicated `Desktop` and `Browser` host projects following the structure already used in `Avalonia_App01`.

## Scope
- Convert the existing `AA05_CommandParCalc` project into the shared Avalonia application project.
- Create `AA05_CommandParCalc.Desktop` as the classic desktop host.
- Create `AA05_CommandParCalc.Browser` as the browser host.
- Keep existing tests targeting the shared project unless a host-specific dependency requires a change.

## Assumptions
- The existing `App` type can remain in the shared project.
- Desktop startup code can move out of the current `Program.cs` into the new desktop host.
- Browser startup can follow the same baseline pattern as `Avalonia_App01.Browser`.

## Open Questions
- Whether browser hosting needs additional app-specific assets beyond the standard `wwwroot` baseline.
- Whether any current code paths assume a desktop-only lifetime and need a single-view fallback.

## Tasks
- [x] Analyze the current AA05 project against the Avalonia_App01 reference structure.
- [x] Convert the AA05 base project into a shared host-agnostic Avalonia project.
- [x] Add the desktop host project and wire it to the shared project.
- [x] Add the browser host project and wire it to the shared project.
- [x] Validate build and relevant AA05 tests.
- [x] Mark this task completed after validation.

## Notes
- This task documents a local workspace restructuring step and should be kept in sync with implementation progress.
- Validation result: targeted build for `AA05_CommandParCalcTests` succeeded, and the full `AA05_CommandParCalcTests` suite passed with 474 successful tests, 0 failed, 0 skipped.
- Workspace-wide build remains affected by unrelated external workload and independent project issues outside the AA05 scope.
