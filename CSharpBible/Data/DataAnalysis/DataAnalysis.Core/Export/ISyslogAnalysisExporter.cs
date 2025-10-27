using System.Threading;
using System.Threading.Tasks;
using DataAnalysis.Core.Models;

namespace DataAnalysis.Core.Export;

/// <summary>
/// Abstraktion für den Export von Syslog-Analyseergebnissen in beliebige Zielformate.
/// </summary>
public interface ISyslogAnalysisExporter
{
 /// <summary>
 /// Exportiert das Analyseergebnis in ein Ziel (z. B. Datei) und liefert den Pfad/Bezeichner zurück.
 /// </summary>
 Task<string> ExportAsync(SyslogAnalysisResult result, string inputPath, string? outputPath, CancellationToken cancellationToken);
}
