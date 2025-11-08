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
/// This is intentionally lightweight; extend as needed.
/// </summary>
public class PasTokenHandler : TokenHandlerBase, ITokenHandler
{
    private static readonly Dictionary<int, Action<TokenDelegate?, string, TokenizeData>> _tokenStateHandler = new()
    {
        {0, HandleDefault},
        {1, HandleString},
        {2, HandleLineComment},
        {3, HandleBlockCommentStar},
        {4, HandleBlockCommentBrace},
    };

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

    private static void HandleDefault(TokenDelegate? token,string code, TokenizeData data)
    {
        switch(code[data.Pos])
        {
            case '\'': // string start
                EmitToken(token,data,CodeBlockType.Operation,code);
                data.Pos2 = data.Pos;
                data.State=1;
                break;
            case '/' when GetNxtChar(data.Pos,code)=='/':
                EmitToken(token,data,CodeBlockType.Operation,code);
                data.Pos2=data.Pos;
                data.State=2; data.Pos++; // consume second /
                break;
            case '(' when GetNxtChar(data.Pos,code)=='*': // (* comment *)
                EmitToken(token,data,CodeBlockType.Operation,code);
                data.Pos2=data.Pos;
                data.State=3; data.Pos++; // skip *
                break;
            case '{': // { comment }
                EmitToken(token,data,CodeBlockType.Operation,code);
                data.Pos2=data.Pos;
                data.State=4;
                break;
            case ';':
                EmitToken(token,data,CodeBlockType.Operation,code,1);
                data.Pos2=data.Pos+1;
                break;
            case ':':
                // label (identifier before colon)
                var text = GetText(data,code);
                if (!string.IsNullOrWhiteSpace(text) && text.Trim().All(ch=>char.IsLetterOrDigit(ch)||ch=='_'))
                {
                    EmitToken(token,data,CodeBlockType.Label,code,1);
                    data.Pos2 = data.Pos+1;
                }
                break;
            case ' ':
            case '\t':
            case '\r':
            case '\n':
                // boundary between identifiers and operations
                if (data.flag)
                {
                    data.flag=false;
                    var t = GetText(data,code).Trim();
                    if (_reservedWords.Contains(t.ToUpper()))
                    {
                        EmitToken(token,data,CodeBlockType.Operation,code);
                        data.Pos2 = data.Pos;
                    }
                }
                break;
            default:
                if (char.IsLetter(code[data.Pos]) || code[data.Pos]=='_')
                {
                    data.flag=true;
                }
                break;
        }
    }

    public bool TryGetValue(int state, out Action<TokenDelegate?, string, TokenizeData> handler)
        => _tokenStateHandler.TryGetValue(state,out handler);
}
