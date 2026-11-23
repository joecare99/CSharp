# Document editor composition

`DocumentEditorRegistry<TEditor>` selects a factory by descending priority and
then ordinal identifier. Factory identifiers must be unique. Selection by
extension is not content validation: the built-in text editors validate their
input, and the Workbench presents a read-only diagnostic for missing files,
unsupported types or binary data presented to a text editor.

An independent editor supplies an `IDocumentEditorSession` for its document
state and load/save operations. Its factory composes that session, a separate
view, a Save As destination selector, and optional contributions in an
`EditorTabViewModel`. Register `IDocumentEditorFactory<EditorTabViewModel>`
with the Workbench service collection; registered factories augment the
built-in text and AXAML factories. The factory must validate its own format.

The session raises `ContentChanging` before user mutations and `StateChanged`
after path, name or dirty-state changes, on the host's UI context. Its save
operation writes the supplied path, updates that identity only after a
successful write, and keeps edits made during the write dirty. The Save As
selector returns null on cancellation and is responsible for confirming
overwrites; it does not write the file. An optional `IDocumentSavePathPolicy`
lets a session require a destination selection for ordinary Save; the session
must still validate that destination. The Workbench reserves the target while
saving, blocks conflicting opens and closes, and keeps a save-selection dialog
from losing its tab. The tab owns and disposes its session and contributions.
Text-only workspace changes cannot be applied to non-text sessions.

The existing `Editor` property on a non-text tab is only a compatibility
status model, not the document buffer. Use the tab's `SaveCommand`,
`SaveAsCommand`, `IsDirty`, `FilePath` and composed `View` for host integration.

## Workbench image editor

The Workbench composes `Avln_ImageEditor.Controls` as a document editor for
PNG, JPEG and BMP. `Avln_ImageEditor.Host` is a separate standalone application;
neither the shared control nor its host depends on the Workbench. The Workbench
build requires the sibling `Avln_ImageEditor` source tree. The shared
editor currently supports zoom, clockwise 90-degree rotation and undo/redo;
brush, eraser, fill and crop are not implemented.

The Workbench reads at most 32 MiB of encoded data. The shared component checks
image headers before decoding, limiting each dimension to 8192 and the total
to 4,194,304 pixels. Invalid images show a diagnostic, not a text editor. These
limits are not a sandbox or a complete validation of native image decoders.

The first Save, including for a PNG input, asks for a **new** PNG file: it
never overwrites the imported source. Save As also requires a new file.
Re-encoding loses metadata and may discard information such as animation or
format-specific features; JPEG and BMP are not saved in their original format.
After a successful export, ordinary Save can update that session-created PNG
only if its bytes still match the previous saved snapshot. A different existing
file is never overwritten. Saves stage a temporary file in the destination
directory, and edits during the write remain dirty. A changed or deleted output
is reported instead of overwritten.

The external-change check and replacement are separate operations: other
processes can still race them. Directory links and filesystem crash/durability
semantics are not protected by an interprocess transaction. A canceled or
failed operation does not intentionally publish a partial destination, but
an interrupted process can leave its temporary file behind.

On Windows the shared image component and standalone host tests passed for
net8, net9 and net10, and the Workbench image/document tests and programmatic
image window smoke passed for net10. The Workbench UI builds for all three
frameworks with package and compiler warnings, including NU1701. Linux, native
file pickers and physical accessibility have not been verified for this
integration.

The Workbench owns one client view with multiple tabs and at most one
replaceable, unprotected preview. `WorkspacePathIdentity` supplies the
platform-dependent normalized path comparison used to reuse documents and
coordinate saves. A tab's text editor and AXAML designer use the same document
buffer and saved snapshot.

`IDocumentEditorContributions<TView>` is UI-framework-neutral. An editor may
provide a palette, an extended property view, or an `IPropertyProvider`; the
host displays them outside the document editor and follows its selection and
draft-state events. The tab owns and disposes its contribution when replaced or
closed. Views and selections are local to the tab, and a view must not be
attached to more than one visual tree. Unapplied drafts protect the tab from
silent replacement or closing.

The AXAML contribution supplies the shared palette and property/binding views;
the document editor contains the designer surface and code editor. The
standalone designer composes the same views independently.

Multiple client views of the same file are not implemented. The session is
separate from its view, but a future multi-view host must explicitly share its
observable document/save state and coordinate ownership while creating
independent controls, carets and selections. The current single-view workspace
must not create separate document states for the same path.
