using BaseLib.Helper;
using System;
using Werner_Flaschbier_Base.Model;

namespace MVVM_Converter_CTDrawGrid.Model;

/// <summary>
/// Class Model.
/// </summary>
public static class Model
{
    private static int actLevel;
    private static FieldDef[]? _levelData;

    /// <summary>
    /// Gets or sets the level data.
    /// </summary>
    /// <value>The level data.</value>
    public static FieldDef[]? LevelData { get => _levelData; set => PropertyHelper.SetProperty(ref _levelData,value, OnPropertyChanged); }
    /// <summary>
    /// Gets or sets the act level.
    /// </summary>
    /// <value>The act level.</value>
    public static int ActLevel { get => actLevel; set => PropertyHelper.SetProperty(ref actLevel, value, OnPropertyChanged); }
    /// <summary>
    /// Gets or sets the property changed.
    /// </summary>
    /// <value>The property changed.</value>
    public static EventHandler<(string, object, object)>? PropertyChanged { get; set; }

    /// <summary>
    /// Loads the level.
    /// </summary>
    public static void LoadLevel()
    {
        actLevel = 0;
        LevelData = LevelDefs.GetLevel(0);
    }

    /// <summary>
    /// Nexts the level.
    /// </summary>
    public static void NextLevel()
    {
        if (ActLevel < LevelDefs.Count)
            ActLevel += 1;
        LevelData = LevelDefs.GetLevel(ActLevel);
    }

    /// <summary>
    /// Previouses the level.
    /// </summary>
    public static void PrevLevel()
    {
        if (ActLevel > 0) 
           ActLevel -= 1;
        LevelData = LevelDefs.GetLevel(ActLevel);
    }

    private static void OnPropertyChanged<T>(string arg1, T arg2, T arg3)
    {
        PropertyChanged?.Invoke(null, (arg1, arg2!, arg3!));
    }
}
