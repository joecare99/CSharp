using System;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using static TranspilerLib.Interfaces.Code.ICodeBase;

namespace TranspilerLib.Models.Scanner;

public abstract class TokenHandlerBase
{

    protected static void EmitToken(TokenDelegate? token, TokenizeData data, CodeBlockType type, string originalCode, int offs = 0)
        => token?.Invoke(new TokenData(originalCode.Substring(data.Pos2, data.Pos - data.Pos2 + offs).Trim(), type, data.Stack,data.Pos2));
    protected static char GetNxtChar(int Pos, string OriginalCode)
        => Pos + 1 < OriginalCode.Length ? OriginalCode[Pos + 1] : '\u0000';

    protected static char GetPrvChar(int Pos, string OriginalCode)
        => OriginalCode[Math.Max(Pos - 1, 0)];

    protected static string GetText(TokenizeData data, string OriginalCode = "", int offs = 0)
        => OriginalCode.Substring(data.Pos2, Math.Max(data.Pos - data.Pos2 + offs, 0));
}