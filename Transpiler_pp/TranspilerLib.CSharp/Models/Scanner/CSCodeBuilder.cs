using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;

/// <summary>
/// C#-specific implementation of <see cref="CodeBuilder"/> that interprets tokens emitted by
/// the C# tokenizer and constructs an <see cref="ICodeBlock"/> tree (operations, labels, blocks, comments, strings).
/// </summary>
/// <remarks>
/// The builder groups tokens into semantic blocks and maintains a mutable state (<see cref="ICodeBuilderData"/>)
/// for label resolution and control-flow constructs (e.g., <c>goto</c>). It assigns <see cref="CodeBlock.SourcePos"/>
/// to created blocks to preserve mapping back to the original source.
/// </remarks>
public class CSCodeBuilder : CodeBuilder
{
    private readonly ConditionalWeakTable<ICodeBuilderData, LexicalBuildState> _lexicalStates = new();

    private sealed class LexicalBuildState
    {
        public StringBuilder Code { get; } = new();
        public int StartPosition { get; set; }
        public int ParenthesisDepth { get; set; }
        public int BracketDepth { get; set; }
        public int QuestionMarkDepth { get; set; }
        public bool ForceNextInstructionBoundary { get; set; }
    }

    private static ICodeBlock CreateDetachedBlock(string name, CodeBlockType type, string code, int sourcePos)
        => new CodeBlock() { Name = name, Type = type, Code = code, SourcePos = sourcePos };

    /// <summary>
    /// Initializes a new instance of the <see cref="CSCodeBuilder"/> class and configures the default
    /// <see cref="CodeBuilder.NewCodeBlock"/> factory to produce <see cref="CodeBlock"/> instances with source positions.
    /// </summary>
    public CSCodeBuilder()
    {
        NewCodeBlock = (name, type, code, parent, pos) => new CodeBlock() { Name = name, Type = type, Code = code, Parent = parent, SourcePos = pos };
    }

    /// <summary>
    /// Processes a single token and updates the current builder state, creating or extending blocks as necessary.
    /// </summary>
    /// <param name="tokenData">The token to process.</param>
    /// <param name="data">The mutable builder state tracking the current block, labels, and gotos.</param>
    /// <remarks>
    /// Dispatches to specialized handlers for operations, labels, strings, comments, blocks and goto statements.
    /// Updates <see cref="ICodeBuilderData.cbtLast"/> to assist with context-sensitive decisions for subsequent tokens.
    /// </remarks>
    public override void OnToken(TokenData tokenData, ICodeBuilderData data)
    {
        if (tokenData.type != CodeBlockType.Unknown)
        {
            data.cbtLast = BuildLexicalToken(tokenData, data);
            return;
        }

        switch (tokenData.type)
        {
            default:
                break;
            case CodeBlockType.Operation when !string.IsNullOrEmpty(tokenData.Code):
                BuildInstruction(tokenData, data);
                break;
            case CodeBlockType.Goto:
                BuildGoto(tokenData, data);
                break;
            case CodeBlockType.Comment:
            case CodeBlockType.LComment:
                BuildComment(tokenData, data);
                break;
            case CodeBlockType.String:
                BuildString(tokenData, data);
                break;
            case CodeBlockType.Label:
                tokenData = BuildLabel(tokenData, data);
                break;
            case CodeBlockType.Block when tokenData.Code == "{":
                BuildBlockStart(tokenData, data);
                break;
            case CodeBlockType.Block:
                BuildBlockEnd(tokenData, data);
                break;

        }
        data.cbtLast = tokenData.type;
    }

    /// <summary>Flushes a pending fine-grained instruction after the source token stream ends.</summary>
    public void Complete(ICodeBuilderData data)
    {
        if (_lexicalStates.TryGetValue(data, out LexicalBuildState? state))
            FlushLexicalInstruction(data, state);
    }

