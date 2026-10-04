using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Genealogy.Gedcom.Models;
using Genealogy.Models;

namespace Genealogy.Gedcom;

internal sealed class GedcomVersionConverter
{
    private const string Gedcom7Terms = "https://gedcom.io/terms/v7/";

    private readonly GenealogyDocument _document;
    private readonly string _sourceVersion;
    private readonly string _targetVersion;

    public GedcomVersionConverter(GenealogyDocument document, string? sourceVersion, string targetVersion)
    {
        _document = document;
        _sourceVersion = sourceVersion ?? targetVersion;
        _targetVersion = targetVersion;
    }

    public void Convert(GedcomSyntaxDocument syntax)
    {
        if (string.Equals(_sourceVersion, _targetVersion, StringComparison.Ordinal))
            return;

        if (_targetVersion == GedcomVersion.Gedcom70)
        {
            for (var index = 0; index < syntax.Records.Count;)
            {
                var record = syntax.Records[index];
                if (record.Tag == "SUBN")
                {
                    var header = syntax.Records.FirstOrDefault(candidate => candidate.Tag == "HEAD");
                    if (header is not null)
                        AddFallback(header, record, record, "The GEDCOM 5.5.1 SUBN record was removed in GEDCOM 7.");
                    syntax.Records.RemoveAt(index);
                    continue;
                }
                if (record.Tag == "NOTE" && record.CrossReference is not null)
                    record.Tag = "SNOTE";
                ConvertChildren(record, syntax, record);
                index++;
            }
        }
        else
        {
            for (var index = 0; index < syntax.Records.Count;)
            {
                var record = syntax.Records[index];
                if (record.Tag == "SNOTE" && record.CrossReference is not null)
                    record.Tag = "NOTE";
                ConvertChildren(record, syntax, record);
                index++;
            }
            foreach (var record in syntax.Records)
                WrapLongNotes(record, record);
        }

        syntax.Version = _targetVersion;
    }

    private void ConvertChildren(
        GedcomSyntaxNode parent,
        GedcomSyntaxDocument syntax,
        GedcomSyntaxNode ownerRecord)
    {
        for (var index = 0; index < parent.Children.Count;)
        {
            var child = parent.Children[index];
            if (_targetVersion == GedcomVersion.Gedcom70
                && child.Tag == "CONC")
            {
                var concatenationTarget = index > 0
                    && parent.Children[index - 1].Tag == "CONT"
                    ? parent.Children[index - 1]
                    : parent;
                concatenationTarget.Value += child.Value;
                if (child.Children.Count > 0)
                    AddFallback(parent, ownerRecord, child, "CONC has nested substructures.");
                parent.Children.RemoveAt(index);
                continue;
            }

            if (TryConvertKnownValue(parent, child, ownerRecord))
            {
                ConvertChildren(child, syntax, ownerRecord);
                index++;
                continue;
            }

            if (TryConvertKnownStructure(parent, child, ownerRecord))
            {
                if (child.Tag == "REMOVED")
                {
                    parent.Children.RemoveAt(index);
                    continue;
                }
                ConvertChildren(child, syntax, ownerRecord);
                index++;
                continue;
            }

            if (IsIncompatible(parent, child))
            {
                AddFallback(parent, ownerRecord, child,
                    $"The {child.Tag} structure has no safe automatic mapping to GEDCOM {_targetVersion}.");
                parent.Children.RemoveAt(index);
                continue;
            }

            ConvertChildren(child, syntax, ownerRecord);
            index++;
        }
    }

    private bool TryConvertKnownValue(
        GedcomSyntaxNode parent,
        GedcomSyntaxNode node,
        GedcomSyntaxNode ownerRecord)
    {
        var convertedValue = _targetVersion == GedcomVersion.Gedcom70
            ? MapLegacyEnumValue(parent, node)
            : MapGedcom7EnumValue(parent, node);
        if (convertedValue is null || convertedValue == node.Value)
            return false;

        var originalValue = node.Value;
        node.Value = convertedValue;
        AddDiagnostic(
            "GEDCOM_ENUM_VALUE_CONVERTED",
            $"Converted {parent.Tag}/{node.Tag} value '{originalValue}' to '{convertedValue}' for GEDCOM {_targetVersion}.",
            ownerRecord,
            node);
        return true;
    }

