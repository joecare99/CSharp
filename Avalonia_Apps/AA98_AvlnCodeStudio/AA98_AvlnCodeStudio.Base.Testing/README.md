# Test discovery

## Workbench discovery context

The current Workbench exposes discovery through `DiscoverAdapterTestsCommand`
and `DiscoveredTestCases`; there is no separate Test Explorer state component.
`ProjectWorkflowViewModelTests` uses controlled discovery tasks to exercise late
success and failure after cancel, project or workspace changes, including
returning to the original project. It checks the current result selection,
status, editor messages, diagnostics and busy state. Windows and Linux net10
runs of the context, forced-overlap and interrupted-build cases passed 15/15
on each platform.

The Workbench currently keeps its shared `ProjectWorkflowBusy` flag set until
the old discovery actually returns. Consequently, if an adapter ignores
cancellation, inspection and discovery for the newly selected project cannot
start; the requested scenario in which B publishes before A returns is not
reachable through these commands. This is a responsiveness limitation, not
evidence that a B result survives a late A completion. Addressing it requires
coordinating the shared project-action lifecycle; initially no product behavior
was changed in this investigation. Empty discovery and the blocked new-project
action have separate controlled tests. These tests are not a physical UI
smoke, real adapter test, or a full no-test-project acceptance. The blocked
action and empty-result tests also passed 2/2 each on Windows and Linux net10. The isolated
builds reported existing NU1701, target-framework support, CS8600/CS8603 and
CA1416 warnings in dependencies; no warnings were emitted in the changed test.
The targeted Windows whitespace format check passed. The existing
`--test-method-only` UI smoke was not run in that initial investigation; see
the later isolated smoke results below.

The Workbench now shows a separate localized wait notice beside adapter
discovery when cancellation is requested during that discovery action,
including its preparatory build. It remains visible across project or workspace
changes while the action is still outstanding, and disappears when that action
returns. The existing context-specific status is not replaced on a stale
completion, and ordinary build, test and debug actions do not display this
notice. Busy guards are unchanged: no new discovery is queued or started.
Return from the action does not establish runner or testhost exit; the notice
explicitly says so. A synchronous cancellation callback can delay UI repaint.

The focused net10 ViewModel tests passed 25/25 on both Windows and Linux with
fresh separate graph-wide artifact roots. Controlled gates cover explicit
cancel, project/workspace changes, returning to the first project, empty
selection, old successful or exceptional completion, an uninterrupted
discovery, cancellation during its preparatory build, and the existing
build/test/debug guards. Windows targeted whitespace checks passed for the
changed ViewModel, resource accessor and test file. Existing NU1701, target
framework support, CS8600/CS8603 and CA1416 dependency warnings remained;
no new compiler warnings were reported in the changed files. These are
fake-service lifecycle checks, not a physical UI interaction or proof of
process exit.

The lifecycle was revisited with permission to change the Workbench. Releasing
its busy guard when cancellation is requested would permit a new discovery to
build while the old discovery is still running, including a refresh of the
same project. `PrepareTestAssemblyAsync` builds into the project's normal
output; `ITestDiscoveryService` receives that live assembly and project
directory and has no contract indicating that its adapter or runner has
stopped accessing them. The real VSTest implementation requests
`CancelDiscovery`, but cancellation itself does not provide a separate,
guaranteed process termination acknowledgement; an injected service can
ignore cancellation entirely. Adapter code may also have
side effects outside the output directory. A generation check would suppress
stale UI updates, not prevent those concurrent side effects. Releasing the
guard would also expose workspace build and test commands while that old
operation is alive. The controlled blocked-action test explicitly exercises
these command guards and verifies that no second project or workspace build
starts. The requested B-before-A and same-project refresh behavior therefore
remains blocked rather than weakening the existing safety guard. A safe
solution needs an explicit isolation/termination contract for discovery and
its adapter processes (including shared output and external effects), not
just a ViewModel change. No process-wide or name-based termination is used.
The revisited focused run passed 20/20 on each of Windows and Linux net10 with
fresh, separate artifact roots. It includes empty selection and an interrupted
build as well as late discovery completion. Windows whitespace verification
passed. The initial Windows test invocation failed because PowerShell split an
unquoted `-m:2`; the quoted retry passed. The builds retained the dependency
warnings listed above, with no warnings in the changed test. The tests do not
demonstrate a B publication while A remains active.

