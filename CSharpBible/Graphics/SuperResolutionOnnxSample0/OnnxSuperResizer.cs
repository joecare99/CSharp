using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SuperResolutionOnnxSample;

/// <summary>
/// Minimaler Super-Resolution-Inferenzpfad für das ONNX "super-resolution-10"-Modell (3x Upscaling, Y-Channel input).
/// Quelle: ONNX Model Zoo -> vision/super_resolution/sub_pixel_cnn_2016 (verified/super-resolution-10.onnx)
/// </summary>
public sealed class OnnxSuperResizer : IDisposable
{
    private const string ModelUrl = "https://github.com/onnx/models/raw/main/verified/vision/super_resolution/sub_pixel_cnn_2016/model/super-resolution-10.onnx";
    private const string ModelFileName = "super-resolution-10.onnx";
    private readonly string _modelPath;
    private InferenceSession? _session;

    public OnnxSuperResizer()
    {
        var modelDir = Path.Combine(AppContext.BaseDirectory, "Models");
        Directory.CreateDirectory(modelDir);
        _modelPath = Path.Combine(modelDir, ModelFileName);
    }

    public async Task EnsureModelAsync()
    {
        if (!File.Exists(_modelPath))
        {
            using var http = new HttpClient();
            var data = await http.GetByteArrayAsync(ModelUrl);
            await File.WriteAllBytesAsync(_modelPath, data);
        }
        _session ??= new InferenceSession(_modelPath);
    }

    public async Task<BitmapImage> UpscaleAsync(BitmapImage input)
    {
        await EnsureModelAsync();
        if (_session == null) throw new InvalidOperationException("Session nicht initialisiert.");

        const int targetW = 224;
        const int targetH = 224;
        const int scale = 3; // Modell skaliert 3x

        var resized = ResizeTo(input, targetW, targetH);
        var (yTensor, cb, cr, w, h) = ToTensorYOnly(resized);
        var inputName = _session.InputMetadata.Keys.First();

        var inputValue = NamedOnnxValue.CreateFromTensor(inputName, yTensor);
        using var results = _session.Run(new[] { inputValue });
        var outputY = results.First().AsEnumerable<float>().ToArray();

        int ow = w * scale;
        int oh = h * scale;
        var cbUp = UpscalePlaneNearest(cb, w, h, ow, oh);
        var crUp = UpscalePlaneNearest(cr, w, h, ow, oh);

        var rgb = new byte[ow * oh * 3];
        for (int i = 0; i < ow * oh; i++)
        {
            double y = outputY[i] * 255.0;
            double cbVal = cbUp[i] - 128.0;
            double crVal = crUp[i] - 128.0;

            int r = (int)Math.Round(y + 1.402 * crVal);
            int g = (int)Math.Round(y - 0.344136 * cbVal - 0.714136 * crVal);
            int b = (int)Math.Round(y + 1.772 * cbVal);

            rgb[i * 3] = ClampByte(r);
            rgb[i * 3 + 1] = ClampByte(g);
            rgb[i * 3 + 2] = ClampByte(b);
        }

        var wb = new WriteableBitmap(ow, oh, 96, 96, System.Windows.Media.PixelFormats.Rgb24, null);
        wb.WritePixels(new System.Windows.Int32Rect(0, 0, ow, oh), rgb, ow * 3, 0);
        return ToBitmapImage(wb);
    }

    private static (Tensor<float> yTensor, byte[] cb, byte[] cr, int w, int h) ToTensorYOnly(BitmapSource bmp)
    {
        // Erwartung: Eingabe 1x1xHxW (Y), Output 1x1x(H*3)x(W*3) mit H=W=224
        var src = new FormatConvertedBitmap(bmp, System.Windows.Media.PixelFormats.Rgb24, null, 0);
        int w = src.PixelWidth;
        int h = src.PixelHeight;
        var buffer = new byte[w * h * 3];
        src.CopyPixels(buffer, w * 3, 0);

        var tensor = new DenseTensor<float>(new[] { 1, 1, h, w });
        var cb = new byte[w * h];
        var cr = new byte[w * h];

        int stride = w * 3;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int idx = y * stride + x * 3;
                byte r = buffer[idx];
                byte g = buffer[idx + 1];
                byte b = buffer[idx + 2];

                double yVal = 0.299 * r + 0.587 * g + 0.114 * b;
                double cbVal = 128 - 0.168736 * r - 0.331264 * g + 0.5 * b;
                double crVal = 128 + 0.5 * r - 0.418688 * g - 0.081312 * b;

                tensor[0, 0, y, x] = (float)(yVal / 255.0);
                cb[y * w + x] = ClampByte(cbVal);
                cr[y * w + x] = ClampByte(crVal);
            }
        }

        return (tensor, cb, cr, w, h);
    }

    private static byte[] UpscalePlaneNearest(byte[] plane, int w, int h, int ow, int oh)
    {
        var result = new byte[ow * oh];
        for (int y = 0; y < oh; y++)
        {
            int sy = y * h / oh;
            for (int x = 0; x < ow; x++)
            {
                int sx = x * w / ow;
                result[y * ow + x] = plane[sy * w + sx];
            }
        }
        return result;
    }

    private static BitmapImage ToBitmapImage(WriteableBitmap wb)
    {
        using var ms = new MemoryStream();
        var enc = new PngBitmapEncoder();
        enc.Frames.Add(BitmapFrame.Create(wb));
        enc.Save(ms);
        ms.Position = 0;
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.StreamSource = ms;
        bmp.EndInit();
        bmp.Freeze();
        return bmp;
    }

    private static BitmapSource ResizeTo(BitmapSource source, int width, int height)
    {
        double sx = width / (double)source.PixelWidth;
        double sy = height / (double)source.PixelHeight;
        var tb = new TransformedBitmap(source, new System.Windows.Media.ScaleTransform(sx, sy));
        tb.Freeze();
        return tb;
    }

    private static byte ClampByte(double v)
    {
        if (v < 0) v = 0;
        if (v > 255) v = 255;
        return (byte)v;
    }

    public void Dispose()
    {
        _session?.Dispose();
    }
}
