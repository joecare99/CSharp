// ***********************************************************************
// Assembly         : WPF_Complex_Layout
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="ComplexLayoutViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF_Complex_Layout.Properties;

namespace WPF_Complex_Layout.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class ComplexLayoutViewModel : BaseViewModelCT
    {
        #region Properties
        [ObservableProperty]
        private string _messageText = "";

        [ObservableProperty]
        private string _messageTitle = Resources.txtMsgTitle;

        [ObservableProperty]
        private bool _showMessage = false;

        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public ComplexLayoutViewModel()
        {
        }

        [RelayCommand]
        public void Button1()
        {
           MessageText = Resources.txtBtn1React;
            ShowMessage = true;
        }

        [RelayCommand]
        public void Button2()
        {
            MessageText = Resources.txtBtn2React;
            ShowMessage = true;
        }

        [RelayCommand]
        public void Msg_OK()
        {
           ShowMessage = false;
        }

        #endregion
    }
}
