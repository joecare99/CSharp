using BaseLib.Helper;
using System;
using System.Linq;
using static VBUnObfusicator.Models.ICSCode;

namespace VBUnObfusicator.Models
{
    public partial class CSCode
    {
        private static class TokenHandler
        {
            private static void EmitToken(TokenDelegate? token, TokenizeData data, CodeBlockType type, string originalCode, int offs = 0)
                => token?.Invoke(new TokenData(originalCode.Substring(data.Pos2, data.Pos - data.Pos2 + offs).Trim(), type, data.Stack));

            internal static void HandleBlockComments(TokenDelegate? token, string code, TokenizeData data)
            {
                if (code[data.Pos] == '*' && data.Pos < code.Length - 2 && code[data.Pos + 1] == '/')
                {
                    EmitToken(token, data, CodeBlockType.Comment, code, 2);
                    data.Pos2 = data.Pos + 2;
                    data.State = 0; // Block-Comment-end
                    data.Pos++;
                }
            }

            internal static void HandleLineComments(TokenDelegate? token, string code, TokenizeData data)
            {
                if (code[data.Pos] == '\r')
                {
                    EmitToken(token, data, CodeBlockType.LComment, code);
                    data.Pos2 = data.Pos + 1;
                    data.State = 0;
                }
            }

            internal static void HandleStrings(TokenDelegate? token, string code, TokenizeData data)
            {
                if (stringEndChars.Contains(code[data.Pos]))

                    switch (code[data.Pos])
                    {
                        case '\\' when data.State != 5: // Escape
                        case '{' when (data.State == 4) && (code[data.Pos + 1] == '{'):
                        case '"' when (data.State == 5) && (code[data.Pos + 1] == '"'):
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
                    }

            }

            internal static void HandleDefault(TokenDelegate? token, string OriginalCode, TokenizeData data)
            {
                char GetNxtChar(int Pos) => Pos + 1 < OriginalCode.Length ? OriginalCode[Pos + 1] : '\u0000';
                switch (OriginalCode[data.Pos])
                {
                    case '{':
                        DefaultBlock(token, OriginalCode, data, xStart:true);
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
                    case '/' when GetNxtChar(data.Pos) == '/':
                        DefaultComment(token, OriginalCode, data, 2);
                        break;
                    case '/' when GetNxtChar(data.Pos) == '*':
                        DefaultComment(token, OriginalCode, data, 3);
                        break;
                    case ' ' or '\r' or '\n' or '\t':
                        DefaultWhitespace(token, OriginalCode, data);
                        break;
                    case (>= 'A' and <= 'Z') or (>= 'a' and <= 'z'):
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
                    EmitToken(token, data, CodeBlockType.Instruction, OriginalCode);
                    data.Pos2 = data.Pos;
                }
            }

            private static void DefaultWhitespace(TokenDelegate? token, string OriginalCode, TokenizeData data)
            {
                if (data.flag && !(data.flag = false)
                    && GetText(data, OriginalCode).EndswithAny(reservedWords))
                {
                    EmitToken(token, data, CodeBlockType.Instruction, OriginalCode);
                    data.Pos2 = data.Pos;
                }
            }

            private static void DefaultComment(TokenDelegate? token, string OriginalCode, TokenizeData data, int iNewState)
            {
                EmitToken(token, data, CodeBlockType.Instruction, OriginalCode);
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
                EmitToken(token, data, CodeBlockType.Instruction, OriginalCode);
                data.Pos2 = data.Pos;
                if (data.State != 1)
                    data.Pos++;
            }

            private static void DefaultLabel(TokenDelegate? token, string OriginalCode, TokenizeData data)
            {
                if (!GetText(data, OriginalCode).Trim().Contains('?'))
                {
                    EmitToken(token, data, CodeBlockType.Label, OriginalCode, 1);
                    data.Pos2 = data.Pos+1;
                }
            }

            private static void DefaultInstructionEnd(TokenDelegate? token, string OriginalCode, TokenizeData data)
            {
                var text = GetText(data, OriginalCode, 1);
                EmitToken(token, data, text.Contains("goto ") ? CodeBlockType.Goto : CodeBlockType.Instruction, OriginalCode, 1);
                data.Pos2 = data.Pos + 1;
            }

            private static void DefaultBlock(TokenDelegate? token, string OriginalCode, TokenizeData data, bool xStart = false, bool xEnd = false)
            {
                if (!string.IsNullOrEmpty(GetText(data, OriginalCode).Trim()))
                    EmitToken(token, data, CodeBlockType.Instruction, OriginalCode);
                if (xStart)
                    data.Stack += 1;
                data.Pos2 = data.Pos;
                EmitToken(token, data, CodeBlockType.Block, OriginalCode, 1);
                data.Pos2 = data.Pos + 1;
                if (xEnd)
                    data.Stack -= 1;
            }

            private static string GetText(TokenizeData data, string OriginalCode = "", int offs = 0)
                => OriginalCode.Substring(data.Pos2, Math.Max(data.Pos - data.Pos2 + offs, 0));
            private static char GetPrvChar(int Pos, string OriginalCode)
                => OriginalCode[Math.Max(Pos - 1, 0)];

        }

    }
}
