using System.Collections.Generic;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;

public abstract class CodeBase : ICodeBase
{

    public string OriginalCode { get; set; } = string.Empty;
    public abstract IEnumerable<TokenData> Tokenize();
    public abstract void Tokenize(ICodeBase.TokenDelegate? token);

    public abstract ICodeBlock Parse(IEnumerable<TokenData>? values = null);

    public string ToCode(ICodeBlock codeBlock, int indent = 4)
    {
        return codeBlock.ToCode(indent);
    }

}