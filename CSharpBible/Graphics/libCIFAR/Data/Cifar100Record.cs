using System;
using System.Collections.Generic;
using System.Text;

namespace libCIFAR.Data;

public class Cifar100Record
{
    public ECifar100Category Label { get; set; }
    public byte SubCategory { get; set; }
    public byte[] ImageData { get; set; } = new byte[3072];

    public void ReadFrom(byte[] buffer, int offset)
    {
        Label = (ECifar100Category)buffer[offset];
        SubCategory = buffer[offset+1];
        Array.Copy(buffer, offset + 2, ImageData, 0, 3072);
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
        Label = (ECifar100Category)stream.ReadByte();    
        SubCategory = (byte)stream.ReadByte();    
        stream.Read(ImageData, 0, 3072);
    }

}
