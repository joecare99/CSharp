using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TranspilerLib.Data;
using TranspilerLib.Models.Scanner;
using TranspilerLib.CSharp.VBLegacyReplace.Models;
using TranspilerLib.CSharp.VBLegacyReplace.Services;

namespace TranspilerLib.CSharp.VBLegacyReplace;

/// <summary>Applies externally defined, token-aware Visual Basic legacy replacement rules to C# source.</summary>
public sealed class LegacyReplacementEngine
{
    /// <summary>The current supported JSON rule-set schema version.</summary>
    public const int CurrentSchemaVersion = 1;
    private const int MaximumNestedApplicationDepth = 128;

    private readonly List<CompiledRule> _rules;
    private readonly List<RuleDiagnostic> _configurationDiagnostics;

    /// <summary>Creates an engine from a validated and compiled rule-set model.</summary>
    /// <param name="ruleSet">The configured replacement rules.</param>
    public LegacyReplacementEngine(ReplacementRuleSet ruleSet)
        : this(ruleSet ?? throw new ArgumentNullException(nameof(ruleSet)), new List<RuleDiagnostic>())
    {
    }

    private LegacyReplacementEngine(ReplacementRuleSet ruleSet, List<RuleDiagnostic> initialDiagnostics)
    {
        _rules = new List<CompiledRule>();
        _configurationDiagnostics = initialDiagnostics;
        if (ruleSet.SchemaVersion != CurrentSchemaVersion)
        {
            _configurationDiagnostics.Add(new RuleDiagnostic(
                RuleDiagnosticKind.Malformed,
                RuleDiagnosticSeverity.Error,
                $"Unsupported rule-set schema version {ruleSet.SchemaVersion}; expected {CurrentSchemaVersion}."));
            return;
        }

        if (ruleSet.Rules is null)
        {
            _configurationDiagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Malformed, RuleDiagnosticSeverity.Error, "The rule set must contain a rules array."));
            return;
        }
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (ReplacementRule? rule in ruleSet.Rules)
        {
            if (rule is null)
            {
                _configurationDiagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Malformed, RuleDiagnosticSeverity.Error, "Rule entry must not be null."));
                continue;
            }
            if (!ids.Add(rule.Id))
            {
                _configurationDiagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Malformed, RuleDiagnosticSeverity.Error, $"Duplicate rule ID '{rule.Id}'.", rule.Id));
                continue;
            }
            if (!ValidateUsings(rule, out string? usingError))
            {
                _configurationDiagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Malformed, RuleDiagnosticSeverity.Error, usingError ?? "Invalid requiredUsings.", rule.Id));
                continue;
            }
            if (!rule.Enabled)
            {
                _configurationDiagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Skipped, RuleDiagnosticSeverity.Information, "Rule is disabled.", rule.Id));
                continue;
            }
            if (!RuleCompiler.TryCompile(rule, out CompiledRule? compiled, out string? error))
            {
                _configurationDiagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Malformed, RuleDiagnosticSeverity.Error, error ?? "Invalid rule definition.", rule.Id));
                continue;
            }
            if (compiled is null)
            {
                _configurationDiagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Malformed, RuleDiagnosticSeverity.Error, "Rule compilation returned no compiled rule.", rule.Id));
                continue;
            }
            _rules.Add(compiled);
        }

        _rules.Sort(static (left, right) =>
        {
            int priority = right.Definition.Priority.CompareTo(left.Definition.Priority);
            if (priority != 0)
                return priority;
            int specificity = right.FixedTokenCount.CompareTo(left.FixedTokenCount);
            return specificity != 0 ? specificity : string.CompareOrdinal(left.Definition.Id, right.Definition.Id);
        });
    }

    /// <summary>Gets diagnostics produced while loading and validating the configured rule set.</summary>
    public IReadOnlyList<RuleDiagnostic> ConfigurationDiagnostics => _configurationDiagnostics.AsReadOnly();

    /// <summary>Creates an engine by deserializing a JSON rule-set document.</summary>
    /// <param name="json">The versioned JSON rule-set document.</param>
    /// <returns>An engine containing valid rules and diagnostics for malformed entries.</returns>
    public static LegacyReplacementEngine LoadJson(string json)
    {
#if NET5_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(json);
#else
        if (json is null)
            throw new ArgumentNullException(nameof(json));
#endif
        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return MalformedJson("JSON rule set must be an object.");

            var diagnostics = new List<RuleDiagnostic>();
            if (!HasProperty(document.RootElement, "schemaVersion"))
                diagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Malformed, RuleDiagnosticSeverity.Error, "JSON rule set must declare schemaVersion."));
            if (!HasProperty(document.RootElement, "rules"))
                diagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Malformed, RuleDiagnosticSeverity.Error, "JSON rule set must declare a rules array."));

            ReplacementRuleSet? ruleSet = JsonSerializer.Deserialize<ReplacementRuleSet>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (ruleSet is null)
                return MalformedJson("JSON rule set must not be null.");
            return new LegacyReplacementEngine(ruleSet, diagnostics);
        }
        catch (JsonException exception)
        {
            return MalformedJson($"Invalid JSON rule set: {exception.Message}");
        }
    }

    private static LegacyReplacementEngine MalformedJson(string message) =>
        new(new ReplacementRuleSet(), new List<RuleDiagnostic>
        {
            new(RuleDiagnosticKind.Malformed, RuleDiagnosticSeverity.Error, message)
        });

    private static bool HasProperty(JsonElement element, string propertyName)
    {
        foreach (JsonProperty property in element.EnumerateObject())
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }
    /// <summary>Creates an engine from a UTF-8 JSON rule-set file.</summary>
    /// <param name="path">The file path to load.</param>
    /// <returns>An engine containing valid rules and diagnostics for malformed entries.</returns>
    public static LegacyReplacementEngine LoadFile(string path)
    {
#if NET5_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
#else
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path must not be null or whitespace.", nameof(path));
#endif
        return LoadJson(File.ReadAllText(path));
    }

    /// <summary>Creates an engine from the conservative rules embedded in this assembly.</summary>
    /// <returns>An engine loaded with the library's default rules.</returns>
    public static LegacyReplacementEngine LoadDefaultRules()
    {
        string? resourceName = typeof(LegacyReplacementEngine).Assembly.GetManifestResourceNames()
            .SingleOrDefault(name => name.EndsWith("default-rules.json", StringComparison.Ordinal));
        if (resourceName is null)
            throw new InvalidOperationException("The embedded default replacement rules resource is missing.");
        using Stream stream = typeof(LegacyReplacementEngine).Assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Unable to open embedded rules resource '{resourceName}'.");
        using var reader = new StreamReader(stream);
        return LoadJson(reader.ReadToEnd());
    }

    /// <summary>Applies the highest-priority, non-overlapping rule matches to source text.</summary>
    /// <param name="source">The C# source text to transform.</param>
    /// <returns>Transformed source, required usings, and applied/skipped/malformed diagnostics.</returns>
    public ReplacementResult Apply(string source)
        => Apply(source, 0);

    private ReplacementResult Apply(string source, int nestedDepth)
    {
#if NET5_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
#else
        if (source is null)
            throw new ArgumentNullException(nameof(source));
#endif
        if (nestedDepth >= MaximumNestedApplicationDepth)
        {
            return new ReplacementResult(source, Array.Empty<string>(), new[]
            {
                new RuleDiagnostic(RuleDiagnosticKind.Skipped, RuleDiagnosticSeverity.Warning,
                    $"Nested replacement depth exceeded the limit of {MaximumNestedApplicationDepth}.", position: 0)
            });
        }

        var diagnostics = new List<RuleDiagnostic>(_configurationDiagnostics);
        var requiredUsings = new SortedSet<string>(StringComparer.Ordinal);
        CSharpTokenizationResult tokenization = CSharpLexer.Tokenize(source);
        if (tokenization.Error is not null)
        {
            diagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Malformed, RuleDiagnosticSeverity.Error,
                $"Unable to tokenize source: {tokenization.Error}", position: tokenization.ErrorPosition));
            return new ReplacementResult(source, Array.Empty<string>(), diagnostics.AsReadOnly());
        }

        var output = new System.Text.StringBuilder(source.Length);
        var appliedIds = new HashSet<string>(StringComparer.Ordinal);
        var skippedIds = new HashSet<string>(StringComparer.Ordinal);
        int copyPosition = 0;
        int tokenIndex = 0;
        IReadOnlyList<TokenData> tokens = tokenization.Tokens
            .Where(token => token.type is not CodeBlockType.LComment and not CodeBlockType.Comment)
            .ToArray();
        while (tokenIndex < tokens.Count)
        {
            bool replaced = false;
            foreach (CompiledRule rule in _rules)
            {
                if (!TryMatchAt(rule, tokens, tokenIndex, out MatchResult? match, out string? skipReason))
                {
                    if (skipReason is not null)
                    {
                        skippedIds.Add(rule.Definition.Id);
                        diagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Skipped, RuleDiagnosticSeverity.Warning,
                            skipReason, rule.Definition.Id, tokens[tokenIndex].Pos));
                    }
                    continue;
                }

                MatchResult appliedMatch = match ?? throw new InvalidOperationException("A successful match did not contain match data.");
                TokenData first = tokens[tokenIndex];
                TokenData last = tokens[appliedMatch.EndTokenIndex - 1];
                output.Append(source, copyPosition, first.Pos - copyPosition);
                var transformedCaptures = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var item in appliedMatch.Captures)
                {
                    string capturedSource = source.Substring(item.Value.Start, item.Value.End - item.Value.Start);
                    if (capturedSource.Length == source.Length)
                        continue;

                    ReplacementResult nestedResult = Apply(capturedSource, nestedDepth + 1);
                    foreach (RuleDiagnostic nestedDiagnostic in nestedResult.Diagnostics)
                    {
                        if (nestedDiagnostic.Kind != RuleDiagnosticKind.Applied &&
                            nestedDiagnostic.Severity != RuleDiagnosticSeverity.Warning)
                            continue;

                        if (nestedDiagnostic.RuleId is string nestedRuleId)
                        {
                            if (nestedDiagnostic.Kind == RuleDiagnosticKind.Applied)
                                appliedIds.Add(nestedRuleId);
                            else
                                skippedIds.Add(nestedRuleId);
                        }
                        diagnostics.Add(new RuleDiagnostic(
                            nestedDiagnostic.Kind,
                            nestedDiagnostic.Severity,
                            nestedDiagnostic.Message,
                            nestedDiagnostic.RuleId,
                            nestedDiagnostic.Position is int nestedPosition
                                ? item.Value.Start + nestedPosition
                                : null));
                    }

                    foreach (string requiredUsing in nestedResult.RequiredUsings)
                        requiredUsings.Add(requiredUsing);
                    if (!string.Equals(capturedSource, nestedResult.Source, StringComparison.Ordinal))
                        transformedCaptures.Add(item.Key, nestedResult.Source);
                }
                output.Append(Render(rule, appliedMatch.Captures, source, transformedCaptures));
                copyPosition = last.End;
                tokenIndex = appliedMatch.EndTokenIndex;
                appliedIds.Add(rule.Definition.Id);
                foreach (string requiredUsing in rule.Definition.RequiredUsings)
                    requiredUsings.Add(requiredUsing);
                diagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Applied, RuleDiagnosticSeverity.Information,
                    "Rule applied.", rule.Definition.Id, first.Pos));
                replaced = true;
                break;
            }

            if (!replaced)
                tokenIndex++;
        }

        output.Append(source, copyPosition, source.Length - copyPosition);
        foreach (CompiledRule rule in _rules)
        {
            string id = rule.Definition.Id;
            if (!appliedIds.Contains(id) && !skippedIds.Contains(id))
                diagnostics.Add(new RuleDiagnostic(RuleDiagnosticKind.Skipped, RuleDiagnosticSeverity.Information,
                    "No matching source template was found.", id));
        }
        return new ReplacementResult(output.ToString(), requiredUsings.ToArray(), diagnostics.AsReadOnly());
    }

    private static bool ValidateUsings(ReplacementRule rule, out string? error)
    {
        error = null;
        if (rule.RequiredUsings is null)
        {
            error = $"Rule '{rule.Id}' must use an array for requiredUsings.";
            return false;
        }
        foreach (string? requiredUsing in rule.RequiredUsings)
        {
            if (string.IsNullOrWhiteSpace(requiredUsing) || requiredUsing.Trim() != requiredUsing ||
                requiredUsing.Contains(';') || requiredUsing.Contains('\r') || requiredUsing.Contains('\n'))
            {
                error = $"Rule '{rule.Id}' contains an invalid required using directive.";
                return false;
            }
        }
        return true;
    }

    private static bool TryMatchAt(CompiledRule rule, IReadOnlyList<TokenData> sourceTokens, int start, out MatchResult? result, out string? skipReason)
    {
        var captures = new Dictionary<string, CapturedExpression>(StringComparer.Ordinal);
        bool matched = MatchParts(rule, sourceTokens, 0, start, captures, out int end, out skipReason);
        result = matched ? new MatchResult(end, captures) : null;
        return matched;
    }

    private static bool MatchParts(CompiledRule rule, IReadOnlyList<TokenData> tokens, int partIndex, int sourceIndex,
        Dictionary<string, CapturedExpression> captures, out int end, out string? skipReason)
    {
        end = sourceIndex;
        skipReason = null;
        if (partIndex == rule.SourceParts.Count)
            return true;

        TemplatePart part = rule.SourceParts[partIndex];
        if (!part.IsPlaceholder)
        {
            if (part.Literal is not TokenData literal)
                throw new InvalidOperationException("Literal template part has no token.");
            if (sourceIndex >= tokens.Count || tokens[sourceIndex].Code != literal.Code)
                return false;
            return MatchParts(rule, tokens, partIndex + 1, sourceIndex + 1, captures, out end, out skipReason);
        }

        TokenData? nextLiteral = partIndex + 1 < rule.SourceParts.Count ? rule.SourceParts[partIndex + 1].Literal : null;
        string? firstTypeError = null;
        foreach (int candidateEnd in FindCandidateEnds(tokens, sourceIndex, nextLiteral))
        {
            if (candidateEnd <= sourceIndex || !IsBalancedRange(tokens, sourceIndex, candidateEnd))
                continue;
            if (part.IsBoolean && !IsBooleanExpression(tokens, sourceIndex, candidateEnd))
            {
                firstTypeError ??= $"Placeholder '{part.PlaceholderName}' requires a syntactically boolean expression; the captured expression's type is unknown.";
                continue;
            }

            string placeholderName = part.PlaceholderName ?? throw new InvalidOperationException("Placeholder template part has no name.");
            TokenData first = tokens[sourceIndex];
            TokenData last = tokens[candidateEnd - 1];
            var candidateCaptures = new Dictionary<string, CapturedExpression>(captures, StringComparer.Ordinal)
            {
                [placeholderName] = new CapturedExpression(first.Pos, last.End)
            };
            if (MatchParts(rule, tokens, partIndex + 1, candidateEnd, candidateCaptures, out end, out string? nestedSkip))
            {
                captures.Clear();
                foreach (var item in candidateCaptures)
                    captures.Add(item.Key, item.Value);
                return true;
            }
            firstTypeError ??= nestedSkip;
        }

        skipReason = firstTypeError;
        return false;
    }

    private static IEnumerable<int> FindCandidateEnds(IReadOnlyList<TokenData> tokens, int start, TokenData? nextLiteral)
    {
        var stack = new Stack<string>();
        for (int index = start; index <= tokens.Count; index++)
        {
            if (stack.Count == 0)
            {
                if (nextLiteral is TokenData literal)
                {
                    if (index < tokens.Count && tokens[index].Code == literal.Code)
                        yield return index;
                    else if (index < tokens.Count && IsExpressionBoundary(tokens[index].Code))
                        yield break;
                }
                else if (index == tokens.Count || IsExpressionBoundary(tokens[index].Code))
                {
                    yield return index;
                    yield break;
                }
            }

            if (index == tokens.Count)
                yield break;
            string current = tokens[index].Code;
            if (TryGetClosing(current, out string? closing))
            {
                stack.Push(closing);
            }
            else if (IsClosing(current))
            {
                if (stack.Count == 0)
                    yield break;
                if (stack.Pop() != current)
                    yield break;
            }
        }
    }
    private static bool IsExpressionBoundary(string token) => token is ";" or "," or ")" or "]" or "}";

    private static bool IsBalancedRange(IReadOnlyList<TokenData> tokens, int start, int end)
    {
        var stack = new Stack<string>();
        for (int index = start; index < end; index++)
        {
            string current = tokens[index].Code;
            if (TryGetClosing(current, out string? closing))
                stack.Push(closing);
            else if (IsClosing(current) && (stack.Count == 0 || stack.Pop() != current))
                return false;
        }
        return stack.Count == 0;
    }

    private static bool IsBooleanExpression(IReadOnlyList<TokenData> tokens, int start, int end)
    {
        while (end - start >= 2 && tokens[start].Code == "(" && EnclosesEntireRange(tokens, start, end))
        {
            start++;
            end--;
        }
        if (start >= end)
            return false;
        if (end - start == 1 && tokens[start].type == CodeBlockType.Identifier && tokens[start].Code is "true" or "false")
            return true;
        if (tokens[start].Code == "!")
            return end - start > 1;

        var stack = new Stack<string>();
        for (int index = start; index < end; index++)
        {
            string current = tokens[index].Code;
            if (stack.Count == 0 && IsBooleanOperator(current))
                return index > start && index + 1 < end;
            if (TryGetClosing(current, out string? closing))
                stack.Push(closing);
            else if (IsClosing(current) && stack.Count > 0)
                stack.Pop();
        }
        return false;
    }

    private static bool IsBooleanOperator(string token) => token is "&&" or "||" or "&" or "|" or "^" or "==" or "!=" or "<" or ">" or "<=" or ">=" or "is";

    private static bool EnclosesEntireRange(IReadOnlyList<TokenData> tokens, int start, int end)
    {
        int depth = 0;
        for (int index = start; index < end; index++)
        {
            if (tokens[index].Code == "(")
                depth++;
            else if (tokens[index].Code == ")")
            {
                depth--;
                if (depth == 0 && index != end - 1)
                    return false;
            }
        }
        return depth == 0;
    }

    private static string Render(CompiledRule rule, Dictionary<string, CapturedExpression> captures, string source,
        Dictionary<string, string> transformedCaptures)
    {
        var output = new System.Text.StringBuilder();
        foreach (ReplacementSegment segment in rule.ReplacementSegments)
        {
            if (segment.PlaceholderName is string name)
            {
                if (transformedCaptures.TryGetValue(name, out string? transformed))
                {
                    output.Append(transformed);
                }
                else
                {
                    CapturedExpression expression = captures[name];
                    output.Append(source, expression.Start, expression.End - expression.Start);
                }
            }
            else
            {
                output.Append(segment.Literal);
            }
        }
        return output.ToString();
    }

    private static bool TryGetClosing(string token, out string closing)
    {
        closing = token switch { "(" => ")", "[" => "]", "{" => "}", _ => string.Empty };
        return closing.Length != 0;
    }

    private static bool IsClosing(string token) => token is ")" or "]" or "}";

    private sealed class CapturedExpression
    {
        public CapturedExpression(int start, int end)
        {
            Start = start;
            End = end;
        }

        public int Start { get; }
        public int End { get; }
        public string? SourceText { get; set; }
    }

    private sealed class MatchResult
    {
        public MatchResult(int endTokenIndex, Dictionary<string, CapturedExpression> captures)
        {
            EndTokenIndex = endTokenIndex;
            Captures = captures;
        }

        public int EndTokenIndex { get; }
        public Dictionary<string, CapturedExpression> Captures { get; }
    }
}



