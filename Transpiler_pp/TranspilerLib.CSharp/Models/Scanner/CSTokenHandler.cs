using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using static TranspilerLib.Interfaces.Code.ICodeBase;

namespace TranspilerLib.Models.Scanner;

public class CSTokenHandler : TokenHandlerBase, ITokenHandler
{

    private static readonly Dictionary<int, Action<TokenDelegate?, string, TokenizeData>> _tokenStateHandler = new()
        {
            { 0, HandleDefault },
            { 1, HandleStrings },
            { 2, HandleLineComments },
            { 3, HandleBlockComments },
            { 4, HandleStrings },
            { 5, HandleStrings },
            { 6, (_, s, d) => d.State = s[d.Pos] == '}' ? 4 : d.State},
        };

    private static char[] _stringEndChars { get; set; } = [];
    private static string[] _reservedWords { get; set; } = [];
    public char[] stringEndChars { set => _stringEndChars = value; }

#if NET7_0_OR_GREATER
    public required
#else
    public
#endif
     string[] reservedWords { set => _reservedWords = value; }

    internal static void HandleBlockComments(TokenDelegate? token, string code, TokenizeData data)
    {
        if (code[data.Pos] == '*' && GetNxtChar(data.Pos,code) == '/')
        {
            EmitToken(token, data, CodeBlockType.Comment, code, 2);
            data.Pos2 = data.Pos + 2;
            data.State = 0; // Block-Comment-end
            data.Pos++;
        }
    }

    internal static void HandleLineComments(TokenDelegate? token, string code, TokenizeData data)
    {
        if (code[data.Pos] == '\r' || data.Pos == code.Length-1 )
        {
            EmitToken(token, data, CodeBlockType.LComment, code);
            data.Pos2 = data.Pos + 1;
            data.State = 0;
        }
    }

    internal static void HandleStrings(TokenDelegate? token, string code, TokenizeData data)
    {
        if (_stringEndChars.Contains(code[data.Pos]))
            switch (code[data.Pos])
            {
                case '\\' when data.State != 5: // Escape
                case '{' when (data.State == 4) && (GetNxtChar(data.Pos, code) == '{'):
                case '"' when (data.State == 5) && (GetNxtChar(data.Pos, code) == '"'):
                    data.Pos++;
                    break;
                case '{' when data.State == 4: // inner statement
                    data.State = 6;
                    break;
                case '"':// String-End
                case '\r' when data.State != 5:
                    if (code[data.Pos] == '\r')
                        EmitToken(token, data, CodeBlockType.String, code);
                    else
                        EmitToken(token, data, CodeBlockType.String, code, 1);
                    data.Pos2 = data.Pos + 1;
                    data.State = 0;
                    break;
                default:
                    break;
            }

    }

    public static void HandleDefault(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        switch (OriginalCode[data.Pos])
        {
            case '{':
                DefaultBlock(token, OriginalCode, data, xStart: true);
                break;
            case '}':
                DefaultBlock(token, OriginalCode, data, xEnd: true);
                break;
            case ';':
                DefaultInstructionEnd(token, OriginalCode, data);
                break;
            case ':':
                DefaultLabel(token, OriginalCode, data);
                break;
            case '"':
                DefaultString(token, OriginalCode, data);
                break;
            case '/' when GetNxtChar(data.Pos,OriginalCode) == '/':
                DefaultComment(token, OriginalCode, data, 2);
                break;
            case '/' when GetNxtChar(data.Pos, OriginalCode) == '*':
                DefaultComment(token, OriginalCode, data, 3);
                break;
            case Char c when CharSets.whitespace.Contains(c):
                DefaultWhitespace(token, OriginalCode, data);
                break;
            case Char c when CharSets.letters.Contains(c):
                DefaultAlpha(token, OriginalCode, data);
                break;
            default:
                break;
        }
    }

    private static void DefaultAlpha(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        if (!data.flag && (data.flag = true)
            && GetText(data, OriginalCode).Trim().EndsWith(")"))
        {
            EmitToken(token, data, CodeBlockType.Operation, OriginalCode);
            data.Pos2 = data.Pos;
        }
    }

    private static void DefaultWhitespace(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        if (data.flag && !(data.flag = false)
            && GetText(data, OriginalCode).EndswithAny(_reservedWords))
        {
            EmitToken(token, data, CodeBlockType.Operation, OriginalCode);
            data.Pos2 = data.Pos;
        }
    }

    private static void DefaultComment(TokenDelegate? token, string OriginalCode, TokenizeData data, int iNewState)
    {
        EmitToken(token, data, CodeBlockType.Operation, OriginalCode);
        data.Pos2 = data.Pos;
        data.State = iNewState;
        data.Pos++;
    }

    private static void DefaultString(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        data.State = 1; // "Normal" String
        if (GetPrvChar(data.Pos, OriginalCode) == '$')
        {
            data.State = 4; // "$"-String
            data.Pos--;
        }
        else if (GetPrvChar(data.Pos, OriginalCode) == '@')
        {
            data.State = 5; // "@"-String
            data.Pos--;
        }
        EmitToken(token, data, CodeBlockType.Operation, OriginalCode);
        data.Pos2 = data.Pos;
        if (data.State != 1)
            data.Pos++;
    }

    private static void DefaultLabel(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        if (!GetText(data, OriginalCode).Trim().Contains('?'))
        {
            EmitToken(token, data, CodeBlockType.Label, OriginalCode, 1);
            data.Pos2 = data.Pos + 1;
        }
    }

    private static void DefaultInstructionEnd(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        var text = GetText(data, OriginalCode, 1);
        EmitToken(token, data, text.Contains("goto ") ? CodeBlockType.Goto : CodeBlockType.Operation, OriginalCode, 1);
        data.Pos2 = data.Pos + 1;
    }

    private static void DefaultBlock(TokenDelegate? token, string OriginalCode, TokenizeData data, bool xStart = false, bool xEnd = false)
    {
        if (!string.IsNullOrEmpty(GetText(data, OriginalCode).Trim()))
            EmitToken(token, data, CodeBlockType.Operation, OriginalCode);
        if (xStart)
            data.Stack += 1;
        data.Pos2 = data.Pos;
        EmitToken(token, data, CodeBlockType.Block, OriginalCode, 1);
        data.Pos2 = data.Pos + 1;
        if (xEnd)
            data.Stack -= 1;
    }

    public bool TryGetValue(int state, out Action<TokenDelegate?, string, TokenizeData> handler)
    {
        return _tokenStateHandler.TryGetValue(state, out handler);
    }
}



