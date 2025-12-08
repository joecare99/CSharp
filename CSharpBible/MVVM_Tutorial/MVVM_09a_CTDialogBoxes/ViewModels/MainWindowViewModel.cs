// ***********************************************************************
// Assembly         : MVVM_09a_CTDialogBoxes
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
using MVVM.ViewModel;
using System.Windows;

namespace MVVM_09a_CTDialogBoxes.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class MainWindowViewModel : BaseViewModelCT
{
    #region Delegates
    /// <summary>
    /// Delegate OpenDialogHandler
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="email">The email.</param>
    /// <returns>System.ValueTuple&lt;System.String, System.String&gt;.</returns>
    public delegate (string name, string email) OpenDialogHandler(string name, string email);

    /// <summary>
    /// Delegate OpenMessageBoxHandler
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="name">The name.</param>
    /// <returns>MessageBoxResult.</returns>
    public delegate MessageBoxResult OpenMessageBoxHandler(string title, string name);
    #endregion
    #region Properties
    /// <summary>
    /// The name
    /// </summary>
    [ObservableProperty]
    string _name = "<Name>";
    /// <summary>
    /// The email
    /// </summary>
    [ObservableProperty]
    string _email = "<Email>";
    /// <summary>
    /// The count
    /// </summary>
    [ObservableProperty]
    private int cnt = 1;

    /// <summary>
    /// Gets or sets the open dialog.
    /// </summary>
    /// <value>The open dialog.</value>
    public OpenDialogHandler? DoOpenDialog { get; set; }
    /// <summary>
    /// Gets or sets the open message box.
    /// </summary>
    /// <value>The open message box.</value>
    public OpenMessageBoxHandler? DoOpenMessageBox { get; set; }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
    }

    [RelayCommand]
    private void OpenMsg()
    {
        if (this.DoOpenMessageBox?.Invoke("Frage", "Willst Du Das ?") == MessageBoxResult.Yes)
        {
            Name = "42 Entwickler";
        }
        else
        {
            Name = "Nö";
        }
    }
    [RelayCommand]
    private void OpenDialog()
    {
        (Name, Email) = DoOpenDialog?.Invoke(this.Name, this.Email) ?? ("", "");
    }

    partial void OnNameChanged(string value)
    {
        Cnt++;
    }

}
