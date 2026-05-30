# Avln_Marble Migration to Avalonia

## Parent
- Vision: `DevOps/Vision.md`

## Goal
Create a new `Avln_Marble` Avalonia application set that ports the existing WPF marble board prototype to a shared Avalonia UI project with Desktop and Browser hosts while keeping `MarbleBoard.Engine` largely unchanged.

## Scope
- Create `Avln_Marble.Display` as the shared Avalonia UI project.
- Create `Avln_Marble.Desktop` as the desktop host.
- Create `Avln_Marble.Browser` as the browser host.
- Reference only `C:\Projekte\CSharp\CSharpBible\Graphics\MarbleBoard.Engine\MarbleBoard.Engine.csproj` from the new projects.
- Port the existing WPF board rendering and interaction behavior to Avalonia.

## Included
- Shared Avalonia views and view models
- Desktop startup wiring
- Browser startup wiring
- Pointer dragging and keyboard interaction
- Visual styling and marble rendering approximations in Avalonia
- Local build validation

## Excluded for Now
- Functional changes in `MarbleBoard.Engine`
- New game rules or board logic beyond the existing prototype behavior
- Packaging, publishing, or deployment automation
- Cross-repository refactoring of the source WPF project

## Success Indicators
- The new projects build successfully.
- The desktop host starts and shows the migrated marble board UI.
- The browser host compiles against the Avalonia browser target.
- Selection, dragging, drop target preview, and keyboard movement remain available.

## Assumptions
- `Avln_Marble.Display` will own UI-specific logic and rendering concerns.
- `MarbleBoard.Engine` remains the shared state and movement model.
- Browser hosting will follow the existing Avalonia browser pattern in this workspace.

## Open Questions
- Which visual effects require simplification because of Avalonia or browser rendering differences?
- A dedicated test project has not been added yet; current validation is based on targeted project builds and should be extended with focused tests in a later increment.

## Tasks
- [x] Inspect existing Avalonia shared, desktop, and browser project patterns
- [x] Create `Avln_Marble` project structure
- [x] Port shared view models and board interaction logic
- [x] Port shared Avalonia views and styling
- [x] Add desktop host bootstrap
- [x] Add browser host bootstrap
- [x] Validate builds and targeted tests

## Validation Notes
- `Avln_Marble.Display` builds successfully on `net10.0`.
- `Avln_Marble.Desktop` builds successfully on `net10.0`.
- `Avln_Marble.Browser` builds successfully on `net10.0-browser`.
- A full workspace build is currently not a reliable quality gate for this feature because unrelated existing projects fail with workload, import, and Edit-and-Continue errors.
- The reported `BoardView` startup `NullReferenceException` was fixed by resolving the `BoardSurface` canvas explicitly instead of relying on a generated field.

## Status
- Completed
