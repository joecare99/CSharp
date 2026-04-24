using System;
using System.Collections.Generic;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models;
using TranspilerLib.Models.Scanner;
using TranspilerLib.Pascal.Data;
using static TranspilerLib.Interfaces.Code.ICodeBase;

namespace TranspilerLib.Pascal.Models.Scanner;

public class PasCode : CodeBase
{
    private static readonly string[] _reserved = PasReservedWords.Words;

    private readonly ITokenHandler _tokenHandler = new PasTokenHandler() { reservedWords = _reserved };
    private readonly ICodeBuilder _codeBuilder = new PasCodeBuilder();
 //   private readonly ICodeOptimizer _optimizer = new CodeOptimizer();

    private static string GetDebug(TokenizeData data, string code) => $"{code.Substring(Math.Max(0, data.Pos - 20), data.Pos - Math.Max(0, data.Pos - 20))}>" + code[data.Pos] + "<" +
        $"{code.Substring(Math.Min(data.Pos + 1, code.Length - 1), Math.Min(40, code.Length - data.Pos - 1))}";

    public override IEnumerable<TokenData> Tokenize()
    {
        List<TokenData> list = new();
        Tokenize(t => list.Add(t));
        foreach (var t in list)
            yield return t;
    }

    public override void Tokenize(TokenDelegate? doToken)
    {
        TokenizeData data = new();
        while (data.Pos < OriginalCode.Length && _tokenHandler.TryGetValue(data.State, out var handler))
        {
            var dbg = GetDebug(data, OriginalCode);
            handler(doToken, OriginalCode, data);
            data.Pos++;
        }
    }

    public override ICodeBlock Parse(IEnumerable<TokenData>? values = null)
    {
        ICodeBlock root = new PasCodeBlock() { Name = "PascalRoot", Code = string.Empty, Parent = null, Type=CodeBlockType.MainBlock  };
        var data = _codeBuilder.NewData(root);

        if (values == null)
            Tokenize(td => _codeBuilder.OnToken(td, data));
        else
            foreach (var v in values)
                _codeBuilder.OnToken(v, data);

        return root;
    }
}
