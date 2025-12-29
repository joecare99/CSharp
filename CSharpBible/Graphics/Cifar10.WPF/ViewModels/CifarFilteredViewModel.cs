using CommonDialogs;
using CommonDialogs.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libCIFAR.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cifar10.WPF.ViewModels;

public partial class CifarFilteredViewModel : ObservableObject
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [ObservableProperty]
    private ECifar10Category _selectedCategory;
    private byte[]? data;

    public Func<IFileDialog, bool>? showfiledlg { get; set; }

    public ObservableCollection<ECifar10Category> Categories { get; } = new ObservableCollection<ECifar10Category>
    {
        ECifar10Category.Airplane,
        ECifar10Category.Automobile,
        ECifar10Category.Bird,
        ECifar10Category.Cat,
        ECifar10Category.Deer,
        ECifar10Category.Dog,
        ECifar10Category.Frog,
        ECifar10Category.Horse,
        ECifar10Category.Ship,
        ECifar10Category.Truck
    };

    public ObservableCollection<BitmapImage> Images { get; } = new ObservableCollection<BitmapImage>();

    public CifarFilteredViewModel()
    {
        SelectedCategory = ECifar10Category.Airplane; // Default
    }

    [RelayCommand]
    private void LoadData()
    {
        var filedlg = new OpenFileDialogProxy()
        {
            Title = "Select CIFAR-10 Data File",
            Filter = "CIFAR-10 Data Files (*.bin)|*.bin|All Files (*.*)|*.*",
            Multiselect = false
        };
        if (showfiledlg?.Invoke(filedlg) == true)
        {
            string filePath = filedlg.FileName;
            data = File.ReadAllBytes(filePath);
            SelectedCategory = ECifar10Category.Airplane; // Reset to default category
        }
    }

    partial void OnSelectedCategoryChanged(ECifar10Category value)
    {
        LoadImagesForCategory();
    }
    private void LoadImagesForCategory()
    {
        Images.Clear();
        if (data == null)
            return;
        Cifar10Record c10r = new();
        for (int i = 0; i < 10000; i++)
        {
            c10r.ReadFrom(data, i * 3073);
            if (c10r.Label == SelectedCategory)
            {
                byte[] imgData = c10r.GetImageAsRgbArray();
                var wb = new WriteableBitmap(32, 32, 96, 96, PixelFormats.Rgb24, null);
                wb.WritePixels(new Int32Rect(0, 0, 32, 32), imgData, 32 * 3, 0);
                using var stream = new MemoryStream();
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wb));
                encoder.Save(stream);
                stream.Position = 0;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();

                Images.Add(bitmap);
            }
        }
    }

}
