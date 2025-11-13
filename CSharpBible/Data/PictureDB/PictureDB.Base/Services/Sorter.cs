using System.Collections.Generic;
using System.Linq;
using PictureDB.Base.Models;
using PictureDB.Base.Services.Interfaces;

namespace PictureDB.Base.Services;

public class Sorter : ISorter
{
    public IEnumerable<ImageResult> SortByScore(IEnumerable<ImageResult> results)
    {
        return results.OrderByDescending(r => r.Score);
    }
}