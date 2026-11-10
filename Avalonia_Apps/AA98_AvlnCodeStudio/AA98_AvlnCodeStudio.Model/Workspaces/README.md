# Project launch settings inspection

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

`AxamlDesignerSession` exposes the safe preview tree and document-local undo
and redo independently of a host. `RemoveSelectedControl` and
`MoveSelectedControl` with direction `-1` or `1` operate on direct children of
simple `StackPanel`, `WrapPanel` and `UniformGrid` elements. These actions do
not reorder Grid children or edit the root or children of `DockPanel`. For a
static Grid with supported children and non-overlapping placements,
`MoveSelectedGridControl` moves a child by one row or column only when its
entire span fits in free cells. `RemoveSelectedControl` also accepts children
of these Grids, even when every cell is occupied. `HasSafeGridLayout` and
`CanAddGridChild` distinguish a safe full Grid from one with room to insert.
Grid definition property elements are retained. Dynamic, ambiguous,
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
