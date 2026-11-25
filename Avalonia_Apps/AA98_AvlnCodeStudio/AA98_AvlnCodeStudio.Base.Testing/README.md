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

The tests can be rerun from the repository root with a fresh artifact root:

```powershell
$root = Join-Path (Get-Location) ("artifacts\vstest-cancel-" + [guid]::NewGuid().ToString("N"))
$props = Join-Path (Get-Location) "tools\build\IsolatedArtifacts.props"
dotnet test AA98_AvlnCodeStudio.Tests\AA98_AvlnCodeStudio.Tests.csproj -f net10.0 "-p:HostArtifactRoot=$root" "-p:CustomAfterDirectoryBuildProps=$props" --filter "FullyQualifiedName~VSTestRunResultTests|FullyQualifiedName~VSTestCancellationIntegrationTests"
```

Use the same isolated-root command with
`--filter "FullyQualifiedName~RunDiscoveredTestCase_LateResultRespectsWorkbenchContext"`
for the workbench regression.

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
