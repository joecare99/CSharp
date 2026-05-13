using System;

namespace SharpHack.ViewModel;

/// <summary>
/// Provides UI-facing save and load actions without coupling view models to file-picker behavior.
/// </summary>
public interface IGameSaveLoadService
{
    /// <summary>
    /// Gets a value indicating whether saving is currently available.
    /// </summary>
    bool CanSave { get; }

    /// <summary>
    /// Gets a value indicating whether loading is currently available.
    /// </summary>
    bool CanLoad { get; }

    /// <summary>
    /// Saves the current game state.
    /// </summary>
    void Save();

    /// <summary>
    /// Loads a previously saved game state.
    /// </summary>
    void Load();

    /// <summary>
    /// Raised when save/load availability changes.
    /// </summary>
    event EventHandler? AvailabilityChanged;
}
