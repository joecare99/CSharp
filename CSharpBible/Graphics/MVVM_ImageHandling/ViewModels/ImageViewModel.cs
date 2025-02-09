// ***********************************************************************
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
using BaseLib.Helper;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MVVM.ViewModel;
using MVVM_ImageHandling.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
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

        private readonly IImageHandlingModel _model;

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
        public ImageViewModel():this(IoC.GetRequiredService<IImageHandlingModel>())
        {
        }

        public ImageViewModel(IImageHandlingModel model)
        {
            _model = model;
            _model.PropertyChanged += OnMPropertyChanged;
            _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            Image2 = $"pack://application:,,,/{_assemblyName};component/Resources/card_♣Ace.png";

            using var emf = new System.Drawing.Imaging.Metafile(new MemoryStream(Properties.Resources.card_D10_emf));
            using var bmp = new System.Drawing.Bitmap(emf.Width, emf.Height);
            bmp.SetResolution(emf.HorizontalResolution, emf.VerticalResolution);
            using System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            g.DrawImage(emf, 0, 0);
            Image3 = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
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
