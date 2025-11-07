// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>

using CommunityToolkit.Mvvm.ComponentModel;

namespace AA21_Buttons.ViewModels;

/// <summary>
/// ViewModel für das Hauptfenster.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private ButtonsViewViewModel? buttonsViewViewModel;

    public MainWindowViewModel(ButtonsViewViewModel buttonsViewViewModel)
    {
        ButtonsViewViewModel = buttonsViewViewModel;
    }
}
