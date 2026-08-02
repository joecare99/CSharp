using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>Writes the legacy-compatible TSV and GEDCOM interchange representations.</summary>
public interface IExportService
{
    Task ExportCsvAsync(string fileName, IReadOnlyCollection<DeathNotice> notices, CancellationToken cancellationToken = default);
    Task ExportGedcomAsync(string fileName, IReadOnlyCollection<DeathNotice> notices, CancellationToken cancellationToken = default);
}
