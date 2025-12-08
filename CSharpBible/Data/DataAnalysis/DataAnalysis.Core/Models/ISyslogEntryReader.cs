using System.Threading;
using System.Threading.Tasks;
using DataAnalysis.Core.Import;

namespace DataAnalysis.Core.Models;

/// <summary>
/// Abstraktion für das Einlesen von Syslog-Einträgen aus unterschiedlichen Quellen/Formaten.
/// </summary>
public interface ISyslogEntryReader
{
   Task<SyslogReadResult> ReadAsync(string inputPath, CancellationToken cancellationToken = default);
}