    private CodeBlockType BuildLexicalToken(TokenData tokenData, ICodeBuilderData data)
    {
        LexicalBuildState state = _lexicalStates.GetValue(data, _ => new LexicalBuildState());
        if (state.ParenthesisDepth == 0 && state.BracketDepth == 0 && state.QuestionMarkDepth == 0 &&
            tokenData.Code is not "{" and not ";" &&
            tokenData.type is not CodeBlockType.LComment and not CodeBlockType.Comment &&
            ((IsPendingControlCondition(state.Code) &&
              !(tokenData.Code == "when" && StartsWithKeyword(state.Code, "catch"))) ||
             state.Code.ToString().Trim() == "try" && (tokenData.Code is "catch" or "finally")))
        {
            data.cbtLast = CodeBlockType.Bracket;
            FlushLexicalInstruction(data, state);
        }

        if (tokenData.type is CodeBlockType.LComment or CodeBlockType.Comment)
        {
            FlushLexicalInstruction(data, state);
            BuildComment(new TokenData(tokenData.Code,
                tokenData.type == CodeBlockType.LComment ? CodeBlockType.LComment : CodeBlockType.Comment,
                tokenData.Level, tokenData.Pos), data);
            return tokenData.type == CodeBlockType.LComment ? CodeBlockType.LComment : CodeBlockType.Comment;
        }

        if (tokenData.Code == "?" && tokenData.type == CodeBlockType.Operator)
        {
            state.QuestionMarkDepth++;
            AppendToken(state, tokenData);
            return CodeBlockType.Operation;
        }

        if (tokenData.type == CodeBlockType.Punctuation)
        {
            switch (tokenData.Code)
            {
                case "{":
                    FlushLexicalInstruction(data, state);
                    BuildBlockStart(new TokenData("{", CodeBlockType.Block, tokenData.Level, tokenData.Pos), data);
                    state.ForceNextInstructionBoundary = false;
                    return CodeBlockType.Block;
                case "}":
                    FlushLexicalInstruction(data, state);
                    BuildBlockEnd(new TokenData("}", CodeBlockType.Block, tokenData.Level, tokenData.Pos), data);
                    state.ForceNextInstructionBoundary = false;
                    return CodeBlockType.Block;
                case "(":
                    state.ParenthesisDepth++;
                    break;
                case ")":
                    state.ParenthesisDepth = Math.Max(0, state.ParenthesisDepth - 1);
                    break;
                case "[":
                    state.BracketDepth++;
                    break;
                case "]":
                    state.BracketDepth = Math.Max(0, state.BracketDepth - 1);
                    break;
                case "?":
                    state.QuestionMarkDepth++;
                    break;
                case ":" when state.ParenthesisDepth == 0 && state.BracketDepth == 0 && state.QuestionMarkDepth == 0:
                    if (IsLabel(state.Code))
                    {
                        AppendToken(state, tokenData);
                        FlushLexicalInstruction(data, state, CodeBlockType.Label);
                        return CodeBlockType.Label;
                    }
                    break;
                case ":" when state.QuestionMarkDepth > 0:
                    state.QuestionMarkDepth--;
                    break;
                case ";":
                    AppendToken(state, tokenData);
                    if (state.ParenthesisDepth == 0 && state.BracketDepth == 0)
                    {
                        string instruction = state.Code.ToString().Trim();
                        CodeBlockType type = instruction.StartsWith("goto ", StringComparison.Ordinal)
                            ? CodeBlockType.Goto
                            : CodeBlockType.Operation;
                        FlushLexicalInstruction(data, state, type);
                        return type;
                    }
                    return CodeBlockType.Operation;
            }
        }

        if (tokenData.type == CodeBlockType.Identifier &&
            tokenData.Code == "else" &&
            state.Code.Length == 0)
        {
            BuildInstruction(new TokenData("else", CodeBlockType.Operation, tokenData.Level, tokenData.Pos), data);
            return CodeBlockType.Operation;
        }

        AppendToken(state, tokenData);
        return CodeBlockType.Operation;
    }

    private static void AppendToken(LexicalBuildState state, TokenData tokenData)
    {
        if (state.Code.Length == 0)
            state.StartPosition = tokenData.Pos;
        else
        {
            string precedingCode = state.Code.ToString().TrimEnd();
            bool preservesLegacyEqualitySpacing = tokenData.Code == "-"
                && tokenData.LeadingTrivia.Length > 0
                && precedingCode.EndsWith("==", StringComparison.Ordinal);
            // Preserve the existing emitted spacing for equality checks against negative literals.
            state.Code.Append(tokenData.LeadingTrivia.Length == 0
                ? string.Empty
                : preservesLegacyEqualitySpacing ? "  " : " ");
        }
        state.Code.Append(tokenData.Code);
    }

