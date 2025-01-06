using System;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner
{
    public class IECCodeBlock : CodeBlock, ICodeBlock
    {

        /// <summary>
        /// Converts to code.
        /// </summary>
        /// <param name="indent">The indent.</param>
        /// <returns>string.</returns>
        public 
            override 
            string ToCode(int indent = 2)
        {
            string codeComment = string.Empty;
            string subCode = string.Empty;
            if (Type is CodeBlockType.Label && Sources.Count > 2)
                codeComment = $" // <========== {Sources.Count}";
            if (SubBlocks?.Count > 0 && Type is CodeBlockType.Operation or CodeBlockType.Assignment)
                subCode = string.Join(' '+Code, SubBlocks.Select((c) => c.ToCode(indent + 2)));
            else if (SubBlocks?.Count > 1)
                subCode = string.Join(' ', SubBlocks.Select((c) => c.ToCode(indent + 2)));
            
            if (new[] { CodeBlockType.LComment, CodeBlockType.FLComment, CodeBlockType.Comment }.Contains(Type))
                return $"{new string(' ', indent)}{Code}{Environment.NewLine}";
            else if (Type == CodeBlockType.Operation && new[] { ":", ";" }.Contains(Code)
                && Next?.Type != CodeBlockType.LComment
                && (Next?.Type == CodeBlockType.Block || Code != ":"))
                return $"{Code}{codeComment}{Environment.NewLine}";
            else if (Type is CodeBlockType.Operation or CodeBlockType.Assignment && SubBlocks?.Count>0)
                return $"{subCode}";
            else if (Type == CodeBlockType.Variable)
                return $" {Code}";
            else
                return $"{new string(' ', Type is CodeBlockType.Block or CodeBlockType.Label ? indent - 4 : 1)}{Code}{codeComment}{(SubBlocks.Count > 0 ? "\r\n" : string.Empty)}{subCode}";
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>string.</returns>
        public override string ToString()
        {
            return $"///{Name} {Type} {Level},{SourcePos},{Index}{(Destination != null ? " Dest:OK" : "")}{(Sources.Count > 0 ? $" {Sources.Count}" : "")}\r\n{Code}{(SubBlocks.Count > 0 ? Environment.NewLine : string.Empty)}{string.Join(Environment.NewLine, SubBlocks)}";
        }
    }
}