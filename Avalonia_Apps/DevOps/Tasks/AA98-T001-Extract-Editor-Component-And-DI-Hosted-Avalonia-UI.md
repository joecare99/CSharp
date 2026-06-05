# AA98-T001 Extract Editor Component And DI Hosted Avalonia UI

## Parent
- Backlog Item: `DevOps/BacklogItems/AA98-Bl010-Component-Registration-Baseline.md`
- Feature: `DevOps/Features/AA98-F03-Workbench-Startup-and-Composition.md`
- Epic: `DevOps/Epics/AA98-E02-Editor-Framework.md`

## Scope
Extract the source editor into the first explicit component so the non-UI editor workflow stays UI-agnostic and OS-agnostic, while the Avalonia editor UI is hosted through dependency injection.

## Goals
- Introduce a component-oriented editor composition seam.
- Keep editor workflow services independent from Avalonia-specific UI types.
- Host the Avalonia editor control through DI so the shell depends on abstractions instead of concrete editor widgets.
- Prepare the editor area for later component registration and alternative UI implementations.
- Restructure the editor into a dedicated component project with a dedicated test project.
- Keep `AA98_AvlnCodeStudio.Base` dependency-light and free of `Model` dependencies.
- Prepare scoped base libraries such as `AA98_AvlnCodeStudio.Base.UI`, `AA98_AvlnCodeStudio.Base.OS`, and a provider-agnostic `AA98_AvlnCodeStudio.Base.AI`.

## Assumptions
- The first component registration remains explicit and solution-internal.
- Editor-facing texts stay in the UI layer.
- Existing single-document behavior remains intact after the architectural extraction.
- A component in this repository is represented by its own project file plus a dedicated test project.
- Scoped base libraries may depend on `AA98_AvlnCodeStudio.Base`, but not on each other.
- Provider-agnostic AI contracts should be part of the architecture baseline from the beginning.

## Tasks
- [x] Create dedicated component and component-test projects for the editor.
- [x] Move editor-specific contracts and workflow code out of `AA98_AvlnCodeStudio.Base`.
- [x] Introduce scoped base libraries for UI, OS, and AI contracts.
- [x] Rewire UI composition and references to the new project structure.
- [x] Validate build and tests after the restructuring.

## Open Questions
- Should the future component host support multiple editor UI implementations side by side?
- Which later shell regions should follow the same factory-based UI composition pattern?
- Which first provider-agnostic AI contracts should be established in `AA98_AvlnCodeStudio.Base.AI` beyond the initial baseline?

## Notes
- The previous single-step extraction was accepted as a functional slice, but its project boundaries need correction so `Base` stays foundational.
- `AA98_AvlnCodeStudio.Base.AI` was created as the initial provider-agnostic AI baseline project, ready for later shared AI contracts.
- The next refinement slice introduces the first provider-agnostic AI baseline contracts in `AA98_AvlnCodeStudio.Base.AI` so later editor, workspace, and chat components can depend on stable shared abstractions.

## Base.AI Baseline Scope
- Introduce provider-agnostic AI message models.
- Introduce provider-agnostic request and response models for text generation.
- Introduce a provider-agnostic AI client contract without binding to a concrete vendor.
- Keep consent, session, and richer context contracts for later refinements.

## Validation
- Build: `run_build` successful.
- Tests: `run_tests` successful with 15 passed, 0 failed, 0 skipped in the currently discovered test assemblies.
- Solution structure: `Avalonia_Apps.sln` contains `AA98_AvlnCodeStudio.Base.UI`, `AA98_AvlnCodeStudio.Base.OS`, `AA98_AvlnCodeStudio.Base.AI`, `AA98_AvlnCodeStudio.Editor`, and `AA98_AvlnCodeStudio.Editor.Tests`.
- Follow-up note: the current Visual Studio workspace discovery did not list `AA98_AvlnCodeStudio.Editor.Tests` in `get_projects_in_solution`, so its tests were not surfaced by the IDE-backed test tool despite the solution entry being present.

## Status
- Done
