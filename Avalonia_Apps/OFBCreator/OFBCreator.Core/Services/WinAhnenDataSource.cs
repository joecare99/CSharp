using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BaseLib.Models.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.Extensions.Logging;
using OFBCreator.Abstractions.Interfaces;

namespace OFBCreator.Core.Services;

/// <summary>
/// WinAhnen-specific data source placeholder.
/// Will read from WinAhnen's proprietary format when the format specification is available.
/// </summary>
public class WinAhnenDataSource : IFamilyDataSource
{
    private readonly ILogger<WinAhnenDataSource>? _logger;

    public string SourceId => "winahnen";
    public string DisplayName => "WinAhnen Import";

    public WinAhnenDataSource(ILogger<WinAhnenDataSource>? logger = null)
    {
        _logger = logger;
    }

    public bool CanRead(Stream stream)
    {
        // Placeholder: detect WinAhnen file signature/header
        return false;
    }

    public async Task<IGenealogy> ImportAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException("WinAhnenDataSource import not yet implemented.");
    }
}
