// ***********************************************************************
// Assembly         : MVVM_09_DialogBoxes
// Author           : Mir
// Created          : 07-21-2022
//
// Last Modified By : Mir
// Last Modified On : 08-09-2022
// ***********************************************************************
// <copyright file="DialogViewModel.cs" company="JC-Soft">
//     Copyright Ã‚Â© JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using AA09_DialogBoxes.Messages;
using System.Threading.Tasks;
using MsBox.Avalonia.Enums;

namespace AA09_DialogBoxes.ViewModels;


public partial class DialogViewModel : ViewModelBase
{
    #region Properties
    /// <summary>
    /// The name
    /// </summary>
    [ObservableProperty]
    public partial string Name { get; set; } = "<Name>";
    /// <summary>
    /// The email
    /// </summary>
    [ObservableProperty]
    public partial string Email { get; set; } = "<Email>";
    /// <summary>
    /// The count
    /// </summary>
    [ObservableProperty]
    private int cnt = 1;
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public DialogViewModel()
    {
    }

    [RelayCommand]
    private async Task OpenMsg()
    {
        var request = new MessageBoxRequestMessage("Frage", "Willst Du Das ?");
        WeakReferenceMessenger.Default.Send(request);
        var result = await request.Response;
        Name = result == ButtonResult.Yes ? "42 Entwickler" : "Nö";
    }

    [RelayCommand]
    private async Task OpenDialog()
    {
        var request = new EditDialogRequestMessage(this.Name, this.Email);
        WeakReferenceMessenger.Default.Send(request);
        var r = await request.Response;
        if (r.Item1 == true)
           (Name,Email) = r.Item2;
    }

    partial void OnNameChanged(string value)
    {
        Cnt++;
    }

}
