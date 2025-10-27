using System.Threading;
using System.Threading.Tasks;
using DataAnalysis.Core.Models;

namespace DataAnalysis.Core.Export;

/// <summary>
/// Abstraktion f�r den Export von Syslog-Analyseergebnissen in beliebige Zielformate.
/// </summary>
public interface ISyslogAnalysisExporter
{
 /// <summary>
 /// Exportiert das Analyseergebnis in ein Ziel (z. B. Datei) und liefert den Pfad/Bezeichner zur�ck.
 /// </summary>
 Task<string> ExportAsync(SyslogAnalysisResult result, string inputPath, string? outputPath, CancellationToken cancellationToken);
}
