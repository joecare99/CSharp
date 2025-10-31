using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avln_ImageView.Models.Interfaces;

namespace Avln_ImageView.ViewModels;

public partial class ImageViewerViewModel : ObservableObject
{
    private readonly IImageViewerModel _model;
    public ArrayList ImageFiles => _model.ImageFiles;

    [ObservableProperty]
    private int _selectedImage;

    [ObservableProperty]
    private IImage? _image;

    [ObservableProperty]
    private string _imageSize = string.Empty;

    [ObservableProperty]
    private string _imageFormat = string.Empty;

    [ObservableProperty]
    private string _fileSize = string.Empty;

    public ImageViewerViewModel(IImageViewerModel model)
    {
        _model = model;
    }

    partial void OnSelectedImageChanged(int value)
    {
        if (value >= 0 && value < ImageFiles.Count)
        {
            var selection = ImageFiles[value];
            if (selection is FileInfo fi)
            {
                // Dispose previous image if needed
                if (Image is Bitmap oldBmp)
                {
                    oldBmp.Dispose();
                }

                var bmp = new Bitmap(fi.FullName);
                Image = bmp;
                var px = bmp.PixelSize;
                ImageSize = px.Width + " x " + px.Height;
                ImageFormat = "Bitmap";
                FileSize = ((fi.Length + 512) / 1024) + "k";
            }
        }
    }
}
