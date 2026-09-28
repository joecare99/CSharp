using System;
using System.Collections.Generic;
using TranspilerLib.Data;

namespace TranspilerLib.Models.Scanner;

/// <summary>Tokenizes C# source into fine-grained lexical tokens with source spans.</summary>
public static class CSharpLexer
{
    private static readonly string[] Operators =
    {
        ">>>=", ">>=", "<<=", "??=", ">>>", ">>", "<<", "=>", "==", "!=", "<=", ">=", "&&", "||",
        "++", "--", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "??", "??=", "?.", "::", "->", 
        ".."
    };

    /// <summary>Tokenizes one C# source string, returning tokens and an optional malformed-input diagnostic.</summary>
    public static CSharpTokenizationResult Tokenize(string source)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        var tokens = new List<TokenData>();
        int position = 0;
        int blockLevel = 0;
        while (position < source.Length)
        {
            int triviaStart = position;
            while (position < source.Length && char.IsWhiteSpace(source[position]))
                position++;
            string leadingTrivia = source.Substring(triviaStart, position - triviaStart);
            if (position >= source.Length)
                break;

            int start = position;
            CodeBlockType type;

            if (source[position] == '/' && Peek(source, position + 1) == '/')
            {
                position += 2;
                while (position < source.Length && source[position] is not '\r' and not '\n')
                    position++;
                type = CodeBlockType.LComment;
            }
            else if (source[position] == '/' && Peek(source, position + 1) == '*')
            {
                int end = source.IndexOf("*/", position + 2, StringComparison.Ordinal);
                if (end < 0)
                    return new CSharpTokenizationResult(tokens, "Unterminated block comment.", start);
                position = end + 2;
                type = CodeBlockType.Comment;
            }
            else if (TryScanQuotedLiteral(source, position, out int literalEnd, out type, out string? literalError))
            {
                if (literalError is not null)
                    return new CSharpTokenizationResult(tokens, literalError, start);
                position = literalEnd;
                type = CodeBlockType.String;
            }
            else if (IsIdentifierStart(source[position]))
            {
                position++;
                while (position < source.Length && IsIdentifierPart(source[position]))
                    position++;
                type = CodeBlockType.Identifier;
            }
            else if (char.IsDigit(source[position]))
            {
                position = ScanNumber(source, position);
                type = CodeBlockType.Number;
            }
            else
            {
                string? matchedOperator = MatchOperator(source, position);
                if (matchedOperator is not null)
                {
                    position += matchedOperator.Length;
                    type = CodeBlockType.Operator;
                }
                else
                {
                    char current = source[position++];
                    type = current is '+' or '-' or '*' or '/' or '%' or '&' or '|' or '^' or '!' or '~' or '=' or '<' or '>' or '?'
                        ? CodeBlockType.Operator
                        : CodeBlockType.Punctuation;
                    type = current switch
                    {
                        '{' or '}' => CodeBlockType.Block,
                        '(' or ')' or '[' or ']' => CodeBlockType.Bracket,
                        ';' or ',' => CodeBlockType.Separator,
                        ':' => CodeBlockType.Label,
                        _ => type
                    };
                    if (current == '{')
                        blockLevel++;
                }
            }

            int tokenLevel = type == CodeBlockType.Punctuation && source[start] == '}'
                ? Math.Max(0, blockLevel - 1)
                : blockLevel;
            tokens.Add(new TokenData(source.Substring(start, position - start), type, tokenLevel, start)
            {
                Length = position - start,
                LeadingTrivia = leadingTrivia
            });

            if (type == CodeBlockType.Punctuation && source[start] == '}')
                blockLevel = Math.Max(0, blockLevel - 1);
        }

