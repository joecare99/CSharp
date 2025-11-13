using System.Collections.Generic;
using PictureDB.Base.Models;

namespace PictureDB.Base.Services.Interfaces;

public interface ISorter
{
    IEnumerable<ImageResult> SortByScore(IEnumerable<ImageResult> results);
}