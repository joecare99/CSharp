using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PdfMarkdownDeterministic.ConsoleApp;

public static class PdfStructureInspector
{
    private static readonly Regex ObjectRegex = new(@"(?ms)(?<id>\d+\s+\d+)\s+obj\s*(?<body>.*?)\s*endobj", RegexOptions.CultureInvariant);
    private static readonly Regex TextOperatorRegex = new(@"\b(?:BT|ET|Tj|TJ)\b", RegexOptions.CultureInvariant);
    private static readonly Regex VectorDrawingRegex = new(@"(?<![A-Za-z])(?:m|l|c|re|S|s|f|F|f\*|B|B\*)\s", RegexOptions.CultureInvariant);
    private static readonly Regex ReferenceRegex = new(@"/(?<value>(?:Im|Fm|X|F)\d+)\s+\d+\s+\d+\s+R", RegexOptions.CultureInvariant);
    private static readonly Regex BaseFontRegex = new(@"/BaseFont\s*/(?<value>[^\s/<>()\[\]]+)", RegexOptions.CultureInvariant);
    private static readonly Regex EncodingRegex = new(@"/Encoding\s*/(?<value>[^\s/<>()\[\]]+)", RegexOptions.CultureInvariant);
    private static readonly Regex FilterRegex = new("/Filter\\s*(?:\\[(?<array>[^\\]]*?)\\]|/(?<value>[^\\s/<>()\\[\\]]+))", RegexOptions.CultureInvariant);
    private static readonly Regex OperatorRegex = new(@"\b(?:BT|ET|Tf|Tj|TJ|Td|TD|Tm|T\*|q|Q|cm|m|l|c|re|h|S|s|f\*|f|B\*|B|n|W\*|W|Do|BI|ID|EI|gs|rg|RG|k|K|w|J|j|M|d|i|ri|Tr|Ts|Tc|Tw|TL|BDC|BMC|EMC|MP|DP|SCN|SC|scn|sc|cs|CS)\b", RegexOptions.CultureInvariant);
    private static readonly HashSet<string> GlyphBoundaryOperators = new(StringComparer.Ordinal) { "q", "Q", "BT", "ET", "BDC", "BMC", "EMC", "Do" };
    private static readonly HashSet<string> GlyphRelevantOperators = new(StringComparer.Ordinal) { "q", "Q", "cm", "m", "l", "c", "re", "h", "S", "s", "f", "f*", "B", "B*", "n", "W", "W*", "Do", "gs", "rg", "RG", "k", "K", "w", "J", "j", "M", "d", "i", "ri", "Tr", "Ts", "Tc", "Tw", "TL" };
    private static readonly Regex MetadataRegex = new(@"/(?<key>Title|Author|Subject|Keywords|Creator|Producer|CreationDate|ModDate)\s*\((?<value>(?:\\.|[^()])*)\)", RegexOptions.CultureInvariant);
    private static readonly Regex TypeRegex = new(@"/Type\s*/(?<value>[A-Za-z0-9]+)", RegexOptions.CultureInvariant);
    private static readonly Regex SubtypeRegex = new(@"/Subtype\s*/(?<value>[A-Za-z0-9]+)", RegexOptions.CultureInvariant);

    public static PdfDocumentInspection Inspect(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        string content = Encoding.Latin1.GetString(bytes);
        IReadOnlyList<PdfObjectSummary> objects = ParseObjects(content);

        Dictionary<string, string> metadata = ExtractMetadata(content, objects);
        List<string> fonts = ExtractDistinctStrings(objects.SelectMany(static obj => new[] { obj.BaseFont, obj.FontName }).Where(static value => !string.IsNullOrWhiteSpace(value))!);
        List<string> encodings = ExtractDistinctStrings(objects.SelectMany(static obj => obj.Encodings).Where(static value => !string.IsNullOrWhiteSpace(value))!);
        List<string> xObjects = ExtractDistinctStrings(objects.SelectMany(static obj => obj.References));
        bool hasTextOperators = objects.Any(static obj => obj.HasTextOperators);
        bool hasVectorDrawingHints = objects.Any(static obj => obj.HasVectorDrawingHints);
        bool hasToUnicodeMap = objects.Any(static obj => obj.HasToUnicodeMap);
        int imageObjectCount = objects.Count(static obj => string.Equals(obj.Type, "XObject", StringComparison.OrdinalIgnoreCase) && string.Equals(obj.Subtype, "Image", StringComparison.OrdinalIgnoreCase));
        int xObjectReferenceCount = CountOccurrences(content, "/XObject");
        int inlineImageMarkerCount = CountOccurrences(content, "BI") + CountOccurrences(content, "EI");
        List<string> contentHints = BuildContentHints(content, objects, hasToUnicodeMap);

        return new PdfDocumentInspection(
            hasTextOperators,
            hasVectorDrawingHints,
            hasToUnicodeMap,
            imageObjectCount,
            xObjectReferenceCount,
            inlineImageMarkerCount,
            metadata,
            fonts,
            encodings,
            xObjects,
            contentHints,
            objects);
    }

