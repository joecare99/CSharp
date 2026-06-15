# AA19-T003 Fix Browser Splash Screen

## Status
Done

## Parent
- Follow-up maintenance task for `AA19_FilterLists`

## Goal
Fix the browser host splash screen in `AA19_FilterLists.Browser` so the loading overlay closes automatically after the Avalonia application content is rendered.

## Scope
- Review the current browser splash implementation in `AA19_FilterLists.Browser`.
- Reuse the existing working splash-close pattern from another browser host in this repository.
- Update the browser host static assets to close the splash after application content appears.
- Validate the browser host with a focused build.

## Assumptions
- The browser application itself starts successfully and only the splash-close behavior is missing.
- The fix should stay local to the browser host static assets and should not require changes in the shared app core.

## Open Questions
- Whether the same splash-close helper should later be centralized across all browser hosts.

## Tasks
- [x] Analyze the existing browser splash implementation.
- [x] Identify a working reference implementation in the repository.
- [x] Update the browser splash markup, script, and styles.
- [x] Validate the browser host build.
- [x] Mark this task completed after validation.

## Validation
- `dotnet build C:\Projekte\CSharp\Avalonia_Apps\AA19_FilterLists\AA19_FilterLists.Browser\AA19_FilterLists.Browser.csproj --configuration Debug -m:1` succeeded.

## Notes
- This task intentionally keeps the splash visible if application startup fails before the first visible render.
