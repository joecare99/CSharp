using System;
using System.Collections.Generic;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models;
using TranspilerLib.Models.Scanner;
using static TranspilerLib.Interfaces.Code.ICodeBase;

namespace TranspilerLib.Pascal.Models.Scanner;

/// <summary>
/// Very small initial Pascal/Delphi token handler. Supports
/// - line comments (//)
/// - block comments (* .. *) and { .. }
/// - strings using single quotes with doubled quote escape
/// - basic block delimiters (BEGIN/END) are emitted as operations then interpreted by builder.
/// Semicolon is treated as separator, not mandatory terminator - we emit it as Operation but do not force block close.
/// </summary>
public class PasTokenHandler : TokenHandlerBase, ITokenHandler
{
    private enum PasTokenState
    {
        Default = 0,
        InString = 1,
        InLineComment = 2,
        InBlockCommentStar = 3,
        InBlockCommentBrace = 4,
        InIdentifier = 5,
        InNumber = 6,
    }

    private static readonly Dictionary<Enum, Action<TokenDelegate?, string, TokenizeData>> _tokenStateHandler = new()
    {
        {PasTokenState.Default, HandleDefault},
        {PasTokenState.InString, HandleString},
        {PasTokenState.InLineComment, HandleLineComment},
        {PasTokenState.InBlockCommentStar, HandleBlockCommentStar},
        {PasTokenState.InBlockCommentBrace, HandleBlockCommentBrace},
        {PasTokenState.InIdentifier, HandleIdentifyer},
        {PasTokenState.InNumber, HandleNumber},
    };

    private static void HandleNumber(TokenDelegate? token, string code, TokenizeData data)
    {
        var nxtChar = GetNxtChar(data.Pos, code);
        if (!char.IsDigit(nxtChar) && nxtChar != 'e' && nxtChar != 'E' && nxtChar != '-' && nxtChar != '.'
           || code[data.Pos] =='-' && !char.IsDigit(nxtChar))
        {
            EmitToken(token, data, CodeBlockType.Number, code, 1);
            data.Pos2 = data.Pos + 1;
            data.State = 0;
        }
    }

    private static string[] _reservedWords = Array.Empty<string>();
    public string[] reservedWords { set => _reservedWords = value.Select(s=>s.ToUpper()).ToArray(); }

    private static void HandleString(TokenDelegate? token,string code, TokenizeData data)
    {
        if (code[data.Pos]=='\'' && GetNxtChar(data.Pos,code)=='\'') // doubled quote -> skip next
        {
            data.Pos++; // stay in string
            return;
        }
        if (code[data.Pos]=='\'')
        {
            EmitToken(token,data,CodeBlockType.String,code,1);
            data.Pos2 = data.Pos+1;
            data.State=0;
        }
    }

    private static void HandleLineComment(TokenDelegate? token,string code, TokenizeData data)
    {
        if (code[data.Pos]=='\n' || data.Pos==code.Length-1)
        {
            EmitToken(token,data,CodeBlockType.LComment,code);
            data.Pos2 = data.Pos+1;
            data.State=0;
        }
    }

    private static void HandleBlockCommentStar(TokenDelegate? token,string code, TokenizeData data)
    {
        if (code[data.Pos]=='*' && GetNxtChar(data.Pos,code)==')')
        {
            EmitToken(token,data,CodeBlockType.Comment,code,2);
            data.Pos2 = data.Pos+2;
            data.State=0;
            data.Pos++; // skip )
        }
    }
    private static void HandleBlockCommentBrace(TokenDelegate? token,string code, TokenizeData data)
    {
        if (code[data.Pos]=='}')
        {
            EmitToken(token,data,CodeBlockType.Comment,code,1);
            data.Pos2 = data.Pos+1;
            data.State=0;
        }
    }

    private static void HandleIdentifyer(TokenDelegate? token, string code, TokenizeData data)
    {
        var nxtChar = GetNxtChar(data.Pos,code);
        if (!char.IsLetterOrDigit(nxtChar) && nxtChar != '_')
        {
            var text = GetText(data,code,1);
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (_reservedWords.Contains(text.ToUpper()))
                {
                    if (text.ToUpper() == "BEGIN" || text.ToUpper() == "END")
                        EmitToken(token, data, CodeBlockType.Block, code,1);
                    else
                        EmitToken(token,data,CodeBlockType.Operation,code, 1);
                }
                else
                {
                    EmitToken(token,data,CodeBlockType.Variable,code, 1);
                }
            }
            data.Pos2 = data.Pos+1;
            data.State=0;
        }
    }

    private static void HandleDefault(TokenDelegate? token,string code, TokenizeData data)
    {
        switch(code[data.Pos])
        {
            case '\'': // string start
                data.Pos2 = data.Pos;
                data.State= (int)PasTokenState.InString;
                break;
            case '/' when GetNxtChar(data.Pos,code)=='/':
                data.Pos2=data.Pos;
                data.State= (int)PasTokenState.InLineComment; 
                data.Pos++; // consume second /
                break;
            case '(' when GetNxtChar(data.Pos,code)=='*': // (* comment *)
                data.Pos2=data.Pos;
                data.State= (int)PasTokenState.InBlockCommentStar; 
                data.Pos++; // skip *
                break;
            case '{': // { comment }
                data.Pos2=data.Pos;
                data.State= (int)PasTokenState.InBlockCommentBrace;
                break;
            case ';':
                // separator - emit standalone token but do not force statement end semantics
                EmitToken(token,data,CodeBlockType.Separator,code,1);
                data.Pos2=data.Pos+1;
                break;
            case ':' when GetNxtChar(data.Pos, code) == '=':
                EmitToken(token,data,CodeBlockType.Assignment,code,2);
                data.Pos2 = data.Pos + 2;
                data.Pos++;
                break;
            case ':' :
                EmitToken(token, data, CodeBlockType.Operation, code, 1);
                data.Pos2 = data.Pos + 1;
                break;
            case ' ':
            case '\t':
            case '\r':
            case '\n':
                // boundary between identifiers and operations
                break;
            default:
                if (char.IsLetter(code[data.Pos]) || code[data.Pos]=='_')
                {
                    data.Pos2 = data.Pos;
                    data.State = (int)PasTokenState.InIdentifier;
                    data.Pos--;
                }
                else if (char.IsDigit(code[data.Pos]) || (code[data.Pos]=='-' && char.IsDigit(GetNxtChar(data.Pos,code))))
                {
                    data.Pos2 = data.Pos;
                    data.State = (int)PasTokenState.InNumber;
                    data.Pos--;
                }
                else
                {
                    // operation
                    EmitToken(token,data,CodeBlockType.Operation,code,1);
                    data.Pos2 = data.Pos+1;
                }
                break;
        }
    }

    public bool TryGetValue(int state, out Action<TokenDelegate?, string, TokenizeData> handler)
        => _tokenStateHandler.TryGetValue((PasTokenState)state,out handler);
}
