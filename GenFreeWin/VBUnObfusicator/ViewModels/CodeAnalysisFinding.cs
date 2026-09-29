namespace VBUnObfusicator.ViewModels
{
    public sealed class CodeAnalysisFinding
    {
        public CodeAnalysisFinding(string severity, string code, string summary, int? originalStart, int? originalLength, int? outputStart, int? outputLength)
        {
            Severity = severity;
            Code = code;
            Summary = summary;
            OriginalStart = originalStart;
            OriginalLength = originalLength;
            OutputStart = outputStart;
            OutputLength = outputLength;
        }

        public string Severity { get; }
        public string Code { get; }
        public string Summary { get; }
        public int? OriginalStart { get; }
        public int? OriginalLength { get; }
        public int? OutputStart { get; }
        public int? OutputLength { get; }
    }
}
