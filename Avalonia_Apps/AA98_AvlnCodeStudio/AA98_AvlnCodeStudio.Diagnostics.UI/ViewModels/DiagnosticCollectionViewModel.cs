using AppKomponentBaseLib.Diagnostics;
using Code.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Diagnostics.Navigation;
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
    private readonly DiagnosticLocationMapper _locationMapper;
    private readonly ICodeNavigator? _navigator;

    public DiagnosticCollectionViewModel()
        : this(new DiagnosticLocationMapper(), null)
    {
    }

    public DiagnosticCollectionViewModel(
        DiagnosticLocationMapper locationMapper,
        ICodeNavigator? navigator)
    {
        _locationMapper = locationMapper ?? throw new ArgumentNullException(nameof(locationMapper));
        _navigator = navigator;
    }

    /// <summary>
    /// Gets the diagnostics currently exposed by the consumer.
    /// </summary>
    public ObservableCollection<Diagnostic> Items { get; } = [];

    [ObservableProperty]
    private string _summaryText = string.Empty;

    [ObservableProperty]
    private Diagnostic? _selectedDiagnostic;

    [ObservableProperty]
    private string _activationErrorText = string.Empty;

    public bool IsSelectedDiagnosticNavigable =>
        SelectedDiagnostic is not null && _locationMapper.Map(SelectedDiagnostic).IsAvailable && _navigator is not null;

    partial void OnSelectedDiagnosticChanged(Diagnostic? value)
    {
        ActivationErrorText = string.Empty;
        OnPropertyChanged(nameof(IsSelectedDiagnosticNavigable));
    }

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
        SelectedDiagnostic = null;
        return ValueTask.CompletedTask;
    }

    public Task ActivateAsync(Diagnostic diagnostic, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(diagnostic);

        DiagnosticLocationMappingResult mapping = _locationMapper.Map(diagnostic);
        if (!mapping.IsAvailable || mapping.Location is null)
        {
            ActivationErrorText = mapping.Reason ?? "The diagnostic has no navigable source location.";
            return Task.CompletedTask;
        }

        if (_navigator is null)
        {
            ActivationErrorText = "No code navigator is available.";
            return Task.CompletedTask;
        }

        return NavigateAsync(mapping.Location, cancellationToken);
    }

    [RelayCommand]
    private async Task ActivateDiagnosticAsync(Diagnostic? diagnostic, CancellationToken cancellationToken)
    {
        if (diagnostic is null)
        {
            ActivationErrorText = "No diagnostic is selected.";
            return;
        }

        await ActivateAsync(diagnostic, cancellationToken).ConfigureAwait(false);
    }

    private async Task NavigateAsync(CodeLocation location, CancellationToken cancellationToken)
    {
        try
        {
            await _navigator!.NavigateAsync(location, cancellationToken).ConfigureAwait(false);
            ActivationErrorText = string.Empty;
        }
        catch (OperationCanceledException)
        {
            ActivationErrorText = "Diagnostic activation was cancelled.";
        }
        catch (Exception exception)
        {
            ActivationErrorText = exception.Message;
        }
    }
}
