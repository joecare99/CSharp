using System.Collections.Generic;
using TranspilerLib.Data;

namespace TranspilerLib.Interfaces.Code
{
    public interface ICodeBuilderData
    {
        ICodeBlock actualBlock { get; set; }
        Dictionary<string, ICodeBlock> labels { get; set; }
        List<ICodeBlock> gotos { get; set; }
        bool xBreak { get; set; }
        CodeBlockType cbtLast { get; set; }
    }
}