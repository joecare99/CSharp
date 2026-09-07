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
    private Bitmap? _bitmap;
    private Bitmap? _bitmap;
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
                _bitmap?.Dispose();

                _bitmap = new Bitmap(fi.FullName);
                Image = _bitmap;
                var px = _bitmap.PixelSize;
                ImageSize = px.Width + " x " + px.Height;
                ImageFormat = "Bitmap";
                FileSize = ((fi.Length + 512) / 1024) + "k";
            }
        }
    }
}
