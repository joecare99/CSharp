using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TranspilerLib.Data;
using TranspilerLib.IEC.Models.Scanner;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Creates typed IEC compilation units from frontend source text by combining declaration extraction
/// with the existing IEC scanner and code-block mapping pipeline for supported implementation statements.
/// This keeps text parsing in the IEC frontend project while shared semantics remain free of scanner dependencies.
/// </summary>
public static class IecFrontendCompilationUnitFactory
{
    /// <summary>
    /// Creates a typed compilation unit from declaration and implementation text.
    /// </summary>
    /// <param name="declarationText">The raw declaration text.</param>
    /// <param name="implementationText">The raw implementation text.</param>
    /// <returns>The typed IEC compilation unit.</returns>
    public static IecCompilationUnit CreateFromSourceText(string declarationText, string implementationText)
    {
        var declarations = IecDeclarationExtractor.ExtractDeclarations(declarationText ?? string.Empty);
        var header = IecDeclarationExtractor.ExtractDeclarationHeader(declarationText ?? string.Empty);
        var statements = NormalizeFunctionResultStatements(ExtractStatementsFromSourceText(implementationText ?? string.Empty), header);
        return new IecCompilationUnit(declarations, statements, header.ArtifactMetadata, header.SourcePos);
    }

    private static IReadOnlyList<IecStatement> NormalizeFunctionResultStatements(IEnumerable<IecStatement> statements, IecDeclarationHeader header)
    {
        return statements
            .Select(statement => NormalizeFunctionResultStatement(statement, header))
            .ToArray();
    }

    private static IecStatement NormalizeFunctionResultStatement(IecStatement statement, IecDeclarationHeader header)
    {
        if (header.ArtifactMetadata.ArtifactKind == IecArtifactKind.Function
            && !string.IsNullOrWhiteSpace(header.ArtifactName)
            && statement is IecAssignmentStatement assignmentStatement
            && string.Equals(assignmentStatement.Target.Identifier, header.ArtifactName, StringComparison.OrdinalIgnoreCase))
        {
            return new IecReturnStatement(assignmentStatement.Value, assignmentStatement.SourcePos);
        }

        if (statement is IecIfStatement ifStatement)
        {
            return new IecIfStatement(
                ifStatement.Condition,
                NormalizeFunctionResultStatements(ifStatement.ThenStatements, header),
                NormalizeFunctionResultStatements(ifStatement.ElseStatements, header),
                ifStatement.SourcePos);
        }

        return statement;
    }

    private static IReadOnlyList<IecStatement> ExtractStatementsFromSourceText(string implementationText)
    {
        var statements = new List<IecStatement>();
        foreach (var segment in SplitImplementationIntoStatementSegments(implementationText))
        {
            if (TryParseStatementSegment(segment, out var parsedStatement))
            {
                statements.Add(parsedStatement);
                continue;
            }

            var parser = new IECCode { OriginalCode = segment };
            var root = parser.Parse();
            FlattenSeparatorBlocks(root);
            statements.AddRange(IecAstMapper.ExtractStatements(root.SubBlocks));
        }

        return statements;
    }

    private static bool TryParseStatementSegment(string segment, out IecStatement parsedStatement)
    {
        parsedStatement = null!;
        var relevantLines = GetRelevantLines(segment);
        if (relevantLines.Count == 0)
        {
            return false;
        }

        if (StartsWithKeyword(relevantLines[0], "IF"))
        {
            return TryParseIfStatementSegment(relevantLines, out parsedStatement);
        }

        if (relevantLines.Count != 1)
        {
            return false;
        }

        return TryParseSimpleStatement(relevantLines[0], out parsedStatement);
    }

