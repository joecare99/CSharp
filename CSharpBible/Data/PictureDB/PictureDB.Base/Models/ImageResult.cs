using PictureDB.Base.Models.Interfaces;
using System;

namespace PictureDB.Base.Models;

public class ImageResult : IImageResult
{
    public string FilePath { get; set; }
    public string Category { get; set; }
    public int Score { get; set; }
}