    private static IReadOnlyList<PdfObjectSummary> ParseObjects(string content)
    {
        List<PdfObjectSummary> objects = [];
        foreach (Match match in ObjectRegex.Matches(content))
        {
            string objectId = match.Groups["id"].Value;
            string body = match.Groups["body"].Value;
            string? type = MatchSingle(TypeRegex, body);
            string? subtype = MatchSingle(SubtypeRegex, body);
            string? baseFont = MatchSingle(BaseFontRegex, body);
            string? fontName = MatchSingle(new Regex(@"/FontName\s*/(?<value>[^\s/<>()\[\]]+)", RegexOptions.CultureInvariant), body);
            List<string> encodings = ExtractDistinctStrings(ExtractMatches(new[] { EncodingRegex }, body).Select(static match => match.Groups["value"].Value));
            List<string> references = ExtractReferences(body);
            bool hasStream = body.Contains("stream", StringComparison.Ordinal);
            string streamContent = ExtractStreamContent(body);
            string? filter = ExtractFilter(body);
            string decodedStreamContent = DecodeStreamIfPossible(streamContent, filter);
            string analysisStreamContent = string.IsNullOrWhiteSpace(decodedStreamContent) ? streamContent : decodedStreamContent;
            bool hasTextOperators = TextOperatorRegex.IsMatch(analysisStreamContent);
            bool hasVectorDrawingHints = VectorDrawingRegex.IsMatch(analysisStreamContent);
            bool hasToUnicodeMap = body.Contains("/ToUnicode", StringComparison.Ordinal);
            string identifier = DetermineIdentifier(objectId, type, subtype, filter, hasStream, hasTextOperators, hasVectorDrawingHints, analysisStreamContent);
            string? streamKind = DetermineStreamKind(analysisStreamContent, hasTextOperators, hasVectorDrawingHints);
            string? rawStreamPreview = hasStream ? CreateStreamPreview(streamContent) : null;
            string? decodedStreamPreview = string.IsNullOrWhiteSpace(decodedStreamContent) ? null : CreateStreamPreview(decodedStreamContent);
            List<string> operatorSummary = SummarizeOperators(analysisStreamContent);
            List<string> drawingHints = ExtractDrawingHints(analysisStreamContent, operatorSummary);
            List<string> glyphCandidates = ExtractGlyphCandidates(analysisStreamContent);

            objects.Add(new PdfObjectSummary(
                objectId,
                identifier,
                type,
                subtype,
                baseFont,
                fontName,
                encodings,
                hasStream,
                hasTextOperators,
                hasVectorDrawingHints,
                hasToUnicodeMap,
                references,
                filter,
                streamKind,
                string.IsNullOrWhiteSpace(decodedStreamContent) ? null : decodedStreamContent,
                rawStreamPreview,
                decodedStreamPreview,
                operatorSummary,
                drawingHints,
                glyphCandidates)
            {
                RawBody = body,
            });
        }

        return objects;
    }

    private static string ExtractStreamContent(string body)
    {
        int streamStart = body.IndexOf("stream", StringComparison.Ordinal);
        if (streamStart < 0)
        {
            return string.Empty;
        }

        streamStart += "stream".Length;
        while (streamStart < body.Length && (body[streamStart] == '\r' || body[streamStart] == '\n' || body[streamStart] == ' ' || body[streamStart] == '\t'))
        {
            streamStart++;
        }

        int streamEnd = body.IndexOf("endstream", streamStart, StringComparison.Ordinal);
        if (streamEnd < 0 || streamEnd <= streamStart)
        {
            return string.Empty;
        }

        return body.Substring(streamStart, streamEnd - streamStart);
    }

