using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace DataAnalysis.Core.Export.Interfaces;

public interface ITableExporter
{
 Task<string> ExportAsync(DataTable table, string inputPath, string? outputPath, CancellationToken cancellationToken = default, Action<double>? progressCallback = null);
}
