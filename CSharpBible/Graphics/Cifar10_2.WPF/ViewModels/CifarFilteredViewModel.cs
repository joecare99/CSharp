using CommonDialogs;
using CommonDialogs.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libCIFAR.Data;
using libMachLearn.Models;
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

    [ObservableProperty]
    private BitmapImage _orginaImage;
    [ObservableProperty]
    private BitmapImage _destImage;
    [ObservableProperty]
    private BitmapImage _genResultImage;

    [ObservableProperty]
    private float _Accuracy;
    private readonly NeuralNetwork nn;

    public Func<IFileDialog, bool>? showfiledlg { get; set; }


    public ObservableCollection<BitmapImage> Images { get; } = new ObservableCollection<BitmapImage>();

    public CifarFilteredViewModel()
    {
        nn = new NeuralNetwork(0.01, 256, 512, 1024); 
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

                byte[] imgData = c10r.ImageData;

            float[] r = ExtractData(imgData, 0, 1024); 
            float[] g = ExtractData(imgData, 1024, 1024);
            float[] b = ExtractData(imgData, 2048, 1024);
            float[] r2 = Resize(r, 32, 16);
            float[] g2 = Resize(g, 32, 16);
            float[] b2 = Resize(b, 32, 16);

            nn.Train(r2, r, 0.2f);
            var s=nn.Layers[2].Deltas.Sum(Math.Abs);
            nn.Train(g2, g, 0.2f);
            s += nn.Layers[2].Deltas.Sum(Math.Abs);
            nn.Train(b2, b, 0.2f);
            s += nn.Layers[2].Deltas.Sum(Math.Abs);

            Accuracy = s;

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

    private float[] Resize(float[] r, int v1, int v2)
    {
        throw new NotImplementedException();
    }

    private float[] ExtractData(byte[] imgData, int v1, int v2)
    {
        throw new NotImplementedException();
    }
}
