using TranspilerLib.DriveBASIC.Data;
using static TranspilerLib.DriveBASIC.Data.ParseDefinitions;

namespace TranspilerLib.DriveBASIC.Models;

public sealed class CommandBuilderState
{
    public bool HasToken { get; private set; }
    public EDriveToken Token { get; private set; }
    public int SubToken { get; private set; }
    public int Param1 { get; set; }
    public int Param2 { get; set; }
    public double Param3 { get; set; }

    public void Initialize(EDriveToken token, int subToken)
    {
        HasToken = true;
        Token = token;
        SubToken = TteTokens.Contains(token) ? subToken * 64 : subToken;
        Param1 = 0;
        Param2 = 0;
        Param3 = 0d;
    }

    public void AddSubToken(int delta) => SubToken += delta;

    public DriveCommand ToDriveCommand()
        => new(Token, [(SubToken & 0xFF), Param1, Param2, Param3]);
}

