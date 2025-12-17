namespace TranspilerLib.DriveBASIC.Data;

public record ParseDef(EDriveToken Token = EDriveToken.tt_Nop, int SubToken=0, string? text =null);
