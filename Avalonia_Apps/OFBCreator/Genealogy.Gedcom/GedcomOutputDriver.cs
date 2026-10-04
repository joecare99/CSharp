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
/// Writes a genealogy document as UTF-8 GEDCOM, retaining imported syntax for semantic roundtrips.
/// </summary>
public sealed class GedcomOutputDriver : IGenealogyOutputDriver
{
    private const string ProviderDataMediaType = "application/vnd.genealogy.gedcom-syntax+json";

    public string ProviderId => "gedcom";

    public async Task WriteAsync(
        GenealogyDocument document,
        Stream destination,
        GenealogyOutputOptions options,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(destination);
        ArgumentNullException.ThrowIfNull(options);
        if (!destination.CanWrite)
            throw new ArgumentException("The destination stream must be writable.", nameof(destination));
        if (document.HasRecoveryIssues && !options.AllowRecovery)
            throw new InvalidOperationException(
                "GEDCOM export was stopped because import recovery skipped or repaired source data; this document is not roundtrip-accepted.");

        var targetVersion = options.TargetVersion ?? GedcomVersion.HighestSupported;
        ValidateVersion(targetVersion);
        var syntax = GetSyntaxDocument(document, options.AllowRecovery);
        var sourceVersion = syntax.Version;
        new GedcomVersionConverter(document, sourceVersion, targetVersion).Convert(syntax);
        SetVersion(syntax, targetVersion);
        if (document.HasRecoveryIssues || syntax.HadRecoveryIssues)
        {
            document.Diagnostics.Add(new GenealogyDiagnostic
            {
                Severity = GenealogyDiagnosticSeverity.Warning,
                Code = "GEDCOM_BEST_EFFORT_OUTPUT",
                Message = "Best-effort output was explicitly allowed after import recovery. Source lines may have been skipped or text repaired; verify this file before relying on it."
            });
        }

        await using var writer = new StreamWriter(
            destination,
            new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            1024,
            leaveOpen: true);
        foreach (var record in syntax.Records)
            await WriteNodeAsync(writer, record, cancellationToken).ConfigureAwait(false);
        await writer.FlushAsync(cancellationToken).ConfigureAwait(false);

        if (sourceVersion is not null && !string.Equals(sourceVersion, targetVersion, StringComparison.Ordinal))
        {
            document.Diagnostics.Add(new GenealogyDiagnostic
            {
                Severity = GenealogyDiagnosticSeverity.Information,
                Code = "GEDCOM_VERSION_CONVERTED",
                Message = $"The GEDCOM header version changed from {sourceVersion} to {targetVersion}; supported structures were converted and incompatible structures were retained as marked NOTE values."
            });
        }

        if (document.ProviderData is null || document.ProviderData.Provider != ProviderId)
        {
            document.Diagnostics.Add(new GenealogyDiagnostic
            {
                Severity = GenealogyDiagnosticSeverity.Information,
                Code = "GEDCOM_EXPORT_CREATED",
                Message = $"A new GEDCOM {targetVersion} file was generated from the canonical genealogy model."
            });
        }
    }

    private static GedcomSyntaxDocument GetSyntaxDocument(GenealogyDocument document, bool allowRecovery)
    {
        if (document.ProviderData is { Provider: "gedcom", MediaType: ProviderDataMediaType } providerData)
        {
            var syntax = JsonSerializer.Deserialize<GedcomSyntaxDocument>(providerData.Data);
            if (syntax is null)
                throw new InvalidDataException("The retained GEDCOM syntax data is invalid.");
            if (syntax.HadRecoveryIssues && !allowRecovery)
                throw new InvalidOperationException("The retained GEDCOM syntax tree contains recovery issues.");
            if (string.Equals(providerData.CanonicalHash, GenealogyModelFingerprint.Create(document), StringComparison.Ordinal))
                return syntax;
            return BuildSyntaxDocument(document, syntax);
        }

        return BuildSyntaxDocument(document, null);
    }

