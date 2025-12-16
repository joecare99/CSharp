using System;

namespace TranspilerLib.Pascal.Data;

/// <summary>
/// Pascal / Delphi reserved words subset. Extend list as needed.
/// </summary>
public static class PasReservedWords
{
    public static readonly string[] Words = Enum.GetNames(typeof(EPasResWords));
}
