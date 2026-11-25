# Test discovery

The test-discovery service automatically uses `*TestAdapter.dll` files beside the built test assembly. To use adapters stored elsewhere inside the project directory, add a `vstest-adapters.json` file beside the project file:

```json
{
  "adapterPaths": [
    "TestAdapters/CustomTestAdapter.dll"
  ]
}
```

Paths are relative to the project directory. Absolute paths, `.`/`..` path segments, missing files, non-adapter DLL names, symbolic links, and junctions are rejected. The manifest is limited to 64 KiB and 32 adapter entries. Adapter packages are not downloaded or installed by the discovery service.

Adapter paths are deduplicated case-insensitively on Windows and case-sensitively on other platforms. On a case-insensitive filesystem mounted on Linux, different spellings of the same path may therefore both be passed to VSTest.

Discovery, single-case execution and debugging use VSTest; Microsoft.Testing.Platform (MTP)-only projects are not supported. Run those projects using their MTP runner, or configure a VSTest adapter for a VSTest-compatible project.

Before single-case execution, rediscovery must match exactly one case by adapter ID, executor URI, source and fully qualified name. The returned result must match the same identity; display names may differ. Missing, ambiguous or mismatched cases are rejected rather than attributed to the selection.

`VSTestIdentityAdapterIntegrationTests` exercises a controlled adapter through
discovery and a real selected-case run. Its normal result passes; a second run
returns a passed result with the selected ID, executor and source but a different
fully qualified name. The adapter traces both names and the ID, and the service
rejects the result as a different case. This reproduces no product defect for
that scenario; it does not cover every adapter, identity mutation or runner.
The Windows net10.0 run used SDK 11.0.100 and Microsoft.NET.Test.Sdk 18.12.0
for the fixture. It does not execute MTP or validate debugger attach.

An error-level VSTest event makes discovery or selected-case execution fail, even if cases or a result were reported; partial results are not presented as successful. An event with no usable message receives an English fallback diagnostic, consistent with the component's other diagnostics.

Aborted discovery and canceled or aborted execution also reject partial results.
When a request returns, caller cancellation is checked before publishing results;
a VSTest abort or execution error without caller cancellation is a failure.
Handler tests cover partial chunks; separate service tests cover pre-canceled
requests.

`VSTestCancellationIntegrationTests` also uses a generated, controlled adapter
and an independently started worker. The adapter records a partial passed result
for the selected case, then signals its own start and waits for cancellation.
The test checks the selected ID, cancellation result, adapter callback, and exit
of the identified worker, VSTest runner and test host. It also suppresses the
start signal and injects termination after a bounded timeout, and terminates a
worker before its launch gate is opened.

On Windows the worker is assigned to a kill-on-close job before that gate; on
Linux it runs as PID 1 in an isolated user, mount and PID namespace, with
`unshare --kill-child` and pidfds. The tests check containment and shutdown of
the identified processes; the namespace/job is owned solely by the fixture.
Linux requires `/usr/bin/unshare` from util-linux 2.41.3 or newer built with
`pidfd_open`, working user/PID namespaces and pidfds; unsupported configurations
are reported inconclusive. Fixture builds and transitive references use a
fresh artifact root and `tools/build/IsolatedArtifacts.props`.

The same fixture also has the controlled adapter record a passed result only
after its `Cancel` callback, then return normally. A marker confirms
`RecordResult` returned; the service must still throw cancellation rather than
return a successful summary. This does not establish that VSTest forwarded that
late result or delivered a successful completion callback. Separate direct
handler tests deliver a late passed result as either a stats chunk or final
chunk with a canceled completion, and verify another handler's selected-case
result remains unchanged. These tests found no product defect in those
scenarios; they do not exercise the UI or prove UI context isolation.

