# C# language navigation

Go to Definition uses the current editor text and snapshots of open C# documents
in the active project's reachable project graph. It does not save those buffers.
The active file must belong to the uniquely nearest workspace project and be
included in its Roslyn project; equally near project owners are rejected.
Multi-target projects require an explicit target framework. Metadata-only
symbols and unresolved source definitions return no location.

When a physical source file is linked into multiple reachable projects, its
open-buffer text is applied to every linked Roslyn document in the snapshot.
Each project still parses that text with its own framework and compilation
settings; no project or source files are saved.

The workbench discards a result if the workspace, selected framework, editor,
caret, source text, or open-document snapshots change while it is computed.
Requests support cancellation, including while waiting for the workspace gate.

The focused net10 tests cover ambiguous ownership, cancellation, unsaved
cross-file snapshots, framework selection and stale workbench results. They
passed on Windows and Linux; the isolated restore still reports existing
`NU1701` compatibility warnings for Microsoft.Build and Microsoft.IO.Redist.

## Saved project references and visual bindings

After an external edit to a saved `.csproj` ProjectReference, choose
**Edit → Refresh binding context from saved projects**. This invalidates
semantic snapshots for projects in the current workspace and queues the active
designer and language analysis again. It does not reload the workspace's
project list, reload editor tabs, save files, or resolve disk/editor conflicts.
An unsaved `.csproj` buffer is not evaluated: the refresh reads the saved
project. Open C# buffers are still overlaid without saving. Successful
Workbench `.csproj` saves also invalidate affected snapshots and refresh the
active designer.

There is no automatic external-project watcher guarantee. In particular, this
command is verified for saved `ProjectReference` removal, addition and
conditions in an existing workspace project; it is not a general external
change monitor or a guarantee for imports, solution membership, arbitrary
MSBuild changes, other designer hosts, or unsaved project XML. The operation
does not introduce a save or conflict policy. Switch or reopen the workspace
as needed for changes to its project list.
