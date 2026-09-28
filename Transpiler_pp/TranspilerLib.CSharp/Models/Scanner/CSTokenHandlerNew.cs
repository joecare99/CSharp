using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;

public class CSTokenHandlerNew : TokenHandlerBase, ITokenHandler
{

    private static readonly Dictionary<int, Action<ICodeBase.TokenDelegate?, string, TokenizeData>?> _tokenStateHandler = new()
        {
            { 0, HandleDefault },
            { 1, HandleStrings },
            { 2, HandleLineComments },
            { 3, HandleBlockComments },
            { 4, HandleStrings },
            { 5, HandleStrings },
            { 7, HandleStrings },
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
     string[] reservedWords
    { set => _reservedWords = value; }

    internal static void HandleBlockComments(ICodeBase.TokenDelegate? token, string code, TokenizeData data)
    {
        if (code[data.Pos] == '*' && GetNxtChar(data.Pos, code) == '/')
        {
            EmitToken(token, data, CodeBlockType.Comment, code, 2);
            data.Pos2 = data.Pos + 2;
            data.State = 0; // Block-Comment-end
            data.Pos++;
        }
    }

    internal static void HandleLineComments(ICodeBase.TokenDelegate? token, string code, TokenizeData data)
    {
        if (code[data.Pos] == '\r' || data.Pos == code.Length - 1)
        {
            EmitToken(token, data, CodeBlockType.LComment, code);
            data.Pos2 = data.Pos + 1;
            data.State = 0;
        }
    }

    internal static void HandleStrings(ICodeBase.TokenDelegate? token, string code, TokenizeData data)
    {
        if (_stringEndChars.Contains(code[data.Pos]))
            switch (code[data.Pos])
            {
                case '\\' when data.State != 5: // Escape
                case '{' when (data.State == 4) && (GetNxtChar(data.Pos, code) == '{'):
                case '"' when (data.State == 5 || data.State == 7) && (GetNxtChar(data.Pos, code) == '"'):
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

    public static void HandleDefault(ICodeBase.TokenDelegate? token, string OriginalCode, TokenizeData data)
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
            case '/' when GetNxtChar(data.Pos, OriginalCode) == '/':
                DefaultComment(token, OriginalCode, data, 2);
                break;
            case '/' when GetNxtChar(data.Pos, OriginalCode) == '*':
                DefaultComment(token, OriginalCode, data, 3);
                break;
            case Char c when CharSets.whitespace.Contains(c):
                DefaultWhitespace(token, OriginalCode, data);
                break;
            case Char c when CharSets.letters.Contains(c) || (c == '_') || (c == '@'):
                DefaultAlpha(token, OriginalCode, data);
                break;
            case Char c when CharSets.numbers.Contains(c) || (c == '-' && CharSets.numbers.Contains(GetNxtChar(data.Pos, OriginalCode))):
                DefaultNumber(token, OriginalCode, data);
                break;
            default:
                DefaultOther(token, OriginalCode, data);
                break;
        }
    }

    private static void DefaultOther(ICodeBase.TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        // Scan until whitespace or an Identcharacter is found
        char[] operatorChars = ['+', '-', '*', '/', '%', '=', '!', '<', '>', '&', '|', '^', '~', '?', '.', ',', ':'];
        while ((++data.Pos < OriginalCode.Length)
            && operatorChars.Contains(OriginalCode[data.Pos]));
        data.Pos--;

        EmitToken(token, data, CodeBlockType.Operation, OriginalCode);
        data.Pos2 = data.Pos + 1;
    }

    private static void DefaultNumber(ICodeBase.TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        // Scan until the end of the number
        while ((++data.Pos < OriginalCode.Length) 
            && CharSets.numbersExt.Contains(OriginalCode[data.Pos])) ;
        char[] typeChar = ['M','L','F','D','U','l','u','d','f'];
        string[] typeStrings = ["UL", "LU"];
        if (data.Pos > OriginalCode.Length || !typeChar.Contains(OriginalCode[data.Pos]))
            data.Pos--;
        else
            if (typeStrings.Contains(string.Concat(OriginalCode[data.Pos], GetNxtChar(data.Pos, OriginalCode)).ToUpper()))
            data.Pos++;

        EmitToken(token, data, CodeBlockType.Number, OriginalCode);
        data.Pos2 = data.Pos + 1;
    }

    private static void DefaultAlpha(ICodeBase.TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        // Scan until the end of the identifier
        while ((++data.Pos < OriginalCode.Length)
            && (CharSets.lettersAndNumbers.Contains(OriginalCode[data.Pos]) 
            || OriginalCode[data.Pos] == '_'));
        data.Pos--;
        var text = GetText(data);

        EmitToken(token, data, _reservedWords.Contains(text) ? CodeBlockType.Operation : CodeBlockType.Variable, OriginalCode);
        data.Pos2 = data.Pos+1;
    }

    private static void DefaultWhitespace(ICodeBase.TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        while ((++data.Pos < OriginalCode.Length) 
            && CharSets.whitespace.Contains(OriginalCode[data.Pos]));
        data.Pos--;
    }

    private static void DefaultComment(ICodeBase.TokenDelegate? token, string OriginalCode, TokenizeData data, int iNewState)
    {
        data.Pos2 = data.Pos;
        data.State = iNewState;
        data.Pos++;
    }

    private static void DefaultString(ICodeBase.TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        data.State = 1; // "Normal" String
        if (GetPrvChar(data.Pos, OriginalCode) == '$')
        {
            // Todo: Multiblock-String-Start
            data.State = 4; // "$"-String
            data.Pos--;
        }
        else if (GetPrvChar(data.Pos, OriginalCode) == '@')
        {
            data.State = 5; // "@"-String
            data.Pos--;
        }
        else if (GetNxtChar(data.Pos, OriginalCode) == '"' && GetNxtChar(data.Pos+1, OriginalCode) == '"')
        {
            // """ -String
            data.State = 7; // Char-String
        }
        data.Pos2 = data.Pos;
        if (data.State != 1)
            data.Pos++;
    }

    private static void DefaultLabel(ICodeBase.TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        EmitToken(token, data, CodeBlockType.Label, OriginalCode, 1);
        data.Pos2 = data.Pos + 1;
    }

    private static void DefaultInstructionEnd(ICodeBase.TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        EmitToken(token, data, CodeBlockType.Separator, OriginalCode, 1);
        data.Pos2 = data.Pos + 1;
    }

    private static void DefaultBlock(ICodeBase.TokenDelegate? token, string OriginalCode, TokenizeData data, bool xStart = false, bool xEnd = false)
    {
        if (xStart)
            data.Stack += 1;
        data.Pos2 = data.Pos;
        EmitToken(token, data, CodeBlockType.Block, OriginalCode, 1);
        data.Pos2 = data.Pos + 1;
        if (xEnd)
            data.Stack -= 1;
    }

    public bool TryGetValue(int state, [NotNullWhen(true)] out Action<ICodeBase.TokenDelegate?, string, TokenizeData>? handler)
        => _tokenStateHandler.TryGetValue(state, out handler);
}