    private static Dictionary<string, string> ExtractMetadata(string content, IReadOnlyList<PdfObjectSummary> objects)
    {
        Dictionary<string, string> metadata = new(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in MetadataRegex.Matches(content))
        {
            string key = match.Groups["key"].Value;
            string value = match.Groups["value"].Value
                .Replace("\\(", "(", StringComparison.Ordinal)
                .Replace("\\)", ")", StringComparison.Ordinal)
                .Replace("\\\\", "\\", StringComparison.Ordinal);
            metadata[key] = value;
        }

        foreach (PdfObjectSummary obj in objects)
        {
            if (obj.Type is null && !obj.HasStream)
            {
                continue;
            }

            // Capture a few common metadata keys from object dictionaries when present.
            foreach (string key in new[] { "Title", "Author", "Subject", "Keywords", "Creator", "Producer", "CreationDate", "ModDate" })
            {
                string marker = $"/{key} ";
                int index = obj.RawBody.IndexOf(marker, StringComparison.Ordinal);
                if (index < 0)
                {
                    continue;
                }

                int valueStart = index + marker.Length;
                if (valueStart >= obj.RawBody.Length || obj.RawBody[valueStart] != '(')
                {
                    continue;
                }

                int valueEnd = FindClosingParen(obj.RawBody, valueStart);
                if (valueEnd > valueStart)
                {
                    metadata[key] = obj.RawBody.Substring(valueStart + 1, valueEnd - valueStart - 1)
                        .Replace("\\(", "(", StringComparison.Ordinal)
                        .Replace("\\)", ")", StringComparison.Ordinal)
                        .Replace("\\\\", "\\", StringComparison.Ordinal);
                }
            }
        }

        return metadata;
    }

    private static List<string> BuildContentHints(string content, IReadOnlyList<PdfObjectSummary> objects, bool hasToUnicodeMap)
    {
        List<string> contentHints = [];
        if (Regex.IsMatch(content, @"\bDo\b", RegexOptions.CultureInvariant))
        {
            contentHints.Add("Do operator present");
        }
        if (Regex.IsMatch(content, @"\bTf\b", RegexOptions.CultureInvariant))
        {
            contentHints.Add("Tf operator present");
        }
        if (Regex.IsMatch(content, @"\bBT\b", RegexOptions.CultureInvariant))
        {
            contentHints.Add("BT/ET text block present");
        }
        if (Regex.IsMatch(content, @"\bBI\b", RegexOptions.CultureInvariant))
        {
            contentHints.Add("Inline image begin marker present");
        }
        if (hasToUnicodeMap)
        {
            contentHints.Add("ToUnicode map present");
        }
        if (objects.Any(static obj => string.Equals(obj.Filter, "FlateDecode", StringComparison.OrdinalIgnoreCase) || (obj.Filter?.Contains("FlateDecode", StringComparison.OrdinalIgnoreCase) ?? false)))
        {
            contentHints.Add("FlateDecode stream present");
        }
        if (objects.Any(static obj => string.Equals(obj.StreamKind, "XML", StringComparison.OrdinalIgnoreCase)))
        {
            contentHints.Add("XML stream present");
        }
        if (objects.Any(static obj => obj.HasVectorDrawingHints))
        {
            contentHints.Add("Vector drawing operators present in stream content");
        }

        return contentHints;
    }

    private static List<string> ExtractReferences(string body) => ExtractDistinctStrings(ExtractMatches(new[] { ReferenceRegex }, body).Select(static match => match.Groups["value"].Value));

    private static IEnumerable<Match> ExtractMatches(IEnumerable<Regex> regexes, string input)
    {
        foreach (Regex regex in regexes)
        {
            foreach (Match match in regex.Matches(input))
            {
                yield return match;
            }
        }
    }

    private static List<string> ExtractDistinctStrings(IEnumerable<string> values)
    {
        HashSet<string> distinctValues = new(StringComparer.Ordinal);
        foreach (string value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                distinctValues.Add(value);
            }
        }

