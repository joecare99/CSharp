using TranspilerLib.DriveBASIC.Data.Interfaces;

namespace TranspilerLib.DriveBASIC.Models;


public sealed class CompilerLabel : ILabel
{
    public string? Name { get; set; }
    public int Index { get; set; } = -1;
}

