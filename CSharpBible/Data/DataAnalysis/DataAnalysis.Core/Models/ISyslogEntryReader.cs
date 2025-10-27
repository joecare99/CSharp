using System.Threading;
using System.Threading.Tasks;
using DataAnalysis.Core.Import;

namespace DataAnalysis.Core.Models;

/// <summary>
/// Abstraktion f�r das Einlesen von Syslog-Eintr�gen aus unterschiedlichen Quellen/Formaten.
/// </summary>
public interface ISyslogEntryReader
{
   Task<SyslogReadResult> ReadAsync(string inputPath, CancellationToken cancellationToken = default);
}
