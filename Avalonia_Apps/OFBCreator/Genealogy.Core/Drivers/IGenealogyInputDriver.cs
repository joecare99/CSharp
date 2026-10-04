using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Genealogy.Models;

namespace Genealogy.Drivers;

/// <summary>
/// Reads a provider format and projects its data into the canonical genealogy model.
/// </summary>
public interface IGenealogyInputDriver
{
    string ProviderId { get; }

    bool CanRead(Stream stream);

    Task<GenealogyDocument> ReadAsync(Stream stream, CancellationToken cancellationToken = default);
}
