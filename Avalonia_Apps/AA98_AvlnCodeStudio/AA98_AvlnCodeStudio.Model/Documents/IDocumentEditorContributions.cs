using Property.Editor;
using System;

namespace AA98_AvlnCodeStudio.Model.Documents;

/// <summary>
/// Supplies optional, view-local tools and selected-object properties for a document editor.
/// The host owns the contribution until its editor view is closed.
/// </summary>
/// <typeparam name="TView">The host's view type; the contract does not depend on a UI framework.</typeparam>
public interface IDocumentEditorContributions<out TView> : IDisposable where TView : class
{
    TView? PaletteView { get; }

    TView? PropertiesView { get; }

    IPropertyProvider? SelectedProperties { get; }

    bool HasUnappliedDrafts { get; }

    event EventHandler? SelectionChanged;

    event EventHandler? DraftStateChanged;
}
