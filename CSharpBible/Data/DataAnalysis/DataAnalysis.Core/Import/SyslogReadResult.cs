using System.Collections.Generic;
using DataAnalysis.Core.Models;

namespace DataAnalysis.Core.Import;

public sealed class SyslogReadResult
{
 public required int TotalLines { get; init; }
 public required IList<SyslogEntry> Entries { get; init; } = new List<SyslogEntry>();
}
