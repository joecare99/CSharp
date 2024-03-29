﻿// ***********************************************************************
// Assembly         : MVVM_ImageHandling
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
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using MVVM_ImageHandling.Models;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MVVM_ImageHandling.ViewModels
{
    /// <summary>
    /// Class MainWindowViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    public partial class ImageViewModel : BaseViewModelCT
    {
        #region Properties
        public static Func<ITemplateModel> GetModel { get; set; } = () => new TemplateModel();

        private readonly ITemplateModel _model;

        public DateTime Now => _model.Now;

        [ObservableProperty]
        private string _image2;

        [ObservableProperty]
        private ImageSource _image3;
        private string _assemblyName;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public ImageViewModel():this(GetModel())
        {
        }

        public ImageViewModel(ITemplateModel model)
        {
            _model = model;
            _model.PropertyChanged += OnMPropertyChanged;
            _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            Image2 = $"pack://application:,,,/{_assemblyName};component/Resources/card_♣Ace.png";
            var ms = new MemoryStream();
        //    Properties.Resources.card_D10_emf.Save(ms, ImageFormat.Png);
            ms.Position = 0L;
      //      Image3 = LoadImageFromStream(ms); 
        }

        public BitmapImage LoadImageFromStream(Stream memoryStream)
        {
            BitmapImage bitmapImage = new();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private void OnMPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName); 
        }

        [RelayCommand]
        private void SetClub()
        {
            Image2 = $"pack://application:,,,/{_assemblyName};component/Resources/card_♣Ace.png";
        }

        [RelayCommand]
        private void SetHeart()
        {
            Image2 = $"pack://application:,,,/{_assemblyName};component/Resources/card_♥Ace.png";
        }
        [RelayCommand]
        private void SetDiamond()
        {
            Image2 = $"pack://application:,,,/{_assemblyName};component/Resources/card_♦Ace.png";
        }
        #endregion
    }
}
