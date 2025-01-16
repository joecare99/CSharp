// ***********************************************************************
// Assembly         : MVVM_34a_CTBindingEventArgs
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="MainWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using System.Windows.Input;

namespace MVVM_34a_CTBindingEventArgs.ViewModels;

/// <summary>
/// Class MainWindowViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class EventsBindingViewModel : BaseViewModelCT
{
    #region Properties
    [ObservableProperty]  
    private string _state="";     
    #endregion
    
    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public EventsBindingViewModel()
    {
    }

    [RelayCommand]  
    private void LostFocus(object? _) 
        => State = "Lost focus";

    [RelayCommand]
    private void GotFocus(object? _) 
        => State = "Got focus";

    private bool CanKeyDown(object? s)
        => s is KeyEventArgs { Key: Key.Enter or Key.Escape };
    [RelayCommand(CanExecute = nameof(CanKeyDown))]
    private void KeyDown(object? s) 
        => State = $"TextChanged({(s as KeyEventArgs)?.Key})";

#if !NET5_0_OR_GREATER
    /// <summary>
    /// Finalizes an instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    ~EventsBindingViewModel()
    {
        return;
    }
#endif
    #endregion
}