    private static string? MapLegacyEnumValue(GedcomSyntaxNode parent, GedcomSyntaxNode node)
    {
        var value = node.Value.ToUpperInvariant();
        if (parent.Tag == "NAME" && node.Tag == "TYPE")
        {
            return value is "AKA" or "BIRTH" or "IMMIGRANT" or "MAIDEN" or "MARRIED" or "PROFESSIONAL" or "RELIGIOUS"
                ? value
                : null;
        }

        if (parent.Tag == "FAMC" && node.Tag == "STAT")
            return value is "CHALLENGED" or "DISPROVEN" or "PROVEN" ? value : null;
        if (parent.Tag == "FAMC" && node.Tag == "PEDI")
            return value is "ADOPTED" or "BIRTH" or "FOSTER" or "SEALING" ? value : null;
        if (node.Tag == "RESN")
            return value is "CONFIDENTIALITY" or "LOCKED" or "PRIVACY" ? value : null;
        if (parent.Tag == "SLGC" && node.Tag == "STAT")
        {
            return value switch
            {
                "DNS/CAN" => "DNS_CAN",
                "PRE-1970" => "PRE_1970",
                _ => null
            };
        }

        return null;
    }

    private static string? MapGedcom7EnumValue(GedcomSyntaxNode parent, GedcomSyntaxNode node)
    {
        var value = node.Value.ToUpperInvariant();
        if (parent.Tag == "NAME" && node.Tag == "TYPE")
        {
            return value is "AKA" or "BIRTH" or "IMMIGRANT" or "MAIDEN" or "MARRIED" or "PROFESSIONAL" or "RELIGIOUS"
                ? value.ToLowerInvariant()
                : null;
        }

        if (parent.Tag == "FAMC" && node.Tag == "STAT")
            return value is "CHALLENGED" or "DISPROVEN" or "PROVEN" ? value.ToLowerInvariant() : null;
        if (parent.Tag == "FAMC" && node.Tag == "PEDI")
            return value is "ADOPTED" or "BIRTH" or "FOSTER" or "SEALING" ? value.ToLowerInvariant() : null;
        if (node.Tag == "RESN")
            return value is "CONFIDENTIALITY" or "LOCKED" or "PRIVACY" ? value.ToLowerInvariant() : null;
        if (parent.Tag == "SLGC" && node.Tag == "STAT")
        {
            return value switch
            {
                "DNS_CAN" => "DNS/CAN",
                "PRE_1970" => "PRE-1970",
                _ => null
            };
        }

        return null;
    }

