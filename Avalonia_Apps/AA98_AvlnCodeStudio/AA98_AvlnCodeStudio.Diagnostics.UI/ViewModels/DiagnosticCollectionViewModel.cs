using AppKomponentBaseLib.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;

/// <summary>
/// Provides a reusable diagnostics collection view model for Avalonia-based hosts.
/// </summary>
public sealed partial class DiagnosticCollectionViewModel : ObservableObject, IDiagnosticConsumer
{
    /// <summary>
    /// Gets the diagnostics currently exposed by the consumer.
    /// </summary>
    public ObservableCollection<Diagnostic> Items { get; } = [];

    [ObservableProperty]
    private string _summaryText = string.Empty;

    /// <inheritdoc/>
    public ValueTask ConsumeAsync(IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(diagnostics);

        Items.Clear();
        int count = 0;
        foreach (Diagnostic diagnostic in diagnostics)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Items.Add(diagnostic);
            count++;
        }

        SummaryText = $"Diagnostics: {count}";
        return ValueTask.CompletedTask;
    }
}