    private void FlushLexicalInstruction(ICodeBuilderData data, LexicalBuildState state, CodeBlockType type = CodeBlockType.Operation)
    {
        if (state.Code.Length == 0)
            return;

        string code = state.Code.ToString().Trim();
        int sourcePosition = state.StartPosition;
        state.Code.Clear();
        state.ParenthesisDepth = 0;
        state.BracketDepth = 0;
        state.QuestionMarkDepth = 0;
        if (code.Length == 0)
            return;

        if (state.ForceNextInstructionBoundary)
        {
            data.cbtLast = CodeBlockType.Bracket;
            state.ForceNextInstructionBoundary = false;
        }

        if (type == CodeBlockType.Operation && IsPendingControlCondition(new StringBuilder(code)))
            state.ForceNextInstructionBoundary = true;

        var instruction = new TokenData(code, type, data.actualBlock.Level, sourcePosition);
        if (type == CodeBlockType.Goto)
            BuildGoto(instruction, data);
        else if (type == CodeBlockType.Label)
        {
            if (code.StartsWith("case ", StringComparison.Ordinal) &&
                (code.IndexOf('"') >= 0 || code.IndexOf('\'') >= 0))
            {
                BuildInstruction(new TokenData("case ", CodeBlockType.Operation, data.actualBlock.Level, sourcePosition), data);
                BuildLabel(new TokenData(code.Substring(5), CodeBlockType.Label, data.actualBlock.Level, sourcePosition + 5), data);
            }
            else
            {
                BuildLabel(instruction, data);
            }
        }
        else
            BuildInstruction(instruction, data);
    }

    private static bool IsPendingControlCondition(StringBuilder code)
    {
        string instruction = code.ToString().TrimStart();
        if (instruction.TrimEnd() == "try")
            return true;

        int openingParenthesis = instruction.IndexOf('(');
        if (openingParenthesis <= 0 || !instruction.EndsWith(")", StringComparison.Ordinal))
            return false;

        string keyword = instruction.Substring(0, openingParenthesis).Trim();
        return keyword is "if" or "while" or "for" or "foreach" or "switch" or "using" or "lock" or "catch";
    }

    private static bool StartsWithKeyword(StringBuilder code, string keyword)
    {
        string instruction = code.ToString().TrimStart();
        return instruction.StartsWith(keyword, StringComparison.Ordinal) &&
            (instruction.Length == keyword.Length || char.IsWhiteSpace(instruction[keyword.Length]) || instruction[keyword.Length] == '(');
    }

    private static bool IsLabel(StringBuilder code)
    {
        string candidate = code.ToString().Trim();
        if (candidate.Length == 0 || candidate.Contains('?'))
            return false;
        return candidate.StartsWith("case ", StringComparison.Ordinal) ||
            candidate.StartsWith("default", StringComparison.Ordinal) ||
            IsIdentifier(candidate);
    }

    private static bool IsIdentifier(string value)
    {
        int index = value.StartsWith("@", StringComparison.Ordinal) ? 1 : 0;
        if (index >= value.Length || !(char.IsLetter(value[index]) || value[index] == '_'))
            return false;
        for (index++; index < value.Length; index++)
            if (!(char.IsLetterOrDigit(value[index]) || value[index] == '_'))
                return false;
        return true;
    }

    private void BuildBlockEnd(TokenData tokenData, ICodeBuilderData data)
    {
        var parentBlock = data.actualBlock.Parent;
        _ = parentBlock != null
            ? NewCodeBlock($"{tokenData.type}End", tokenData.type, tokenData.Code, parentBlock, tokenData.Pos)
            : CreateDetachedBlock($"{tokenData.type}End", tokenData.type, tokenData.Code, tokenData.Pos);
        data.actualBlock = parentBlock ?? data.actualBlock;
    }

    private void BuildBlockStart(TokenData tokenData, ICodeBuilderData data)
    {
        data.actualBlock = NewCodeBlock($"{tokenData.type}Start", tokenData.type, tokenData.Code, data.actualBlock, tokenData.Pos);
        data.xBreak = true;
    }

    private TokenData BuildLabel(TokenData tokenData, ICodeBuilderData data)
    {
        if (!data.actualBlock.Code.StartsWith("case ") && (tokenData.Code.Contains(",") || tokenData.Code.Contains("("))) //not cElse label
        {
            if (data.actualBlock.Type is not CodeBlockType.Operation || data.actualBlock.Code.EndsWith(";"))
            {
                data.actualBlock = data.actualBlock.Parent != null
                    ? NewCodeBlock($"Operation", CodeBlockType.Operation, "", data.actualBlock.Parent, tokenData.Pos)
                    : CreateDetachedBlock($"Operation", CodeBlockType.Operation, "", tokenData.Pos);
            }
            data.actualBlock.Code += tokenData.Code + " ";
            tokenData.type = CodeBlockType.Operation; //!! not cElse label
        }
        else
        {
            if (data.actualBlock.Type is CodeBlockType.Operation
                && data.actualBlock.Code.StartsWith("case ")
                && tokenData.Code.EndsWith(":"))
            {
                data.actualBlock.Type = CodeBlockType.Label;
                if (tokenData.Code.StartsWith("+"))
                    data.actualBlock.Code += " "; // Padding
                data.actualBlock.Code += tokenData.Code;
                tokenData.Code = data.actualBlock.Code.Substring(5);
            }
            else
            {
                data.actualBlock = data.actualBlock.Parent != null
                    ? NewCodeBlock($"{tokenData.type}", tokenData.type, tokenData.Code, data.actualBlock.Parent, tokenData.Pos)
                    : CreateDetachedBlock($"{tokenData.type}", tokenData.type, tokenData.Code, tokenData.Pos);
            }
            if (!data.labels.ContainsKey(tokenData.Code))
                data.labels.Add(tokenData.Code, data.actualBlock);
        }

        return tokenData;
    }

