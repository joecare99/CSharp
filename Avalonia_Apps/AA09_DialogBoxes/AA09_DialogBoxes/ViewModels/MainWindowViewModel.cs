// ***********************************************************************
// Assembly         : AA09_DialogBoxes
// Author           : Mir
// Created          : 07-21-2022
//
// Last Modified By : Mir
// Last Modified On : 08-09-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright Â© JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using AA09_DialogBoxes.Messages;
using Avalonia.ViewModels;
using System.Threading.Tasks;
using MsBox.Avalonia.Enums;

namespace AA09_DialogBoxes.ViewModels;

public enum MsgBoxResult { None, Yes, No }

public partial class MainWindowViewModel : ViewModelBase
{
    #region Properties
    [ObservableProperty]
    public partial string Name { get; set; }= "<Name>";
    [ObservableProperty]
    public partial string Email { get; set; }= "<Email>";
    [ObservableProperty]
    private int cnt = 1;
    #endregion

    public MainWindowViewModel() { }

    [RelayCommand]
    private async Task OpenMsg()
    {
        var request = new MessageBoxRequestMessage("Frage", "Willst Du Das ?");
        WeakReferenceMessenger.Default.Send(request);
        var result = await request.Response;
        if (result == ButtonResult.Yes)
            Name = "42 Entwickler";
        else
            Name = "Nö";
    }

    [RelayCommand]
    private async Task OpenDialog()
    {
        var request = new EditDialogRequestMessage(this.Name, this.Email);
        WeakReferenceMessenger.Default.Send(request);
        var r = await request.Response;
        if (r.Item1)
            (Name, Email) = r.Item2;       
    }

    partial void OnNameChanged(string value)
    {
        Cnt++;
    }
}
