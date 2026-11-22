# Project settings review smoke

Run from the physical workspace path (not a junction or symlink), with Git and
an interactive desktop available. Use a fresh artifact root for each platform:

```powershell
$root = Join-Path (Get-Location).Path ("artifacts\project-settings-win-" + [guid]::NewGuid().ToString("N"))
.\tools\build\dotnet-isolated.ps1 run --project tools\ProjectSettingsReviewSmoke\ProjectSettingsReviewSmoke.csproj "-p:HostArtifactRoot=$root"
```

In WSL/Linux, run from the physical workspace with the Linux SDK on `PATH`:

```bash
root="$PWD/artifacts/project-settings-linux-$(date +%s%N)"
bash tools/build/dotnet-isolated.sh run --project tools/ProjectSettingsReviewSmoke/ProjectSettingsReviewSmoke.csproj "-p:HostArtifactRoot=$root"
```

The smoke creates and deletes a synthetic project and Git repository under
`artifacts`, with isolated configuration and no real credentials. It opens the
real Workbench project settings dialog and review host. For reload recovery it
adds an independent synthetic section, removes the project file to provoke a
real provider reload failure, then checks the displayed error, blocked Apply,
retry, preservation and application of the unrelated draft, and a failed
window-close discard followed by recovery.

It also opens the actual shared Config dialog with in-memory fake layer stores:
Apply and OK cancel after a confirmed write, Cancel and window-close cancel
during discard, and a draft-reset cancellation is awaited directly (the shared
dialog hides section actions). It then enables the actual `ConfigUiView`
section actions and invokes Load, Reset and Save through their buttons while
fake stores are suspended; it cancels the in-flight operation and checks the
visible status, recovery and retry. It also checks direct `ExecuteAsync`
propagation, shared and independent drafts, and cancellation of retry. These
scenarios use no real credentials or configuration stores.

Interaction is programmatic, including an Avalonia automation invocation for
the recovery and dialog buttons. This is not a physical keyboard, screen-reader
or accessibility acceptance test.

## Linux/net10 evidence (2026-11-22)

On WSL Ubuntu 26.04, SDK 10.0.404 and WSLg, the existing
`SharedConfigDraftIntegrationTests` passed 20/20; the shared
`Config.UI.ConfigService.Tests` and `Config.UI.Avalonia.Tests` passed 28/28
and 46/46 respectively. The window smoke completed successfully, including
button and direct-command cancellation, close/discard, post-save refresh,
reset, recovery and independent-draft checks. Stores and projects were
synthetic; no real credentials were used.

Runs used `tools/build/dotnet-isolated.sh`, `-p:RestoreFallbackFolders=`,
the existing Linux NuGet cache as the restore source, and fresh roots under
`artifacts/fleet-config-linux-1122-84671c27`. Graph checks propagated
`VerifyIsolation` to every node: 185 for the integration test project, 20
each for the shared test projects, and 152 for the smoke. The full smoke build
log is in that root's `smoke/smoke.log`. Warnings were not suppressed:
NU1701, unsupported-TFM package warnings and compiler warnings remain.
These results do not validate physical input, accessibility or other runtimes.
