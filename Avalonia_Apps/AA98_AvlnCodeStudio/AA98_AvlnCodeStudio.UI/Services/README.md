# C# language navigation

Go to Definition uses the current editor text and snapshots of open C# documents
in the active project's reachable project graph. It does not save those buffers.
The active file must belong to the uniquely nearest workspace project and be
included in its Roslyn project; equally near project owners are rejected.
Multi-target projects require an explicit target framework. Metadata-only
symbols and unresolved source definitions return no location.

The workbench discards a result if the workspace, selected framework, editor,
caret, source text, or open-document snapshots change while it is computed.
Requests support cancellation, including while waiting for the workspace gate.

The focused net10 tests cover ambiguous ownership, cancellation, unsaved
cross-file snapshots, framework selection and stale workbench results. They
passed on Windows and Linux; the isolated restore still reports existing
`NU1701` compatibility warnings for Microsoft.Build and Microsoft.IO.Redist.
