using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM.ViewModel;
using MVVM_Converter_CTDrawGrid.Models.Interfaces;
using System;
using Werner_Flaschbier_Base.Model;

namespace MVVM_Converter_CTDrawGrid.Model;

/// <summary>
/// Class DrawGridModel.
/// </summary>
public partial class DrawGridModel : BaseViewModelCT , IDrawGridModel
{
    /// <summary>
    /// Gets or sets the act level.
    /// </summary>
    /// <value>The act level.</value>
    [ObservableProperty]
    private  int _actLevel;

    /// <summary>
    /// Gets or sets the level data.
    /// </summary>
    /// <value>The level data.</value>
    [ObservableProperty]
    private  FieldDef[]? _levelData;


    /// <summary>
    /// Loads the level.
    /// </summary>
    public void LoadLevel()
    {
        _actLevel = 0;
        LevelData = LevelDefs.GetLevel(0);
    }

    /// <summary>
    /// Nexts the level.
    /// </summary>
    public  void NextLevel()
    {
        if (ActLevel < LevelDefs.Count)
            ActLevel += 1;
        LevelData = LevelDefs.GetLevel(ActLevel);
    }

    /// <summary>
    /// Previouses the level.
    /// </summary>
    public  void PrevLevel()
    {
        if (ActLevel > 0) 
           ActLevel -= 1;
        LevelData = LevelDefs.GetLevel(ActLevel);
    }

}
