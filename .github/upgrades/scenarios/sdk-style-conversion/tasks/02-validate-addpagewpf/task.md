# 02-validate-addpagewpf: Validate the converted project

Rebuild the converted project and confirm there are no build regressions relative to the baseline. Check that no `packages.config` file is left behind for the converted project and record the validation outcome, including the absence of project-specific automated tests if none exist.

**Done when**: The converted project rebuilds successfully, no project-local `packages.config` remains, and validation notes clearly state the test status for this project.

## Research Findings

### Scope assessment
- Validation scope is limited to `CSharpBible/AddPageWPF/AddPageWPF.csproj` and its project-local artifacts.
- No decomposition is required because the task is a single-project validation pass.

### Validation checkpoints
- Rebuild the converted WPF project with full Visual Studio `MSBuild.exe`, matching the build-tool decision already recorded for this scenario.
- Confirm the converted project still builds warning-free after the SDK-style follow-up fixes.
- Confirm that no `packages.config` file exists under `CSharpBible/AddPageWPF`.
- Confirm that no project-specific automated tests exist for `AddPageWPF`, and record that status explicitly.

### Files in scope
- `CSharpBible/AddPageWPF/AddPageWPF.csproj`
- `CSharpBible/AddPageWPF/Directory.Build.props`
- `.github/upgrades/scenarios/sdk-style-conversion/tasks/02-validate-addpagewpf/progress-details.md`

### Validation findings to record
- Build result for the converted project
- Presence/absence of `packages.config`
- Test status (`übersprungen` if no direct tests exist)
