using Avln_ImageView.Models.Interfaces;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace Avln_ImageView.Models;

public class ImageViewerModel : IImageViewerModel
{
    private ArrayList _imageFiles = new();
    public ArrayList ImageFiles
    {
        get => _imageFiles;
        private set { _imageFiles = value; OnPropertyChanged(); }
    }

    public ImageViewerModel()
    {
        ImageFiles = LoadFiles();
    }

    private static ArrayList LoadFiles()
    {
        var lst = new ArrayList();
        var currDir = Directory.GetCurrentDirectory();
        var temp = Path.Combine(currDir, "myData");
        if (Directory.Exists(temp))
        {
            foreach (var image in Directory.GetFiles(temp, "*.jpg"))
            {
                lst.Add(new FileInfo(image));
            }
        }
        return lst;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
