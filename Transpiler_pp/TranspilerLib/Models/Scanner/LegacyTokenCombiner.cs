using System;
using System.Collections.Generic;
using System.Text;
using TranspilerLib.Data;

namespace TranspilerLib.Models.Scanner;

/// <summary>
/// Converts fine-grained lexical tokens into the semantic token stream consumed by legacy code builders.
/// </summary>
public static class LegacyTokenCombiner
{
    /// <summary>
    /// Combines lexical tokens into operations, labels, blocks, comments, strings, and goto statements.
    /// </summary>
    /// <param name="tokens">The lexical tokens to combine.</param>
    /// <returns>The compatible legacy token stream.</returns>
    public static IEnumerable<TokenData> Combine(IEnumerable<TokenData> tokens)
    {
        if (tokens is null)
            throw new ArgumentNullException(nameof(tokens));

        var operation = new StringBuilder();
        var conditionalDepth = 0;
        var operationLevel = 0;
        var operationPosition = 0;

        foreach (TokenData token in tokens)
        {
            if (token.type == CodeBlockType.Block)
            {
                foreach (TokenData combined in FlushOperation())
                    yield return combined;

                yield return CreateToken(token.Code, CodeBlockType.Block, token.Level, token.Pos);
                continue;
            }

            if (token.type is CodeBlockType.Comment or CodeBlockType.LComment)
            {
                foreach (TokenData combined in FlushOperation())
                    yield return combined;

                yield return CreateToken(token.Code, token.type, token.Level, token.Pos);
                continue;
            }

            if (token.type == CodeBlockType.String)
            {
                foreach (TokenData combined in FlushOperation())
                    yield return combined;

                yield return CreateToken(token.Code, CodeBlockType.String, token.Level, token.Pos);
                continue;
            }

            string current = operation.ToString().Trim();
            if (operation.Length > 0 && current == "else" && token.Code == "if")
            {
                foreach (TokenData combined in FlushOperation())
                    yield return combined;
            }

            if (operation.Length == 0)
            {
                operationLevel = token.Level;
                operationPosition = token.Pos;
            }
            else if (token.LeadingTrivia.Length > 0)
            {
                operation.Append(' ');
            }

            operation.Append(token.Code);

            if (token.Code == "?")
                conditionalDepth++;
            else if (token.Code == ":" && conditionalDepth > 0)
                conditionalDepth--;

            if (token.Code == ";")
            {
                foreach (TokenData combined in FlushOperation())
                    yield return combined;
            }
            else if (token.Code == ":" && conditionalDepth == 0)
            {
                string code = operation.ToString();
                operation.Clear();
                yield return CreateToken(code, CodeBlockType.Label, operationLevel, operationPosition);
            }
        }

        foreach (TokenData combined in FlushOperation())
            yield return combined;

        IEnumerable<TokenData> FlushOperation()
        {
            if (operation.Length == 0)
                yield break;

            string code = operation.ToString();
            operation.Clear();
            conditionalDepth = 0;
            CodeBlockType type = code.TrimStart().StartsWith("goto ", StringComparison.Ordinal)
                ? CodeBlockType.Goto
                : CodeBlockType.Operation;
            yield return CreateToken(code, type, operationLevel, operationPosition);
        }
    }

    private static TokenData CreateToken(string code, CodeBlockType type, int level, int position)
        => new(code, type, level, position) { Length = code.Length };
}
