using System;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.Pascal.Models.Scanner;

/// <summary>
/// Pascal-specific builder. Interprets BEGIN/END and statement boundaries to form blocks.
/// Adds simple support for FUNCTION/PROCEDURE declarations.
/// </summary>
public class PasCodeBuilder : CodeBuilder
{
    public PasCodeBuilder()
    {
        NewCodeBlock = (name, type, code, parent, pos) => new CodeBlock() { Name = name, Type = type, Code = code, Parent = parent, SourcePos = pos };
    }

    public override void OnToken(TokenData tokenData, ICodeBuilderData data)
    {
        switch (tokenData.type)
        {
            default:
                base.OnToken(tokenData, data);
                break;
            case CodeBlockType.Operation when !string.IsNullOrEmpty(tokenData.Code):
                BuildOperation(tokenData, data);
                break;
            case CodeBlockType.Label:
                BuildLabel(tokenData, data);
                break;
            case CodeBlockType.Block:
                base.OnToken(tokenData, data);
                break;
        }
        data.cbtLast = tokenData.type;
    }

    private void BuildLabel(TokenData tokenData, ICodeBuilderData data)
    {
        // treat labels as own blocks at parent level
        var parent = data.actualBlock.Parent ?? data.actualBlock;
        data.actualBlock = NewCodeBlock("Label", CodeBlockType.Label, tokenData.Code, parent, tokenData.Pos);
    }

    private void BuildOperation(TokenData tokenData, ICodeBuilderData data)
    {
        var text = tokenData.Code.Trim();
        var upper = text.ToUpperInvariant();
        switch (upper)
        {
            case "BEGIN":
                // open a compound statement as child of current context
                data.actualBlock = NewCodeBlock("Block", CodeBlockType.Block, "begin", data.actualBlock, tokenData.Pos);
                break;
            case "END":
                // close the current compound statement only (semicolon is a separator in Pascal)
                if (data.actualBlock?.Parent != null)
                    data.actualBlock = data.actualBlock.Parent;
                break;
            default:
                // function/procedure header starts
                if (upper.StartsWith("FUNCTION ") || upper.StartsWith("PROCEDURE "))
                {
                    // create a new function/procedure sibling at the parent level
                    var parent = data.actualBlock.Parent ?? data.actualBlock;
                    data.actualBlock = NewCodeBlock("Function", CodeBlockType.Function, text, parent, tokenData.Pos);
                    break;
                }
                // generic statement accumulation similar to CS builder
                if (data.actualBlock.Type is not CodeBlockType.Operation and not CodeBlockType.MainBlock
                    || (!string.IsNullOrEmpty(data.actualBlock.Code) && data.actualBlock.Code.EndsWith(";")))
                {
                    var parent = (data.actualBlock.Type is CodeBlockType.Function or CodeBlockType.Block)
                        ? data.actualBlock
                        : data.actualBlock.Parent;
                    data.actualBlock = NewCodeBlock("Operation", CodeBlockType.Operation, text, parent, tokenData.Pos);
                }
                else
                {
                    var pad = (data.actualBlock.Code.EndsWith("\"") && text.StartsWith("+")) || text.StartsWith("(")
                        ? " " : string.Empty;
                    data.actualBlock.Code += pad + tokenData.Code;
                }
                break;
        }
    }
}
