// ***********************************************************************
// Assembly         : MVVM_36_ComToolKtSavesWork
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
using MVVM.ViewModel;
using MVVM_36_ComToolKtSavesWork.Models;
using System;
using System.ComponentModel;

namespace MVVM_36_ComToolKtSavesWork.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class CommunityToolkit2ViewModel : BaseViewModelCT
    {
        #region Properties
        public static Func<ICommunityToolkit2Model> GetModel { get; set; } = () => new CommunityToolkit2Model();

        private readonly ICommunityToolkit2Model _model;

        public DateTime Now => _model.Now;
        #endregion
  
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public CommunityToolkit2ViewModel():this(GetModel())
        {
        }

        public CommunityToolkit2ViewModel(ICommunityToolkit2Model model)
        {
            _model = model;
            _model.PropertyChanged += OnMPropertyChanged;
        }

        private void OnMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName); 
        }

        #endregion
    }
}
