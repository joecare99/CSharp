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

Interaction is programmatic, including an Avalonia automation invocation for
the recovery and dialog buttons. This is not a physical keyboard, screen-reader
or accessibility acceptance test.
