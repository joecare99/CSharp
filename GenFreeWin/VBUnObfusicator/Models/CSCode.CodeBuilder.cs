using System;
using System.Collections.Generic;
using System.Linq;
using static VBUnObfusicator.Models.ICSCode;

namespace VBUnObfusicator.Models
{
    public partial class CSCode
    {
        private static class CodeBuilder
        {
            public class CodeBuilderData
            {
                public Dictionary<string, ICodeBlock> labels = new();
                public List<ICodeBlock> gotos = new();
                public ICodeBlock actualBlock;
                public bool xBreak = false;
                public CodeBlockType cbtLast = CodeBlockType.Unknown;
                public CodeBuilderData(ICodeBlock codeBlock) {
                    actualBlock = codeBlock;
                }
            }

            public static void OnToken(TokenData tokenData,CodeBuilderData data)
            {
                switch (tokenData.type)
                {
                    default:
                        break;
                    case CodeBlockType.Instruction when !string.IsNullOrEmpty(tokenData.Code):
                        BuildInstruction(tokenData, data);
                        break;
                    case CodeBlockType.Goto:
                        BuildGoto(tokenData, data);
                        break;
                    case CodeBlockType.Comment:
                    case CodeBlockType.LComment:
                        BuildComment(tokenData, data);
                        break;
                    case CodeBlockType.String:
                        BuildString(tokenData, data);
                        break;
                    case CodeBlockType.Label:
                        tokenData = BuildLabel(tokenData, data);
                        break;
                    case CodeBlockType.Block when tokenData.Code == "{":
                        BuildBlockStart(tokenData, data);
                        break;
                    case CodeBlockType.Block:
                        BuildBlockEnd(tokenData, data);
                        break;

                }
                data.cbtLast = tokenData.type;
            }

            private static void BuildBlockEnd(TokenData tokenData, CodeBuilderData data)
            {
                _ = new CodeBlock() { Name = $"{tokenData.type}End", Type = tokenData.type, Code = tokenData.Code, Parent = data.actualBlock.Parent };
                data.actualBlock = data.actualBlock.Parent;
            }

            private static void BuildBlockStart(TokenData tokenData, CodeBuilderData data)
            {
                data.actualBlock = new CodeBlock() { Name = $"{tokenData.type}Start", Type = tokenData.type, Code = tokenData.Code, Parent = (ICodeBlock?)data.actualBlock };
                data.xBreak = true;
            }

            private static TokenData BuildLabel(TokenData tokenData, CodeBuilderData data)
            {
                if (tokenData.Code.Contains(",") || tokenData.Code.Contains("(")) //not cElse label
                {
                    if (data.actualBlock.Type is not CodeBlockType.Instruction || data.actualBlock.Code.EndsWith(";"))
                        data.actualBlock = new CodeBlock() { Name = $"Instruction", Type = CodeBlockType.Instruction, Code = "", Parent = data.actualBlock.Parent };
                    data.actualBlock.Code += tokenData.Code + " ";
                    tokenData.type = CodeBlockType.Instruction; //!! not cElse label
                }
                else
                {
                    if (data.actualBlock.Type is CodeBlockType.Instruction && data.actualBlock.Code.StartsWith("case ") && tokenData.Code == ":")
                    {
                        data.actualBlock.Type = CodeBlockType.Label;
                        data.actualBlock.Code += tokenData.Code;
                    }
                    else
                        data.actualBlock = new CodeBlock() { Name = $"{tokenData.type}", Type = tokenData.type, Code = tokenData.Code, Parent = data.actualBlock.Parent };
                    if (!data.labels.ContainsKey(tokenData.Code))
                        data.labels.Add(tokenData.Code, data.actualBlock);
                }

                return tokenData;
            }

            private static void BuildString(TokenData tokenData, CodeBuilderData data)
            {
                if (data.actualBlock.Type is not CodeBlockType.Instruction)
                    data.actualBlock = new CodeBlock() { Name = $"Instruction", Type = CodeBlockType.Instruction, Code = "", Parent = data.actualBlock.Parent };
                else if (data.actualBlock.Code.EndsWith("+") || data.actualBlock.Code.EndsWith("=") || data.actualBlock.Code.EndsWith(",") || data.actualBlock.Code.EndsWith("case"))
                    data.actualBlock.Code += " ";
                data.actualBlock.Code += tokenData.Code;
            }

            private static void BuildComment(TokenData tokenData, CodeBuilderData data)
            {
                data.actualBlock = new CodeBlock() { Name = $"Comment", Type = tokenData.type, Code = tokenData.Code, Parent = data.actualBlock.Parent };
            }

            private static void BuildGoto(TokenData tokenData, CodeBuilderData data)
            {
                data.actualBlock = new CodeBlock() { Name = $"{tokenData.type}", Type = tokenData.type, Code = tokenData.Code, Parent = data.actualBlock.Parent };
                data.gotos.Add((ICodeBlock?)data.actualBlock);
                data.xBreak = false;
            }

            private static void BuildInstruction(TokenData tokenData, CodeBuilderData data)
            {
                if (data.actualBlock.Type is not CodeBlockType.Instruction and not CodeBlockType.MainBlock
                    || (data.cbtLast is not CodeBlockType.Instruction and not CodeBlockType.String and not CodeBlockType.Unknown)
                    || (!string.IsNullOrEmpty(data.actualBlock.Code) && data.actualBlock.Code.EndsWith(";")))
                {
                    data.actualBlock = new CodeBlock() { Name = $"{tokenData.type}", Type = tokenData.type, Code = tokenData.Code, Parent = data.actualBlock.Parent };
                }
                else
                    data.actualBlock.Code += ((data.actualBlock.Code.EndsWith("\"") && tokenData.Code.StartsWith("+"))
                        || tokenData.Code.StartsWith("(")
                        || (letters.Contains(tokenData.Code[0]) 
                            && !string.IsNullOrEmpty(data.actualBlock.Code)
                            && !data.actualBlock.Code.EndsWith(" ") ) ? " " : "") + tokenData.Code;
                data.xBreak = tokenData.Code.Contains("break;");
            }
        }

    }
}
