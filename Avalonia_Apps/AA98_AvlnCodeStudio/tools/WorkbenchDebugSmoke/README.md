# Workbench project-debug smoke test

Run this smoke in a desktop session with the .NET 10 SDK and NetCoreDbg
installed:

```powershell
$env:AVLN_NETCOREDBG_PATH = "C:\path\to\netcoredbg.exe"
dotnet run --project tools\WorkbenchDebugSmoke\WorkbenchDebugSmoke.csproj
```

On Linux, point `AVLN_NETCOREDBG_PATH` to the installed adapter and run the
same project under a desktop session such as WSLg. The smoke creates a
temporary net10 debuggee and drives the real Workbench project selection,
inspection, build, runtime refresh and DAP services. It verifies stop-at-entry,
a source breakpoint, stack and local-variable inspection, step-over, continue
and stop. The temporary project uses no user source files or credentials.

## Entry-stop lifecycle and fake-adapter regression

For a launch with stop-at-entry, the first `stopped` event completes the
entry-stop wait. If `exited` or `terminated` arrives first, startup fails after
the `configurationDone` response rather than waiting for the ten-second
entry-stop timeout. Cancellation still cancels startup and cleans up the
adapter. This does not change attach requests or their non-terminating detach
contract; it also does not bypass the separate request timeout while waiting
for `configurationDone`.

`StartSession_StopAtEntryReportsEarlyExitAndCleansUpOnCancellation` runs a
separate fake adapter process. It covers both early-exit events, checks that
startup fails within five seconds and that the adapter exits, and cancels a
start after configuration without an entry stop. Existing fake-adapter tests
also exercise launch, inspection, attach, detach and attach cancellation. These
are protocol regressions, not evidence that a real adapter can launch or attach.

## Recorded net10 validation

The following are the actual filters used for the recorded runs. The artifact
roots are isolated and `IsolatedArtifacts.props` also isolates external project
references. Use a fresh root for another run; do not share one with concurrent
builds.

Windows, with no NetCoreDbg configured:

```powershell
$root = Join-Path (Get-Location) 'artifacts\fleet-debug-next-win-84671c27'
$env:TEMP = Join-Path $root 'temp'
$env:TMP = $env:TEMP
dotnet test AA98_AvlnCodeStudio.Tests\AA98_AvlnCodeStudio.Tests.csproj --framework net10.0 --no-restore "-p:HostArtifactRoot=$root" "-p:DirectoryBuildPropsPath=$((Get-Location).Path)\tools\build\IsolatedArtifacts.props" --filter 'FullyQualifiedName~NetCoreDbgDebugSessionServiceTests|FullyQualifiedName~ProjectWorkflowViewModelTests' --logger 'console;verbosity=normal' --nologo
```

Result: **37 passed, 2 skipped**. The two skipped tests require a real
NetCoreDbg; the fake-adapter regression passed.

Linux/WSL, with the installed NetCoreDbg 3.2.0-1:

```sh
root="$PWD/artifacts/fleet-debug-next-linux-84671c27"
mkdir -p "$root/temp"
export TMPDIR="$root/temp" DOTNET_ROOT="$HOME/.dotnet"
export PATH="$HOME/.dotnet:$HOME/.local/bin:$PATH"
export AVLN_NETCOREDBG_PATH="$HOME/.local/bin/netcoredbg"
dotnet test AA98_AvlnCodeStudio.Tests/AA98_AvlnCodeStudio.Tests.csproj --framework net10.0 "-p:HostArtifactRoot=$root" "-p:DirectoryBuildPropsPath=$PWD/tools/build/IsolatedArtifacts.props" --filter 'FullyQualifiedName~NetCoreDbgDebugSessionServiceTests|FullyQualifiedName~TestHostDebugIntegrationTests|FullyQualifiedName~ProjectWorkflowViewModelTests' --nologo --verbosity quiet
```

Result: **40 passed, 2 skipped, 2 failed**. A separate run of the
`NetCoreDbgDebugSessionServiceTests` filter produced **7 passed, 2 failed**:
the fake-adapter tests passed, while both real-adapter tests failed.

## Linux real-adapter limitation

The real launch returned `configurationDone` error `0x80004005`; real attach
timed out, with the adapter later reporting `0x80131c08`. Both failures were
also reproduced individually with an isolated assembly built from the
pre-change debugger source. The launch target printed its output without
stopping; a longer-lived owned target also failed. Adapter stderr was empty.

The investigation's `TMPDIR` was on WSL DrvFS and its 112-character absolute
path was too long for an `AF_UNIX` socket. A short relative socket bind on the
same filesystem returned `Operation not supported`; the managed target had no
diagnostic socket. Session file-write restrictions prevented a repeat using a
short native-Linux temporary directory, so the exact cause remains unconfirmed
and neither real-adapter test should be reported as passing.

`ptrace_scope=1` also blocks a sibling process from tracing an ordinary target.
Allowing tracing only for the owned test target did not resolve attach, so
ptrace is not established as the sole cause. No global ptrace policy was
changed. A real-adapter rerun requires an appropriately short native-Linux
temporary directory and an environment that permits the intended attach.
