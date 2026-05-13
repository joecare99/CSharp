using System;
using SharpHack.Engine;
using SharpHack.Persist;
using SharpHack.ViewModel;

namespace SharpHack.Console;

/// <summary>
/// Provides console-oriented save and load actions for the active game session.
/// </summary>
internal sealed class ConsoleGameSaveLoadService : IGameSaveLoadService
{
    private readonly GamePersist _gamePersist;
    private readonly GameSession _session;
    private readonly Action<RestoreGameState> _applyRestoredState;
    private readonly Action<string> _reportStatus;

    public ConsoleGameSaveLoadService(
        GamePersist gamePersist,
        GameSession session,
        Action<RestoreGameState> applyRestoredState,
        Action<string> reportStatus)
    {
        _gamePersist = gamePersist ?? throw new ArgumentNullException(nameof(gamePersist));
        _session = session ?? throw new ArgumentNullException(nameof(session));
        _applyRestoredState = applyRestoredState ?? throw new ArgumentNullException(nameof(applyRestoredState));
        _reportStatus = reportStatus ?? throw new ArgumentNullException(nameof(reportStatus));
    }

    public bool CanSave => true;

    public bool CanLoad => _gamePersist.HasSavedRun();

    public event EventHandler? AvailabilityChanged;

    public void Save()
    {
        _gamePersist.SaveRun(
            _session.Map,
            _session.Player,
            _session.Enemies,
            _session.Level,
            _session.RunState,
            _session.TurnsTaken,
            _session.VictoryObjective,
            _session.CompletionSummary);

        _reportStatus("Run saved.");
        OnAvailabilityChanged();
    }

    public void Load()
    {
        if (!_gamePersist.TryLoadRun(out var restoreGameState) || restoreGameState == null)
        {
            _reportStatus("No saved run found.");
            OnAvailabilityChanged();
            return;
        }

        _applyRestoredState(restoreGameState);
        _reportStatus("Run loaded.");
        OnAvailabilityChanged();
    }

    private void OnAvailabilityChanged()
    {
        AvailabilityChanged?.Invoke(this, EventArgs.Empty);
    }
}