    private static GedcomSyntaxDocument BuildSyntaxDocument(
        GenealogyDocument document,
        GedcomSyntaxDocument? originalSyntax)
    {
        var syntax = new GedcomSyntaxDocument
        {
            Version = originalSyntax?.Version ?? GedcomVersion.HighestSupported
        };
        var canonicalHeader = document.OtherContent.FirstOrDefault(content =>
            string.Equals(content.TypeCode, "HEAD", StringComparison.OrdinalIgnoreCase));
        var originalHeader = originalSyntax?.Records.FirstOrDefault(record =>
            string.Equals(record.Tag, "HEAD", StringComparison.OrdinalIgnoreCase));
        var header = canonicalHeader is not null
            ? BuildSyntaxNode(canonicalHeader, 0)
            : originalHeader is null ? CreateHeader() : CloneNode(originalHeader);
        syntax.Records.Add(header);

        var xrefsById = AllocateXrefs(document.Records);
        foreach (var record in document.Records)
        {
            var recordNode = new GedcomSyntaxNode
            {
                Level = 0,
                CrossReference = xrefsById[record.Id],
                Tag = GetRecordTag(record)
            };
            foreach (var content in record.Content)
                recordNode.Children.Add(BuildSyntaxNode(content, 1));
            SynchronizeTypedPersonFields(record, recordNode);
            SynchronizeAssociations(record, recordNode, xrefsById);
            syntax.Records.Add(recordNode);
        }

        foreach (var content in document.OtherContent)
        {
            if (content.TypeCode.Equals("HEAD", StringComparison.OrdinalIgnoreCase)
                || content.TypeCode.Equals("TRLR", StringComparison.OrdinalIgnoreCase))
                continue;
            syntax.Records.Add(BuildSyntaxNode(content, 0));
        }

        var canonicalTrailer = document.OtherContent.FirstOrDefault(content =>
            string.Equals(content.TypeCode, "TRLR", StringComparison.OrdinalIgnoreCase));
        var originalTrailer = originalSyntax?.Records.FirstOrDefault(record =>
            string.Equals(record.Tag, "TRLR", StringComparison.OrdinalIgnoreCase));
        syntax.Records.Add(canonicalTrailer is not null
            ? BuildSyntaxNode(canonicalTrailer, 0)
            : originalTrailer is null ? new GedcomSyntaxNode { Level = 0, Tag = "TRLR" } : CloneNode(originalTrailer));
        return syntax;
    }

    private static Dictionary<Guid, string> AllocateXrefs(IList<GenealogyRecord> records)
    {
        var allocatedIds = new HashSet<string>(StringComparer.Ordinal);
        var nextIdByPrefix = new Dictionary<string, int>(StringComparer.Ordinal);
        var xrefsById = new Dictionary<Guid, string>();
        foreach (var record in records)
        {
            var externalId = record.Identifiers.FirstOrDefault(identifier =>
                string.Equals(identifier.Provider, "gedcom", StringComparison.OrdinalIgnoreCase))?.Value;
            var xref = NormalizeXref(externalId);
            if (xref is null || !allocatedIds.Add(xref))
            {
                var tag = GetRecordTag(record);
                var prefix = tag == "INDI" ? "I" : tag == "FAM" ? "F" : tag[..Math.Min(tag.Length, 4)];
                nextIdByPrefix.TryGetValue(prefix, out var nextId);
                do
                {
                    xref = $"@{prefix}{++nextId}@";
                } while (!allocatedIds.Add(xref));
                nextIdByPrefix[prefix] = nextId;
            }
            xrefsById.Add(record.Id, xref);
        }
        return xrefsById;
    }

    private static string GetRecordTag(GenealogyRecord record)
    {
        return record.TypeCode ?? record.Kind switch
        {
            "Person" => "INDI",
            "Family" => "FAM",
            "Source" => "SOUR",
            "Repository" => "REPO",
            "Media" => "OBJE",
            "Note" => "NOTE",
            "Place" => "PLAC",
            _ => "EVEN"
        };
    }