    private static IReadOnlyList<string> GetRelevantLines(string segment)
    {
        return segment
            .Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None)
            .Select(RemoveLineComment)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();
    }

    private static string RemoveLineComment(string line)
    {
        var commentIndex = line.IndexOf("//", StringComparison.Ordinal);
        return commentIndex >= 0 ? line[..commentIndex] : line;
    }

    private static bool TryParseIfStatementSegment(IReadOnlyList<string> lines, out IecStatement parsedStatement)
    {
        parsedStatement = null!;
        if (lines.Count < 2 || lines.Any(line => StartsWithKeyword(line, "ELSIF")))
        {
            return false;
        }

        var header = lines[0];
        var thenIndex = header.LastIndexOf("THEN", StringComparison.OrdinalIgnoreCase);
        if (thenIndex < 0)
        {
            return false;
        }

        var conditionText = header[2..thenIndex].Trim();
        if (!TryParseExpressionText(conditionText, out var condition))
        {
            return false;
        }

        var thenStatements = new List<IecStatement>();
        var elseStatements = new List<IecStatement>();
        var targetList = thenStatements;

        for (var i = 1; i < lines.Count; i++)
        {
            var line = lines[i];
            if (StartsWithKeyword(line, "ELSE"))
            {
                targetList = elseStatements;
                continue;
            }

            if (StartsWithKeyword(line, "END_IF"))
            {
                parsedStatement = new IecIfStatement(condition, thenStatements, elseStatements);
                return true;
            }

            if (StartsWithKeyword(line, "IF"))
            {
                var nestedLines = CollectNestedIfSegment(lines, ref i);
                if (!TryParseIfStatementSegment(nestedLines, out var nestedStatement))
                {
                    return false;
                }

                targetList.Add(nestedStatement);
                continue;
            }

            if (!TryParseSimpleStatement(line, out var childStatement))
            {
                return false;
            }

            targetList.Add(childStatement);
        }

        return false;
    }

    private static IReadOnlyList<string> CollectNestedIfSegment(IReadOnlyList<string> lines, ref int index)
    {
        var nestedLines = new List<string>();
        var iIfDepth = 0;

        for (; index < lines.Count; index++)
        {
            var line = lines[index];
            nestedLines.Add(line);

            if (StartsWithKeyword(line, "IF"))
            {
                iIfDepth++;
            }

            if (!StartsWithKeyword(line, "END_IF"))
            {
                continue;
            }

            iIfDepth--;
            if (iIfDepth == 0)
            {
                return nestedLines;
            }
        }

        return nestedLines;
    }

    private static bool TryParseSimpleStatement(string line, out IecStatement parsedStatement)
    {
        parsedStatement = null!;
        if (TryParseAssignmentStatement(line, out var assignmentStatement))
        {
            parsedStatement = assignmentStatement;
            return true;
        }

        if (TryParseReturnStatement(line, out var returnStatement))
        {
            parsedStatement = returnStatement;
            return true;
        }

        return false;
    }

    private static bool TryParseAssignmentStatement(string line, out IecAssignmentStatement assignmentStatement)
    {
        assignmentStatement = null!;
        if (!line.EndsWith(";", StringComparison.Ordinal))
        {
            return false;
        }

        var content = line[..^1].Trim();
        var assignmentIndex = content.IndexOf(":=", StringComparison.Ordinal);
        if (assignmentIndex <= 0)
        {
            return false;
        }

        var targetText = content[..assignmentIndex].Trim();
        var valueText = content[(assignmentIndex + 2)..].Trim();
        if (string.IsNullOrWhiteSpace(targetText)
            || string.IsNullOrWhiteSpace(valueText)
            || !TryParseExpressionText(valueText, out var expression))
        {
            return false;
        }

        assignmentStatement = new IecAssignmentStatement(new IecIdentifierExpression(targetText), expression);
        return true;
    }

    private static bool TryParseReturnStatement(string line, out IecReturnStatement returnStatement)
    {
        returnStatement = null!;
        if (!StartsWithKeyword(line, "RETURN"))
        {
            return false;
        }

        var expressionText = line["RETURN".Length..].Trim().TrimEnd(';').Trim();
        if (!TryParseExpressionText(expressionText, out var expression))
        {
            return false;
        }

        returnStatement = new IecReturnStatement(expression);
        return true;
    }

    private static bool TryParseExpressionText(string expressionText, out IecExpression expression)
    {
        expression = null!;
        if (string.IsNullOrWhiteSpace(expressionText))
        {
            return false;
        }

        try
        {
            var parser = new StructuredTextExpressionParser(expressionText);
            expression = parser.Parse();
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static IEnumerable<string> SplitImplementationIntoStatementSegments(string implementationText)
    {
        var currentSegment = new StringBuilder();
        var iIfDepth = 0;

        foreach (var rawLine in implementationText.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None))
        {
            if (string.IsNullOrWhiteSpace(rawLine))
            {
                continue;
            }

            var trimmedLine = rawLine.Trim();
            var relevantLine = RemoveLineComment(rawLine).TrimEnd();
            AppendSegmentLine(currentSegment, rawLine);

            if (StartsWithKeyword(trimmedLine, "IF"))
            {
                iIfDepth++;
            }

            if (StartsWithKeyword(trimmedLine, "END_IF"))
            {
                iIfDepth = Math.Max(0, iIfDepth - 1);
                if (iIfDepth == 0)
                {
                    yield return currentSegment.ToString();
                    currentSegment.Clear();
                }

                continue;
            }

            if (iIfDepth > 0)
            {
                continue;
            }

            if (relevantLine.EndsWith(";", StringComparison.Ordinal))
            {
                yield return currentSegment.ToString();
                currentSegment.Clear();
            }
        }

        if (currentSegment.Length > 0)
        {
            yield return currentSegment.ToString();
        }
    }

    private static void AppendSegmentLine(StringBuilder currentSegment, string line)
    {
        if (currentSegment.Length > 0)
        {
            currentSegment.AppendLine();
        }

        currentSegment.Append(line);
    }

    private static bool StartsWithKeyword(string line, string keyword)
        => line.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
            && (line.Length == keyword.Length || char.IsWhiteSpace(line[keyword.Length]));

    private static void FlattenSeparatorBlocks(ICodeBlock parent)
    {
        for (var i = 0; i < parent.SubBlocks.Count; i++)
        {
            FlattenSeparatorBlocks(parent.SubBlocks[i]);
        }

        for (var i = 0; i < parent.SubBlocks.Count; i++)
        {
            if (!IsTransparentSeparator(parent.SubBlocks[i]))
            {
                continue;
            }

            var separator = parent.SubBlocks[i];
            var childBlocks = separator.SubBlocks.ToList();
            parent.SubBlocks.RemoveAt(i);

            for (var j = 0; j < childBlocks.Count; j++)
            {
                parent.SubBlocks.Insert(i + j, childBlocks[j]);
            }

            i--;
        }
    }

    private static void CollapseExpressionContinuationSiblings(ICodeBlock parent)
    {
        for (var i = 0; i < parent.SubBlocks.Count; i++)
        {
            CollapseExpressionContinuationSiblings(parent.SubBlocks[i]);
        }

        for (var i = 0; i < parent.SubBlocks.Count; i++)
        {
            if (parent.SubBlocks[i].Type != CodeBlockType.Assignment)
            {
                continue;
            }

            var assignment = parent.SubBlocks[i];
            var mergeIndex = i + 1;
            while (mergeIndex < parent.SubBlocks.Count && IsExpressionContinuationSibling(parent.SubBlocks[mergeIndex]))
            {
                var continuation = parent.SubBlocks[mergeIndex];
                continuation.Parent = assignment;
            }
        }
    }

    private static bool IsExpressionContinuationSibling(ICodeBlock block)
        => block.Type is CodeBlockType.Operation or CodeBlockType.Bracket;

    private static void RefreshAssignmentStatements(ICodeBlock parent)
    {
        for (var i = 0; i < parent.SubBlocks.Count; i++)
        {
            RefreshAssignmentStatements(parent.SubBlocks[i]);
        }

        if (parent.Type == CodeBlockType.Assignment)
        {
            if (parent is IECCodeBlock iecBlock)
            {
                iecBlock.AstNode = null;
            }

            _ = IecAstMapper.TryAttachAssignmentStatement(parent);
        }
    }

    private static bool IsTransparentSeparator(ICodeBlock block)
        => block.Type == CodeBlockType.Block && string.Equals(block.Code, ";", StringComparison.Ordinal);

    private sealed class StructuredTextExpressionParser
    {
        private readonly string _text;
        private int _position;
        private Token _current;

        public StructuredTextExpressionParser(string text)
        {
            _text = text;
            _current = ReadNextToken();
        }

        public IecExpression Parse()
        {
            var expression = ParseOrExpression();
            if (_current.Kind != TokenKind.End)
            {
                throw new FormatException($"Unexpected token '{_current.Text}' in expression '{_text}'.");
            }

            return expression;
        }

        private IecExpression ParseOrExpression()
        {
            var expression = ParseAndExpression();
            while (IsOperator("OR"))
            {
                ReadToken();
                expression = new IecBinaryExpression(expression, IecBinaryOperator.Or, ParseAndExpression());
            }

            return expression;
        }

        private IecExpression ParseAndExpression()
        {
            var expression = ParseComparisonExpression();
            while (IsOperator("AND"))
            {
                ReadToken();
                expression = new IecBinaryExpression(expression, IecBinaryOperator.And, ParseComparisonExpression());
            }

            return expression;
        }

        private IecExpression ParseComparisonExpression()
        {
            var expression = ParseAdditiveExpression();
            while (_current.Kind == TokenKind.Operator
                && _current.Text is "=" or "<>" or "<" or "<=" or "=<" or ">" or ">=")
            {
                var operatorText = _current.Text;
                ReadToken();
                expression = new IecBinaryExpression(expression, MapBinaryOperator(operatorText, _text), ParseAdditiveExpression());
            }

            return expression;
        }

        private IecExpression ParseAdditiveExpression()
        {
            var expression = ParseMultiplicativeExpression();
            while (_current.Kind == TokenKind.Operator && _current.Text is "+" or "-")
            {
                var operatorText = _current.Text;
                ReadToken();
                expression = new IecBinaryExpression(expression, MapBinaryOperator(operatorText, _text), ParseMultiplicativeExpression());
            }

            return expression;
        }

        private IecExpression ParseMultiplicativeExpression()
        {
            var expression = ParseUnaryExpression();
            while (_current.Kind == TokenKind.Operator && _current.Text is "*" or "/")
            {
                var operatorText = _current.Text;
                ReadToken();
                expression = new IecBinaryExpression(expression, MapBinaryOperator(operatorText, _text), ParseUnaryExpression());
            }

            return expression;
        }

        private IecExpression ParseUnaryExpression()
        {
            if (_current.Kind == TokenKind.Operator && _current.Text is "+" or "-")
            {
                var operatorText = _current.Text;
                ReadToken();
                return new IecUnaryExpression(operatorText == "+" ? IecUnaryOperator.Plus : IecUnaryOperator.Negate, ParseUnaryExpression());
            }

            return ParsePrimaryExpression();
        }

        private IecExpression ParsePrimaryExpression()
        {
            if (_current.Kind == TokenKind.OpenParen)
            {
                ReadToken();
                var nestedExpression = ParseOrExpression();
                Expect(TokenKind.CloseParen);
                return nestedExpression;
            }

            if (_current.Kind != TokenKind.Symbol)
            {
                throw new FormatException($"Unexpected token '{_current.Text}' in expression '{_text}'.");
            }

            var symbolText = _current.Text;
            ReadToken();
            if (_current.Kind == TokenKind.OpenParen)
            {
                ReadToken();
                var arguments = new List<IecExpression>();
                if (_current.Kind != TokenKind.CloseParen)
                {
                    do
                    {
                        arguments.Add(ParseOrExpression());
                        if (_current.Kind != TokenKind.Comma)
                        {
                            break;
                        }

                        ReadToken();
                    }
                    while (true);
                }

                Expect(TokenKind.CloseParen);
                return new IecFunctionCallExpression(symbolText, arguments);
            }

            return CreateLeafExpression(symbolText);
        }

        private IecExpression CreateLeafExpression(string symbolText)
        {
            if (bool.TryParse(symbolText, out var xBoolean))
            {
                return new IecLiteralExpression(xBoolean);
            }

            if (TryParseNumericLiteral(symbolText, out var numericValue))
            {
                return new IecLiteralExpression(numericValue);
            }

            return new IecIdentifierExpression(symbolText);
        }

        private static bool TryParseNumericLiteral(string text, out object numericValue)
        {
            numericValue = null!;
            var numberPart = text.Contains('#', StringComparison.Ordinal)
                ? text[(text.IndexOf('#', StringComparison.Ordinal) + 1)..]
                : text;

            if (double.TryParse(numberPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var fDouble)
                && (numberPart.Contains('.', StringComparison.Ordinal) || numberPart.Contains('e', StringComparison.OrdinalIgnoreCase) || text.Contains('#', StringComparison.Ordinal)))
            {
                numericValue = fDouble;
                return true;
            }

            if (long.TryParse(numberPart, NumberStyles.Integer, CultureInfo.InvariantCulture, out var iLong))
            {
                numericValue = iLong is >= int.MinValue and <= int.MaxValue ? (int)iLong : iLong;
                return true;
            }

            return false;
        }

        private static IecBinaryOperator MapBinaryOperator(string operatorText, string expressionText)
        {
            return operatorText.ToUpperInvariant() switch
            {
                "+" => IecBinaryOperator.Add,
                "-" => IecBinaryOperator.Subtract,
                "*" => IecBinaryOperator.Multiply,
                "/" => IecBinaryOperator.Divide,
                "=" => IecBinaryOperator.Equal,
                "<>" => IecBinaryOperator.NotEqual,
                "<" => IecBinaryOperator.LessThan,
                "<=" or "=<" => IecBinaryOperator.LessThanOrEqual,
                ">" => IecBinaryOperator.GreaterThan,
                ">=" => IecBinaryOperator.GreaterThanOrEqual,
                "AND" => IecBinaryOperator.And,
                "OR" => IecBinaryOperator.Or,
                _ => throw new FormatException($"Unsupported operator '{operatorText}' in expression '{expressionText}'."),
            };
        }

        private bool IsOperator(string operatorText)
            => _current.Kind == TokenKind.Operator && string.Equals(_current.Text, operatorText, StringComparison.OrdinalIgnoreCase);

        private void Expect(TokenKind kind)
        {
            if (_current.Kind != kind)
            {
                throw new FormatException($"Expected token '{kind}' in expression '{_text}', but found '{_current.Text}'.");
            }

            ReadToken();
        }

        private void ReadToken()
        {
            _current = ReadNextToken();
        }

        private Token ReadNextToken()
        {
            while (_position < _text.Length && char.IsWhiteSpace(_text[_position]))
            {
                _position++;
            }

            if (_position >= _text.Length)
            {
                return new Token(TokenKind.End, string.Empty);
            }

            var current = _text[_position];
            if (current == '(')
            {
                _position++;
                return new Token(TokenKind.OpenParen, "(");
            }

            if (current == ')')
            {
                _position++;
                return new Token(TokenKind.CloseParen, ")");
            }

            if (current == ',')
            {
                _position++;
                return new Token(TokenKind.Comma, ",");
            }

            if (current is '+' or '-' or '*' or '/')
            {
                _position++;
                return new Token(TokenKind.Operator, current.ToString());
            }

            if (current is '<' or '>' or '=')
            {
                var start = _position++;
                if (_position < _text.Length
                    && ((current == '<' && _text[_position] is '=' or '>')
                        || (current == '>' && _text[_position] == '=')
                        || (current == '=' && _text[_position] == '<')))
                {
                    _position++;
                }

                return new Token(TokenKind.Operator, _text[start.._position]);
            }

            var symbolStart = _position;
            while (_position < _text.Length && !IsTokenDelimiter(_text[_position]))
            {
                _position++;
            }

            var symbol = _text[symbolStart.._position];
            if (string.Equals(symbol, "AND", StringComparison.OrdinalIgnoreCase)
                || string.Equals(symbol, "OR", StringComparison.OrdinalIgnoreCase))
            {
                return new Token(TokenKind.Operator, symbol);
            }

            return new Token(TokenKind.Symbol, symbol);
        }

        private static bool IsTokenDelimiter(char character)
            => char.IsWhiteSpace(character) || character is '(' or ')' or ',' or '+' or '-' or '*' or '/' or '<' or '>' or '=';

        private readonly record struct Token(TokenKind Kind, string Text);

        private enum TokenKind
        {
            End,
            Symbol,
            Operator,
            OpenParen,
            CloseParen,
            Comma,
        }
    }
}
