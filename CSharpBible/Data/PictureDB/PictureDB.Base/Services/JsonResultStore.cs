using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using PictureDB.Base.Models;
using PictureDB.Base.Services.Interfaces;

namespace PictureDB.Base.Services;

public class JsonResultStore : IResultStore
{
    public void SaveResults(IEnumerable<ImageResult> results, string path)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(results, options);
        File.WriteAllText(path, json);
    }
}
