using System;
using TranspilerLib.DriveBASIC.Data.Interfaces;

namespace TranspilerLib.DriveBASIC.Data;

public class DriveCommand : IDriveCommand
{
    public EDriveToken Token { get; }
    public byte SubToken { get; }
    public int Par1 { get; set; }
    public int Par2 { get; }
    public double Par3 { get; }

    public DriveCommand(EDriveToken token, byte sub = 0, int par1 = 0, int par2 = 0, double par3 = 0)
    {
        this.Token = token;
        this.SubToken = sub;
        this.Par1 = par1;
        this.Par2 = par2;
        this.Par3 = par3;
    }

    public DriveCommand(EDriveToken token, object[] oExp)
    {
        this.Token = token;
        var Pc = 0;
        foreach (var o in oExp)
        {
            switch (o)
            {
                case int i when ++Pc == 1: SubToken = (byte)i; break;
                case int i when Pc == 2: Par1 = i; break;
                case int i when Pc == 3: Par2 = i; break;
                case int i when Pc == 4: Par3 = i; break;
                case double d: Par3 = d; Pc = 4; break;
                case decimal f: Par3 = (double)f; Pc = 4; break;
                default:
                    throw new ArgumentException($"Ungültiger Parameter-Typ {o.GetType()} in DriveCommand-Array", nameof(oExp));
            }
        }
    }

    public override string ToString()
    {
        return $"DriveCommand(Token={Token}, SubToken={SubToken}, Par1={Par1}, Par2={Par2}, Par3={Par3})";
    }

    public override bool Equals(object? obj)
    {
        return obj is IDriveCommand dc?
            Token == dc.Token 
            && SubToken == dc.SubToken 
            && Par1 == dc.Par1
            && Par2 == dc.Par2
            && Par3 == dc.Par3
            : false;
    }
}