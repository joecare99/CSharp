using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using static TranspilerLib.Interfaces.Code.ICodeBase;

namespace TranspilerLib.Models.Scanner;

public class IECTokenHandler : TokenHandlerBase, ITokenHandler
{

    private static readonly Dictionary<int, Action<TokenDelegate?, string, TokenizeData>> _tokenStateHandler = new()
        {
            { 0, HandleDefault },
            { 1, HandleAlpha },
            { 2, HandleLineComments },
            { 3, HandleBlockComments },
            { 4, HandleStrings },
            { 5, HandleStrings },
            { 6, HandleStrings },
            { 7, (_, s, d) => d.State = s[d.Pos] == '}' ? 4 : d.State},
            { 8, HandleNumbers },
        };

    public static Dictionary<IECResWords, TypeCode> _sysTypes { get; set; }
    public static string[] _reservedWords { get; set; }
    public static Dictionary<IECResWords, IECResWords> _blockWords { get; set; }
    public
#if NET7_0_OR_GREATER
required
#endif
Dictionary<IECResWords, TypeCode> sysTypes
    { set => _sysTypes = value; }
    public
#if NET7_0_OR_GREATER
required
#endif
 string[] reservedWords
    { set => _reservedWords = value; }
    public
#if NET7_0_OR_GREATER
required
#endif
Dictionary<IECResWords, IECResWords> blockWords
    { set => _blockWords = value; }

    public static void HandleDefault(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        switch (OriginalCode[data.Pos])
        {
            case '/' when GetNxtChar(data.Pos, OriginalCode) == '/':
                DefaultComment(token, OriginalCode, data, 2);
                break;
            case '(' when GetNxtChar(data.Pos, OriginalCode) == '*':
                DefaultComment(token, OriginalCode, data, 3);
                break;
            case char c when CharSets.bracketsSet.Contains(c):
                DefaultBrackets(token, c, OriginalCode, data);
                break;
            case char c when CharSets.numbers.Contains(c):
                DefaultNumbers(token, OriginalCode, data);
                break;
            case char c when CharSets.operatorSet.Contains(c):
                DefaultOperator(token, OriginalCode, data);
                break;
            case '"':
                DefaultString(token, OriginalCode, data);
                break;
            case char c when CharSets.whitespace.Contains(c):
                DefaultWhitespace(token, OriginalCode, data);
                break;
            case char c when CharSets.letters.Contains(c) || c == '_':
                DefaultAlpha(token, OriginalCode, data);
                break;
            default:
                break;
        }
    }

    private static void DefaultBrackets(TokenDelegate? token, char c, string originalCode, TokenizeData data)
    {
        var _iBrackets = CharSets.bracketsSet.IndexOf(c);
        if (_iBrackets % 2 != 0)
            data.Stack -= 1;
        EmitToken(token, data, CodeBlockType.Bracket, originalCode, 1);
        data.Pos2 = data.Pos + 1;
        if (_iBrackets % 2 == 0)
            data.Stack += 1;
    }

