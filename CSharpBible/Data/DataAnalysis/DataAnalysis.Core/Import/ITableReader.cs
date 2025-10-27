using System.Data;

namespace DataAnalysis.Core.Import;

public interface ITableReader
{
    Task<DataTable> ReadTableAsync(string inputPath, CancellationToken cancellationToken = default);
}