`ProjectWorkflowViewModelTests.RunDiscoveredTestCase_LateResultRespectsWorkbenchContext`
uses a controllable fake execution service that returns a failed selected-case
summary after cancellation, project selection change, or opening another
workspace. It verifies cancellation was signaled and that the late result,
diagnostics and status do not replace the current context; the unchanged-context
control publishes the same result and diagnostic. No product defect was
reproduced in these cases. This tests the workbench with a fake, not a real
adapter, and does not cover every callback ordering or diagnostic refresh race.

On WSL Ubuntu 26.04.1 with SDK 10.0.404 and `net10.0`, all four
data rows passed (4 executed, 0 failed, 0 skipped). The restore reported
13 warnings; the test build reported 268 warnings and 0 errors, including
NU1701 compatibility and compiler warnings. A fresh isolated artifact root
was used. The substituted execution service deliberately returns the summary
after cancellation; this checks view-model handling, not a real VSTest
runner, adapter, cancellation propagation through them, or a rendered UI.

`RunDiscoveredTestCase_LateErrorRespectsWorkbenchContext` uses a controlled
fake that throws an `InvalidOperationException` after execution starts. It
checks explicit cancellation, changing the selected case or project, opening
another workspace, and an unchanged-context control. It verifies the current
result and diagnostics remain intact and that the error is only reported in
the unchanged context. Explicit cancellation and case selection report
cancellation; project and workspace changes preserve the new status. The
regression exposed a late error being reported after cancellation; selected-case
execution now treats that error as cancellation when its token is canceled.
On Windows net10.0 with SDK 11.0.100, all ten selected-case workbench tests
passed, including the five late-error rows and the earlier late-result rows.
On WSL Ubuntu with SDK 10.0.404, all nine late-error and late-result rows passed
on net10.0 in a separate artifact root.
This is a view-model test, not a rendered UI or an adapter integration test:
it does not establish how a real adapter or VSTest host emits late errors, or
cover all exception types, callback orderings, or diagnostic refresh races.

`DebugDiscoveredTestCase_LateErrorRespectsWorkbenchContext` exercises the
separate debug workflow with the same five context scenarios. A gated fake
debug execution service invokes the workbench attach callback, holds its
session lease, then disposes it and throws an `InvalidOperationException`.
The fake debugger records the attach and breakpoint request for a synthetic
PID; no process is attached, started or terminated by these fakes. The test
checks the session is cleared and stopped once, and verifies status, error,
results and diagnostics. Before the fix, explicit cancellation and a changed
case incorrectly displayed the late error; the debug workflow now reports
cancellation instead, while unchanged context still displays the error.
On Windows net10.0 with SDK 11.0.100, all 15 selected debug/run workbench
tests passed, including the five debug rows. On WSL Ubuntu with SDK 10.0.404,
all five debug rows passed on net10.0 in a separate artifact root; restore and
build also emitted existing compatibility and compiler warnings.

This is a workbench view-model and session-lease test with a real Avalonia
editor control, not a rendered UI or real adapter/debugger integration.
Lease disposal is performed by the fake service; it does not verify VSTest
test-host startup, cancellation, process exit or real debugger detachment,
nor all error types and callback orderings. The five debug rows require a
graphical display on Linux.

The controlled cancellation fixture also runs the debug path with a real
VSTest runner, test host and adapter, but a simulated attach lease. The
adapter records a passed result before cancellation; the fixture verifies
the selected identity and host PID, cancellation instead of a summary, one
lease disposal, and exit of its owned worker, runner and host. It does not
prove that VSTest forwarded the partial result. Debug cancellation aborts the
run and terminates the host, so this is not evidence that an adapter sent a
late error after cancellation.

The controlled error adapter additionally reports an error after a passed
result on the debug path without cancellation. The service rejects it and
disposes the simulated attach lease. This is separate from the workbench's
gated late-error test above; neither test proves real debugger detachment or
an end-to-end late error reaching the workbench after abort.