        return distinctValues.OrderBy(static value => value, StringComparer.Ordinal).ToList();
    }

    private static string? MatchSingle(Regex regex, string input)
    {
        Match match = regex.Match(input);
        return match.Success ? match.Groups["value"].Value : null;
    }

    private static int CountOccurrences(string input, string token)
    {
        int count = 0;
        int index = 0;
        while ((index = input.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += token.Length;
        }

        return count;
    }

    private static int FindClosingParen(string input, int openingParenIndex)
    {
        int depth = 0;
        for (int index = openingParenIndex; index < input.Length; index++)
        {
            if (input[index] == '(')
            {
                depth++;
            }
            else if (input[index] == ')')
            {
                depth--;
                if (depth == 0)
                {
                    return index;
                }
            }
        }

        return -1;
    }

    private static string DetermineIdentifier(string objectId, string? type, string? subtype, string? filter, bool hasStream, bool hasTextOperators, bool hasVectorDrawingHints, string streamContent)
    {
        if (!string.IsNullOrWhiteSpace(type) && !string.Equals(type, "unknown", StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(subtype) || string.Equals(subtype, "unknown", StringComparison.OrdinalIgnoreCase)
                ? type
                : $"{type}/{subtype}";
        }

        if (LooksLikeXmlStream(streamContent))
        {
            return $"{objectId} (XML stream)";
        }

        if (!string.IsNullOrWhiteSpace(filter) && filter.Contains("FlateDecode", StringComparison.OrdinalIgnoreCase))
        {
            return $"{objectId} (FlateDecode stream)";
        }

        if (hasStream && hasTextOperators && hasVectorDrawingHints)
        {
            return $"{objectId} (content stream)";
        }

        if (hasStream && hasTextOperators)
        {
            return $"{objectId} (text stream)";
        }

        if (hasStream && hasVectorDrawingHints)
        {
            return $"{objectId} (drawing stream)";
        }

        return objectId;
    }

    private static string? DetermineStreamKind(string streamContent, bool hasTextOperators, bool hasVectorDrawingHints)
    {
        if (LooksLikeXmlStream(streamContent))
        {
            return "XML";
        }

        if (hasTextOperators && hasVectorDrawingHints)
        {
            return "Content";
        }

        if (hasTextOperators)
        {
            return "Text";
        }

        if (hasVectorDrawingHints)
        {
            return "Vector";
        }

        return null;
    }

    private static bool LooksLikeXmlStream(string streamContent)
    {
        if (string.IsNullOrWhiteSpace(streamContent))
        {
            return false;
        }

        string trimmed = streamContent.TrimStart();
        return trimmed.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("<x:xmpmeta", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("<rdf:RDF", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("<", StringComparison.OrdinalIgnoreCase);
    }

    private static string? ExtractFilter(string body)
    {
        Match match = FilterRegex.Match(body);
        if (!match.Success)
        {
            return null;
        }

        if (match.Groups["value"].Success)
        {
            return match.Groups["value"].Value;
        }

        if (!match.Groups["array"].Success)
        {
            return null;
        }

        string[] parts = match.Groups["array"].Value.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.FirstOrDefault(part => part.Contains("FlateDecode", StringComparison.OrdinalIgnoreCase)) ?? match.Groups["array"].Value;
    }

    private static List<string> SummarizeOperators(string streamContent)
    {
        Dictionary<string, int> counts = new(StringComparer.Ordinal);
        foreach (Match match in OperatorRegex.Matches(streamContent))
        {
            string op = match.Value;
            counts[op] = counts.TryGetValue(op, out int count) ? count + 1 : 1;
        }

        return counts
            .OrderByDescending(static pair => pair.Value)
            .ThenBy(static pair => pair.Key, StringComparer.Ordinal)
            .Select(static pair => $"{pair.Key}×{pair.Value}")
            .ToList();
    }

    private static List<string> ExtractDrawingHints(string streamContent, IReadOnlyList<string> operatorSummary)
    {
        List<string> hints = [];
        HashSet<string> operators = new(operatorSummary.Select(static item => item.Split('×')[0]), StringComparer.Ordinal);

        if (operators.Contains("m") || operators.Contains("l") || operators.Contains("c") || operators.Contains("re") || operators.Contains("h"))
        {
            hints.Add("Path construction operators present (possible outline/glyph shapes)");
        }

        if (operators.Contains("f") || operators.Contains("f*") || operators.Contains("S") || operators.Contains("s") || operators.Contains("B") || operators.Contains("B*"))
        {
            hints.Add("Painting operators present (filled/stroked shapes)");
        }

        if (operators.Contains("cm"))
        {
            hints.Add("Transformation operators present (object placement/scaling)");
        }

        if (operators.Contains("q") || operators.Contains("Q"))
        {
            hints.Add("Graphics state save/restore present");
        }

        if (operators.Contains("Do"))
        {
            hints.Add("XObject invocation present");
        }

        if (operators.Contains("BT") || operators.Contains("ET") || operators.Contains("Tf") || operators.Contains("Tj") || operators.Contains("TJ"))
        {
            hints.Add("Text operators present");
        }

        return hints;
    }

    private static List<string> ExtractGlyphCandidates(string streamContent)
    {
        List<string> glyphCandidates = [];
        if (string.IsNullOrWhiteSpace(streamContent))
        {
            return glyphCandidates;
        }

        Dictionary<string, int> signatures = new(StringComparer.Ordinal);
        List<string> block = [];

        void FlushBlock()
        {
            string signature = CreateGlyphSignature(block);
            if (!string.IsNullOrWhiteSpace(signature) && ContainsGlyphLikeOperators(block))
            {
                signatures[signature] = signatures.TryGetValue(signature, out int count) ? count + 1 : 1;
            }

            block.Clear();
        }

        foreach (Match match in OperatorRegex.Matches(streamContent))
        {
            string token = match.Value;
            block.Add(token);

            if (GlyphBoundaryOperators.Contains(token) || token is "f" or "f*" or "S" or "s" or "B" or "B*" or "Do")
            {
                FlushBlock();
            }
        }

        FlushBlock();

        glyphCandidates.AddRange(signatures
            .OrderByDescending(static pair => pair.Value)
            .ThenBy(static pair => pair.Key, StringComparer.Ordinal)
            .Take(20)
            .Select(static pair => $"{pair.Key}×{pair.Value}"));

        return glyphCandidates;
    }

    private static bool ContainsGlyphLikeOperators(IEnumerable<string> operators)
        => operators.Any(static op => op is "m" or "l" or "c" or "re" or "h");

    private static string CreateGlyphSignature(IEnumerable<string> operators)
    {
        List<string> filtered = [];
        string? previous = null;

        foreach (string op in operators)
        {
            if (!GlyphRelevantOperators.Contains(op))
            {
                continue;
            }

            if (string.Equals(previous, op, StringComparison.Ordinal))
            {
                continue;
            }

            filtered.Add(op);
            previous = op;
        }

        string signature = string.Join(" ", filtered);
        return signature.Length <= 120 ? signature : signature[..120] + "…";
    }

    private static string DecodeStreamIfPossible(string streamContent, string? filter)
    {
        if (string.IsNullOrWhiteSpace(streamContent) || string.IsNullOrWhiteSpace(filter))
        {
            return string.Empty;
        }

        if (!filter.Contains("FlateDecode", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        byte[] compressedBytes = Encoding.Latin1.GetBytes(streamContent);
        try
        {
            using MemoryStream input = new(compressedBytes);
            using ZLibStream zlibStream = new(input, CompressionMode.Decompress, leaveOpen: false);
            using MemoryStream output = new();
            zlibStream.CopyTo(output);
            return Encoding.Latin1.GetString(output.ToArray());
        }
        catch
        {
            try
            {
                using MemoryStream input = new(compressedBytes);
                using DeflateStream deflateStream = new(input, CompressionMode.Decompress, leaveOpen: false);
                using MemoryStream output = new();
                deflateStream.CopyTo(output);
                return Encoding.Latin1.GetString(output.ToArray());
            }
            catch
            {
                return string.Empty;
            }
        }
    }

    private static string CreateStreamPreview(string streamContent)
    {
        if (string.IsNullOrWhiteSpace(streamContent))
        {
            return string.Empty;
        }

        string normalized = Regex.Replace(streamContent, @"\s+", " ", RegexOptions.CultureInvariant).Trim();
        return normalized.Length <= 240 ? normalized : normalized[..240] + "…";
    }

    private static string NormalizePdfOperator(string token)
        => token is "BT" or "ET" or "BDC" or "BMC" or "EMC" or "Do" or "q" or "Q"
            ? $"<{token}>"
            : token;

    internal static string ExportDecodedStreamForLlm(PdfObjectSummary pdfObject)
        => pdfObject.DecodedStreamContent ?? string.Empty;
}
