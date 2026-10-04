using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genealogy.Models;

namespace Genealogy.Drivers;

/// <summary>
/// Writes a canonical genealogy document to a provider format.
/// </summary>
public interface IGenealogyOutputDriver
{
    string ProviderId { get; }

    Task WriteAsync(
        GenealogyDocument document,
        Stream destination,
        GenealogyOutputOptions options,
        CancellationToken cancellationToken = default);
}
