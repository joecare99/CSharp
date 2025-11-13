using System;
using System.IO;
using PictureDB.Base.Services.Interfaces;

namespace PictureDB.Base.Services;

public class ImageProcessor : IImageProcessor
{
    public string ConvertToBase64(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        return Convert.ToBase64String(bytes);
    }
}