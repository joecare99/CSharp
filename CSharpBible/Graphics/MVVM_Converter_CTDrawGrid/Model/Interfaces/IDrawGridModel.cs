using System.ComponentModel;
using Werner_Flaschbier_Base.Model;

namespace MVVM_Converter_CTDrawGrid.Models.Interfaces;

public interface IDrawGridModel :INotifyPropertyChanged
{
    FieldDef[]? LevelData { get; set; }
    int ActLevel { get; set; }

    void LoadLevel();
    void NextLevel();
    void PrevLevel();
}