using System;
using System.Security.Cryptography;
using System.Text;
using Genealogy.Models;

namespace Genealogy.Gedcom.Models;

internal static class GenealogyModelFingerprint
{
    public static string Create(GenealogyDocument document)
    {
        var builder = new StringBuilder();
        foreach (var record in document.Records)
            AppendRecord(builder, record);
        foreach (var content in document.OtherContent)
            AppendNode(builder, content);
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
    }

    private static void AppendRecord(StringBuilder builder, GenealogyRecord record)
    {
        AppendToken(builder, record.Id.ToString("D"));
        AppendToken(builder, record.Kind);
        AppendToken(builder, record.TypeCode);
        AppendToken(builder, record.DisplayName);
        AppendToken(builder, record.GivenName);
        AppendToken(builder, record.Surname);
        AppendToken(builder, record.Sex);
        foreach (var identifier in record.Identifiers)
        {
            AppendToken(builder, identifier.Provider);
            AppendToken(builder, identifier.Value);
        }
        foreach (var content in record.Content)
            AppendNode(builder, content);
        foreach (var association in record.Associations)
        {
            AppendToken(builder, association.Id.ToString("D"));
            AppendToken(builder, association.SourceRecordId.ToString("D"));
            AppendToken(builder, association.TargetRecordId?.ToString("D"));
            AppendToken(builder, association.TargetIdentifier?.Provider);
            AppendToken(builder, association.TargetIdentifier?.Value);
            AppendToken(builder, association.Role);
            AppendToken(builder, association.Detail);
        }
    }

    private static void AppendNode(StringBuilder builder, GenealogyNode node)
    {
        AppendToken(builder, node.GetType().FullName);
        AppendToken(builder, node.Id.ToString("D"));
        AppendToken(builder, node.TypeCode);
        AppendToken(builder, node.Value);
        if (node is UserDefinedEvent userDefinedEvent)
            AppendToken(builder, userDefinedEvent.Context);
        foreach (var reference in node.References)
        {
            AppendToken(builder, reference.Provider);
            AppendToken(builder, reference.Value);
        }
        foreach (var child in node.Children)
            AppendNode(builder, child);
        AppendToken(builder, "|");
    }

    private static void AppendToken(StringBuilder builder, string? value)
    {
        var token = value ?? string.Empty;
        builder.Append(token.Length).Append(':').Append(token);
    }
}
