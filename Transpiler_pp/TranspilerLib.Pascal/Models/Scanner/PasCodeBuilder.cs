using System;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.Pascal.Models.Scanner;

/// <summary>
/// Pascal-specific builder. Interprets BEGIN/END and statement boundaries to form blocks.
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
        switch (text.ToUpper())
        {
            case "BEGIN":
                tokenData.type = CodeBlockType.Block;
                tokenData.Level = Math.Max(data.actualBlock.Level - 1, 0);
                base.OnToken(tokenData, data);
                break;
            case "END":
            case "END;":
                // Emit block end as block token to step up
                var td = tokenData; td.type = CodeBlockType.Block; td.Level = Math.Max(data.actualBlock.Level - 2, 0);
                base.OnToken(td, data);
                // climb up until statement level
                if (data.actualBlock?.Parent != null)
                    data.actualBlock = data.actualBlock.Parent;
                break;
            default:
                // generic statement accumulation similar to CS builder
                if (data.actualBlock.Type is not CodeBlockType.Operation and not CodeBlockType.MainBlock
                    || (!string.IsNullOrEmpty(data.actualBlock.Code) && data.actualBlock.Code.EndsWith(";")))
                {
                    data.actualBlock = NewCodeBlock("Operation", CodeBlockType.Operation, text, data.actualBlock.Parent, tokenData.Pos);
                }
                else
                {
                    var pad = (data.actualBlock.Code.EndsWith("\"") && text.StartsWith("+")) || text.StartsWith("(")
                        ? " " : string.Empty;
                    data.actualBlock.Code += pad + tokenData.Code;
                }
                if (text.EndsWith(";"))
                {
                    var b = new TokenData(";", CodeBlockType.Block, Math.Max(data.actualBlock.Level - 1, 0), tokenData.Pos);
                    base.OnToken(b, data);
                }
                break;
        }
    }
}
