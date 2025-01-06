using System;
using System.Linq;
using System.Windows.Documents;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;

public class IECCodeBuilder : CodeBuilder
{
    public IECCodeBuilder()
    {
        NewCodeBlock = (name, type, code, parent, pos) => new IECCodeBlock() { Name = name, Type = type, Code = code, Parent = parent, SourcePos = pos };
    }

    public override void OnToken(TokenData tokenData, ICodeBuilderData data)
    {
        switch (tokenData.type)
        {
            default:
                base.OnToken(tokenData, data);
                break;
            case CodeBlockType.Operation when !string.IsNullOrEmpty(tokenData.Code):
                BuildInstruction(tokenData, data);
                break;
            case CodeBlockType.Number when !string.IsNullOrEmpty(tokenData.Code):
                BuildNumber(tokenData, data);
                break;
            case CodeBlockType.Variable when !string.IsNullOrEmpty(tokenData.Code):
                BuildVariable(tokenData, data);
                break;
            case CodeBlockType.Function when !string.IsNullOrEmpty(tokenData.Code):
                BuildFunction(tokenData, data);
                break;
            case CodeBlockType.Bracket when !string.IsNullOrEmpty(tokenData.Code):
                BuildBracket(tokenData, data);
                break;
        }
        data.cbtLast = tokenData.type;
    }

    private void BuildBracket(TokenData tokenData, ICodeBuilderData data)
    {
        switch (tokenData.Code)
        {
            default:
                {
                    var td = tokenData;
                    td.Level = data.actualBlock.Level - 1;
                    base.OnToken(td, data);
                    data.actualBlock = data.actualBlock.Parent;
                }
                break;
            case "(" when data.actualBlock.Type is CodeBlockType.Variable or CodeBlockType.Function:
            case "[" when data.actualBlock.Type is CodeBlockType.Variable or CodeBlockType.Function:
                data.actualBlock.Type = CodeBlockType.Function;
                data.actualBlock.Code += tokenData.Code;
                break;
            case "(" when data.actualBlock.SubBlocks?.Last()?.Type is CodeBlockType.Variable or CodeBlockType.Function:
                data.actualBlock = data.actualBlock.SubBlocks.Last();
                data.actualBlock.Type = CodeBlockType.Function;
                data.actualBlock.Code += tokenData.Code;
                break;
            case "(":
            case "[":
                {
                    var td = tokenData;
                    td.Level = data.actualBlock.Level - 1;
                    base.OnToken(td, data);
                }
                break;
        }
    }

    private void BuildFunction(TokenData tokenData, ICodeBuilderData data)
    {
        switch (tokenData.Code.ToUpper())
        {
            default:
                {
                    ICodeBlock block = data.actualBlock;
                    var td = tokenData;
                    td.Level = block.Level;
                    base.OnToken(td, data);
                    if (data.actualBlock.Type is CodeBlockType.Operation or CodeBlockType.Assignment)
                    {
                        data.actualBlock.Parent = block;
                        data.actualBlock = block;
                    }
                }
                break;
            case "BEGIN" or "THEN" or "ELSE" or "ELSIF" or "DO":
                {                // ToDo: Do some sanity-checks here
                    var td = tokenData;
                    td.type = CodeBlockType.Block;
                    td.Level = tokenData.Level - 1;
                    while (data.actualBlock.Level > tokenData.Level)
                        data.actualBlock = data.actualBlock.Parent;
                    base.OnToken(td, data);
                }
                break;
        }
    }

    private void BuildVariable(TokenData tokenData, ICodeBuilderData data)
    {
        switch (data.actualBlock.Type)
        {
            case CodeBlockType.Variable or CodeBlockType.Function when data.actualBlock.Code.EndsWith("."):
                data.actualBlock.Type = CodeBlockType.Variable;
                data.actualBlock.Code += tokenData.Code;
                break;
            case CodeBlockType.Function:
                {
                    var td = tokenData;
                    td.Level = data.actualBlock.Level;
                    base.OnToken(td, data);
                }
                break;
            case CodeBlockType.Operation when  
                data.actualBlock.SubBlocks?.LastOrDefault() is ICodeBlock cb 
                && cb.Type is CodeBlockType.Variable 
                && cb.Code.EndsWith("."):
                {
                    cb.Type = CodeBlockType.Variable;
                    cb.Code += tokenData.Code;
                }
                break;
            case CodeBlockType.Operation 
                or CodeBlockType.Assignment: // ?
                {
                    ICodeBlock block = data.actualBlock;
                    var td = tokenData;
                    td.Level = block.Level;
                    base.OnToken(td, data);
                    data.actualBlock.Parent = block;
                    data.actualBlock = block;
                }
                break;
            default:
                base.OnToken(tokenData, data);
                break;
        }
    }

