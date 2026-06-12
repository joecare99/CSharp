using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AA40_Wizzard.Model;

/// <summary>
/// Stores the current wizard selections and default option sets.
/// </summary>
public partial class WizzardModel : ObservableObject, IWizzardModel, IDisposable
{
    private const string AppStartedMessage = "WizzardModel created";
    private const string AppStoppedMessage = "WizzardModel stopped";

    private readonly ISystemClock _systemClock;
    private readonly ILogSink _logSink;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="WizzardModel"/> class.
    /// </summary>
    /// <param name="systemClock">The clock used for current time access.</param>
    /// <param name="logSink">The log sink used for lifecycle logging.</param>
    public WizzardModel(ISystemClock systemClock, ILogSink logSink)
    {
        _systemClock = systemClock;
        _logSink = logSink;
        _logSink.Log(AppStartedMessage);
    }

    /// <inheritdoc />
    public DateTime Now => _systemClock.Now;

    /// <inheritdoc />
    [ObservableProperty]
    private int _mainSelection = -1;

    /// <inheritdoc />
    [ObservableProperty]
    private IList<int> _mainOptions = [0, 1, 3, 4, 6, 8, 9, 10];

    /// <inheritdoc />
    [ObservableProperty]
    private int _subSelection = -1;

    /// <inheritdoc />
    [ObservableProperty]
    private IList<int> _subOptions = [1, 2, 3, 5, 6, 7, 9, 11];

    /// <inheritdoc />
    [ObservableProperty]
    private int _additional1 = -1;

    /// <inheritdoc />
    [ObservableProperty]
    private int _additional2 = -1;

    /// <inheritdoc />
    [ObservableProperty]
    private int _additional3 = -1;

    /// <inheritdoc />
    [ObservableProperty]
    private IList<int> _additOptions = [10, 12, 13, 15, 16, 17, 19, 21];

    partial void OnMainSelectionChanged(int value)
        => ResetAdditionalSelections();

    partial void OnSubSelectionChanged(int value)
        => ResetAdditionalSelections();

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _logSink.Log(AppStoppedMessage);
        GC.SuppressFinalize(this);
    }

    private void ResetAdditionalSelections()
    {
        Additional1 = -1;
        Additional2 = -1;
        Additional3 = -1;
    }
}
