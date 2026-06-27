# AA98-T042 Inspect Linux Shell Startup Path

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl038-Linux-Shell-Startup-Baseline.md`

## Goal
Inspect the current AA98 shell startup path and identify Linux blockers before applying fixes.

## Scope
- Locate the main AA98 shell executable and startup composition path.
- Identify Windows-only assumptions in startup, path handling, process usage, and configuration.
- Record blockers as platform, dependency, configuration, or missing implementation.

## Execution Notes
1. Review shell project structure and startup code.
2. Check existing tests and diagnostics around startup.
3. Record concrete findings in the backlog item or a follow-up planning note.

## Findings
- Primary AA98 shell startup path is `AA98_AvlnCodeStudio.UI/Program.cs` using `BuildAvaloniaApp().StartWithClassicDesktopLifetime(args)`.
- Runtime composition happens in `AA98_AvlnCodeStudio.UI/App.axaml.cs` via `OnFrameworkInitializationCompleted`, where desktop-only service wiring and `MainWindow` creation are coupled to `IClassicDesktopStyleApplicationLifetime`.
- Startup currently has no explicit repeatable smoke-validation seam; task `AA98-T044` should add composition or startup validation that can run without manual full UI investigation.
- File dialogs are registered through `AddAvaloniaCommonDialogs(() => desktop.MainWindow)` and depend on an available Avalonia `TopLevel`; this is platform-neutral in intent but should be covered by startup diagnostics because missing display/session integration on Linux would surface here late.
- Follow-up editor/workspace blocker for `AA98-T045`: `EditorWorkflow.GetCurrentDirectory()` falls back to `Environment.SpecialFolder.MyDocuments`, which is not Windows-only but is an environment-sensitive startup/editing assumption that should be normalized for Linux self-hosting.
- Follow-up test blocker for `AA98-T045` and `AA98-T047`: current editor tests use hard-coded Windows paths such as `C:\Temp\sample.cs`, which hides Linux path behavior.

## Blocker Classification
- Dependency: AA98 shell startup currently depends on Avalonia desktop lifetime and an interactive desktop environment.
- Configuration: there is no documented first Linux smoke target or startup-validation path yet.
- Missing implementation: no dedicated startup smoke test or composition validation exists for the AA98 shell baseline.
- Platform: editor-adjacent path defaults and tests still reflect Windows-shaped assumptions and should be made Linux-explicit in later tasks.

## Recommended Follow-Up For AA98-T043
1. Add a minimal startup/composition validation seam for the AA98 shell services and main window creation.
2. Improve startup diagnostics around desktop lifetime and service-provider creation so Linux failures are actionable.
3. Keep direct editor path normalization changes for `AA98-T046`, but reflect the dependency in shell-startup notes.

## Acceptance Criteria
- Startup path and blockers are documented.
- Follow-up implementation work for `AA98-T043` has enough detail to begin.

## Validation
- No code changes are required for this inspection task.
- If exploratory commands are run, record their exact result in the task notes.

## Status
- Completed
