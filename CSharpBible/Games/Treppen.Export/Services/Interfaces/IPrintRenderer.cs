using System.IO;
using Treppen.Base;
using Treppen.Export.Models;

namespace Treppen.Export.Services.Interfaces;

public interface IPrintRenderer
{
    Task RenderAsync(IHeightLabyrinth labyrinth, PrintOptions options, Stream output);
}
