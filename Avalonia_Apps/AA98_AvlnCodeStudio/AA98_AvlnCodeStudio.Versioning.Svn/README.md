# Workspace version-control availability

Opening a workspace selects Git or SVN from the nearest repository marker. The
status adapters start the system `git` or `svn` client; a marker does not imply
that the corresponding client is installed.

If process startup reports a missing client while the working directory still
exists, workspace opening retains the workspace and explorer. The workbench
version-control summary and editor notification identify the client and include
the system diagnostic. Repository status is unavailable and SVN update remains
disabled rather than presenting a successful status. Make the client available
on PATH and reopen the workspace to refresh status. No client is installed and
no repository files are changed by this handling.

This handling is limited to startup error 2. Other process errors, cancellation,
and status parsing errors retain their existing error paths. A missing executable
dependency or interpreter can also produce error 2; the diagnostic includes
the underlying system message. Status calls outside workspace opening still
report the exception to their caller rather than returning a fabricated status.

## Verification

On Windows (SDK 11.0.100) and WSL Ubuntu 26.04 (SDK 10.0.404), the net10
`MissingVersionControlClientTests` and `WorkspaceVersionControlServiceTests`
selection passed 9/9 on each, using separate fresh artifact roots and the
early-import isolation props. The Linux restore used empty
`RestoreFallbackFolders`. The test injects a real SVN runner with a nonexistent
executable under an ancestor `.svn` marker, then opens a file through the
explorer. It also checks a real missing Git executable and missing working
directory, a normal simulated SVN status, cancellation, another startup error,
and an older failed request completing after a newer open.

Whitespace verification passed for the changed C# files. No compiler warnings
were reported in those files; existing NU1701, target-framework support,
nullable, CA1416 and MSTEST0044 warnings remain elsewhere in the graph.
These are focused view-model/process-start tests, not a full-suite or visual
desktop test. The status-bar text can be truncated by the existing layout;
the error is also exposed through the editor notification binding. Error 2
cannot distinguish every missing interpreter/dependency or a concurrent
working-directory race from a missing executable.

The workspace-opening version is checked again after the diagnostics refresh
and before VCS state is reset. A synchronous diagnostics collection-change
callback can reenter workspace opening even though the current diagnostics
consumer completes synchronously. A controlled regression covers both a newer
missing-client warning and a newer successful status; this is not a guarantee
of general thread safety for concurrent view-model access. Before the guard,
both regression cases failed on Windows. Afterward, the expanded focused
selection passed 11/11 on Windows and WSL/net10 with separate fresh artifact
roots, and whitespace verification passed for the two changed C# files.

Property-change callbacks may also reenter during the VCS reset or warning
publication itself. The workspace-opening path rechecks its ownership after
the notifying reset steps and between warning and editor-notification
publication. Controlled tests cover those boundaries and cancellation during
reset. Four of six boundary cases failed before the change; the expanded
selection passed 18/18 on Windows and WSL/net10 with separate fresh artifact
roots, and whitespace verification passed. These tests do not establish general
thread safety.
