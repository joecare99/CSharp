using System.IO;

public class MnistReader
{
    public static IEnumerable<MnistImage> ReadTrainingData(string imagesPath, string labelsPath)
    {
        using var imgStream = new FileStream(imagesPath, FileMode.Open);
        using var lblStream = new FileStream(labelsPath, FileMode.Open);
        using var imgBr = new BinaryReader(imgStream);
        using var lblBr = new BinaryReader(lblStream);

        // Header überspringen (Magic Numbers)
        imgBr.ReadInt32(); // Magic
        int numImages = ReadBigEndianInt32(imgBr);
        int rows = ReadBigEndianInt32(imgBr);
        int cols = ReadBigEndianInt32(imgBr);

        lblBr.ReadInt32(); // Magic
        lblBr.ReadInt32(); // Count

        for (int i = 0; i < numImages; i++)
        {
            float[] pixels = new float[rows * cols];
            for (int p = 0; p < pixels.Length; p++)
            {
                // Normalisierung: 0-255 -> 0.0-1.0
                pixels[p] = imgBr.ReadByte() / 255.0f;
            }

            int label = lblBr.ReadByte();
            float[] target = new float[10];
            target[label] = 1.0f; // One-Hot Encoding

            yield return new MnistImage { Data = pixels, Label = target, ActualDigit = label };
        }
    }

    // MNIST nutzt Big-Endian, C# Standard ist Little-Endian
    private static int ReadBigEndianInt32(BinaryReader br)
    {
        var bytes = br.ReadBytes(4);
        Array.Reverse(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }
}

public struct MnistImage
{
    public float[] Data;   // Die 784 Pixel
    public float[] Label;  // Das Ziel-Array (z.B. [0,0,1,0...])
    public int ActualDigit; // Die echte Ziffer für die Anzeige
}