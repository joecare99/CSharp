// ***********************************************************************
// Assembly         : WPF_StickyNotesDemo
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
using WPF_StickyNotesDemo.Models;
using System;
using System.ComponentModel;

namespace WPF_StickyNotesDemo.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class StickyNotesViewModel : BaseViewModelCT
    {
        #region Properties
        public static Func<IStickyNotesModel> GetModel { get; set; } = () => new StickyNotesModel();

        private readonly IStickyNotesModel _model;

        public DateTime Now => _model.Now;
        #endregion
  
        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public StickyNotesViewModel():this(GetModel())
        {
        }

        public StickyNotesViewModel(IStickyNotesModel model)
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