On Windows with SDK 11.0.100, a fresh isolated net10.0 testing-only harness
linking the existing fixture and handler tests passed 31 and skipped 2; the
skipped breakpoint/paused-cancellation rows require `AVLN_NETCOREDBG_PATH`.
No debugger was configured. The full workbench test project could not be
restored with the current central package versions: `Avalonia.AvaloniaEdit`
12.2.0 and `Markdown.Avalonia` 12.2.0 were unavailable from the configured
sources. Therefore the workbench late-error rows were not rerun in this
validation. No product defect was reproduced, and this is not a DAP or
rendered-UI acceptance.

After AA98-local package versions were corrected, the same four test classes
ran in the complete Windows/net10 test project without source exclusions:
31 passed and the same two real-debugger rows were skipped. Five workbench
late-error rows also passed in a separate focused run. These results still use
the simulated attach lease and do not establish real debugger detachment.

On WSL Ubuntu 26.04.1 with SDK 10.0.404 and NetCoreDbg 3.2.0-1, a separate
isolated testing-only harness attempted the paused-abort row twice against the
real test host and debugger. Its temporary copy of
`TestHostDebugIntegrationTests` allowed Linux through the original Windows-only
guard, selected only the cancellation row, and logged the host PID; the
repository fixture was not changed. Both attempts failed before a session was
returned: DAP `initialize` and `attach` requests returned, but
`configurationDone` timed out. The test did not reach its breakpoint or abort,
and no debugger lease or detach was exercised. A process-tree observation
identified the owned test host, runner and NetCoreDbg in the second attempt;
all observed processes had exited afterwards.

The workspace and isolated `TMPDIR` were on WSL v9fs/DrvFS; even a short
relative Unix-domain socket bind there returned `EOPNOTSUPP` (errno 95).
`ptrace_scope` was 1 and was not changed. Neither observation establishes the
sole cause of the timeout, and the accepted DAP attach response does not prove
the debugger actually attached at the OS level. A real late error after debug
abort, debugger detachment and end-to-end workbench behavior remain unverified.

On Windows with SDK 11.0.100 and the session's NetCoreDbg 3.2.0-1092,
`DebugCase_AttachesBeforeTheSelectedTestAndStopsAtItsBreakpoint` passed both
net10.0 rows twice in the regular test project, with no package overrides or
source exclusions. The first run restored into a fresh isolated artifact root;
it emitted NU1701 and framework-support warnings. The preflight with
`--no-restore` failed with NETSDK1004 because that fresh root had no assets.
The session records the release archive's earlier SHA-256 verification; the
executable's current SHA-256 is
`23fc802866a03c5aa02484937d61ecbccf0ca731718d1840b0871f153340a823`.
The archive itself was not rechecked for this run.

Both rows attach the real debugger to the selected real test host, stop at a
breakpoint and inspect a nonempty stack. One continues and verifies the passed
selected case; the other cancels while paused and requires an
`OperationCanceledException`; it does not independently verify delivery of
the VSTest abort request. The service awaits the debugger lease release;
the test does not independently assert a DAP disconnect response or the
target's survival after detach. During the repeat, process handles were retained
for 23 observed descendants, including both NetCoreDbg processes and test
hosts; all observed processes exited. This is not a guarantee about processes
too short-lived to observe or cleanup after a crash.

In the same regular-project build, the five gated fake debug late-error rows
and four fake run late-result rows passed. The real debugger test does not
inject an adapter error after abort or exercise the workbench UI; the fake
workbench tests do not establish how a real host forwards a late error. A real
late-error-after-abort scenario and full DAP/UI acceptance remain unverified.

For a narrower cancellation-error probe on Windows, a temporary copy of the
controlled adapter/owned-worker fixture used real VSTest runner and host
processes and the real debug execution service, with a simulated debugger
lease. A file gate held each selected case until the host and runner were
identified. Without cancellation, the adapter sent an error after recording a
passed result: `SendMessage` returned and the debug service reported that error.
This control establishes delivery for that non-canceled ordering only.

