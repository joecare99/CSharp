using System;
using System.Collections.Generic;
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
            bool hasTextOperators = TextOperatorRegex.IsMatch(streamContent);
            bool hasVectorDrawingHints = VectorDrawingRegex.IsMatch(streamContent);
            bool hasToUnicodeMap = body.Contains("/ToUnicode", StringComparison.Ordinal);

            objects.Add(new PdfObjectSummary(
                objectId,
                type,
                subtype,
                baseFont,
                fontName,
                encodings,
                hasStream,
                hasTextOperators,
                hasVectorDrawingHints,
                hasToUnicodeMap,
                references)
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
}
