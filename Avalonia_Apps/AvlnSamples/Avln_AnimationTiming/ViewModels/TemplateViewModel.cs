// ***********************************************************************
// Assembly   : Avln_AnimationTiming
// Author  : Mir
// Created          : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="TemplateViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.ViewModels;
using Avln_AnimationTiming.Models;
using System;
using System.ComponentModel;

namespace Avln_AnimationTiming.ViewModels
{
 /// <summary>
    /// Class TemplateViewModel.
    /// Implements the <see cref="BaseViewModelCT" />
  /// </summary>
    /// <seealso cref="BaseViewModelCT" />
    public partial class TemplateViewModel : BaseViewModelCT
    {
        #region Properties
        public static Func<ITemplateModel> GetModel { get; set; } = () => new TemplateModel();

        private readonly ITemplateModel _model;

        public DateTime Now => _model.Now;
        #endregion
  
 #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateViewModel"/> class.
  /// </summary>
        public TemplateViewModel() : this(GetModel())
        {
        }

      public TemplateViewModel(ITemplateModel model)
      {
 _model = model;
_model.PropertyChanged += OnMPropertyChanged;
        }

        private void OnMPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
        OnPropertyChanged(e.PropertyName); 
        }
        #endregion
    }
}
