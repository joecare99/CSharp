using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace GenFree.ViewModels.Interfaces;

public interface IRepoViewModel: INotifyPropertyChanged
{
    bool BtnDeleteVisible { get; }
    IRelayCommand FormLoadCommand { get; }
    IRelayCommand LinkClickCommand { get; }
    IRelayCommand SaveCommand { get; }
    IRelayCommand Save2Command { get; }
    IRelayCommand CloseCommand { get; }
    IRelayCommand NewEntryCommand { get; }
    IRelayCommand DeleteCommand { get; }

    int SourceCount { get; }
    string RepoName { get; set; }
    string TextBox2_Text { get; set; }
    string TextBox3_Text { get; set; }
    string TextBox4_Text { get; set; }
    string TextBox5_Text { get; set; }
    string TextBox6_Text { get; set; }
    float FontSize { get; }
    object HintFarb { get; }

}