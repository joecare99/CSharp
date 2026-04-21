using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace TraceCsv2realCsv
{
    /// <summary>
    /// Creates localized help text for the console converter entry point.
    /// </summary>
    public sealed class TraceCsv2realCsvHelpTextProvider
    {
        /// <summary>
        /// Builds a compact localized usage description.
        /// </summary>
        /// <param name="conversionService">Conversion service that provides the loaded filter metadata.</param>
        /// <param name="uiCulture">UI culture used for the description.</param>
        /// <returns>Localized help text.</returns>
        public string BuildHelpText(TraceCsvConversionService conversionService, CultureInfo? uiCulture = null)
        {
            var effectiveCulture = uiCulture ?? CultureInfo.CurrentUICulture;
            return IsGerman(effectiveCulture)
                ? BuildGermanHelpText(conversionService)
                : BuildEnglishHelpText(conversionService);
        }

        private static bool IsGerman(CultureInfo culture)
        {
            return string.Equals(culture.TwoLetterISOLanguageName, "de", System.StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildGermanHelpText(TraceCsvConversionService conversionService)
        {
            var builder = new StringBuilder();
            builder.AppendLine("TraceCsv2realCsv konvertiert unterstützte Trace-Eingaben in eine flache CSV-Ausgabe.");
            builder.AppendLine("Aufruf: TraceCsv2realCsv <Eingabedatei> [Ausgabedatei]");
            builder.AppendLine("Wenn keine Ausgabedatei angegeben ist, wird der Eingabedateiname übernommen und die Endung auf .csv gesetzt.");
            builder.AppendLine("Die Endung .trace.csv wird dabei zu .csv verkürzt.");
            AppendFilterList(builder, "Geladene Input-Filter", conversionService.LoadedInputFilterIds);
            AppendFilterList(builder, "Geladene Output-Filter", conversionService.LoadedOutputFilterNames);
            return builder.ToString();
        }

        private static string BuildEnglishHelpText(TraceCsvConversionService conversionService)
        {
            var builder = new StringBuilder();
            builder.AppendLine("TraceCsv2realCsv converts supported trace inputs to flat CSV output.");
            builder.AppendLine("Usage: TraceCsv2realCsv <input-file> [output-file]");
            builder.AppendLine("If no output file is specified, the input file name is reused and the extension is changed to .csv.");
            builder.AppendLine("The extension .trace.csv is shortened to .csv.");
            AppendFilterList(builder, "Loaded input filters", conversionService.LoadedInputFilterIds);
            AppendFilterList(builder, "Loaded output filters", conversionService.LoadedOutputFilterNames);
            return builder.ToString();
        }

        private static void AppendFilterList(StringBuilder builder, string heading, IEnumerable<string> filters)
        {
            builder.AppendLine($"{heading}:");
            foreach (var filter in filters)
                builder.AppendLine($"- {filter}");
        }
    }
}
