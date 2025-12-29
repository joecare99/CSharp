using System;
using System.Collections.Generic;
using System.Text;

namespace libCIFAR.Data;

public class Cifar10Record
{
    public ECifar10Category Label { get; set; }
    public byte[] ImageData { get; set; } = new byte[3072];

    public void ReadFrom(byte[] buffer, int offset)
    {
        Label = (ECifar10Category)buffer[offset];
        Array.Copy(buffer, offset + 1, ImageData, 0, 3072);
    }

    public byte[] GetImageAsRgbArray()
    {
        byte[] rgbImage = new byte[32 * 32 * 3];
        for (int i = 0; i < 32 * 32; i++)
        {
            rgbImage[i * 3] = ImageData[i]; // Red
            rgbImage[i * 3 + 1] = ImageData[i + 1024]; // Green
            rgbImage[i * 3 + 2] = ImageData[i + 2048]; // Blue
        }
        return rgbImage;
    }

    public void ReadFromStream(System.IO.Stream stream)
    {
        Label = (ECifar10Category)stream.ReadByte();    
        stream.Read(ImageData, 0, 3072);
    }

}
