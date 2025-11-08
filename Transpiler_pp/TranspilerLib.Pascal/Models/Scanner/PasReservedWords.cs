namespace TranspilerLib.Pascal.Models.Scanner;

/// <summary>
/// Pascal / Delphi reserved words subset. Extend list as needed.
/// </summary>
public static class PasReservedWords
{
    public static readonly string[] Words = new[]
    {
        "PROGRAM","UNIT","INTERFACE","IMPLEMENTATION","USES","BEGIN","END","VAR","CONST","TYPE","FUNCTION","PROCEDURE","IF","THEN","ELSE","FOR","TO","DOWNTO","DO","WHILE","REPEAT","UNTIL","CASE","OF","WITH","TRY","EXCEPT","FINALLY","CLASS","PRIVATE","PUBLIC","PROTECTED","PUBLISHED" 
    };
}
