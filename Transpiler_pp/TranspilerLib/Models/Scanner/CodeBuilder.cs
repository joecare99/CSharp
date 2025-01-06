using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;

public abstract class CodeBuilder : ICodeBuilder
{
    public Func<string, CodeBlockType, string, ICodeBlock, int, ICodeBlock> NewCodeBlock { get; set; } =
    (name, type, code, parent, pos) => new CodeBlock() { Name = name, Type = type, Code = code, Parent = parent, SourcePos = pos };

    private class CodeBuilderData : ICodeBuilderData
    {
        public Dictionary<string, ICodeBlock> labels { get; set; } = new();
        public List<ICodeBlock> gotos { get; set; } = new();
        public ICodeBlock actualBlock { get; set; }
        public bool xBreak { get; set; } = false;
        public CodeBlockType cbtLast { get; set; } = CodeBlockType.Unknown;
        public CodeBuilderData(ICodeBlock codeBlock)
        {
            actualBlock = codeBlock;
        }
    }

    public virtual ICodeBuilderData NewData(ICodeBlock block)
        => new CodeBuilderData(block);

    public virtual void OnToken(TokenData tokenData, ICodeBuilderData data)
    {
        ICodeBlock NewBlock = (data.actualBlock.Level) switch
        {
             int i when i == tokenData.Level + 1 
                => NewCodeBlock($"{tokenData.type}", tokenData.type, tokenData.Code, data.actualBlock.Parent, tokenData.Pos),
             int i when i == tokenData.Level 
                => NewCodeBlock($"{tokenData.type}", tokenData.type, tokenData.Code, data.actualBlock, tokenData.Pos),
        //     int i when i == tokenData.Level - 1 => NewCodeBlock($"{tokenData.type}", tokenData.type, tokenData.Code, data.actualBlock, tokenData.Pos),
             int i when i == tokenData.Level + 2 
                => NewCodeBlock($"{tokenData.type}", tokenData.type, tokenData.Code, data.actualBlock.Parent.Parent, tokenData.Pos),
        // unwanted  
            int i when i == tokenData.Level + 3 
                => NewCodeBlock($"{tokenData.type}", tokenData.type, tokenData.Code, data.actualBlock.Parent.Parent.Parent, tokenData.Pos),
            _ => throw new NotImplementedException()
        };

        data.actualBlock = NewBlock;
    }

}
