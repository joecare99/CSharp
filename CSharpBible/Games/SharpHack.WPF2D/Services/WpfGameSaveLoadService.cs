using System;
using System.IO;
using CommonDialogs;
using CommonDialogs.Interfaces;
using SharpHack.Engine;
using SharpHack.Persist;
using SharpHack.ViewModel;

namespace SharpHack.WPF2D.Services;

/// <summary>
/// Provides WPF-specific save and load actions while keeping dialog interactions in the UI layer.
/// </summary>
public sealed class WpfGameSaveLoadService : IGameSaveLoadService
{
    private readonly GamePersist _gamePersist;
    private readonly Func<LayeredGameViewModel> _getCurrentGame;
    private readonly Action<RestoreGameState> _applyRestoredState;
    private readonly Action<string> _reportStatus;
    private readonly IOpenFileDialog _openFileDialog;
    private readonly IFileDialog _saveFileDialog;

    public WpfGameSaveLoadService(
        GamePersist gamePersist,
        Func<LayeredGameViewModel> getCurrentGame,
        Action<RestoreGameState> applyRestoredState,
        Action<string> reportStatus,
        IOpenFileDialog? openFileDialog = null,
        IFileDialog? saveFileDialog = null)
    {
        _gamePersist = gamePersist ?? throw new ArgumentNullException(nameof(gamePersist));
        _getCurrentGame = getCurrentGame ?? throw new ArgumentNullException(nameof(getCurrentGame));
        _applyRestoredState = applyRestoredState ?? throw new ArgumentNullException(nameof(applyRestoredState));
        _reportStatus = reportStatus ?? throw new ArgumentNullException(nameof(reportStatus));
        _openFileDialog = openFileDialog ?? new OpenFileDialogProxy();
        _saveFileDialog = saveFileDialog ?? new SaveFileDialogProxy();
    }

    public bool CanSave => true;

    public bool CanLoad => true;

    public event EventHandler? AvailabilityChanged;

    public void Save()
    {
        ConfigureSaveDialog(_saveFileDialog);
        if (_saveFileDialog.ShowDialog() != true || string.IsNullOrWhiteSpace(_saveFileDialog.FileName))
        {
            return;
        }

        try
        {
            var game = _getCurrentGame();
            _gamePersist.SaveRun(
                game.Map,
                game.Player,
                game.Enemies,
                game.Level,
                game.RunState,
                game.TurnsTaken,
                game.VictoryObjective,
                game.CompletionSummary);

            var defaultRunPath = GetDefaultRunFilePath();
            if (!string.Equals(defaultRunPath, _saveFileDialog.FileName, StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(defaultRunPath, _saveFileDialog.FileName, overwrite: true);
            }

            _reportStatus("Run saved.");
            OnAvailabilityChanged();
        }
        catch (SavePersistenceException)
        {
            _reportStatus("Save failed.");
            throw;
        }
    }

    public void Load()
    {
        ConfigureOpenDialog(_openFileDialog);
        if (_openFileDialog.ShowDialog() != true || string.IsNullOrWhiteSpace(_openFileDialog.FileName))
        {
            return;
        }

        try
        {
            var defaultRunPath = GetDefaultRunFilePath();
            if (!string.Equals(defaultRunPath, _openFileDialog.FileName, StringComparison.OrdinalIgnoreCase))
            {
                var directory = Path.GetDirectoryName(defaultRunPath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.Copy(_openFileDialog.FileName, defaultRunPath, overwrite: true);
            }

            var restoredState = _gamePersist.LoadRun();
            _applyRestoredState(restoredState);
            _reportStatus("Run loaded.");
            OnAvailabilityChanged();
        }
        catch (SavePersistenceException)
        {
            _reportStatus("Load failed.");
            throw;
        }
    }

    private static void ConfigureOpenDialog(IOpenFileDialog openFileDialog)
    {
        openFileDialog.Filter = "SharpHack Save (*.shs)|*.shs|All files (*.*)|*.*";
        openFileDialog.FilterIndex = 1;
        openFileDialog.DefaultExt = "shs";
        openFileDialog.CheckFileExists = true;
        openFileDialog.RestoreDirectory = true;
        openFileDialog.Title = "Load SharpHack Run";
    }

    private static void ConfigureSaveDialog(IFileDialog saveFileDialog)
    {
        saveFileDialog.Filter = "SharpHack Save (*.shs)|*.shs|All files (*.*)|*.*";
        saveFileDialog.FilterIndex = 1;
        saveFileDialog.DefaultExt = "shs";
        saveFileDialog.AddExtension = true;
        saveFileDialog.RestoreDirectory = true;
        saveFileDialog.Title = "Save SharpHack Run";
        saveFileDialog.FileName = GetDefaultRunFilePath();
    }

    private static string GetDefaultRunFilePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SharpHack",
            "Saves",
            "current-run.shs");
    }

    private void OnAvailabilityChanged()
    {
        AvailabilityChanged?.Invoke(this, EventArgs.Empty);
    }
}
