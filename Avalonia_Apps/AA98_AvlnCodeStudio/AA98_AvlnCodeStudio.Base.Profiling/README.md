# .NET Profiling component

This UI-independent component can opt in to `dotnet-counters`, `dotnet-trace`, and
`dotnet-dump` for a caller-selected, already-running process. It never launches or
selects the target application. A host registers it explicitly with
`AddDotNetProfiling`; profiling does not start during registration.

The CLI tools must be installed and available on `PATH`, or supplied through
`DotNetProfilingToolPaths`. Counters and traces run until the returned session is
stopped or its cancellation token is canceled. Heap dumps complete after collection.
Trace and heap-dump files are assigned generated names below the selected directory's
`dotnet-profiling` child directory; symlink/reparse-point output directories are
rejected. Tool failures and missing output artifacts are reported as
`DotNetProfilingException`.

The session streams output lines through `OutputReceived`, while retaining at most
64 Ki characters per stdout/stderr stream. Truncated final text includes a marker and
the result exposes `StandardOutputTruncated` / `StandardErrorTruncated`.
`StopAsync` force-stops the tool; this is not a successful trace/dump completion.
The result has `WasStopped == true`, `Succeeded == false`, and no completed
`ArtifactPath`. A partial file may remain at the session's intended `ArtifactPath`;
hosts must not treat it as a valid trace or dump.

Profiling may expose sensitive process data. The host should make the operation
explicit to the user and handle resulting artifacts according to its data-retention
policy.
