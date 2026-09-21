using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BaseLib.Models.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.Extensions.Logging;
using OFBCreator.Abstractions.Interfaces;

namespace OFBCreator.Core.Services;

/// <summary>
/// SecureStore data source placeholder.
/// Will read from SecureStore format when the format specification is available.
/// </summary>
public class SecureStoreDataSource : IFamilyDataSource
{
    private readonly ILogger<SecureStoreDataSource>? _logger;

    public string SourceId => "securestore";
    public string DisplayName => "SecureStore Import";

    public SecureStoreDataSource(ILogger<SecureStoreDataSource>? logger = null)
    {
        _logger = logger;
    }

    public bool CanRead(Stream stream)
    {
        // Placeholder: detect SecureStore file signature/header
        return false;
    }

    public async Task<IGenealogy> ImportAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException("SecureStoreDataSource import not yet implemented.");
    }
}