    private void BuildNumber(TokenData tokenData, ICodeBuilderData data)
    {
        if (data.actualBlock.Type is CodeBlockType.Operation or CodeBlockType.Assignment)
        {
            ICodeBlock block = data.actualBlock;
            var td = tokenData;
            td.Level = block.Level;
            base.OnToken(td, data);
            data.actualBlock.Parent = block;
            data.actualBlock = block;
        }
        else
        {
            base.OnToken(tokenData, data);
        }
    }

    private void BuildInstruction(TokenData tokenData, ICodeBuilderData data)
    {
        switch (tokenData.Code, data.actualBlock.Type)
        {
            default:
                {
                    var td = tokenData;
                    td.Level = Math.Max( data.actualBlock.Level - 1,0);
                    base.OnToken(td, data);
                }
                break;
            case ("-", _)
                when data.cbtLast is CodeBlockType.Operation :
            case ( ".",CodeBlockType.Variable or CodeBlockType.Function):
                data.actualBlock.Code += tokenData.Code;
                if (data.actualBlock.Type == CodeBlockType.Function)
                {
                    data.actualBlock.Type = CodeBlockType.Variable;
                }
                break;
            case (".",_) when data.actualBlock.SubBlocks?.Last()?.Type is CodeBlockType.Variable or CodeBlockType.Function:
                {
                    var block = data.actualBlock.SubBlocks.Last();
                    block.Type = CodeBlockType.Variable;
                    block.Code += tokenData.Code;
                }
                break;
            case (":=", CodeBlockType.Variable or CodeBlockType.Function):
                {
                    ICodeBlock block = data.actualBlock;
                    if (block.Type == CodeBlockType.Function && !block.Code.EndsWith("("))
                        block.Type = CodeBlockType.Variable;
                    var td = tokenData;
                    td.Level = block.Level-1;
                    td.type = CodeBlockType.Assignment;
                    base.OnToken(td, data);
                    block.Parent = data.actualBlock;
                }
                break;
            case (",",_) :
                {
                    var td = tokenData;
                    td.Level = data.actualBlock.Level - 2;
                    base.OnToken(td, data);
                }
                break;
            case (";",_):
                {
                    var td = tokenData;
                    td.Level = data.actualBlock.Level - 1;
                    td.type = CodeBlockType.Block;
                    base.OnToken(td, data);
                    while (data.actualBlock.Level > tokenData.Level)
                        data.actualBlock = data.actualBlock.Parent;
                }
                break;
            case ("/",CodeBlockType.Number or CodeBlockType.Variable or CodeBlockType.Operation or CodeBlockType.Function):
            case ("*" , CodeBlockType.Number or CodeBlockType.Variable or CodeBlockType.Operation or CodeBlockType.Function):
            case ("+" , CodeBlockType.Number or CodeBlockType.Variable or CodeBlockType.Operation or CodeBlockType.Function):
            case ("-", CodeBlockType.Number or CodeBlockType.Variable or CodeBlockType.Operation or CodeBlockType.Function) 
                when data.cbtLast != CodeBlockType.Operation:
            case ("<", CodeBlockType.Number or CodeBlockType.Variable or CodeBlockType.Operation or CodeBlockType.Function):
            case ("=<", CodeBlockType.Number or CodeBlockType.Variable or CodeBlockType.Operation or CodeBlockType.Function):
            case ("=", CodeBlockType.Number or CodeBlockType.Variable or CodeBlockType.Operation or CodeBlockType.Function):
            case (">=", CodeBlockType.Number or CodeBlockType.Variable or CodeBlockType.Operation or CodeBlockType.Function):
            case (">", CodeBlockType.Number or CodeBlockType.Variable or CodeBlockType.Operation or CodeBlockType.Function):
            case ("AND", CodeBlockType.Number or CodeBlockType.Variable or CodeBlockType.Operation or CodeBlockType.Function):
            case ("OR", CodeBlockType.Number or CodeBlockType.Variable or CodeBlockType.Operation or CodeBlockType.Function):
                {
                    ICodeBlock block = data.actualBlock;
                    var td = tokenData;
                    td.Level = block.Level - 1;
                    base.OnToken(td, data);
                    if (block.Type == CodeBlockType.Operation
                        && new[] { "*", "/" }.Contains(td.Code)
                        && new[] { "+", "-" }.Contains(block.Code))
                    {
                        var subBlock = block.SubBlocks.Last();
                        data.actualBlock.Parent = block;
                        subBlock.Parent = data.actualBlock;
                        // data.actualBlock = block;

                    }
                    else
                    if (block.Type == CodeBlockType.Operation
                        && new[] { "*", "/" }.Contains(block.Code)
                        && new[] { "+", "-" }.Contains(block.Parent.Code)
                        && new[] { "+", "-" }.Contains(td.Code))
                    {
                        data.actualBlock.Parent = block.Parent.Parent;
                        block.Parent.Parent = data.actualBlock;
                    }
                    else
                        block.Parent = data.actualBlock;
                }
                break;
        }
    }
}
