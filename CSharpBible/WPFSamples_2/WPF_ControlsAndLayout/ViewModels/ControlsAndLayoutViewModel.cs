// ***********************************************************************
// Assembly         : WPF_ControlsAndLayout
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
using WPF_ControlsAndLayout.Models;
using System;
using System.ComponentModel;
using WPF_ControlsAndLayout.Models.Interfaces;

namespace WPF_ControlsAndLayout.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class ControlsAndLayoutViewModel : BaseViewModelCT
    {
        #region Properties
        public static Func<IControlsAndLayoutModel> GetModel { get; set; } = () => new ControlsAndLayoutModel();

        private readonly IControlsAndLayoutModel _model;

        public DateTime Now => _model.Now;
        #endregion
  
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public ControlsAndLayoutViewModel():this(GetModel())
        {
        }

        public ControlsAndLayoutViewModel(IControlsAndLayoutModel model)
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
