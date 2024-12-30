// ***********************************************************************
// Assembly         : WPF_MoveWindow
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="MoveWindowViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using WPF_MoveWindow.Models;
using System;
using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF_MoveWindow.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class MoveWindowViewModel : BaseViewModelCT
    {
        #region Properties
        static Func<IMoveWindowModel> GetModel { get; } = () => new MoveWindowModel();
        IMoveWindowModel _model;

        [ObservableProperty]
        public string _locationX=string.Empty;

        [ObservableProperty]
        public string _locationY=string.Empty;
        public bool EnableKoorInput => _model.EnableKoorInput;
        public IRelayCommand MoveBtnCommand => _model.MoveBtnCommand;
        public ObservableCollection<string> FeedBackList => _model.FeedBackList;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MoveWindowViewModel() : this(GetModel())
        {
        }

        public MoveWindowViewModel(IMoveWindowModel value)
        {
            _model = value;
            _model.PropertyChanged += ModelPropChg;
        }

        private void ModelPropChg(object? sender, PropertyChangedEventArgs e)
        {
           if (e.PropertyName == nameof(_model.TargetLocation))
            {
                LocationX = _model.TargetLocation.X.ToString();
                LocationY = _model.TargetLocation.Y.ToString();
            }
           else
                OnPropertyChanged(e.PropertyName);
        }

        partial void OnLocationXChanged(string? newValue)
        {
            if (double.TryParse(newValue, out double value))
                _model.TargetLocation = new Point(value, _model.TargetLocation.Y);
        }

        partial void OnLocationYChanged(string? newValue)
        {
            if (double.TryParse(newValue, out double value))
                _model.TargetLocation = new Point(_model.TargetLocation.X, value);
        }

        #endregion
    }
}
