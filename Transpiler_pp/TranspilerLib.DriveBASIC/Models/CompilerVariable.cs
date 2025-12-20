using TranspilerLib.DriveBASIC.Data;
using TranspilerLib.DriveBASIC.Data.Interfaces;

namespace TranspilerLib.DriveBASIC.Models;

public sealed class CompilerVariable : IVariable
{
    public string? Name { get; set; }
    public int Index { get; set; }
    public object? Value { get; set; }
    public EVarType Type { get; set; }
}

