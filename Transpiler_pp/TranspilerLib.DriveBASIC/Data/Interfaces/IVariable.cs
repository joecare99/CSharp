namespace TranspilerLib.DriveBASIC.Data.Interfaces;

public interface IVariable
{
    string? Name { get; }
    int Index { get; }
    object? Value { get; set; }
}