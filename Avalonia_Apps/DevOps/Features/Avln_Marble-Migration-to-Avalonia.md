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
- Should a dedicated test project be added immediately, or can validation begin with build coverage and later focused tests?

## Tasks
- [x] Inspect existing Avalonia shared, desktop, and browser project patterns
- [ ] Create `Avln_Marble` project structure
- [ ] Port shared view models and board interaction logic
- [ ] Port shared Avalonia views and styling
- [ ] Add desktop host bootstrap
- [ ] Add browser host bootstrap
- [ ] Validate builds and targeted tests

## Status
- In Progress
