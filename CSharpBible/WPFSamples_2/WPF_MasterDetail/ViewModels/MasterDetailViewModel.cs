// ***********************************************************************
// Assembly         : WPF_MasterDetail
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
using WPF_MasterDetail.Models;
using System;
using System.ComponentModel;

namespace WPF_MasterDetail.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class MasterDetailViewModel : BaseViewModelCT
    {
        #region Properties
        public static Func<IMasterDetailModel> GetModel { get; set; } = () => new MasterDetailModel();

        private readonly IMasterDetailModel _model;

        public DateTime Now => _model.Now;
        #endregion
  
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MasterDetailViewModel():this(GetModel())
        {
        }

        public MasterDetailViewModel(IMasterDetailModel model)
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