        return new CSharpTokenizationResult(tokens);
    }

    private static char Peek(string source, int position) => position < source.Length ? source[position] : '\0';

    private static string? MatchOperator(string source, int position)
    {
        foreach (string candidate in Operators)
        {
            if (position + candidate.Length <= source.Length &&
                string.CompareOrdinal(source, position, candidate, 0, candidate.Length) == 0)
            {
                return candidate;
            }
        }

        return null;
    }

    private static int ScanNumber(string source, int start)
    {
        int position = start + 1;
        while (position < source.Length &&
            (char.IsLetterOrDigit(source[position]) || source[position] == '_' ||
             source[position] == '.' && Peek(source, position + 1) != '.'))
        {
            position++;
        }

        return position;
    }

    private static bool TryScanQuotedLiteral(string source, int start, out int end, out CodeBlockType kind, out string? error)
    {
        end = start;
        kind = CodeBlockType.Unknown;
        error = null;

        int quotePosition = start;
        bool verbatim = false;
        bool interpolated = false;
        while (quotePosition < source.Length && source[quotePosition] is '$' or '@')
        {
            if (source[quotePosition] == '@')
                verbatim = true;
            else
                interpolated = true;
            quotePosition++;
        }

        if (quotePosition >= source.Length || source[quotePosition] is not '"' and not '\'')
            return false;

        char quote = source[quotePosition];
        kind = quote == '"' ? CodeBlockType.String : CodeBlockType.Character;
        int quoteCount = 1;
        if (quote == '"')
        {
            while (quotePosition + quoteCount < source.Length && source[quotePosition + quoteCount] == '"')
                quoteCount++;
        }

        if (quote == '"' && quoteCount >= 3)
        {
            int cursor = quotePosition + quoteCount;
            while (cursor < source.Length)
            {
                if (source[cursor] == '"' && CountRepeated(source, cursor, '"') >= quoteCount)
                {
                    end = cursor + quoteCount;
                    return true;
                }
                cursor++;
            }

            error = "Unterminated raw string literal.";
            return true;
        }

        if (interpolated && quote == '"')
        {
            int interpolatedEnd = ScanInterpolatedString(source, quotePosition, verbatim);
            if (interpolatedEnd < 0)
            {
                error = "Unterminated interpolated string literal.";
                return true;
            }
            end = interpolatedEnd;
            return true;
        }

        int index = quotePosition + 1;
        while (index < source.Length)
        {
            char current = source[index];
            if (!verbatim && current == '\\')
            {
                index += 2;
                continue;
            }

            if (current == quote)
            {
                if (verbatim && index + 1 < source.Length && source[index + 1] == quote)
                {
                    index += 2;
                    continue;
                }
                end = index + 1;
                return true;
            }

            if (quote == '\'' && current is '\r' or '\n')
            {
                error = "Unterminated character literal.";
                return true;
            }
            if (quote == '"' && !verbatim && current is '\r' or '\n')
            {
                error = "Unterminated string literal.";
                return true;
            }
            index++;
        }

        error = quote == '"' ? "Unterminated string literal." : "Unterminated character literal.";
        return true;
    }

    private static int ScanInterpolatedString(string source, int quotePosition, bool verbatim)
    {
        int position = quotePosition + 1;
        while (position < source.Length)
        {
            char current = source[position];
            if (!verbatim && current == '\\')
            {
                position += 2;
                continue;
            }

            if (current == '"')
            {
                if (verbatim && Peek(source, position + 1) == '"')
                {
                    position += 2;
                    continue;
                }
                return position + 1;
            }

            if (current == '{')
            {
                if (Peek(source, position + 1) == '{')
                {
                    position += 2;
                    continue;
                }

                position = ScanInterpolationExpression(source, position + 1);
                if (position < 0)
                    return -1;
                continue;
            }

            if (current == '}' && Peek(source, position + 1) == '}')
            {
                position += 2;
                continue;
            }

            position++;
        }

        return -1;
    }

    private static int ScanInterpolationExpression(string source, int position)
    {
        int braceDepth = 1;
        while (position < source.Length)
        {
            char current = source[position];
            if (current == '/' && Peek(source, position + 1) == '/')
            {
                position += 2;
                while (position < source.Length && source[position] is not '\r' and not '\n')
                    position++;
                continue;
            }

            if (current == '/' && Peek(source, position + 1) == '*')
            {
                int commentEnd = source.IndexOf("*/", position + 2, StringComparison.Ordinal);
                if (commentEnd < 0)
                    return -1;
                position = commentEnd + 2;
                continue;
            }

            if (current is '"' or '\'')
            {
                if (!TryScanQuotedLiteral(source, position, out int nestedEnd, out _, out string? nestedError) ||
                    nestedError is not null)
                    return -1;
                position = nestedEnd;
                continue;
            }

            if (current == '{')
                braceDepth++;
            else if (current == '}' && --braceDepth == 0)
                return position + 1;

            position++;
        }

        return -1;
    }

    private static int CountRepeated(string source, int position, char value)
    {
        int count = 0;
        while (position + count < source.Length && source[position + count] == value)
            count++;
        return count;
    }

    private static bool IsIdentifierStart(char value) => char.IsLetter(value) || value is '_' or '@';
    private static bool IsIdentifierPart(char value) => char.IsLetterOrDigit(value) || value == '_';
}

/// <summary>Contains fine-grained C# tokens and any lexical error encountered.</summary>
public sealed class CSharpTokenizationResult
{
    internal CSharpTokenizationResult(List<TokenData> tokens, string? error = null, int? errorPosition = null)
    {
        Tokens = tokens.AsReadOnly();
        Error = error;
        ErrorPosition = errorPosition;
    }

    /// <summary>Gets tokens emitted before a lexical error or end of source.</summary>
    public IReadOnlyList<TokenData> Tokens { get; }

    /// <summary>Gets the lexical error message, if any.</summary>
    public string? Error { get; }

    /// <summary>Gets the zero-based source offset of the lexical error, if any.</summary>
    public int? ErrorPosition { get; }
}