    private static void SynchronizeTypedPersonFields(GenealogyRecord record, GedcomSyntaxNode recordNode)
    {
        if (record.Kind != "Person")
            return;

        if (record.GivenName is not null || record.Surname is not null)
        {
            var name = recordNode.Children.FirstOrDefault(child =>
                string.Equals(child.Tag, "NAME", StringComparison.OrdinalIgnoreCase));
            if (name is null)
            {
                name = Child(recordNode, "NAME", string.Empty);
            }
            if (record.GivenName is not null)
                SetChildValue(name, "GIVN", record.GivenName);
            if (record.Surname is not null)
                SetChildValue(name, "SURN", record.Surname);
            name.Value = $"{record.GivenName ?? string.Empty} /{record.Surname ?? string.Empty}/".Trim();
        }

        if (record.Sex is not null)
            SetChildValue(recordNode, "SEX", record.Sex);
    }

    private static void SetChildValue(GedcomSyntaxNode parent, string tag, string value)
    {
        var child = parent.Children.FirstOrDefault(candidate =>
            string.Equals(candidate.Tag, tag, StringComparison.OrdinalIgnoreCase));
        if (child is null)
            Child(parent, tag, value);
        else
            child.Value = value;
    }

    private static void SynchronizeAssociations(
        GenealogyRecord record,
        GedcomSyntaxNode recordNode,
        IReadOnlyDictionary<Guid, string> xrefsById)
    {
        var used = new HashSet<GedcomSyntaxNode>();
        foreach (var association in record.Associations)
        {
            var tag = GetAssociationTag(record, association.Role);
            if (tag is null)
                continue;
            var xref = association.TargetIdentifier is { Provider: "gedcom" } identifier
                ? NormalizeXref(identifier.Value)
                : association.TargetRecordId is Guid targetId && xrefsById.TryGetValue(targetId, out var mappedXref)
                    ? mappedXref
                    : null;
            if (xref is null)
                throw new InvalidOperationException(
                    $"Association '{association.Role}' on record '{record.Id}' has no GEDCOM target identifier.");

            var node = recordNode.Children.FirstOrDefault(candidate =>
                !used.Contains(candidate)
                && string.Equals(candidate.Tag, tag, StringComparison.OrdinalIgnoreCase)
                && NormalizeXref(candidate.Value) == xref);
            node ??= recordNode.Children.FirstOrDefault(candidate =>
                !used.Contains(candidate)
                && string.Equals(candidate.Tag, tag, StringComparison.OrdinalIgnoreCase));
            if (node is null)
            {
                node = Child(recordNode, tag, xref);
            }
            else
            {
                node.Value = xref;
            }
            used.Add(node);

            if (tag == "ASSO")
                SetChildValue(node, "RELA", association.Role);
            if (association.Detail is not null)
                SetChildValue(node, "NOTE", association.Detail);
        }

        foreach (var existing in recordNode.Children.Where(child =>
                     IsAssociationTag(child.Tag) && !used.Contains(child)).ToList())
            recordNode.Children.Remove(existing);
    }

    private static string? GetAssociationTag(GenealogyRecord record, string role)
    {
        if (record.Kind == "Family")
        {
            return role switch
            {
                "Husband" => "HUSB",
                "Wife" => "WIFE",
                "Child" => "CHIL",
                _ => null
            };
        }
        if (record.Kind == "Person")
        {
            return role switch
            {
                "ChildInFamily" => "FAMC",
                "PartnerInFamily" => "FAMS",
                _ => "ASSO"
            };
        }
        return null;
    }

    private static bool IsAssociationTag(string tag) =>
        tag.Equals("HUSB", StringComparison.OrdinalIgnoreCase)
        || tag.Equals("WIFE", StringComparison.OrdinalIgnoreCase)
        || tag.Equals("CHIL", StringComparison.OrdinalIgnoreCase)
        || tag.Equals("FAMC", StringComparison.OrdinalIgnoreCase)
        || tag.Equals("FAMS", StringComparison.OrdinalIgnoreCase)
        || tag.Equals("ASSO", StringComparison.OrdinalIgnoreCase);

