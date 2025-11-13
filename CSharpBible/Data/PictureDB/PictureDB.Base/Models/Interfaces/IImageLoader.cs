using System.Collections.Generic;

namespace PictureDB.Base.Models.Interfaces;

public interface IImageLoader
{
    IEnumerable<string> LoadImages(string folderPath);
}