In three debug-cancellation attempts, the worker requested cancellation after
the adapter recorded the partial result. An error was to be sent only after
the adapter's `Cancel` callback, with separate markers before and after
`SendMessage`. None of the callback or send markers was observed; each worker
reported cancellation, the simulated lease was disposed, and the identified
worker, runner and host exited. The product requests termination of its known
hosts before scheduling VSTest `AbortTestRun`; neither host termination nor
abort guarantees that the adapter's `Cancel` callback can send an error. The
missing markers do not prove that no callback began, nor that every adapter or
ordering behaves identically. No post-abort error send or forwarding was
observed in these attempts, so they cannot establish handling by VSTest,
the debugger, or the workbench. The temporary experiment was not added as a
permanent test.

The tests can be rerun from the repository root with a fresh artifact root:

```powershell
$root = Join-Path (Get-Location) ("artifacts\vstest-cancel-" + [guid]::NewGuid().ToString("N"))
$props = Join-Path (Get-Location) "tools\build\IsolatedArtifacts.props"
dotnet test AA98_AvlnCodeStudio.Tests\AA98_AvlnCodeStudio.Tests.csproj -f net10.0 "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$props" --filter "FullyQualifiedName~VSTestRunResultTests|FullyQualifiedName~VSTestCancellationIntegrationTests"
```

Use the same isolated-root command with
`--filter "FullyQualifiedName~RunDiscoveredTestCase_LateResultRespectsWorkbenchContext"`
for the workbench result regression, or
`--filter "FullyQualifiedName~RunDiscoveredTestCase_LateErrorRespectsWorkbenchContext"`
for the run late-error regression, or
`--filter "FullyQualifiedName~DebugDiscoveredTestCase_LateErrorRespectsWorkbenchContext"`
for the debug late-error regression.

These tests do not guarantee timely cancellation of uncooperative adapters or
cover every runner version, callback ordering or startup race. The late-result
fixture is cooperative: an adapter that never returns or does not respond to
cancellation is not covered. They do not promise cleanup if the outer test
process or operating system itself fails.

`VSTestErrorAdapterIntegrationTests` builds a controlled adapter and test source using
`Microsoft.NET.Test.Sdk` 18.12.0. It verifies successful discovery and execution,
then sends nonempty error events after reporting a case or passed result and
checks that the service rejects both operations. With the tested VSTest hosts,
the adapter's `IMessageLogger` and `IFrameworkHandle` reject null, empty and
whitespace messages with `ArgumentException` before they reach the translation
layer. The fallback for those messages is therefore covered by direct handler
tests, not claimed as an adapter-to-translation-layer end-to-end case. This does
not establish behavior for every adapter or VSTest version. The same controlled
adapter also checks empty and nonempty discovery with a synthetic RID-specific
MTP entry in the adjacent dependency metadata; it does not execute MTP.

The fixture builds in a unique directory beside the test output's `bin` tree,
with private output and intermediate paths and without inherited directory build
props. This avoids redirecting its outputs into the parent host artifact root
and keeps its Windows paths shorter: a nested fixture with a 268-character
intermediate DLL path reproduced MSB3030 with the SDK 11 compiler.

The diagnostic reads the adjacent `.deps.json` (up to 16 MiB), selecting only the target named by `runtimeTarget.name`. It recognizes a `Microsoft.Testing.Platform/<version>` library whose `runtime` contains `Microsoft.Testing.Platform.dll` either as a root-level name or as the final path component. It also reports a possible RID-specific asset when `runtimeTargets` contains that DLL with `assetType` set to `runtime` and a nonempty `rid`. It does not resolve RID compatibility or establish whether that asset is selected. A missing adapter then produces an explicit diagnostic; if adapters exist but VSTest discovers no cases, discovery adds a warning with the same guidance. An asset in dependency metadata does not establish a direct assembly reference or prove that a project is MTP-only: VSTest discovery is still attempted when adapters exist, and nonempty results are unchanged.

Missing, unreadable, malformed, oversized or inconclusive metadata leaves existing VSTest behavior unchanged. Dependency-only references are not recognized; the diagnostic cannot detect every MTP-only project. No MTP execution is provided.
