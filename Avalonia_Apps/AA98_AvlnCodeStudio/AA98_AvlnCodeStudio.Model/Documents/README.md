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
overwrites; it does not write the file. The Workbench reserves the target while
saving, blocks conflicting opens and closes, and keeps a save-selection dialog
from losing its tab. The tab owns and disposes its session and contributions.
Text-only workspace changes cannot be applied to non-text sessions.

The existing `Editor` property on a non-text tab is only a compatibility
status model, not the document buffer. Use the tab's `SaveCommand`,
`SaveAsCommand`, `IsDirty`, `FilePath` and composed `View` for host integration.

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
