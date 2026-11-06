using Property.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;

/// <summary>Adapts the selected AXAML control's properties to the shared property-provider contract.</summary>
public sealed class AxamlDesignerPropertyProvider : IPropertyProvider
{
    private readonly AxamlDesignerViewModel _designer;

    public AxamlDesignerPropertyProvider(AxamlDesignerViewModel designer)
    {
        _designer = designer ?? throw new ArgumentNullException(nameof(designer));
    }

    public PropertyProviderContext Context
    {
        get
        {
            string target = _designer.SelectedNode?.TypeName ?? "No selected control";
            string? readOnlyReason = _designer.SelectedNode is null
                ? "Select a supported control in the AXAML preview to edit its properties."
                : null;
            return new PropertyProviderContext(
                PropertyScope.Session,
                "AXAML designer",
                target,
                readOnlyReason: readOnlyReason);
        }
    }

    public IReadOnlyList<IPropertyItem> Properties =>
        _designer.PropertyRows.Cast<IPropertyItem>().ToArray();

    public bool IsReadOnly =>
        _designer.SelectedNode is null
        || _designer.PropertyRows.All(static property => !property.IsEditable);

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task ApplyAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _designer.ApplyPropertyDrafts();
        return Task.CompletedTask;
    }

    public Task DiscardAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _designer.DiscardPropertyDrafts();
        return Task.CompletedTask;
    }

    public Task ResetDraftAsync(CancellationToken cancellationToken = default) =>
        DiscardAsync(cancellationToken);
}
