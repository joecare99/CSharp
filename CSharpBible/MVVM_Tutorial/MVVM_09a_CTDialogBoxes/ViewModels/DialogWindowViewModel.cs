// ***********************************************************************
// Assembly         : MVVM_09a_CTDialogBoxes
// Author           : Mir
// Created          : 12-29-2021
//
// Last Modified By : Mir
// Last Modified On : 07-20-2022
// ***********************************************************************
// <copyright file="DialogWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MVVM_09a_CTDialogBoxes.ViewModels
{
    /// <summary>
    /// Class DialogWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class DialogWindowViewModel : BaseViewModelCT
    {
        /// <summary>
        /// The name
        /// </summary>
        [ObservableProperty]
        private string _name = "";
        /// <summary>
        /// The email
        /// </summary>
        [ObservableProperty] 
        private string _email = "";

        /// <summary>
        /// Occurs when [ok].
        /// </summary>
        public event EventHandler DoOK;
        /// <summary>
        /// Occurs when [cancel].
        /// </summary>
        public event EventHandler DoCancel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogWindowViewModel"/> class.
        /// </summary>
        public DialogWindowViewModel()
        {
        }

        [RelayCommand]
        private void Cancel()
        {
            Name = String.Empty;
            Email = String.Empty;
            this.DoCancel?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void OK()
        {
            this.DoOK?.Invoke(this, EventArgs.Empty);
        }
    }
}