The 18.12.0 TranslationLayer
[source](https://github.com/microsoft/vstest/blob/v18.12.0/src/Microsoft.TestPlatform.VsTestConsole.TranslationLayer/VsTestConsoleProcessManager.cs)
shows that `ShutdownProcess` waits up to one second for the runner, but its
fallback calls `Process.Kill()` and disposes the handle without awaiting exit
or confirming descendant testhost exit. The ordinary discovery path has no
owned testhost handles. Thus return from `EndSession` or the service task is
not proof that all discovery processes and side effects have stopped. The
existing Workbench releases its busy guard at task completion; that fact is not
a process-exit guarantee. An experimental queue waiting only for that task was
discarded rather than automatically building against potentially active
outputs. A safe automatic queue needs an owned process-tree or equivalent
quiescence contract, plus output and external-side-effect isolation; no such
guarantee was established here.

The retained narrow change suppresses unexpected late discovery exceptions
only when canceled or stale, preserving cancellation/current-selection status;
unexpected errors from the current uncanceled operation still propagate.
Controlled tests cover these exceptions and the existing busy/build guard.
The final focused `DiscoverAdapterTests` net10 run passed 21/21 on Windows
and 21/21 on Linux with separate fresh graph-wide artifact roots; targeted
Windows whitespace verification passed. Existing NU1701, target-framework
support, CS8600/CS8603 and CA1416 dependency warnings remained, with no new
compiler warnings in the changed files. These are fake-service tests, not
proof of real process exit or acceptance of a real no-test project.

The `--test-method-only` fixture and its child processes can use a single
exclusive, platform-specific artifact root: restore and the runtime builder's
`--no-restore` build must share it. The launcher must export both
`HostArtifactRoot` and `CustomAfterDirectoryBuildProps`; passing them only as
properties to the initial `dotnet` command does not export them to children.
The fixture now uses its own package-management props, and checks its restored
assets, inspected assembly, and absence of default fixture `bin`/`obj` during
discovery and the subsequent exact test run. The real interactive Windows and
WSLg net10 smokes passed source and adapter discovery and single-case
execution; see `tools/WorkbenchArchitectureSmoke/README.md` for reproduction,
graph checks and limitations. They do not test B-before-A or prove runner or
testhost exit after cancellation.

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

`ProjectWorkflowViewModelTests.DiscoverAdapterTests_LateCompletionRespectsWorkbenchContext`
holds a fake discovery service after the build, then returns cases and a warning
or throws an `InvalidOperationException`. The six rows cover an unchanged
context, a project change and a different workspace entry in the same root.
After a change the fake ignores cancellation; the test checks that the current
case, selection, status, notification, results and diagnostics survive. The
unchanged controls publish the cases and warning or report the error. No product
defect was reproduced for these scenarios. On Windows net10.0, SDK 11.0.100,
all six rows passed with isolated build artifacts. On WSL Ubuntu with SDK
10.0.404, all six rows also passed in a separate artifact root; restore
reported NU1701 compatibility warnings.

This tests the workbench view model with a substituted discovery service, not
a rendered UI or an actual VSTest host or adapter. It does not establish real
adapter cancellation behavior, cover every exception or callback ordering, or
test MTP.

The same test now includes explicit cancellation through
`CancelProjectWorkflowCommand`, which is bound to the project workflow cancel
button and cancels the discovery command's token. The gated fake ignores that
token and later returns a case and warning or throws an `InvalidOperationException`.
The result was already discarded; the late exception previously replaced the
cancellation status with a failure. Discovery now reports cancellation for
that exception when its token is canceled. The unchanged-context controls still
publish the result or report the error; current cases, selection, diagnostics,
results and notification are preserved after cancellation. This exercises the
command and view model, not a physical button or real VSTest callbacks.
Before the fix, the cancellation/error row failed while the cancellation/result
row passed. Afterward, the ten `DiscoverAdapterTests` rows passed on Windows
net10.0 with SDK 11.0.100 and on WSL Ubuntu net10.0 with SDK 10.0.404, using
separate artifact roots. Both builds emitted compatibility and compiler warnings.

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
for the debug late-error regression, or
`--filter "FullyQualifiedName~DiscoverAdapterTests_LateCompletionRespectsWorkbenchContext"`
for the discovery context and explicit cancellation checks.

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

### Selected-case workbench lifecycle

The existing selected-case commands require a current inspection and selection,
build the inspected project and then call the case execution service. The debug
command also requires a saved C# editor and an available debugger; its attach
callback adopts a session and the service owns the returned lease. Stopping a
test debug session requests cancellation; disposal stops that session and clears
it only if its ID is still active. The view model displays summary cases grouped
by class/name, not by ID. The VSTest service separately rediscovers the selected
ID, executor URI, source and fully qualified name, requires exactly one match and
checks the returned case against that identity.

The gated workbench tests exercise cancellation or selection/project changes
during build, including returning to the original project, and forced command
overlap during execution. The latter is a programmatic call despite a disabled
command, not a second user-initiated run: it cancels the first token but does
not start a second service call. Late result and exception tests cover unchanged,
canceled, changed case, project and workspace contexts. Debug result tests also
exercise the actual stop command with a fake attach and session lease. Current
results and diagnostics survive stale completions; an unchanged run publishes
its result and an unchanged error is reported. Cancellation is reported in
the current context, whereas a changed project/workspace keeps its own status.
No additional product defect was reproduced in these scenarios, so no product
code was changed. The 26 selected-case run/debug rows passed on Windows
net10.0 (SDK 11.0.100) and WSL net10.0 (SDK 10.0.404), each with a separate
artifact root. The builds emitted existing NU1701, target-framework support,
CS8600, CS8603, CA1416 and MSTEST0044 warnings outside the changed test code.

These are deterministic view-model tests using substituted services, not an
actual adapter, test host, debugger or rendered UI. They do not prove real
adapter cancellation, debugger detachment, new-session interleavings, or
arbitrary cross-thread/diagnostic-refresh races. The debug fixture uses a real
Avalonia editor and needs a graphical display on Linux.

### Selected-case xUnit error banners

The run handler treats one exact xUnit v3 VSTest failure banner as a test
failure only for a successfully completed run with exactly one failed result
matching the selected case. Previously, any number of matching banners were
discarded, including a possible second infrastructure error; a mismatched
result also discarded the accompanying error text. Both issues were reproduced
by focused handler tests. The handler now rejects multiple error events and
preserves error messages on identity mismatch. Even identical duplicate banners
are rejected, so a benign duplicate also becomes an infrastructure failure.
A single infrastructure message identical to the expected banner is still
indistinguishable from the actual banner using these events.

The regression tests cover duplicate and differently timestamped banners,
additional errors, similar but nonmatching messages, missing or unsuccessful
completion, zero or multiple results, and a different result identity. Before
the fix, three new rows failed. Afterward, `VSTestRunResultTests` and
`FindTestMethodCandidatesAsync_XunitCandidateRunsParameterizedCases` passed
47/47 on Windows net10.0 (SDK 11.0.100) and WSL Ubuntu net10.0 (SDK 10.0.404),
with separate private artifact roots. The latter builds and runs a real xUnit
v3 adapter, including a failing selected case. This does not cover every adapter
version or event ordering. Whitespace format verification passed for both
changed C# files. Existing build warnings included NU1701, unsupported net8/9
dependency injection, CS8600, CS8603, CA1416 and MSTEST0044.
