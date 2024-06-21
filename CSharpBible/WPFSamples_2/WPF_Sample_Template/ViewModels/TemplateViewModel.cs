// ***********************************************************************
// Assembly         : WPF_Sample_Template
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
using WPF_Sample_Template.Models;
using System;
using System.ComponentModel;

namespace WPF_Sample_Template.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class TemplateViewModel : BaseViewModelCT
    {
        #region Properties
        public static Func<ITemplateModel> GetModel { get; set; } = () => new TemplateModel();

        private readonly ITemplateModel _model;

        public DateTime Now => _model.Now;
        #endregion
  
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public TemplateViewModel():this(GetModel())
        {
        }

        public TemplateViewModel(ITemplateModel model)
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
