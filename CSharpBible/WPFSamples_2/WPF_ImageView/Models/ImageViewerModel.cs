using CommunityToolkit.Mvvm.ComponentModel;
using ImageView.Models.Interfaces;
using System.Collections;
using System.IO;

namespace ImageViewer.Models;

public partial class ImageViewerModel : ObservableObject, IImageViewerModel
{
    [ObservableProperty]
    private ArrayList _imageFiles;

    public ImageViewerModel()
    {
        ImageFiles = GetImageFileInfo();
    }
    private ArrayList GetImageFileInfo()
    {
        var imageFiles = new ArrayList();
        //Get directory path of myData
        var currDir = Directory.GetCurrentDirectory();
        var temp = currDir + "\\myData";
        var files = Directory.GetFiles(temp, "*.jpg");
        foreach (var image in files)
        {
            var info = new FileInfo(image);
            imageFiles.Add(info);
        }
        return imageFiles;
    }
}