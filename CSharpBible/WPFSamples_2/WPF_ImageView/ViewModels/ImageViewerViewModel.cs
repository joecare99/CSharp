using CommunityToolkit.Mvvm.ComponentModel;
using ImageView.Models.Interfaces;
using ImageView.ViewModels.Interfaces;
using System;
using System.Collections;
using System.IO;
using System.Windows.Media.Imaging;

namespace ImageView.ViewModels;

public partial class ImageViewerViewModel : ObservableObject, IImageViewerViewModel
{
    private readonly IImageViewerModel _model;
    public ArrayList ImageFiles => _model.ImageFiles;

    [ObservableProperty]
    private int _selectedImage;

    [ObservableProperty]
    private object _image;

    [ObservableProperty]
    private string _imageSize;

    [ObservableProperty]
    private string _imageFormat;
    
    [ObservableProperty]
    private string _fileSize;

    public ImageViewerViewModel(IImageViewerModel model)
    {
        _model = model;
        model.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
    }

    partial void OnSelectedImageChanged(int value)
    {
        if (value >= 0 && value < ImageFiles.Count )
        {
                var selection = ImageFiles[value];

                if (selection is FileInfo fi)
                {
                    //Set currentImage to selected Image
                    var selLoc = new Uri(fi.FullName);
                    var id = new BitmapImage(selLoc);
                    Image = selLoc;

                    //Setup Info Text
                    ImageSize = id.PixelWidth + " x " + id.PixelHeight;
                    ImageFormat = id.Format.ToString();
                    FileSize = ((fi.Length + 512) / 1024) + "k";
                }
            }

    }
}