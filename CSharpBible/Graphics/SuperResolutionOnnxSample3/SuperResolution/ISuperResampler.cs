using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SuperResolutionOnnxSample.SuperResolution;

public interface ISuperResampler
{
    int Scale { get; }
    int InputSize { get; }

    Task<byte[]> UpscaleRgb24TileAsync(BitmapSource tileRgb24); // returns RGB24 bytes (InputSize*Scale)
}
