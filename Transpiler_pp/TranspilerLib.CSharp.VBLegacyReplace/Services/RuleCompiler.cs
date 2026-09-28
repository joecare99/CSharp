using System;
using System.Collections.Generic;
using TranspilerLib.Data;
using TranspilerLib.Models.Scanner;
using TranspilerLib.CSharp.VBLegacyReplace.Models;

namespace TranspilerLib.CSharp.VBLegacyReplace.Services;

internal static class RuleCompiler
{
    public static bool TryCompile(ReplacementRule rule, out CompiledRule? compiled, out string? error)
    {
        compiled = null;
        error = null;
        if (string.IsNullOrWhiteSpace(rule.Id))
            return Fail("Rule ID must not be empty.", out error);
        if (string.IsNullOrWhiteSpace(rule.SourceTemplate))
            return Fail("Source template must not be empty.", out error);
        if (rule.ReplacementTemplate is null)
            return Fail("Replacement template must not be null.", out error);

        if (!TryParseSegments(rule.SourceTemplate, out List<ReplacementSegment> sourceSegments, out error) ||
            !TryParseSegments(rule.ReplacementTemplate, out List<ReplacementSegment> replacementSegments, out error))
            return false;

        var sourceParts = new List<TemplatePart>();
        var declarations = new Dictionary<string, bool>(StringComparer.Ordinal);
        int fixedTokenCount = 0;
        foreach (ReplacementSegment segment in sourceSegments)
        {
            if (segment.PlaceholderName is string name)
            {
                if (declarations.ContainsKey(name))
                    return Fail($"Placeholder '{name}' occurs more than once in the source template.", out error);
                declarations.Add(name, segment.Literal == "bool");
                sourceParts.Add(TemplatePart.ForPlaceholder(name, segment.Literal == "bool"));
                continue;
            }

            CSharpTokenizationResult tokenized = CSharpLexer.Tokenize(segment.Literal ?? string.Empty);
            if (tokenized.Error is not null)
                return Fail($"Invalid source template: {tokenized.Error}", out error);
            foreach (TokenData token in tokenized.Tokens)
            {
                sourceParts.Add(TemplatePart.ForLiteral(token));
                fixedTokenCount++;
            }
        }

        foreach (ReplacementSegment segment in replacementSegments)
        {
            if (segment.PlaceholderName is not string name)
                continue;
            if (!declarations.TryGetValue(name, out bool isBoolean))
                return Fail($"Replacement references undeclared placeholder '{name}'.", out error);
            if (segment.Literal is not null && (segment.Literal == "bool") != isBoolean)
                return Fail($"Placeholder '{name}' must use the same type in both templates.", out error);
        }

        if (sourceParts.Count == 0)
            return Fail("Source template must contain at least one token.", out error);
        if (HasAdjacentPlaceholders(sourceParts))
            return Fail("Adjacent placeholders are ambiguous; place a literal token between them.", out error);
        if (!HasBalancedTemplateDelimiters(sourceParts))
            return Fail("Source template has unbalanced parentheses, brackets, or braces.", out error);

        compiled = new CompiledRule(rule, sourceParts, replacementSegments, fixedTokenCount);
        return true;
    }

    private static bool TryParseSegments(string template, out List<ReplacementSegment> segments, out string? error)
    {
        segments = new List<ReplacementSegment>();
        error = null;
        CSharpTokenizationResult tokenized = CSharpLexer.Tokenize(template);
        if (tokenized.Error is not null)
            return Fail($"Invalid template: {tokenized.Error}", out error);

        var placeholders = new List<(int Start, int End, string Name, bool IsBoolean)>();
        foreach (TokenData token in tokenized.Tokens)
        {
            if (token.Code != "<")
                continue;
            if (!TryParsePlaceholder(template, token.Pos, out int end, out string? name, out bool isBoolean, out bool malformed))
            {
                if (malformed)
                    return Fail($"Malformed placeholder at character {token.Pos}.", out error);
                continue;
            }
            if (malformed)
                return Fail($"Malformed placeholder at character {token.Pos}.", out error);
            if (name is null)
                return Fail($"Placeholder at character {token.Pos} has no name.", out error);
            placeholders.Add((token.Pos, end, name, isBoolean));
        }

        int literalStart = 0;
        foreach ((int start, int end, string name, bool isBoolean) in placeholders)
        {
            if (start > literalStart)
                segments.Add(ReplacementSegment.ForLiteral(template.Substring(literalStart, start - literalStart)));
            segments.Add(new ReplacementSegment(isBoolean ? "bool" : null, name));
            literalStart = end;
        }
        if (literalStart < template.Length)
            segments.Add(ReplacementSegment.ForLiteral(template.Substring(literalStart)));
        if (segments.Count == 0)
            segments.Add(ReplacementSegment.ForLiteral(string.Empty));
        return true;
    }
    private static bool TryParsePlaceholder(string text, int start, out int end, out string? name, out bool isBoolean, out bool malformed)
    {
        end = start;
        name = null;
        isBoolean = false;
        malformed = false;
        int close = text.IndexOf('>', start + 1);
        if (close < 0)
        {
            if (text.Length - start >= 6 && string.CompareOrdinal(text, start, "<Param", 0, 6) == 0)
            {
                malformed = true;
                return true;
            }
            return false;
        }
        string body = text.Substring(start + 1, close - start - 1);
        if (!body.StartsWith("Param", StringComparison.Ordinal))
            return false;
        int cursor = 5;
        int digitStart = cursor;
        while (cursor < body.Length && char.IsDigit(body[cursor]))
            cursor++;
        if (cursor == digitStart)
        {
            malformed = true;
            return true;
        }

        string? type = null;
        if (cursor < body.Length)
        {
            if (body[cursor] != ':')
            {
                malformed = true;
                return true;
            }
            type = body.Substring(cursor + 1);
            if (type != "bool")
            {
                malformed = true;
                return true;
            }
        }

        end = close + 1;
        name = body.Substring(0, cursor);
        isBoolean = type == "bool";
        return true;
    }

    private static bool HasAdjacentPlaceholders(List<TemplatePart> parts)
    {
        for (int index = 1; index < parts.Count; index++)
            if (parts[index - 1].IsPlaceholder && parts[index].IsPlaceholder)
                return true;
        return false;
    }

    private static bool HasBalancedTemplateDelimiters(List<TemplatePart> parts)
    {
        var stack = new Stack<string>();
        foreach (TemplatePart part in parts)
        {
            if (part.Literal is not TokenData token)
                continue;
            if (TryGetClosing(token.Code, out string? closing))
            {
                stack.Push(closing);
            }
            else if (IsClosing(token.Code))
            {
                if (stack.Count == 0 || stack.Pop() != token.Code)
                    return false;
            }
        }
        return stack.Count == 0;
    }

    private static bool TryGetClosing(string token, out string closing)
    {
        closing = token switch { "(" => ")", "[" => "]", "{" => "}", _ => string.Empty };
        return closing.Length != 0;
    }

    private static bool IsClosing(string token) => token is ")" or "]" or "}";

    private static bool Fail(string message, out string? error)
    {
        error = message;
        return false;
    }
}











