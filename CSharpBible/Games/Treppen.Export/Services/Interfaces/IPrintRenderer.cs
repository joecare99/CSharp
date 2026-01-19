using System.IO;
using System.Threading.Tasks;
using Treppen.Base;
using Treppen.Export.Models;

namespace Treppen.Export.Services.Interfaces;

public interface IPrintRenderer
{
    Task RenderAsync(IHeightLabyrinth labyrinth, PrintOptions options, Stream output);
}