    private static GedcomSyntaxNode CreateHeader()
    {
        var header = new GedcomSyntaxNode { Level = 0, Tag = "HEAD" };
        var source = Child(header, "SOUR", "OFBCreator");
        Child(source, "VERS", "1.0");
        Child(header, "CHAR", "UTF-8");
        var gedc = Child(header, "GEDC", string.Empty);
        Child(gedc, "VERS", GedcomVersion.HighestSupported);
        Child(gedc, "FORM", "LINEAGE-LINKED");
        return header;
    }

    private static GedcomSyntaxNode CloneNode(GedcomSyntaxNode node)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(node);
        return JsonSerializer.Deserialize<GedcomSyntaxNode>(bytes)
            ?? throw new InvalidDataException("A retained GEDCOM record could not be cloned.");
    }

    private static GedcomSyntaxNode BuildSyntaxNode(GenealogyNode node, int level)
    {
        var result = new GedcomSyntaxNode
        {
            Level = level,
            CrossReference = level == 0
                ? node.References.FirstOrDefault(identifier => identifier.Provider == "gedcom")?.Value
                : null,
            Tag = node.TypeCode,
            Value = node.Value
        };
        foreach (var child in node.Children)
            result.Children.Add(BuildSyntaxNode(child, level + 1));
        return result;
    }

    private static string? NormalizeXref(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return value.StartsWith('@') && value.EndsWith('@') ? value : $"@{value.Trim('@')}@";
    }

    private static void SetVersion(GedcomSyntaxDocument syntax, string targetVersion)
    {
        syntax.Version = targetVersion;
        var header = syntax.Records.FirstOrDefault(record =>
            string.Equals(record.Tag, "HEAD", StringComparison.OrdinalIgnoreCase));
        if (header is null)
        {
            header = new GedcomSyntaxNode { Level = 0, Tag = "HEAD" };
            syntax.Records.Insert(0, header);
        }

        var gedc = header.Children.FirstOrDefault(child =>
            string.Equals(child.Tag, "GEDC", StringComparison.OrdinalIgnoreCase));
        if (gedc is null)
        {
            gedc = Child(header, "GEDC", string.Empty);
        }

        var version = gedc.Children.FirstOrDefault(child =>
            string.Equals(child.Tag, "VERS", StringComparison.OrdinalIgnoreCase));
        if (version is null)
        {
            version = Child(gedc, "VERS", targetVersion);
        }
        else
        {
            version.Value = targetVersion;
        }

        var characterSet = header.Children.FirstOrDefault(child =>
            string.Equals(child.Tag, "CHAR", StringComparison.OrdinalIgnoreCase));
        if (characterSet is null)
        {
            Child(header, "CHAR", "UTF-8");
        }
        else
        {
            characterSet.Value = "UTF-8";
        }

        if (syntax.Records.All(record => !string.Equals(record.Tag, "TRLR", StringComparison.OrdinalIgnoreCase)))
            syntax.Records.Add(new GedcomSyntaxNode { Level = 0, Tag = "TRLR" });
    }

    private static GedcomSyntaxNode Child(GedcomSyntaxNode parent, string tag, string value)
    {
        var child = new GedcomSyntaxNode
        {
            Level = parent.Level + 1,
            Tag = tag,
            Value = value
        };
        parent.Children.Add(child);
        return child;
    }

    private static async Task WriteNodeAsync(
        StreamWriter writer,
        GedcomSyntaxNode node,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var line = node.Level.ToString(System.Globalization.CultureInfo.InvariantCulture) + " ";
        if (node.CrossReference is not null)
            line += node.CrossReference + " ";
        line += node.Tag;
        if (node.Value.Length > 0)
            line += " " + node.Value;
        await writer.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
        foreach (var child in node.Children)
            await WriteNodeAsync(writer, child, cancellationToken).ConfigureAwait(false);
    }

    private static void ValidateVersion(string version)
    {
        if (version != GedcomVersion.Gedcom551 && version != GedcomVersion.Gedcom70)
            throw new ArgumentOutOfRangeException(nameof(version), version,
                $"Supported target versions are {GedcomVersion.Gedcom551} and {GedcomVersion.Gedcom70}.");
    }
}
