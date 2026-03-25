using CommonDialogs;
using CommonDialogs.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using libCIFAR.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cifar10.WPF.Model;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Cifar10.WPF.ViewModels;

public partial class CifarFilteredViewModel : ObservableObject
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [ObservableProperty]
    private ECifar10Category _selectedCategory;
    private byte[]? data;

    [ObservableProperty]
    private BitmapImage _orginalImage;
    [ObservableProperty]
    private BitmapImage _destImage;
    [ObservableProperty]
    private BitmapImage _genResultImage;

    [ObservableProperty]
    private float _Accuracy;

    private string _statusText = string.Empty;
    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    private readonly MLContext mlContext = new();
    private UpscaleModel? upscaler;

    private const int BatchSize = 100;
    private const int SamplesPerImage = 64;

    public Func<IFileDialog, bool>? showfiledlg { get; set; }


    public ObservableCollection<BitmapImage> Images { get; } = new ObservableCollection<BitmapImage>();

    public CifarFilteredViewModel()
    {
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
            SetProperty(ref _statusText, $"Geladene Samples: {data.Length / 3073}");
        }
    }

    partial void OnSelectedCategoryChanged(ECifar10Category value)
    {
    }

    [RelayCommand]
    private async Task ProcImages()
    {
        if (data == null)
            return;

        var rand = new Random();
        int totalRecords = data.Length / 3073;
        int batchCount = (int)Math.Ceiling(totalRecords / (double)BatchSize);
        if (batchCount == 0)
            return;

        for (int batchIndex = 0; batchIndex < batchCount; batchIndex++)
        {
            int start = batchIndex * BatchSize;
            int count = Math.Min(BatchSize, totalRecords - start);
            if (count <= 0) break;

            var samples = new List<UpscalePixelInput>(count * SamplesPerImage * 3);

            float[]? sampleR = null;
            float[]? sampleG = null;
            float[]? sampleB = null;
            float[]? sampleR16 = null;
            float[]? sampleG16 = null;
            float[]? sampleB16 = null;

            var c10r = new Cifar10Record();
            for (int i = 0; i < count; i++)
            {
                c10r.ReadFrom(data, (start + i) * 3073);

                float[] r = ExtractData(c10r.ImageData, 0, 1024);
                float[] g = ExtractData(c10r.ImageData, 1024, 1024);
                float[] b = ExtractData(c10r.ImageData, 2048, 1024);
                float[] r2 = Resize(r, 32, 16);
                float[] g2 = Resize(g, 32, 16);
                float[] b2 = Resize(b, 32, 16);

                var lowResFeatures = UpscaleModel.MergeLowResChannels(r2, g2, b2);

                for (int s = 0; s < SamplesPerImage; s++)
                {
                    int idx = rand.Next(32 * 32);
                    int px = idx % 32;
                    int py = idx / 32;
                    int hiIdx = py * 32 + px;

                    samples.Add(UpscaleModel.CreateSample(lowResFeatures, px, py, 0, r[hiIdx]));
                    samples.Add(UpscaleModel.CreateSample(lowResFeatures, px, py, 1, g[hiIdx]));
                    samples.Add(UpscaleModel.CreateSample(lowResFeatures, px, py, 2, b[hiIdx]));
                }

                if (sampleR == null || rand.NextDouble() < 1.0 / (i + 1))
                {
                    sampleR = r;
                    sampleG = g;
                    sampleB = b;
                    sampleR16 = r2;
                    sampleG16 = g2;
                    sampleB16 = b2;
                }
            }

            RegressionMetrics? metrics = null;
            upscaler = new UpscaleModel(mlContext);
            await Task.Run(() =>
            {
                metrics = upscaler.Train(samples);
            });

            if (metrics != null)
                Accuracy = (float)metrics.RSquared;

            SetProperty(ref _statusText, $"Batch {batchIndex + 1}/{batchCount} - Records {count}, R²: {Accuracy:F3}");

            if (sampleR != null && sampleG != null && sampleB != null && sampleR16 != null && sampleG16 != null && sampleB16 != null)
            {
                OrginalImage = CreateImage(sampleR, sampleG, sampleB, 32);
                DestImage = CreateImage(sampleR16, sampleG16, sampleB16, 16);

                if (upscaler != null)
                {
                    var lowRes = UpscaleModel.MergeLowResChannels(sampleR16, sampleG16, sampleB16);
                    var (pr, pg, pb) = await Task.Run(() => upscaler.Upscale(lowRes));
                    GenResultImage = CreateImage(pr, pg, pb, 32);
                }
            }

            await Task.Yield();
        }
    }

    public byte[] Combine(float[] r, float[] g, float[] b)
    {
        byte[] rgbImage = new byte[r.Length * 3];
        for (int i = 0; i < r.Length; i++)
        {
            rgbImage[i * 3] = ToByte(r[i]); // Red
            rgbImage[i * 3 + 1] = ToByte(g[i]); // Green
            rgbImage[i * 3 + 2] = ToByte(b[i]); // Blue
        }
        return rgbImage;
    }

    private static BitmapImage WritableBM2BMImage(WriteableBitmap wb)
    {
        var stream = new MemoryStream();
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
        return bitmap;
    }

    private float[] Resize(float[] r, int v1, int v2)
    {
        var result = new float[v2 * v2];
        for (int y = 0; y < v2; y++)
        {
            for (int x = 0; x < v2; x++)
            {
                int srcX = x * v1 / v2;
                int srcY = y * v1 / v2;
                result[y * v2 + x] = 0.25f * (r[srcY * v1 + srcX] + r[srcY * v1 + srcX + 1] + r[srcY * v1 + srcX + v1] + r[srcY * v1 + srcX + v1 + 1]);
            }
        }
        return result;
    }

    private float[] ExtractData(byte[] imgData, int v1, int v2)
    {
        return imgData.Skip(v1).Take(v2).Select(b => (float)b / 255f).ToArray();
    }

    private BitmapImage CreateImage(float[] r, float[] g, float[] b, int size)
    {
        var wb = new WriteableBitmap(size, size, 96, 96, PixelFormats.Rgb24, null);
        wb.WritePixels(new Int32Rect(0, 0, size, size), Combine(r, g, b), size * 3, 0);
        return WritableBM2BMImage(wb);
    }

    private static byte ToByte(float v)
    {
        if (v < 0f) v = 0f;
        if (v > 1f) v = 1f;
        return (byte)(v * 255f);
    }
}
