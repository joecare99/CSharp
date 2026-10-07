using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using OFBCreator.Projects.Models;

namespace OFBCreator.Projects.Services;

/// <summary>
/// Validates the constrained rule grammar persisted in an OFB project.
/// </summary>
public static partial class OFBExportRuleValidator
{
    private static readonly HashSet<string> PersonFields = new(StringComparer.Ordinal)
    {
        "givenName", "surname", "displayName",
        "title", "religion", "occupation",
        "birthDate", "baptismDate", "deathDate", "burialDate",
        "birthPlace", "baptismPlace", "deathPlace", "burialPlace",
        "residence", "occupationPlace"
    };

    private static readonly HashSet<string> FamilyFields = new(StringComparer.Ordinal)
    {
        "marriageDate", "marriagePlace"
    };

    /// <summary>Validates rule identities, ordering, target keys, actions, and field-specific values.</summary>
    /// <param name="rules">The complete rule set to validate, including disabled rules.</param>
    public static void Validate(IReadOnlyList<OFBExportRule>? rules)
    {
        if (rules is null)
            throw new InvalidDataException("Project export rules cannot be null.");

        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var orders = new HashSet<int>();
        foreach (var rule in rules)
        {
            if (rule is null)
                throw new InvalidDataException("Project export rules cannot contain null entries.");
            if (!Guid.TryParse(rule.Id, out _) || !ids.Add(rule.Id))
                throw new InvalidDataException($"Export rule identifier '{rule.Id}' is invalid or duplicated.");
            if (!orders.Add(rule.Order))
                throw new InvalidDataException($"Export rule order {rule.Order} is duplicated.");
            if (!TryParseTarget(rule.TargetId, out _))
                throw new InvalidDataException($"Export rule '{rule.Id}' has an invalid provider-qualified target.");
            ValidateRule(rule);
        }
    }

    /// <summary>Parses the provider-qualified target format <c>provider:person|family:external-id</c>.</summary>
    /// <param name="targetId">The serialized rule target.</param>
    /// <param name="target">The parsed provider, owner kind, and external identifier.</param>
    /// <returns><see langword="true"/> when the target has a supported, non-empty structure.</returns>
    public static bool TryParseTarget(string? targetId, out (string ProviderId, string Kind, string ExternalId) target)
    {
        target = default;
        if (string.IsNullOrWhiteSpace(targetId))
            return false;

        var firstSeparator = targetId.IndexOf(':');
        var secondSeparator = firstSeparator < 0 ? -1 : targetId.IndexOf(':', firstSeparator + 1);
        if (firstSeparator <= 0 || secondSeparator <= firstSeparator + 1 || secondSeparator == targetId.Length - 1)
            return false;

        var providerId = targetId[..firstSeparator];
        var kind = targetId[(firstSeparator + 1)..secondSeparator];
        var externalId = targetId[(secondSeparator + 1)..];
        if (!ProviderIdPattern().IsMatch(providerId)
            || kind is not ("person" or "family")
            || string.IsNullOrWhiteSpace(externalId))
            return false;

        target = (providerId, kind, externalId);
        return true;
    }

    private static void ValidateRule(OFBExportRule rule)
    {
        if (rule.TargetKind is not ("person" or "family" or "fact"))
            throw new InvalidDataException($"Export rule '{rule.Id}' has unknown target kind '{rule.TargetKind}'.");
        if (!TryParseTarget(rule.TargetId, out var target))
            throw new InvalidDataException($"Export rule '{rule.Id}' has an invalid target.");
        if (rule.TargetKind == "person" && target.Kind != "person"
            || rule.TargetKind == "family" && target.Kind != "family"
            || rule.TargetKind == "fact" && target.Kind is not ("person" or "family"))
            throw new InvalidDataException($"Export rule '{rule.Id}' target kind does not match its target identifier.");

        switch (rule.TargetKind)
        {
            case "person":
                ValidateAction(rule, "include", "exclude", "replace", "redact", "generalize");
                if (rule.Action is "include" or "exclude")
                {
                    RequireNoFieldOrValue(rule);
                    return;
                }
                RequireField(rule, PersonFields);
                ValidateTransform(rule, isDate: rule.Field?.EndsWith("Date", StringComparison.Ordinal) == true,
                    isPlace: rule.Field?.EndsWith("Place", StringComparison.Ordinal) == true
                        || rule.Field is "residence" or "occupationPlace");
                break;
            case "family":
                ValidateAction(rule, "include", "exclude", "replace", "redact", "generalize");
                if (rule.Action is "include" or "exclude")
                {
                    RequireNoFieldOrValue(rule);
                    return;
                }
                RequireField(rule, FamilyFields);
                ValidateTransform(rule, isDate: rule.Field == "marriageDate", isPlace: rule.Field == "marriagePlace");
                break;
            case "fact":
                ValidateAction(rule, "exclude");
                if (string.IsNullOrWhiteSpace(rule.Field))
                    throw new InvalidDataException($"Fact rule '{rule.Id}' requires a fact type in 'Field'.");
                if (rule.Value is not null)
                    throw new InvalidDataException($"Fact rule '{rule.Id}' cannot have a replacement value.");
                if (rule.Occurrence is < 1)
                    throw new InvalidDataException($"Fact rule '{rule.Id}' occurrence must be one-based.");
                break;
        }
    }

    private static void ValidateTransform(OFBExportRule rule, bool isDate, bool isPlace)
    {
        switch (rule.Action)
        {
            case "replace":
                if (string.IsNullOrWhiteSpace(rule.Value))
                    throw new InvalidDataException($"Replacement rule '{rule.Id}' requires a value.");
                if (rule.Occurrence is not null)
                    throw new InvalidDataException($"Replacement rule '{rule.Id}' cannot target a fact occurrence.");
                break;
            case "redact":
                if (rule.Value is not null || rule.Occurrence is not null)
                    throw new InvalidDataException($"Redaction rule '{rule.Id}' cannot have a value or occurrence.");
                break;
            case "generalize":
                if (isDate && rule.Value is not ("year" or "decade"))
                    throw new InvalidDataException($"Date rule '{rule.Id}' must generalize to 'year' or 'decade'.");
                if (isPlace && string.IsNullOrWhiteSpace(rule.Value))
                    throw new InvalidDataException($"Place rule '{rule.Id}' requires a generalized place value.");
                if (!isDate && !isPlace)
                    throw new InvalidDataException($"Rule '{rule.Id}' cannot generalize field '{rule.Field}'.");
                if (rule.Occurrence is not null)
                    throw new InvalidDataException($"Generalization rule '{rule.Id}' cannot target a fact occurrence.");
                break;
        }
    }

    private static void ValidateAction(OFBExportRule rule, params string[] supported)
    {
        if (!supported.Contains(rule.Action, StringComparer.Ordinal))
            throw new InvalidDataException($"Export rule '{rule.Id}' has unsupported action '{rule.Action}'.");
    }

    private static void RequireField(OFBExportRule rule, HashSet<string> supported)
    {
        if (rule.Field is null || !supported.Contains(rule.Field))
            throw new InvalidDataException($"Export rule '{rule.Id}' has unsupported field '{rule.Field}'.");
    }

    private static void RequireNoFieldOrValue(OFBExportRule rule)
    {
        if (rule.Field is not null || rule.Value is not null || rule.Occurrence is not null)
            throw new InvalidDataException($"Inclusion rule '{rule.Id}' cannot have a field, value, or occurrence.");
    }

    [GeneratedRegex("^[a-zA-Z0-9._-]+$", RegexOptions.CultureInvariant)]
    private static partial Regex ProviderIdPattern();
}
