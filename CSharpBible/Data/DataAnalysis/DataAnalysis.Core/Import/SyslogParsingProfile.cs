using System;
using System.Collections.Generic;
using DataAnalysis.Core.Models;

namespace DataAnalysis.Core.Import;

/// <summary>
/// Beschreibt, wie Log-Zeilen zu Feldern gemappt werden sollen (Regex, Gruppen, Formate, Aliasse).
/// </summary>
public sealed class SyslogParsingProfile
{
 public string? RegexPattern { get; init; }

 /// <summary>
 /// Mapping kanonischer Felder (Timestamp, Severity, Source, Message) zu Regex-Gruppennamen.
 /// </summary>
 public IDictionary<string, string> FieldGroupMap { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
 {
 ["Timestamp"] = "ts",
 ["Severity"] = "sev",
 ["Source"] = "src",
 ["Message"] = "msg",
 };

 /// <summary>
 /// Kultureinstellungen (Culture-Names) die beim Timestamp-Parsing probiert werden.
 /// </summary>
 public IList<string> Cultures { get; init; } = new List<string> { "", "de-DE", "en-US" }; // "" => InvariantCulture

 /// <summary>
 /// Timestamp-Formate (DateTimeOffset.TryParseExact) die probiert werden.
 /// </summary>
 public IList<string> TimestampFormats { get; init; } = new List<string>
 {
 "yyyy-MM-dd HH:mm:ss",
 "yyyy-MM-dd HH:mm:ss,fff",
 "yyyy-MM-dd HH:mm:ss.fff",
 "dd.MM.yyyy HH:mm:ss",
 "dd.MM.yyyy HH:mm:ss,fff",
 "dd.MM.yyyy HH:mm:ss.fff",
 "yy-MM-dd HH:mm:ss",
 "yy-MM-dd HH:mm:ss,fff",
 "yy-MM-dd HH:mm:ss.fff",
 "yyyy-MM-ddTHH:mm:ssK",
 "yyyy-MM-ddTHH:mm:ss.fffK",
 "yyyy-MM-dd HH:mm:ssK",
 "yyyy-MM-dd HH:mm:ss.fffK",
 };

 /// <summary>
 /// Zuordnung frei definierter Schweregrad-Tokens zu SyslogSeverity.
 /// </summary>
 public IDictionary<string, SyslogSeverity> SeverityAliases { get; init; } = new Dictionary<string, SyslogSeverity>(StringComparer.OrdinalIgnoreCase)
 {
 ["TRACE"] = SyslogSeverity.Trace,
 ["DEBUG"] = SyslogSeverity.Debug,
 ["INFO"] = SyslogSeverity.Info,
 ["WARN"] = SyslogSeverity.Warn,
 ["WARNING"] = SyslogSeverity.Warn,
 ["ERROR"] = SyslogSeverity.Error,
 ["ERR"] = SyslogSeverity.Error,
 ["FATAL"] = SyslogSeverity.Fatal,
 ["ALARM"] = SyslogSeverity.Fatal,
 };

 /// <summary>
 /// Wenn true, wird bei fehlgeschlagener Regex auch heuristisch geparst.
 /// </summary>
 public bool EnableHeuristics { get; init; } = true;

 /// <summary>
 /// Optionale Regeln, um Felder (z. B. Message) per Regex in weitere Attribute zu zerlegen.
 /// </summary>
 public IList<FieldExtractionRule> ExtractionRules { get; init; } = new List<FieldExtractionRule>();
}
