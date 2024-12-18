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
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Xml;
using System.Windows.Markup;
using System.Windows;
using System.ComponentModel.DataAnnotations;

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
        protected static ControlsAndLayoutViewModel This { get; set; }

        private readonly IControlsAndLayoutModel _model;

        public DateTime Now => _model.Now;

        public Action<string> ParseCurrentBuffer { get; internal set; }

        [ObservableProperty]
        private string _exTitle = string.Empty;

        [ObservableProperty]
        private string _exDescription = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [CustomValidation(validatorType:typeof(ControlsAndLayoutViewModel),method:nameof(XamlValidator))]
        private string _xamlText= string.Empty;

        [ObservableProperty]
        private string _errorText= string.Empty;

        [ObservableProperty]
        private int _layoutIndex = -1;

        [ObservableProperty]
        private int _controlIndex = -1;

        [ObservableProperty]
        private GridLength _previewRowHeight = new GridLength(1, GridUnitType.Star);

        [ObservableProperty]
        private GridLength _codeRowHeight = new GridLength(0.5,GridUnitType.Star);
        private int cnt=0;
        #endregion

        #region Methods
        public static System.ComponentModel.DataAnnotations.ValidationResult XamlValidator(string value, ValidationContext context)
        {
            return This?._XamlValidator(value, context);
        }

        private System.ComponentModel.DataAnnotations.ValidationResult _XamlValidator(string value, ValidationContext context)
        {
            if (string.IsNullOrWhiteSpace(value))
                return System.ComponentModel.DataAnnotations.ValidationResult.Success;

            try
            {
                ParseCurrentBuffer?.Invoke(value);
                ErrorText = "";
            }
            catch (XamlParseException ex)
            {
                ErrorText = ex.Message;
                return new System.ComponentModel.DataAnnotations.ValidationResult(ex.Message);
            }
            catch (Exception)
            {
                // Ignore
            }
            return System.ComponentModel.DataAnnotations.ValidationResult.Success;
        }

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

        [RelayCommand]
        private void SelectionChanged(object? sender)
        {
            if (sender == null)
                return;

            XamlText = (sender as XmlNode)?.InnerText ??"";
            ExTitle = (sender as XmlNode)?.Attributes["Title"]?.Value ??"";
            ExDescription = (sender as XmlNode)?.Attributes["Description"]?.Value ?? "";
        }

        partial void OnXamlTextChanged(string newValue)
        {
            This = this;
            cnt = 0;
            if (string.IsNullOrWhiteSpace(newValue))
                return;

        }

        partial void OnLayoutIndexChanged(int newValue)
        {
            if (newValue == -1)
                return;

            ControlIndex = -1;
        }

        partial void OnControlIndexChanged(int newValue)
        {
            if (newValue == -1)
                return;

            LayoutIndex = -1;
        }

        [RelayCommand]
        private void ShowPreview()
        {
            PreviewRowHeight = new GridLength(1, GridUnitType.Star);
            CodeRowHeight = new GridLength(0);
        }

        [RelayCommand]
        private void ShowCode()
        {
            PreviewRowHeight = new GridLength(0);
            CodeRowHeight = new GridLength(1, GridUnitType.Star);
        }

        [RelayCommand]
        private void ShowSplit()
        {
            PreviewRowHeight = new GridLength(1, GridUnitType.Star);
            CodeRowHeight = new GridLength(1, GridUnitType.Star);
        }

        [RelayCommand]
        private void Test1()
        {
            ErrorText = $"Test1:{++cnt}";
        }
        #endregion
    }
}