    private static void DefaultAlpha(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {

        if (!data.flag && (data.flag = true)
            && GetText(data, OriginalCode).Trim().EndsWith(")"))
        {
            EmitToken(token, data, CodeBlockType.Operation, OriginalCode);
            data.Pos2 = data.Pos;
        }
        data.State = 1;
    }
    private static void DefaultWhitespace(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        if (data.flag && !(data.flag = false))
        {
            string testc;
            if (!string.IsNullOrEmpty(testc = GetText(data, OriginalCode).Trim()))
            {
                if (_reservedWords.Contains(testc))
                    EmitToken(token, data, CodeBlockType.Declaration, OriginalCode);
                else
                    EmitToken(token, data, CodeBlockType.Operation, OriginalCode);
            }
            data.Pos2 = data.Pos;
        }
    }

    private static void DefaultNumbers(TokenDelegate? token, string originalCode, TokenizeData data)
    {
        data.State = 8;
    }
    private static void DefaultString(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        data.State = 6; // "Normal" String
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

    private static void DefaultOperator(TokenDelegate? token, string originalCode, TokenizeData data)
    {
        char cNxt = GetNxtChar(data.Pos, originalCode);
        if (CharSets.operatorSet.Contains(cNxt) && cNxt != '/') ;
        else
        {
            EmitToken(token, data, CodeBlockType.Operation, originalCode, 1);
            data.Pos2 = data.Pos + 1;
        }
    }
    private static void DefaultComment(TokenDelegate? token, string OriginalCode, TokenizeData data, int iNewState)
    {
        if (!string.IsNullOrWhiteSpace(GetText(data, OriginalCode)))
            EmitToken(token, data, CodeBlockType.Operation, OriginalCode);
        data.Pos2 = data.Pos;
        data.State = iNewState;
        data.Pos++;
    }

    private static void HandleNumbers(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        char c = char.ToLower(OriginalCode[data.Pos]);
        if (CharSets.numbersExt.Contains(c)
            && (c != '.' || GetNxtChar(data.Pos, OriginalCode) != '.')
            && (c != '-' || char.ToLower(GetPrvChar(data.Pos, OriginalCode)) == 'e'))
        {

            if (data.Pos == OriginalCode.Length - 1)
            {
                data.Pos++;
                EmitToken(token, data, CodeBlockType.Number, OriginalCode);
            }
            return;
        }
        else
        {
            EmitToken(token, data, CodeBlockType.Number, OriginalCode);
            data.Pos2 = data.Pos;
            data.State = 0;
            HandleDefault(token, OriginalCode, data);
        }
    }
    private static void HandleAlpha(TokenDelegate? token, string OriginalCode, TokenizeData data)
    {
        if (CharSets.lettersAndNumbers.Contains(OriginalCode[data.Pos]) || OriginalCode[data.Pos] == '_')
        {
            if (data.Pos == OriginalCode.Length - 1)
            {
                data.Pos++;
                TestEmit(token, OriginalCode, data);
            }
            return;
        }
        else if (OriginalCode[data.Pos] == '#' && CharSets.numbers.Contains(GetNxtChar(data.Pos, OriginalCode)))
        {
            int _iResWrdIdx = _reservedWords.IndexOf(GetText(data, OriginalCode).Trim().ToUpper());
            if (_iResWrdIdx > -1 && _sysTypes.TryGetValue((IECResWords)_iResWrdIdx, out var typeCode))
            {
                data.State = 8;
            }
            else
            {
                TestEmit(token, OriginalCode, data);
                HandleDefault(token, OriginalCode, data);
            }
        }
        else
        {
            TestEmit(token, OriginalCode, data);
            HandleDefault(token, OriginalCode, data);
        }

        static void TestEmit(TokenDelegate? token, string OriginalCode, TokenizeData data)
        {
            int _iResWrdIdx = _reservedWords.IndexOf(GetText(data, OriginalCode).Trim().ToUpper());
            if (_iResWrdIdx > -1)
            {
                if (_blockWords.TryGetValue((IECResWords)_iResWrdIdx, out var iECResWordEnd))
                {
                    EmitToken(token, data, CodeBlockType.Block, OriginalCode);
                    data.Stack += 1;
                }
                else if (_blockWords.Values.Contains((IECResWords)_iResWrdIdx))
                {
                    data.Stack -= 1;
                    EmitToken(token, data, CodeBlockType.Block, OriginalCode);
                }
                else
                {
                    EmitToken(token, data, CodeBlockType.Function, OriginalCode);
                }
            }
            else
            {
                EmitToken(token, data, CodeBlockType.Variable, OriginalCode);
            }
            data.Pos2 = data.Pos;
            data.State = 0;
        }
    }


    private static void HandleBlockComments(TokenDelegate? token, string code, TokenizeData data)
    {
        if (code[data.Pos] == '*' && data.Pos < code.Length - 2 && code[data.Pos + 1] == ')')
        {
            EmitToken(token, data, CodeBlockType.Comment, code, 2);
            data.Pos2 = data.Pos + 2;
            data.State = 0; // Block-Comment-end
            data.Pos++;
        }
    }

    private static void HandleLineComments(TokenDelegate? token, string code, TokenizeData data)
    {
        if (code[data.Pos] == '\r')
        {
            var p = data.Pos2;
            while (p > 0 && code[--p] is ' ' or '\t')
                ;
            if (code[p] == '\n' || code[p] == '\r' || p == 0)
                EmitToken(token, data, CodeBlockType.FLComment, code);
            else
                EmitToken(token, data, CodeBlockType.LComment, code);
            data.Pos2 = data.Pos + 1;
            data.State = 0;
        }
    }

    private static void HandleStrings(TokenDelegate? token, string code, TokenizeData data)
    {
        if (new[] { '"', '\'' }.Contains(code[data.Pos]))

            switch (code[data.Pos])
            {
                case '"':// String-End
                case '\r' when data.State != 5:
                    if (code[data.Pos] == '\r')
                        EmitToken(token, data, CodeBlockType.String, code);
                    else
                        EmitToken(token, data, CodeBlockType.String, code, 1);
                    data.Pos2 = data.Pos + 1;
                    data.State = 0;
                    break;
            }
    }


    public bool TryGetValue(int state, out Action<TokenDelegate?, string, TokenizeData> handler)
    {
        return _tokenStateHandler.TryGetValue(state, out handler);
    }
}



