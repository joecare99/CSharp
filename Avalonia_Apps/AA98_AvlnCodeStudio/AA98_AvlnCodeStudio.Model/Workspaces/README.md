# C# quick info

The Workbench's Edit menu and Ctrl+Shift+F1 show Roslyn quick info for the
active C# document at the caret. Resting the mouse over a C# symbol also shows
the information at the pointer without moving the caret. The hover is dismissed
by pointer movement, scrolling, editing, or a workspace/tab change. The caret
display is dismissed by editing, moving the caret, switching workspace/tab, or
the close button. Requests use an immutable snapshot of open C# editor buffers,
including unsaved text in related files in the selected project, without saving
them or changing the shared Roslyn workspace. Conflicting open buffer paths are
omitted. A unique closest workspace project containing the active document must
be available. Ambiguous or unlisted documents return no result.
Only the currently loaded Roslyn project configuration is queried, not all target
frameworks. Project loading uses MSBuild and should only be used with trusted
projects. Hover is mouse-only; the caret command remains available to keyboard users.

The Edit menu and Ctrl+Space request Roslyn completion suggestions at the caret.
The list is filtered by the current prefix and shows at most 50 suggestions.
Applying one uses the provider's text change and caret position as one undoable
editor change. Moving the caret, editing, or switching the tab or workspace
invalidates outstanding suggestions and requests. Completion uses the same
open-buffer snapshot and unique-project restriction described above; it does not
provide an automatic completion popup or an interactive snippet session.

The Edit menu and Ctrl+Shift+Space request Roslyn signature help for the active
argument list. The panel marks the selected overload and reports the active
parameter. It uses the same open-buffer snapshot, project restriction and stale
result checks as quick info and completion.

## Project launch settings inspection

`ProjectLaunchSettingsReader` is a UI-independent reader for the
`Properties\launchSettings.json` belonging to a selected C# project. The
Workbench uses `ProjectLaunchProfilesProvider` to display the same metadata in
Project Settings. Inspection does not select, execute, create, or modify any
profile; the current Workbench debugger does not apply launch profiles.

```csharp
ProjectLaunchSettingsSnapshot snapshot =
    await new ProjectLaunchSettingsReader().ReadAsync(projectPath, cancellationToken);

if (snapshot.FileExists && snapshot.HasProfilesObject)
{
    foreach (ProjectLaunchProfile profile in snapshot.Profiles)
    {
        // Inspect Name, CommandName, presence flags and Diagnostic.
    }
}
```

The reader accepts UTF-8 BOM, JSON comments and trailing commas. A missing file,
a missing `profiles` object and an empty object are distinct results. Invalid
JSON or structures, files over 1 MiB, more than 256 profiles, and invalid or
overlong names are reported as errors rather than replaced with defaults.
Profile names that collide without regard to case are marked ambiguous.
I/O and cancellation errors propagate to the caller.

Argument and environment **values** are not returned. Their flags indicate
that a field is present, even when its value is `null` or invalid. Profile names
and command names are returned as metadata; callers should not put secrets in
them or assume that arbitrary project files cannot contain sensitive text
there. The snapshot is informational and does not guarantee that another tool
supports a given profile.

## Explicit project workflow

`ProjectWorkflowContext` and `DotNetProjectTargetResolver` can be used without
the Workbench. A context accepts only an existing project listed in the loaded
workspace and captures the chosen configuration and optional target framework.
Inspection invokes `dotnet msbuild` to evaluate the project's actual
properties; it does not build the project or modify its files.

```csharp
ProjectWorkflowContext context =
    ProjectWorkflowContext.Create(workspace, projectPath, "Debug", targetFramework);
ProjectTargetInspection inspection =
    await new DotNetProjectTargetResolver().InspectAsync(context, cancellationToken);
```

**Inspection is not a sandbox:** MSBuild imports and property functions may
execute during evaluation. Hosts must make inspection an explicit action for
trusted projects. If several target frameworks are available, the caller must
choose one and inspect again. `TargetPath` is an evaluated expected path, not
proof that the file exists. `CanDebug` also requires an executable .NET Core
target; a build must still succeed and its artifact and runtime metadata must
be verified before launch.

