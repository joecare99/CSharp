namespace TranspilerLib.DriveBASIC.Data.Interfaces;

public interface IDriveCommand
{
    EDriveToken Token { get; }
    byte SubToken { get; }
    int Par1 { get; }
    int Par2 { get; }
    double Par3 { get; }
}