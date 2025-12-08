using System.Collections.Generic;
using PictureDB.Base.Models;

namespace PictureDB.Base.Services.Interfaces;

public interface IResultStore
{
    void SaveResults(IEnumerable<ImageResult> results, string path);
}
