# BUB-T001 Migrate Bubbles To Avalonia With Desktop And Browser Hosts

## Parent
- Backlog Item: `Bubbles Avalonia modernization and multi-host preparation`

## Goal
Convert `..\bubbles\Bubbels` from a WPF desktop application into a shared Avalonia application project with MVVM and Dependency Injection, plus dedicated desktop and browser hosts, while preparing the architecture for future remote hosting and correcting the new project family to proper English bubble naming.

## Scope
- Convert the existing `Bubbels` project into a shared Avalonia application project.
- Add a `Bubbles.Desktop` host for desktop startup semantics in the new project family.
- Add a `Bubbles.Browser` host for browser startup semantics in the new project family.
- Reduce WPF-specific coupling in ViewModels and views.
- Prepare host-specific composition so future remote or hosted entry points can reuse the same application logic.
- Keep the game model in `..\bubbles\BubbelsModel` as the reusable domain layer.

## Assumptions
- The current game logic in `BubbelsModel` remains the primary business layer.
- Shared UI composition should live in the common Avalonia app project, not in the host projects.
- Browser support is introduced as a host baseline, even if some desktop-only behaviors are simplified during the first migration slice.
- Remote-readiness in this task means host-agnostic composition and UI/application separation, not a completed network backend.

## Open Questions
- Whether current animation behavior should be fully ported now or temporarily simplified for the Avalonia baseline.
- Whether a dedicated application service layer should be extracted from the current ViewModels in a follow-up task.
- Whether remote hosting should later use the browser host, a dedicated web API, or streamed desktop/session infrastructure.

## Tasks
- [x] Analyze the current Bubbels WPF structure against existing Avalonia host patterns.
- [x] Document the migration task and architecture intent in DevOps.
- [x] Create a new parallel Avalonia project family under `Avln_Bubbles` while keeping the original `..\bubbles` sources unchanged.
- [x] Add dedicated desktop and browser host projects for the new Avalonia project family.
- [x] Refactor the new ViewModels and composition to use MVVM and DI with reduced UI-framework coupling.
- [x] Migrate the main views and controls into Avalonia within the new `Avln_Bubbles.View` project.
- [x] Validate build and relevant tests.
- [x] Mark this task completed after validation.
- [x] Correct the new project family naming from `Bubbel/Bubbels` to `Bubble/Bubbles` in code, namespaces, assemblies, and UI text.

## Notes
- This task tracks both the UI migration and the host split required for browser and future remote readiness.
- Host-specific startup should stay thin so new hosts can be added later without moving game logic again.
- Updated direction after user clarification: the original `..\bubbles` projects remain unchanged; the migration result is implemented as a new project family in `Avln_Bubbles`, while visible code identifiers were later corrected to proper `Bubble/Bubbles` spelling.
- Validation result will be refreshed after the naming correction build and test run.
