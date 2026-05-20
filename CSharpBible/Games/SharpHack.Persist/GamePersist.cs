using SharpHack.Base.Interfaces;
using SharpHack.Base.Data;
using SharpHack.Base.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpHack.Persist;

public class GamePersist : IGamePersist
{
    private readonly string _saveDirectory;
    private readonly SaveFileWriter _saveFileWriter;
    private readonly SaveFileReader _saveFileReader;

    public GamePersist()
        : this(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SharpHack", "Saves"), new SaveFileWriter(), new SaveFileReader())
    {
    }

    public GamePersist(string saveDirectory, SaveFileWriter saveFileWriter, SaveFileReader saveFileReader)
    {
        if (string.IsNullOrWhiteSpace(saveDirectory))
        {
            throw new ArgumentException("A save directory is required.", nameof(saveDirectory));
        }

        _saveDirectory = saveDirectory;
        _saveFileWriter = saveFileWriter ?? throw new ArgumentNullException(nameof(saveFileWriter));
        _saveFileReader = saveFileReader ?? throw new ArgumentNullException(nameof(saveFileReader));
    }

    public bool LoadLevel(int level, out IMap? map, out IList<ICreature>? enemies)
    {
        var saveFilePath = GetSaveFilePath(level);
        if (!File.Exists(saveFilePath))
        {
            map = null;
            enemies = null;
            return false;
        }

        try
        {
            var saveGame = _saveFileReader.Read(saveFilePath);
            var restored = SaveGameMapper.MapToRuntimeState(saveGame);

            if (restored.Map == null || restored.Player == null)
            {
                throw new SaveFileCorruptException("The save file could not be restored into a valid level state.");
            }

            map = restored.Map;
            enemies = restored.Enemies;
            return true;
        }
        catch (SaveFileMissingException)
        {
            map = null;
            enemies = null;
            return false;
        }
        catch (NotSupportedException ex)
        {
            throw new SaveFileIncompatibleException("The save file uses an unsupported version or data shape.", ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new SaveFileCorruptException("The save file contents are inconsistent and could not be restored.", ex);
        }
    }

    public void SaveRun(
        IMap map,
        ICreature player,
        IList<ICreature> enemies,
        int level,
        GameRunState runState,
        int turnsTaken,
        string victoryObjective,
        string completionSummary)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        if (player == null)
        {
            throw new ArgumentNullException(nameof(player));
        }

        if (enemies == null)
        {
            throw new ArgumentNullException(nameof(enemies));
        }

        var saveGame = SaveGameMapper.MapToSaveGame(
            map,
            player,
            enemies,
            level,
            runState,
            turnsTaken,
            victoryObjective,
            completionSummary,
            source: nameof(GamePersist),
            diagnosticsTag: "current-run");

        _saveFileWriter.Write(GetRunSaveFilePath(), saveGame);
    }

    public bool HasSavedRun()
    {
        return File.Exists(GetRunSaveFilePath());
    }

    public bool TryLoadRun(out RestoreGameState? restoredState)
    {
        if (!HasSavedRun())
        {
            restoredState = null;
            return false;
        }

        restoredState = LoadRun();
        return true;
    }

    public RestoreGameState LoadRun()
    {
        try
        {
            var saveGame = _saveFileReader.Read(GetRunSaveFilePath());
            var restored = SaveGameMapper.MapToRuntimeState(saveGame);

            if (restored.Map == null || restored.Player == null)
            {
                throw new SaveFileCorruptException("The save file could not be restored into a valid run state.");
            }

            return restored;
        }
        catch (NotSupportedException ex)
        {
            throw new SaveFileIncompatibleException("The save file uses an unsupported version or data shape.", ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new SaveFileCorruptException("The save file contents are inconsistent and could not be restored.", ex);
        }
    }

    public void SaveLevel(int level, IMap map, ICreature player, IList<ICreature> enemies)
    {
        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        if (player == null)
        {
            throw new ArgumentNullException(nameof(player));
        }

        if (enemies == null)
        {
            throw new ArgumentNullException(nameof(enemies));
        }

        var saveGame = SaveGameMapper.MapToSaveGame(
            map,
            player,
            enemies,
            level,
            GameRunState.Running,
            turnsTaken: 0,
            victoryObjective: string.Empty,
            completionSummary: string.Empty,
            source: nameof(GamePersist),
            diagnosticsTag: $"level-{level}");

        _saveFileWriter.Write(GetSaveFilePath(level), saveGame);
    }

    private string GetSaveFilePath(int level)
    {
        return Path.Combine(_saveDirectory, $"level-{level}.shs");
    }

    private string GetRunSaveFilePath()
    {
        return Path.Combine(_saveDirectory, "current-run.shs");
    }
}
