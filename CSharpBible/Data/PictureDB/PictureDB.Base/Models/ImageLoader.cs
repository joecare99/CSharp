using System.Collections.Generic;
using System.IO;
using PictureDB.Base.Models.Interfaces;

namespace PictureDB.Base.Models;

public class ImageLoader : IImageLoader
{
    public IEnumerable<string> LoadImages(string folderPath)
    {
        return Directory.GetFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly);
    }
}