    private bool TryConvertKnownStructure(
        GedcomSyntaxNode parent,
        GedcomSyntaxNode node,
        GedcomSyntaxNode ownerRecord)
    {
        if (_targetVersion == GedcomVersion.Gedcom70)
        {
            if (node.Tag == "NOTE" && IsPointer(node.Value))
            {
                node.Tag = "SNOTE";
                AddDiagnostic("GEDCOM_NOTE_TO_SNOTE", "Converted a shared-note pointer to the GEDCOM 7 SNOTE structure.", ownerRecord, node);
                return true;
            }

            if (node.Tag is "AFN" or "RFN" or "RIN")
            {
                var legacyTag = node.Tag;
                node.Tag = "EXID";
                node.Children.Add(new GedcomSyntaxNode
                {
                    Level = node.Level + 1,
                    Tag = "TYPE",
                    Value = Gedcom7Terms + legacyTag
                });
                AddDiagnostic("GEDCOM_IDENTIFIER_TO_EXID",
                    $"Converted legacy {legacyTag} identifier to GEDCOM 7 EXID.", ownerRecord, node);
                return true;
            }

            if (node.Tag is "ROMN" or "FONE")
            {
                var type = node.Children.FirstOrDefault(child => child.Tag == "TYPE")?.Value;
                if (TryMapTransliteration(node.Tag, type, out var language))
                {
                    node.Tag = "TRAN";
                    node.Children.RemoveAll(child => child.Tag == "TYPE");
                    node.Children.Add(new GedcomSyntaxNode
                    {
                        Level = node.Level + 1,
                        Tag = "LANG",
                        Value = language
                    });
                    AddDiagnostic("GEDCOM_NAME_TRANSLATION_TO_TRAN",
                        $"Converted legacy {type} name to GEDCOM 7 TRAN with language '{language}'.", ownerRecord, node);
                    return true;
                }
                return false;
            }

            if (node.Tag == "RELA")
            {
                var relation = node.Value;
                node.Tag = "ROLE";
                node.Value = MapLegacyRole(relation, out var phrase);
                if (phrase is not null)
                {
                    node.Children.Add(new GedcomSyntaxNode
                    {
                        Level = node.Level + 1,
                        Tag = "PHRASE",
                        Value = phrase
                    });
                }
                AddDiagnostic("GEDCOM_RELA_TO_ROLE",
                    "Converted free-text RELA to GEDCOM 7 ROLE; unsupported role wording is retained in PHRASE.", ownerRecord, node);
                return true;
            }

            if (node.Tag == "WAC")
            {
                node.Tag = "INIL";
                AddDiagnostic("GEDCOM_WAC_TO_INIL", "Converted WAC to the GEDCOM 7 INIL structure.", ownerRecord, node);
                return true;
            }

            if (node.Tag == "SUBN")
            {
                AddFallback(parent, ownerRecord, node,
                    "The GEDCOM 5.5.1 SUBN structure was removed in GEDCOM 7.");
                node.Tag = "REMOVED";
                return true;
            }

            if (node.Tag == "OBJE" && node.CrossReference is null && node.Children.Count > 0)
                return false;

            if (node.Tag == "SOUR" && !IsPointer(node.Value) && node.Value.Length > 0)
                return false;
        }
        else
        {
            if (node.Tag == "SNOTE" && IsPointer(node.Value))
            {
                node.Tag = "NOTE";
                AddDiagnostic("GEDCOM_SNOTE_TO_NOTE", "Converted a GEDCOM 7 shared-note pointer to GEDCOM 5.5.1 NOTE.", ownerRecord, node);
                return true;
            }

            if (node.Tag == "EXID")
            {
                var type = node.Children.FirstOrDefault(child => child.Tag == "TYPE")?.Value;
                if (TryMapExid(type, out var legacyTag))
                {
                    node.Tag = legacyTag;
                    node.Children.RemoveAll(child => child.Tag == "TYPE");
                    AddDiagnostic("GEDCOM_EXID_TO_LEGACY_IDENTIFIER",
                        $"Converted GEDCOM 7 EXID to legacy {legacyTag}.", ownerRecord, node);
                    return true;
                }
                return false;
            }

            if (node.Tag == "TRAN")
            {
                var language = node.Children.FirstOrDefault(child => child.Tag == "LANG")?.Value;
                if (TryMapLanguage(language, out var legacyTag, out var legacyType))
                {
                    node.Tag = legacyTag;
                    node.Children.RemoveAll(child => child.Tag == "LANG");
                    node.Children.Add(new GedcomSyntaxNode
                    {
                        Level = node.Level + 1,
                        Tag = "TYPE",
                        Value = legacyType
                    });
                    AddDiagnostic("GEDCOM_TRAN_TO_NAME_TRANSLATION",
                        $"Converted GEDCOM 7 TRAN to legacy {legacyTag} TYPE {legacyType}.", ownerRecord, node);
                    return true;
                }
                return false;
            }

            if (node.Tag == "ROLE" && parent.Tag == "ASSO")
            {
                var phrase = node.Children.FirstOrDefault(child => child.Tag == "PHRASE")?.Value;
                node.Tag = "RELA";
                node.Value = node.Value == "OTHER" && phrase is not null ? phrase : node.Value;
                node.Children.RemoveAll(child => child.Tag == "PHRASE");
                AddDiagnostic("GEDCOM_ROLE_TO_RELA",
                    "Converted GEDCOM 7 ROLE to GEDCOM 5.5.1 free-text RELA.", ownerRecord, node);
                return true;
            }

            if (node.Tag == "INIL")
            {
                return false;
            }
        }

        return false;
    }

    private bool IsIncompatible(GedcomSyntaxNode parent, GedcomSyntaxNode node)
    {
        if (_targetVersion == GedcomVersion.Gedcom70)
        {
            return node.Tag is "ROMN" or "FONE" or "AFN" or "RFN" or "RIN"
                || node.Tag == "SUBN"
                || node.Tag == "PHRASE" && !IsGedcom7PhraseContext(parent, node)
                || node.Tag == "OBJE" && node.CrossReference is null && node.Children.Count > 0
                || node.Tag == "SOUR" && !IsPointer(node.Value) && node.Value.Length > 0;
        }

        return node.Tag is "SCHMA" or "INIL" or "PHRASE" or "WAC"
            || node.Tag == "TRAN"
            || node.Tag == "EXID"
            || node.Tag == "SUBN";
    }

    private static bool IsGedcom7PhraseContext(GedcomSyntaxNode parent, GedcomSyntaxNode node) =>
        node.Tag == "PHRASE" && parent.Tag is "ROLE" or "AGE" or "DATE";

    private static string MapLegacyRole(string role, out string? phrase)
    {
        phrase = null;
        if (role.Equals("Witness", StringComparison.OrdinalIgnoreCase))
            return "WITN";
        if (role.Equals("Clergy", StringComparison.OrdinalIgnoreCase))
            return "CLERGY";
        if (role.Equals("Officiator", StringComparison.OrdinalIgnoreCase))
            return "OFFICIATOR";
        if (role.Equals("Godparent", StringComparison.OrdinalIgnoreCase))
        {
            phrase = role;
            return "OTHER";
        }

        phrase = role;
        return "OTHER";
    }

