using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Genealogy.Drivers;
using Genealogy.Gedcom.Models;
using Genealogy.Models;

namespace Genealogy.Gedcom;

/// <summary>
/// Reads GEDCOM line records into the provider-independent genealogy model.
/// The original ordered syntax tree is retained as opaque provider data for roundtrip fidelity.
/// </summary>
public sealed class GedcomInputDriver : IGenealogyInputDriver
{
    private const string ProviderDataMediaType = "application/vnd.genealogy.gedcom-syntax+json";
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);
    private static readonly UTF8Encoding ReplacementUtf8 = new(false, false);
    private static readonly Encoding Windows1252 = CreateWindows1252();

    public string ProviderId => "gedcom";

    public bool CanRead(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead || !stream.CanSeek)
            return false;

        var originalPosition = stream.Position;
        try
        {
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8, true, 256, true);
            var firstLine = reader.ReadLine();
            return firstLine?.TrimStart('\uFEFF', ' ', '\t').StartsWith("0 HEAD", StringComparison.OrdinalIgnoreCase) == true;
        }
        finally
        {
            stream.Position = originalPosition;
        }
    }

    public async Task<GenealogyDocument> ReadAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead)
            throw new ArgumentException("The input stream must be readable.", nameof(stream));

        using var bytes = new MemoryStream();
        await stream.CopyToAsync(bytes, cancellationToken).ConfigureAwait(false);
        var data = bytes.ToArray();
        var document = new GenealogyDocument();
        var text = Decode(data, document);
        var syntax = ParseSyntax(text, document, cancellationToken);
        ValidateBoundaries(syntax, document);
        syntax.Version = DetectVersion(syntax.Records);
        var sourceRecords = new Dictionary<Guid, GedcomSyntaxNode>();
        if (syntax.Version is null)
        {
            AddRecoveryDiagnostic(document, "GEDCOM_VERSION_MISSING",
                "The GEDCOM header does not declare a recognized version.");
        }
        else if (syntax.Version != "5.5" && syntax.Version != GedcomVersion.Gedcom551
            && syntax.Version != GedcomVersion.Gedcom70)
        {
            AddRecoveryDiagnostic(document, "GEDCOM_VERSION_UNSUPPORTED",
                $"GEDCOM version '{syntax.Version}' is not supported by this input driver.");
        }

        foreach (var root in syntax.Records)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ProjectRecord(root, document, sourceRecords);
        }
        cancellationToken.ThrowIfCancellationRequested();
        ProjectAssociations(document, sourceRecords);

        syntax.HadRecoveryIssues = document.HasRecoveryIssues;
        var providerData = new GenealogyProviderData
        {
            Provider = ProviderId,
            MediaType = ProviderDataMediaType,
            Data = JsonSerializer.SerializeToUtf8Bytes(syntax)
        };
        providerData.CanonicalHash = GenealogyModelFingerprint.Create(document);
        document.ProviderData = providerData;
        return document;
    }

    private static string Decode(byte[] bytes, GenealogyDocument document)
    {
        try
        {
            return StrictUtf8.GetString(bytes).TrimStart('\uFEFF');
        }
        catch (DecoderFallbackException)
        {
            var declaredCharacterSet = DetectDeclaredCharacterSet(bytes);
            if (declaredCharacterSet is "ANSI" or "ASCII")
            {
                AddRecoveryDiagnostic(document, "GEDCOM_LEGACY_ENCODING",
                    $"The file declares CHAR {declaredCharacterSet} but contains bytes that are not valid UTF-8. "
                    + "The content was decoded as Windows-1252; verify the resulting text.");
                return Windows1252.GetString(bytes).TrimStart('\uFEFF');
            }

            AddRecoveryDiagnostic(document, "GEDCOM_INVALID_UTF8",
                "The input contains invalid UTF-8 byte sequences; invalid bytes were replaced during recovery.");
            return ReplacementUtf8.GetString(bytes).TrimStart('\uFEFF');
        }
    }

    private static Encoding CreateWindows1252()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        return Encoding.GetEncoding(1252);
    }

    private static string? DetectDeclaredCharacterSet(byte[] bytes)
    {
        var header = Encoding.ASCII.GetString(bytes, 0, Math.Min(bytes.Length, 8192));
        foreach (var line in header.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None))
        {
            const string prefix = "1 CHAR ";
            if (line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return line[prefix.Length..].Trim().ToUpperInvariant();
        }

        return null;
    }

    private static GedcomSyntaxDocument ParseSyntax(
        string text,
        GenealogyDocument document,
        CancellationToken cancellationToken)
    {
        var syntax = new GedcomSyntaxDocument();
        var stack = new Stack<GedcomSyntaxNode>();
        var lines = text.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
        for (var index = 0; index < lines.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var line = lines[index];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (!TryParseLine(line, out var node))
            {
                AddRecoveryDiagnostic(document, "GEDCOM_MALFORMED_LINE",
                    "The line could not be parsed and was skipped.", index + 1);
                continue;
            }
            node.SourceLineNumber = index + 1;

            while (stack.Count > 0 && stack.Peek().Level >= node.Level)
                stack.Pop();

            if (node.Level == 0)
            {
                syntax.Records.Add(node);
            }
            else if (stack.Count == 0)
            {
                syntax.Records.Add(node);
                AddRecoveryDiagnostic(document, "GEDCOM_ORPHAN_LEVEL",
                    "A non-root record has no level-zero parent; it was retained as a root for recovery.", index + 1);
            }
            else
            {
                var parent = stack.Peek();
                parent.Children.Add(node);
                if (node.Level > parent.Level + 1)
                {
                    AddRecoveryDiagnostic(document, "GEDCOM_LEVEL_GAP",
                        "The record level skips one or more nesting levels; its source level and content were retained.", index + 1);
                }
            }

            stack.Push(node);
        }

        if (syntax.Records.Count == 0)
            throw new InvalidDataException("The input does not contain any parseable GEDCOM records.");

        return syntax;
    }

    private static void ValidateBoundaries(GedcomSyntaxDocument syntax, GenealogyDocument document)
    {
        var headers = syntax.Records.FindAll(record =>
            string.Equals(record.Tag, "HEAD", StringComparison.OrdinalIgnoreCase));
        var trailers = syntax.Records.FindAll(record =>
            string.Equals(record.Tag, "TRLR", StringComparison.OrdinalIgnoreCase));
        if (headers.Count != 1)
            AddRecoveryDiagnostic(document, "GEDCOM_HEADER_COUNT",
                $"Expected exactly one HEAD record but found {headers.Count}.");
        if (trailers.Count != 1)
            AddRecoveryDiagnostic(document, "GEDCOM_TRAILER_COUNT",
                $"Expected exactly one TRLR record but found {trailers.Count}.");
        if (syntax.Records.Count > 0 && !string.Equals(syntax.Records[0].Tag, "HEAD", StringComparison.OrdinalIgnoreCase))
            AddRecoveryDiagnostic(document, "GEDCOM_HEADER_POSITION",
                "The HEAD record is not the first root record.");
        if (syntax.Records.Count > 0 && !string.Equals(syntax.Records[^1].Tag, "TRLR", StringComparison.OrdinalIgnoreCase))
            AddRecoveryDiagnostic(document, "GEDCOM_TRAILER_POSITION",
                "The TRLR record is not the last root record.");
    }

    private static bool TryParseLine(string line, out GedcomSyntaxNode node)
    {
        node = new GedcomSyntaxNode();
        var span = line.AsSpan();
        var position = 0;
        while (position < span.Length && char.IsDigit(span[position]))
            position++;
        if (position == 0 || position >= span.Length || span[position] != ' '
            || !int.TryParse(span[..position], out var level))
            return false;

        position++;
        string? xref = null;
        if (position < span.Length && span[position] == '@')
        {
            var closing = span[(position + 1)..].IndexOf('@');
            if (closing < 1)
                return false;
            var xrefLength = closing + 2;
            xref = span.Slice(position, xrefLength).ToString();
            position += xrefLength;
            if (position >= span.Length || span[position] != ' ')
                return false;
            position++;
        }

        var tagStart = position;
        while (position < span.Length && (char.IsLetterOrDigit(span[position]) || span[position] == '_'))
            position++;
        if (position == tagStart)
            return false;

        var tag = span[tagStart..position].ToString();
        var value = string.Empty;
        if (position < span.Length)
        {
            if (span[position] != ' ')
                return false;
            value = span[(position + 1)..].ToString();
        }

        node = new GedcomSyntaxNode
        {
            Level = level,
            CrossReference = xref,
            Tag = tag,
            Value = value
        };
        return true;
    }

    private static string? DetectVersion(IReadOnlyList<GedcomSyntaxNode> records)
    {
        foreach (var header in records)
        {
            if (!string.Equals(header.Tag, "HEAD", StringComparison.OrdinalIgnoreCase))
                continue;
            foreach (var gedc in header.Children)
            {
                if (!string.Equals(gedc.Tag, "GEDC", StringComparison.OrdinalIgnoreCase))
                    continue;
                foreach (var version in gedc.Children)
                {
                    if (string.Equals(version.Tag, "VERS", StringComparison.OrdinalIgnoreCase))
                        return version.Value.Trim();
                }
            }
        }
        return null;
    }

    private static void ProjectRecord(
        GedcomSyntaxNode syntax,
        GenealogyDocument document,
        IDictionary<Guid, GedcomSyntaxNode> sourceRecords)
    {
        if (!TryGetRecordKind(syntax.Tag, out var kind) && syntax.CrossReference is null)
        {
            document.OtherContent.Add(ProjectNode(syntax, "Document"));
            return;
        }
        kind = kind.Length == 0 ? "Other" : kind;

        var record = CreateRecord(kind, syntax.Tag);
        if (syntax.CrossReference is not null)
        {
            record.Identifiers.Add(new GenealogyIdentifier
            {
                Provider = "gedcom",
                Value = syntax.CrossReference
            });
        }

        foreach (var child in syntax.Children)
        {
            var node = ProjectNode(child, syntax.Tag);
            record.Content.Add(node);
            if (string.Equals(child.Tag, "NAME", StringComparison.OrdinalIgnoreCase))
                ProjectName(child, record);
            else if (string.Equals(child.Tag, "SEX", StringComparison.OrdinalIgnoreCase))
                record.Sex = child.Value;
        }

        record.DisplayName ??= record.Kind == "Person"
            ? string.Join(" ", new[] { record.GivenName, record.Surname }.WhereNotNullOrWhiteSpace())
            : null;
        document.Records.Add(record);
        sourceRecords.Add(record.Id, syntax);
    }

    private static GenealogyRecord CreateRecord(string kind, string tag)
    {
        return kind switch
        {
            "Person" => new GenealogyPerson(),
            "Family" => new GenealogyFamily(),
            "Source" => new GenealogySource(),
            "Repository" => new GenealogyRepository(),
            "Media" => new GenealogyMedia(),
            "Note" => new GenealogyNote(),
            "Place" => new GenealogyPlace(),
            _ => new GenealogyRecord { Kind = kind, TypeCode = tag }
        };
    }

    private static void ProjectAssociations(
        GenealogyDocument document,
        IReadOnlyDictionary<Guid, GedcomSyntaxNode> sourceRecords)
    {
        var recordsByXref = new Dictionary<string, (GenealogyRecord Record, GedcomSyntaxNode Syntax)>(StringComparer.Ordinal);
        foreach (var record in document.Records)
        {
            foreach (var identifier in record.Identifiers)
            {
                if (identifier.Provider == "gedcom")
                {
                    var syntax = sourceRecords[record.Id];
                    if (!recordsByXref.TryAdd(identifier.Value, (record, syntax)))
                    {
                        var firstDefinition = recordsByXref[identifier.Value];
                        AddRecoveryDiagnostic(document, "GEDCOM_DUPLICATE_XREF",
                            $"Cross-reference '{identifier.Value}' is defined more than once: "
                            + $"{firstDefinition.Syntax.Tag} at line {firstDefinition.Syntax.SourceLineNumber} "
                            + $"and {syntax.Tag} at line {syntax.SourceLineNumber}. "
                            + "The first definition remains the reference target.",
                            syntax.SourceLineNumber,
                            identifier.Value);
                    }
                }
            }
        }

        foreach (var record in document.Records)
        foreach (var node in record.Content)
            ProjectAssociations(record, node, recordsByXref, document);
    }

    private static bool IsXref(string value) =>
        value.Length > 2 && value[0] == '@' && value[^1] == '@';

    private static void ProjectAssociations(
        GenealogyRecord owner,
        GenealogyNode node,
        IReadOnlyDictionary<string, (GenealogyRecord Record, GedcomSyntaxNode Syntax)> recordsByXref,
        GenealogyDocument document)
    {
        var reference = node.References.FirstOrDefault(identifier => identifier.Provider == "gedcom")?.Value;
        if (reference is null && IsXref(node.Value))
            reference = node.Value;

        if (reference is not null)
        {
            var upperTypeCode = node.TypeCode.ToUpperInvariant();
            var role = (owner.TypeCode?.ToUpperInvariant(), upperTypeCode) switch
            {
                ("FAM", "HUSB") => "Husband",
                ("FAM", "WIFE") => "Wife",
                ("FAM", "CHIL") => "Child",
                ("INDI", "FAMC") => "ChildInFamily",
                ("INDI", "FAMS") => "PartnerInFamily",
                ("INDI", "ASSO") => node.Children.FirstOrDefault(child =>
                    child.TypeCode.Equals("RELA", StringComparison.OrdinalIgnoreCase))?.Value ?? "Associated",
                _ => null
            };
            if (role is null)
            {
                foreach (var child in node.Children)
                    ProjectAssociations(owner, child, recordsByXref, document);
                return;
            }

            var hasTarget = recordsByXref.TryGetValue(reference, out var target);
            owner.Associations.Add(new GenealogyAssociation
            {
                SourceRecordId = owner.Id,
                TargetRecordId = hasTarget ? target.Record.Id : null,
                TargetIdentifier = new GenealogyIdentifier { Provider = "gedcom", Value = reference },
                Role = role,
                Detail = node.Children.FirstOrDefault(child =>
                    child.TypeCode.Equals("NOTE", StringComparison.OrdinalIgnoreCase))?.Value
            });
            if (!hasTarget)
            {
                AddRecoveryDiagnostic(document, "GEDCOM_UNRESOLVED_REFERENCE",
                    $"Cross-reference '{reference}' has no corresponding record.");
            }
        }

        foreach (var child in node.Children)
            ProjectAssociations(owner, child, recordsByXref, document);
    }

    private static GenealogyNode ProjectNode(GedcomSyntaxNode syntax, string context)
    {
        GenealogyNode node = IsKnownStandardTag(syntax.Tag)
            ? new GenealogyFact()
            : new UserDefinedEvent { Context = context };
        node.TypeCode = syntax.Tag;
        node.Value = syntax.Value;
        var reference = syntax.CrossReference ?? (IsXref(syntax.Value) ? syntax.Value : null);
        if (reference is not null)
        {
            node.References.Add(new GenealogyIdentifier
            {
                Provider = "gedcom",
                Value = reference
            });
        }
        foreach (var child in syntax.Children)
            node.Children.Add(ProjectNode(child, syntax.Tag));
        return node;
    }

    private static void ProjectName(GedcomSyntaxNode name, GenealogyRecord record)
    {
        record.DisplayName = name.Value.Replace("/", string.Empty, StringComparison.Ordinal).Trim();
        foreach (var child in name.Children)
        {
            if (string.Equals(child.Tag, "GIVN", StringComparison.OrdinalIgnoreCase))
                record.GivenName = child.Value;
            else if (string.Equals(child.Tag, "SURN", StringComparison.OrdinalIgnoreCase))
                record.Surname = child.Value;
        }
        if (record.GivenName is null || record.Surname is null)
        {
            var firstDelimiter = name.Value.IndexOf('/');
            var lastDelimiter = name.Value.LastIndexOf('/');
            if (firstDelimiter >= 0 && lastDelimiter > firstDelimiter)
            {
                record.GivenName ??= name.Value[..firstDelimiter].Trim();
                record.Surname ??= name.Value[(firstDelimiter + 1)..lastDelimiter].Trim();
            }
            else
            {
                record.GivenName ??= name.Value.Trim();
            }
        }
    }

    private static bool TryGetRecordKind(string tag, out string kind)
    {
        kind = tag.ToUpperInvariant() switch
        {
            "INDI" => "Person",
            "FAM" => "Family",
            "SOUR" => "Source",
            "REPO" => "Repository",
            "OBJE" => "Media",
            "NOTE" => "Note",
            "PLAC" => "Place",
            _ => string.Empty
        };
        return kind.Length != 0;
    }

    private static bool IsKnownStandardTag(string tag)
    {
        return StandardTags.Contains(tag);
    }

    private static readonly HashSet<string> StandardTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "ABBR", "ADDR", "ADR1", "ADR2", "ADOP", "AFN", "AGE", "AGNC", "ALIA", "ANCE", "ANCI", "ANUL",
        "ASSO", "AUTH",         "BAPL", "BAPM", "BARM", "BASM", "BIRT", "BLES", "BLOB", "BURI", "CALN", "CAST",
        "CAUS", "CEME", "CENS", "CHAN", "CHAR", "CHIL", "CHRA", "CITY", "CONC", "CONF", "COPR", "CORP",
        "CREM", "CTRY", "DATA", "DATE", "DEAT", "DESC", "DESI", "DEST", "DIV", "DIVF", "DSCR", "EDUC",
        "EMAIL", "EMIG", "ENDL", "ENGA", "EVEN", "FAM", "FAMC", "FAMF", "FAMS", "FAX", "FCOM", "FILE",
        "FORM", "GEDC", "GIVN", "GRAD", "HEAD", "HUSB", "IDNO", "IMMI", "INDI", "LANG", "LEGA", "MARB",
        "MARC", "MARL", "MARR", "MARS", "MEDI", "NAME", "NATI", "NATU", "NCHI", "NICK", "NMR", "NOTE",
        "NPFX", "NSFX", "OBJE", "OCCU", "ORDI", "ORDN", "PAGE", "PEDI", "PHON", "PLAC", "POST", "PROB",
        "PROP", "PUBL", "QUAY", "REFN", "RELA", "RELI", "REPO", "RESI", "RESN", "RETI", "RFN", "RIN",
        "ROLE", "ROMN", "SOUR", "SPFX", "SSN", "STAE", "STAT", "SUBM", "SUBN", "SURN", "TEMP", "TEXT",
        "TIME", "TITL", "TRLR", "TYPE", "UID", "VERS", "WIFE", "WILL", "WWW", "Y"
    };

    private static void AddRecoveryDiagnostic(
        GenealogyDocument document,
        string code,
        string message,
        int? lineNumber = null,
        string? subject = null)
    {
        document.HasRecoveryIssues = true;
        document.Diagnostics.Add(new GenealogyDiagnostic
        {
            Severity = GenealogyDiagnosticSeverity.Warning,
            Code = code,
            Message = message,
            LineNumber = lineNumber,
            Subject = subject
        });
    }
}

internal static class StringEnumerableExtensions
{
    public static IEnumerable<string> WhereNotNullOrWhiteSpace(this IEnumerable<string?> values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                yield return value;
        }
    }
}