    private void BuildString(TokenData tokenData, ICodeBuilderData data)
    {
        if (data.actualBlock.Type is not CodeBlockType.Operation)
            data.actualBlock = new CodeBlock()
            {
                Name = $"Operation",
                Type = CodeBlockType.Operation,
                Code = "",
                Parent = data.actualBlock.Parent,
                SourcePos = tokenData.Pos
            };
        else if (data.actualBlock.Code.EndsWith("+")
            || data.actualBlock.Code.EndsWith("=")
            || data.actualBlock.Code.EndsWith(",")
            || data.actualBlock.Code.EndsWith("case"))
            data.actualBlock.Code += " ";
        data.actualBlock.Code += tokenData.Code;
    }

    private void BuildComment(TokenData tokenData, ICodeBuilderData data)
    {
        data.actualBlock = new CodeBlock()
        {
            Name = $"Comment",
            Type = tokenData.type,
            Code = tokenData.Code,
            Parent = data.actualBlock.Parent ?? data.actualBlock,
            SourcePos = tokenData.Pos
        };
    }

    private void BuildGoto(TokenData tokenData, ICodeBuilderData data)
    {
        data.actualBlock = new CodeBlock()
        {
            Name = $"{tokenData.type}",
            Type = tokenData.type,
            Code = tokenData.Code,
            Parent = data.actualBlock?.Parent,
            SourcePos = tokenData.Pos
        };
        data.gotos.Add(data.actualBlock);
        data.xBreak = false;
    }

    private void BuildInstruction(TokenData tokenData, ICodeBuilderData data)
    {
        var previousCode = data.actualBlock.Code.Trim();
        if (data.actualBlock.Type == CodeBlockType.Operation
            && previousCode == "else"
            && tokenData.Code.StartsWith("if", StringComparison.OrdinalIgnoreCase))
        {
            data.actualBlock = new CodeBlock()
            {
                Name = $"{tokenData.type}",
                Type = tokenData.type,
                Code = tokenData.Code,
                Parent = data.actualBlock.Parent ?? data.actualBlock,
                SourcePos = tokenData.Pos
            };
            data.xBreak = tokenData.Code.Contains("break;");
            return;
        }

        if (data.actualBlock.Type == CodeBlockType.Operation
            && IsPendingControlCondition(new StringBuilder(previousCode))
            && StartsWithKeyword(new StringBuilder(previousCode), "catch")
            && StartsWithKeyword(new StringBuilder(tokenData.Code.TrimStart()), "when"))
        {
            data.actualBlock.Code += " " + tokenData.Code.TrimStart();
            return;
        }

        if (data.actualBlock.Type is not CodeBlockType.Operation and not CodeBlockType.MainBlock
            || IsPendingControlCondition(new StringBuilder(previousCode))
            || (data.cbtLast is not CodeBlockType.Operation and not CodeBlockType.String and not CodeBlockType.Unknown)
            || (!string.IsNullOrEmpty(data.actualBlock.Code) && data.actualBlock.Code.EndsWith(";")))
        {
            data.actualBlock = new CodeBlock()
            {
                Name = $"{tokenData.type}",
                Type = tokenData.type,
                Code = tokenData.Code,
                Parent = data.actualBlock.Parent ?? data.actualBlock,
                SourcePos = tokenData.Pos
            };
        }
        else
        {
            var existingCode = data.actualBlock.Code;
            var tokenStartsIdentifier = tokenData.Code.Length > 0
                && (CharSets.lettersAndNumbers.Contains(tokenData.Code[0]) || tokenData.Code[0] == '_');
            var needsSeparator = existingCode.EndsWith("\"") && tokenData.Code.StartsWith("+")
                || tokenData.Code.StartsWith("(")
                || tokenStartsIdentifier
                    && !string.IsNullOrEmpty(existingCode)
                    && !existingCode.EndsWith(" ");
            data.actualBlock.Code += (needsSeparator ? " " : "") + tokenData.Code;
        }
        data.xBreak = tokenData.Code.Contains("break;");
    }

}