The Workbench's Project tab keeps this selection only for the current session.
Its selected-project Build, Test, and Build and Debug actions do not replace the
workspace-wide commands or manual DLL debug path. Project tests pass the chosen
configuration and framework to `dotnet test`. Before debugging, a regular `dotnet build
--no-restore` refreshes runtime files even if the builder reused stale
dependency metadata. Restore dependencies separately when necessary. No
launch-profile argument or environment values are applied. Inspection and tests are cancellable; the
current Workbench builder runs synchronously, so a cancellation or selection
change prevents subsequent launch but may have to wait for an ongoing build to
finish.

The same tab can find source-level test method candidates using Roslyn for the
selected project, configuration and framework. It recognizes common MSTest,
xUnit and NUnit attributes semantically, including derived attributes; it
does not enumerate adapter-discovered cases, inherited methods, nested/generic
fixtures, static methods, or unsaved editor buffers. A candidate is a hint, not
proof that a test adapter will discover or run it. Loading a project requires
trusted MSBuild evaluation and restored dependencies.

The separate **Build and Discover VSTest Cases** action builds the selected
project without restoring packages, finds the VSTest runner in the SDK selected
for that project and loads adapter DLLs copied next to its target assembly.
It lists adapter-reported cases, including parameterized cases and their
adapter-defined IDs, independently of source candidates and run results.
Case IDs and qualified names are adapter-specific and are not assumed to be
stable or valid filters. **Run Selected Adapter Case** rebuilds the project,
rediscovers the selected case, and executes the original adapter test object
only when its identity matches exactly one current case. It refuses missing or
ambiguous matches, reports a failure if the adapter returns a different number
of results, and retains adapter error messages. The **Debug Selected Case at
Editor Caret** action attaches NetCoreDbg to processes reported by VSTest and
sets one breakpoint at the caret in the active saved C# file before the selected
case starts. For successive attach requests, the previous session is detached
before the next process is attached; simultaneous multi-process debugging is not
supported. The file must still match its saved text after the build. Whether the
breakpoint can bind or is reached depends on debug symbols and executed code;
the test can finish without stopping. Stop, project changes and test selection
changes cancel the debug run, stop its reported hosts and their child processes,
and detach the debugger. Process termination is best-effort and can fail due to
operating-system permissions or races.
Only runners that launch the host and request debugger attach are supported.
Adapters outside the output directory and Microsoft.Testing.Platform-only projects
are not supported. Discovery can execute adapter code; build, discovery and
execution must only be used with trusted projects. Cancellation asks VSTest to
cancel, but a stalled adapter or startup can delay completion.

An explicitly entered or selected fully qualified test method can be run with
an exact VSTest `FullyQualifiedName` filter. Depending on the adapter, a
parameterized method may have multiple matching cases; a missing name yields
no tests, not a whole-project run. Debugging is available for adapter-discovered
cases, not directly from the method-name field.
The Project tab shows individual results reported by the last selected-project
test run, grouped by the test class when TRX provides it. This result tree is
not test discovery: it does not list tests omitted from the run. Test and execution IDs
are runner-reported run metadata, not guaranteed stable identities. Qualified
names may include parameter values and are not necessarily valid test filters.
The reusable `TestRunRequest` retains its
original substring behavior unless `MatchTargetsExactly` is enabled; exact mode
requires at least one nonblank target. Running tests executes project code and
must only be done for trusted projects.

## Debugger component attach

The independent `NetCoreDbgDebugSessionService` can attach to an existing .NET
process using `DebugLaunchRequest.AttachToExistingProcess` and a positive
`ProcessId`. Launch arguments and `StopAtEntry` are not valid for attach. The
caller must have permission to debug the target and the .NET runtime must be
available in it. Stopping an attached session requests a detach rather than
termination of the target; a launched session still requests termination.
Stack and stepping operations use the thread reported by the last stopped event.
The host must provide NetCoreDbg on PATH or via `AVLN_NETCOREDBG_PATH`.
This is also used by the Workbench's test-case debugging action; there is no
general-purpose Workbench attach command.

## Reviewed workspace changes

`ProjectAssemblyNameProvider` expects its review callback to **approve or
decline only**. It applies the approved proposal itself through
`WorkspaceFileChangeService`. The reusable
`WorkspaceFileChangeReviewDialog.CreateApprovalOnly` supports that contract;
its original constructor instead reviews **and applies** the proposal. Passing
an applying callback to the project provider would apply the same change twice.
Hosts must also reject a review if its workspace context is no longer active.

The change service preserves UTF-8 with or without a BOM and BOM-marked
UTF-16/UTF-32 when replacing existing files. Files without a BOM must be valid
UTF-8; invalid Unicode is reported before review. Newly created files use
UTF-8 without a BOM. Undo and redo retain the corresponding original and
proposed bytes. This contract does not change ordinary editor save behavior.

