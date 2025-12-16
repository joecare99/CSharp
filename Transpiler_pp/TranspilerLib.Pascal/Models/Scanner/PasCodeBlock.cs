using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranspilerLib.Data;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.Pascal.Models.Scanner;

public class PasCodeBlock : CodeBlock
{
    public override string ToCode(int indent = 4)
    {
        switch (Type)
        {
            case CodeBlockType.Function:
                return $"{new string(' ', indent)}{Code};\r\n{string.Join(string.Empty, SubBlocks.Select(sb => sb.ToCode(indent + 4)))}{new string(' ', indent)}end;\r\n";
            case CodeBlockType.Block:
                return $"{new string(' ', indent)}begin\r\n{string.Join(string.Empty, SubBlocks.Select(sb => sb.ToCode(indent + 4)))}{new string(' ', indent)}end;\r\n";
            case CodeBlockType.MainBlock:
                return $"{new string(' ', indent)}{Code}{(string.IsNullOrEmpty(Code) ? "" : ";")}\r\n{string.Join(string.Empty, SubBlocks.Select(sb => sb.ToCode(indent)))}";
            default:
                return $"{new string(' ', indent)}{Code};\r\n";
        }
    }
    /// <summary>
    /// Debug-/Diagnoseausgabe des Blockes inklusive Meta-Informationen (Name, Typ, Position, Quellverweise).
    /// </summary>
    public override string ToString()
    {
        return $"///{Name} {Type} {Level},{SourcePos},{Index}{(Destination != null ? " Dest:OK" : "")}{(Sources.Count > 0 ? $" {Sources.Count}" : "")}\r\n{Code}{(SubBlocks.Count > 0 ? Environment.NewLine : string.Empty)}{string.Join(Environment.NewLine, SubBlocks)}";
    }

}