    private static bool TryMapTransliteration(string tag, string? type, out string language)
    {
        language = (tag, type?.ToLowerInvariant()) switch
        {
            ("FONE", "hangul") => "ko-hang",
            ("FONE", "kana") => "ja-hrkt",
            ("ROMN", "pinyin") => "und-Latn-pinyin",
            ("ROMN", "romaji") => "ja-Latn",
            ("ROMN", "wadegiles") => "zh-Latn-wadegile",
            _ => string.Empty
        };
        return language.Length > 0;
    }

    private static bool TryMapLanguage(string? language, out string tag, out string type)
    {
        (tag, type) = language?.ToLowerInvariant() switch
        {
            "ko-hang" => ("FONE", "hangul"),
            "ja-hrkt" => ("FONE", "kana"),
            "und-latn-pinyin" => ("ROMN", "pinyin"),
            "ja-latn" => ("ROMN", "romaji"),
            "zh-latn-wadegile" => ("ROMN", "wadegiles"),
            _ => (string.Empty, string.Empty)
        };
        return tag.Length > 0;
    }

    private static bool TryMapExid(string? type, out string legacyTag)
    {
        legacyTag = type switch
        {
            Gedcom7Terms + "AFN" => "AFN",
            Gedcom7Terms + "RFN" => "RFN",
            Gedcom7Terms + "RIN" => "RIN",
            _ => string.Empty
        };
        return legacyTag.Length > 0;
    }

    private static bool IsPointer(string value) =>
        value.Length > 2 && value[0] == '@' && value[^1] == '@';

    private void WrapLongNotes(GedcomSyntaxNode node, GedcomSyntaxNode ownerRecord)
    {
        if (node.Tag == "NOTE" && node.Value.Length > 0)
        {
            var recordPrefix = $"{node.Level} "
                + (node.CrossReference is null ? string.Empty : node.CrossReference + " ")
                + node.Tag + " ";
            var continuationPrefix = $"{node.Level + 1} CONC ";
            var chunkLength = Math.Min(255 - recordPrefix.Length, 255 - continuationPrefix.Length);
            if (chunkLength <= 0)
                throw new InvalidOperationException(
                    $"The GEDCOM 5.5.1 line prefix for NOTE at level {node.Level} leaves no room for text.");
            if (node.Value.Length > chunkLength)
            {
                var originalLength = node.Value.Length;
                var remaining = node.Value;
                node.Value = TakeChunk(ref remaining, chunkLength);
                var continuations = new List<GedcomSyntaxNode>();
                while (remaining.Length > 0)
                {
                    continuations.Add(new GedcomSyntaxNode
                    {
                        Level = node.Level + 1,
                        Tag = "CONC",
                        Value = TakeChunk(ref remaining, chunkLength)
                    });
                }
                node.Children.InsertRange(0, continuations);
                AddDiagnostic("GEDCOM_NOTE_WRAPPED",
                    $"Wrapped a {originalLength}-character NOTE value into GEDCOM 5.5.1 continuation lines.",
                    ownerRecord,
                    node);
            }
        }

        foreach (var child in node.Children)
            WrapLongNotes(child, ownerRecord);
    }

    private static string TakeChunk(ref string value, int maximumLength)
    {
        var length = Math.Min(maximumLength, value.Length);
        if (length < value.Length && char.IsHighSurrogate(value[length - 1]))
            length--;
        var result = value[..length];
        value = value[length..];
        return result;
    }

    private void AddFallback(
        GedcomSyntaxNode parent,
        GedcomSyntaxNode ownerRecord,
        GedcomSyntaxNode unsupported,
        string reason)
    {
        var payload = JsonSerializer.Serialize(unsupported);
        parent.Children.Add(new GedcomSyntaxNode
        {
            Level = parent.Level + 1,
            Tag = "NOTE",
            Value = $"[GEDCOM conversion fallback: {reason}] {payload}"
        });
        AddDiagnostic("GEDCOM_VERSION_FALLBACK",
            $"{reason} The original structure is retained in a marked NOTE.", ownerRecord, unsupported);
    }

    private void AddDiagnostic(
        string code,
        string message,
        GedcomSyntaxNode ownerRecord,
        GedcomSyntaxNode subject)
    {
        _document.Diagnostics.Add(new GenealogyDiagnostic
        {
            Severity = code == "GEDCOM_VERSION_FALLBACK"
                ? GenealogyDiagnosticSeverity.Warning
                : GenealogyDiagnosticSeverity.Information,
            Code = code,
            Message = message,
            Subject = ownerRecord.CrossReference is null
                ? $"{ownerRecord.Tag}/{subject.Tag}"
                : $"{ownerRecord.CrossReference}/{subject.Tag}"
        });
    }
}
