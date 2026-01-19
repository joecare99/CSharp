using System.IO;
using System.Threading.Tasks;
using Treppen.Base;
using Treppen.Export.Models;
using Treppen.Export.Services.Interfaces;

namespace Treppen.Print;

public sealed class PrintService
{
    private readonly IPrintRenderer _renderer;

    public PrintService(IPrintRenderer renderer)
    {
        _renderer = renderer;
    }

    public Task ExportAsync(IHeightLabyrinth labyrinth, PrintOptions options, Stream output)
        => _renderer.RenderAsync(labyrinth, options, output);
}