An opt-in desktop regression exercises the Workbench project-settings dialog,
both review modes, cancellation, conflicts, context changes and undo/redo:

From the repository root on Windows (use native path separators on Linux):

```powershell
dotnet run --project tools\ProjectSettingsReviewSmoke\ProjectSettingsReviewSmoke.csproj
```

It requires .NET 10 and a graphical Windows or Linux session. It creates
temporary synthetic projects and isolated user/system configuration, triggers
the UI programmatically and removes its temporary files afterward. It is not a
physical keyboard, screen-reader or accessibility acceptance test.

## AXAML structure editing

The Workbench and standalone designers also select preview elements by clicking their inert
visuals, or by moving the source-editor caret inside an element. Tree,
inspector and preview follow the same selection; selecting in the tree or
preview moves the source caret without focusing the editor. Selection does not
change markup. The source mapping is available only for valid, unambiguous
XML; recovery previews and unrepresented markup are not guessed. Invisible or
unrealized preview items can still be selected in the tree or source.

`AxamlDesignerSession` exposes the safe preview tree and document-local undo
and redo independently of a host. `RemoveSelectedControl` and
`MoveSelectedControl` with direction `-1` or `1` operate on direct children of
simple `StackPanel`, `WrapPanel`, `UniformGrid` and `DockPanel` elements. For a
DockPanel, child placement attributes must be unique, literal, valid Avalonia
placements. With `LastChildFill` enabled (including its default), the final
child is treated as the client and cannot be moved; other adjacent children
can be reordered without crossing it. With `LastChildFill` disabled, all
children may be reordered. Safe direct children can be removed in either
mode. Invalid or ambiguous placement markup blocks these operations. The
actions do not reorder Grid children. For a static Grid with supported
children and non-overlapping placements,
`MoveSelectedGridControl` moves a child by one row or column only when its
entire span fits in free cells. `RemoveSelectedControl` also accepts children
of these Grids, even when every cell is occupied. `HasSafeGridLayout` and
`CanAddGridChild` distinguish a safe full Grid from one with room to insert.
The selected safe Grid can also append an `Auto` row or column. Existing
attribute lists or definition property elements are extended in place;
implicit star tracks are made explicit so existing cells retain their layout.
The final explicit row or column can be removed only when every child's
placement and span ends before that track; implicit tracks, single-track
definitions, occupied end tracks and spans crossing the end track are retained.
The designer can change the size of any explicit row or column track through
the selected-track editor. It accepts `Auto`, star weights and finite
non-negative pixel values; dynamic values and implicit tracks are not editable.
Attribute lists retain their separators, spacing and quotes; property-element
updates preserve element prefixes and existing attribute quotes.
Each action preserves source prefixes and is one document-local undo step.
Expansion stops before the 100,000-cell safety limit. Dynamic, ambiguous,
out-of-bounds or overlapping placements, unknown direct children and
ambiguous source/preview hierarchies block edits. The preview is a safe
subset, not a complete AXAML outline.
Removal is also available for a direct supported child of `Border`,
`ContentControl`, `Expander`, `GroupBox`, `ScrollViewer` and `Viewbox`.
These single-content containers are explicitly opted into the capability
catalog; property-element content and collapsed roots are rejected. Adding
content to an empty single-content container revalidates the live source
hierarchy to prevent stale previews from creating a second child.
Direct `Window` and `UserControl` children are excluded from Grid structural
edits and insertion because the preview can collapse their content roots and
misidentify their cells. Grid insertion also revalidates the direct source
hierarchy against the selected preview.

Attached Grid placements support either unqualified attributes in the
Avalonia default namespace (or namespace-less snippets) or attributes
qualified with an Avalonia namespace prefix. Duplicate, foreign-namespace
and unresolved unqualified placements are rejected. Insertions and missing
placement attributes use an in-scope prefix when needed; no namespace
declaration is rewritten.

Operations preserve the surrounding source. Direct panel comments and other
ambiguous content block changes; moving a whole subtree preserves its nested
comments. Removing a subtree with comments, property elements or unsupported
content is rejected. The availability properties describe preview-level
eligibility: an operation may still reject source content with a diagnostic.
Removing a named control does not update references in other files.
Changes affect the session buffer only; hosts must protect their own pending
property drafts and explicitly save the document.
