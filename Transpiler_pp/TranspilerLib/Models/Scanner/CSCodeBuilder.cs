using System;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;

public class CSCodeBuilder : CodeBuilder
{
    public CSCodeBuilder()
    {
        NewCodeBlock = (name, type, code, parent, pos) => new CodeBlock() { Name = name, Type = type, Code = code, Parent = parent, SourcePos = pos };
    }

    public override void OnToken(TokenData tokenData, ICodeBuilderData data)
    {
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

    private void BuildBlockEnd(TokenData tokenData, ICodeBuilderData data)
    {
        _ = NewCodeBlock($"{tokenData.type}End", tokenData.type, tokenData.Code, data.actualBlock?.Parent, tokenData.Pos);
        data.actualBlock = data.actualBlock?.Parent;
    }

    private void BuildBlockStart(TokenData tokenData, ICodeBuilderData data)
    {
        data.actualBlock = NewCodeBlock($"{tokenData.type}Start", tokenData.type, tokenData.Code, (ICodeBlock?)data.actualBlock, tokenData.Pos);
        data.xBreak = true;
    }

    private TokenData BuildLabel(TokenData tokenData, ICodeBuilderData data)
    {
        if (!data.actualBlock.Code.StartsWith("case ") && (tokenData.Code.Contains(",") || tokenData.Code.Contains("("))) //not cElse label
        {
            if (data.actualBlock.Type is not CodeBlockType.Operation || data.actualBlock.Code.EndsWith(";"))
                data.actualBlock = NewCodeBlock($"Operation", CodeBlockType.Operation, "", data.actualBlock.Parent, tokenData.Pos);
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
                data.actualBlock = NewCodeBlock($"{tokenData.type}", tokenData.type, tokenData.Code, data.actualBlock.Parent, tokenData.Pos);
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
            Parent = data.actualBlock.Parent,
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
        data.gotos.Add((ICodeBlock?)data.actualBlock);
        data.xBreak = false;
    }

    private void BuildInstruction(TokenData tokenData, ICodeBuilderData data)
    {
        if (data.actualBlock.Type is not CodeBlockType.Operation and not CodeBlockType.MainBlock
            || (data.cbtLast is not CodeBlockType.Operation and not CodeBlockType.String and not CodeBlockType.Unknown)
            || (!string.IsNullOrEmpty(data.actualBlock.Code) && data.actualBlock.Code.EndsWith(";")))
        {
            data.actualBlock = new CodeBlock()
            {
                Name = $"{tokenData.type}",
                Type = tokenData.type,
                Code = tokenData.Code,
                Parent = data.actualBlock.Parent
            };
        }
        else
            data.actualBlock.Code += ((data.actualBlock.Code.EndsWith("\"") && tokenData.Code.StartsWith("+"))
                || tokenData.Code.StartsWith("(")
                || (CharSets.lettersAndNumbers.Contains(tokenData.Code[0])
                    && !string.IsNullOrEmpty(data.actualBlock.Code)
                    && !data.actualBlock.Code.EndsWith(" ")) ? " " : "") + tokenData.Code;
        data.xBreak = tokenData.Code.Contains("break;");
    }

}
