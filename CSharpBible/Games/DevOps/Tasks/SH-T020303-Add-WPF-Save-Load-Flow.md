# SH-T020303 - Add WPF Save Load Flow

## Work Item Type
Task

## Parent
`SH-BI0203 - Add Save Load UI Flows`

## Goal
Allow WPF players to save and load runs.

## Scope
- Add WPF command bindings or menu items.
- Use UI-layer services for file selection.
- Keep save/load logic in shared services or ViewModels.

## Done
- Selected WPF UI can save and load a run.
- File dialog dependencies remain UI-local.

## Implementation Notes
- Added WPF save and load commands to the main window toolbar and exposed status feedback in the main WPF view model.
- Wired WPF2D startup and game factories to use durable run persistence plus restored-session creation.
- Kept file selection in the WPF layer via `CommonDialogs_net` dialog proxies so the save/load service stays UI-local and injectable on modern .NET targets.

## Validation
- Build: successful.
- Tests: `SharpHack.ViewModel.Tests` successful, 18 passed, 0 failed, 0 skipped.
