using CommunityToolkit.Mvvm.Input;
using GenFree.Helper;
using System;
using System.Collections.ObjectModel;
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
    IRelayCommand List2DblCommand { get; }
    IRelayCommand Sources_DblCommand { get; }

    ObservableCollection<IListItem<int>> Sources_Items { get; }
    ObservableCollection<IListItem<int>> Repolist_Items { get; }
    IListItem<int> Repolist_SelectedItem { get; }
    IListItem<int> Sources_SelectedItem { get; }

    int SourceCount { get; }
    string RepoName_Text { get; set; }
    string TextBox2_Text { get; set; }
    string TextBox3_Text { get; set; }
    string TextBox4_Text { get; set; }
    string TextBox5_Text { get; set; }
    string TextBox6_Text { get; set; }
    string RichTextBox1_Text { get; set; }
    string RichTextBox2_Text { get; set; }
    float FontSize { get; }
    object HintFarb { get; }
    Action DoClose { get; set; }
}