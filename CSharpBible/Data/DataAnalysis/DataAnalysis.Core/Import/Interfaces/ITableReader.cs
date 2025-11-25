using System.Data;

namespace DataAnalysis.Core.Import.Interfaces;

public interface ITableReader
{
    Task<DataTable> ReadTableAsync(string inputPath, CancellationToken cancellationToken = default, Action<double>? progressCallback = null);
}