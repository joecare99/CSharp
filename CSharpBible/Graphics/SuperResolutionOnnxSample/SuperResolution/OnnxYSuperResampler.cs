using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SuperResolutionOnnxSample.ImageTiling;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SuperResolutionOnnxSample.SuperResolution;

public sealed class OnnxYSuperResampler : ISuperResampler, IDisposable
{
    private const string ModelUrl = "https://media.githubusercontent.com/media/onnx/models/refs/heads/main/validated/vision/super_resolution/sub_pixel_cnn_2016/model/super-resolution-10.onnx?download=true";
    private const string ModelFileName = "super-resolution-10.onnx";

    public int Scale => 3;
    public int InputSize => 224;

    private readonly string _modelPath;
    private InferenceSession? _session;

    public OnnxYSuperResampler()
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

    public async Task<byte[]> UpscaleRgb24TileAsync(BitmapSource tileRgb24)
    {
        await EnsureModelAsync();
        if (_session == null) throw new InvalidOperationException("Session nicht initialisiert.");

        tileRgb24 = WpfBitmapHelpers.EnsureRgb24(tileRgb24);
        if (tileRgb24.PixelWidth != InputSize || tileRgb24.PixelHeight != InputSize)
            throw new InvalidOperationException($"Tile muss {InputSize}x{InputSize} sein.");

        var (yTensor, cb, cr) = ToTensorYOnly(tileRgb24);
        var inputName = _session.InputMetadata.Keys.First();

        var inputValue = NamedOnnxValue.CreateFromTensor(inputName, yTensor);
        using var results = _session.Run(new[] { inputValue });
        var outputY = results.First().AsEnumerable<float>().ToArray();

        int ow = InputSize * Scale;
        int oh = InputSize * Scale;
        var cbUp = UpscalePlaneNearest(cb, InputSize, InputSize, ow, oh);
        var crUp = UpscalePlaneNearest(cr, InputSize, InputSize, ow, oh);

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

        return rgb;
    }

    public async Task<byte[]> UpscaleBgra32TileAsync(BitmapSource tileBgra32)
    {
        await EnsureModelAsync();
        if (_session == null) throw new InvalidOperationException("Session nicht initialisiert.");

        tileBgra32 = WpfBitmapHelpers.EnsureBgra32(tileBgra32);
        if (tileBgra32.PixelWidth != InputSize || tileBgra32.PixelHeight != InputSize)
            throw new InvalidOperationException($"Tile muss {InputSize}x{InputSize} sein.");

        var (yTensor, cb, cr, aTensor) = ToTensorYAndAlpha(tileBgra32);
        var inputName = _session.InputMetadata.Keys.First();

        var inputY = NamedOnnxValue.CreateFromTensor(inputName, yTensor);
        using var resultsY = _session.Run(new[] { inputY });
        var outputY = resultsY.First().AsEnumerable<float>().ToArray();

        var inputA = NamedOnnxValue.CreateFromTensor(inputName, aTensor);
        using var resultsA = _session.Run(new[] { inputA });
        var outputA = resultsA.First().AsEnumerable<float>().ToArray();

        int ow = InputSize * Scale;
        int oh = InputSize * Scale;
        var cbUp = UpscalePlaneNearest(cb, InputSize, InputSize, ow, oh);
        var crUp = UpscalePlaneNearest(cr, InputSize, InputSize, ow, oh);

        var bgra = new byte[ow * oh * 4];
        for (int i = 0; i < ow * oh; i++)
        {
            double y = outputY[i] * 255.0;
            double cbVal = cbUp[i] - 128.0;
            double crVal = crUp[i] - 128.0;

            int r = (int)Math.Round(y + 1.402 * crVal);
            int g = (int)Math.Round(y - 0.344136 * cbVal - 0.714136 * crVal);
            int b = (int)Math.Round(y + 1.772 * cbVal);

            double a = outputA[i] * 255.0;

            int o = i * 4;
            bgra[o] = ClampByte(b);
            bgra[o + 1] = ClampByte(g);
            bgra[o + 2] = ClampByte(r);
            bgra[o + 3] = ClampByte(a);
        }

        return bgra;
    }

    private static (Tensor<float> yTensor, byte[] cb, byte[] cr) ToTensorYOnly(BitmapSource src)
    {
        int w = src.PixelWidth;
        int h = src.PixelHeight;
        var buffer = WpfBitmapHelpers.CopyRgb24Bytes(src);

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

        return (tensor, cb, cr);
    }

    private static (Tensor<float> yTensor, byte[] cb, byte[] cr, Tensor<float> aTensor) ToTensorYAndAlpha(BitmapSource src)
    {
        int w = src.PixelWidth;
        int h = src.PixelHeight;
        var buffer = WpfBitmapHelpers.CopyBgra32Bytes(src);

        var yTensor = new DenseTensor<float>(new[] { 1, 1, h, w });
        var aTensor = new DenseTensor<float>(new[] { 1, 1, h, w });
        var cb = new byte[w * h];
        var cr = new byte[w * h];

        int stride = w * 4;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int idx = y * stride + x * 4;
                byte b = buffer[idx];
                byte g = buffer[idx + 1];
                byte r = buffer[idx + 2];
                byte a = buffer[idx + 3];

                double yVal = 0.299 * r + 0.587 * g + 0.114 * b;
                double cbVal = 128 - 0.168736 * r - 0.331264 * g + 0.5 * b;
                double crVal = 128 + 0.5 * r - 0.418688 * g - 0.081312 * b;

                yTensor[0, 0, y, x] = (float)(yVal / 255.0);
                aTensor[0, 0, y, x] = (float)(a / 255.0);
                cb[y * w + x] = ClampByte(cbVal);
                cr[y * w + x] = ClampByte(crVal);
            }
        }

        return (yTensor, cb, cr, aTensor);
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

    private static byte ClampByte(double v)
    {
        if (v < 0) v = 0;
        if (v > 255) v = 255;
        return (byte)v;
    }

    public void Dispose() => _session?.Dispose();
}
