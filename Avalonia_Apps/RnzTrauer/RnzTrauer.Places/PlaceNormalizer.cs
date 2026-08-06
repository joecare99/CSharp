using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RnzTrauer.Places;

public sealed class PlaceNormalizer
{
    public PlaceMatch Resolve(
        string? input,
        IReadOnlyCollection<string> knownPlaces,
        IReadOnlyDictionary<string, IReadOnlyList<string>>? aliases = null)
    {
        ArgumentNullException.ThrowIfNull(knownPlaces);
        var normalized = Normalize(input);
        if (normalized is null)
            return new PlaceMatch(input ?? string.Empty, null, PlaceMatchKind.Empty, Array.Empty<string>());

        if (aliases is not null)
        {
            var alias = aliases.FirstOrDefault(pair =>
                string.Equals(Normalize(pair.Key), normalized, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(alias.Key))
            {
                var aliasMatches = alias.Value
                    .Where(candidate => knownPlaces.Any(known =>
                        string.Equals(Normalize(known), Normalize(candidate), StringComparison.OrdinalIgnoreCase)))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
                if (aliasMatches.Length == 1)
                    return new PlaceMatch(input ?? string.Empty, aliasMatches[0], PlaceMatchKind.Known, aliasMatches);
                if (aliasMatches.Length > 1)
                    return new PlaceMatch(input ?? string.Empty, normalized, PlaceMatchKind.Ambiguous, aliasMatches);
            }
        }

        var matches = knownPlaces
            .Where(place => string.Equals(Normalize(place), normalized, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (matches.Length == 1)
            return new PlaceMatch(input ?? string.Empty, matches[0], PlaceMatchKind.Known, matches);
        if (matches.Length > 1)
            return new PlaceMatch(input ?? string.Empty, normalized, PlaceMatchKind.Ambiguous, matches);
        return new PlaceMatch(input ?? string.Empty, normalized, PlaceMatchKind.Unknown, Array.Empty<string>());
    }

    public static string? Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var value = input.Normalize(NormalizationForm.FormC)
            .Replace('\u00a0', ' ')
            .Trim();
        var builder = new StringBuilder(value.Length);
        var whitespace = false;
        foreach (var character in value)
        {
            if (char.IsWhiteSpace(character))
            {
                whitespace = true;
                continue;
            }

            if (whitespace && builder.Length > 0)
                builder.Append(' ');
            builder.Append(character);
            whitespace = false;
        }

        return builder.ToString();
    }
}
