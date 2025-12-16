using System;

namespace PictureDB.Base.Models.Interfaces;

public interface IImageResult
{
    public string FilePath { get; set; }
    public string Category { get; set; }
    public int Score { get; set; }
}