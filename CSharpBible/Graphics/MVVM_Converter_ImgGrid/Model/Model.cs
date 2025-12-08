using BaseLib.Helper;
using System;
using Werner_Flaschbier_Base.Model;

namespace MVVM_Converter_ImgGrid.Model;

public static class Model
{
    private static int actLevel;
    private static FieldDef[]? _levelData;

    public static FieldDef[]? LevelData { get => _levelData; set => PropertyHelper.SetProperty(ref _levelData,value, OnPropertyChanged); }
    public static int ActLevel { get => actLevel; set => PropertyHelper.SetProperty(ref actLevel, value, OnPropertyChanged); }
    public static EventHandler<(string, object, object)>? PropertyChanged { get; set; }

    public static void LoadLevel()
    {
        actLevel = 0;
        LevelData = LevelDefs.GetLevel(0);
    }

    public static void NextLevel()
    {
        if (ActLevel < LevelDefs.Count)
            ActLevel += 1;
        LevelData = LevelDefs.GetLevel(ActLevel);
    }

    public static void PrevLevel()
    {
        if (ActLevel > 0) 
           ActLevel -= 1;
        LevelData = LevelDefs.GetLevel(ActLevel);
    }

    private static void OnPropertyChanged<T>(string arg1, T arg2, T arg3)
    {
        PropertyChanged?.Invoke(null, (arg1, arg2, arg3));
    }
}
