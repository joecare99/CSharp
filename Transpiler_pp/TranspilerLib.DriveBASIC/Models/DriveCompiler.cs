using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.DriveBASIC.Data;
using TranspilerLib.DriveBASIC.Data.Interfaces;

namespace TranspilerLib.DriveBASIC.Models;

public partial class DriveCompiler
{
    //  resourcestring
    const string
        StrKeineQuelleIstZw = "Keine Quelle, ist zwingend erforderlich",
        StrLeererTokenCodeOK = "Leerer TokenCode (OK)",
        StrLeeresLabelArray = "Leeres Label-Array (OK)",
        StrFehlerBeimParsen = "Fehler beim Parsen :",
        StrKeinSystemVariab = "Kein (System-)Variablen-Array (OK)",
        StrLeeresMessageArray = "Leeres Message-Array (OK)",
        StrFehlerBeiAusdruck = "Fehler bei Ausdruck",
        StrSyntaxError = "Syntax Error :",
        StrUnbekannterPlatzhal = "Unbekannter Platzhalter",
        StrFehlerInBuildComm = "Fehler in Build-Command",
        StrUngueltigesZeichenF = "Ungültiges Zeichen für Platzhalter: ",
        StrReferenzNichtGefun = "Referenz nicht gefunden ";

    public IList<string> Log { get; set; }
    public IEnumerable<string> SourceCode { get; set; }
    public IList<string> Messages { get; set; }
    public IList<IVariable> Variables { get; set; }
    public IList<ILabel> Labels { get; set; }
    public IList<IDriveCommand> TokenCode { get; set; }

    ParseDef[] parseDefs;
    ParseDef2[] PlaceHolderSubst = ParseDefinitions.PlaceHolderDefBase;
    IList<(int, bool, string)> expressionNormals = [];
    public IReadOnlyList<(int, bool, string)> ExpressionNormals => expressionNormals.ToList();

    public static string CExpression { get; } = ParseDefinitions.CExpression;
    private readonly string CToken = ParseDefinitions.CToken;
    private readonly string CCommand = ParseDefinitions.CCommand;

    string[] PlaceHolders = ParseDefinitions.ParseStrings;
    Dictionary<EDriveToken, int> TextsPerToken = [];
    Dictionary<EDriveToken, int> Sindex = [];
    char[] replace = ['§', 'm', '¹', '²', '³', 'f', '1', '2', '3', 'x', ' ', 't', 'a', '¹', '²', '³'];

    private static readonly Dictionary<char, int> AxisMap = new()
    {
        ['x'] = 1,
        ['1'] = 1,
        ['y'] = 2,
        ['2'] = 2,
        ['z'] = 3,
        ['3'] = 3,
        ['a'] = 4,
        ['4'] = 4,
        ['b'] = 5,
        ['5'] = 5,
        ['c'] = 6,
        ['6'] = 6
    };


    private const int VarTypeStride = 0x4000;
    private const int PointBase = 0x8000;
    private const int DimensionBase = 0xC000;
    private const int MaxRecursionDepth = 15;

    public DriveCompiler()
    {
        parseDefs = ParseDefinitions.ParseDefBase
            .Select(def => DefinitionUsesExpression(def.text)
                ? def with { SubToken = def.SubToken * 64 }
                : def)
            .ToArray();

        foreach (var def in parseDefs)
        {
            if (!TextsPerToken.TryGetValue(def.Token, out _))
                TextsPerToken.Add(def.Token, 0);
            TextsPerToken[def.Token] += 1;
        }
        foreach (EDriveToken def in Enum.GetValues(typeof(EDriveToken)))
        {
            if (!Sindex.TryGetValue(def, out _))
                Sindex.Add(def, 0);
            if (Sindex.TryGetValue(def - 1, out int s1))
                Sindex[def] = s1 + (TextsPerToken.TryGetValue(def, out int i) ? i : 0);
        }

        BuildExpressionNormal(CExpression, expressionNormals.Add);
    }

    private string GetAxisName(int par1) 
        => AxisMap.ContainsValue(par1) ? $"{AxisMap.Keys.ToList()[AxisMap.Values.IndexOf(par1)]}" : par1.ToString(CultureInfo.InvariantCulture);


    private int ParsePlaceholderIndex(string placeholderToken)
    {
        for (int i = 0; i < PlaceHolders.Length; i++)
        {
            if (placeholderToken.EndsWith(PlaceHolders[i], StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    private bool IsSystemPlaceholder(string candidate, out bool setParam2Used)
    {
        foreach (var sysPlaceHolder in ParseDefinitions.SysPHCharset)
        {
            if (candidate.StartsWith(sysPlaceHolder.Placeholder, StringComparison.OrdinalIgnoreCase))
            {
                setParam2Used = candidate.EndsWith(":PARAM2>", StringComparison.OrdinalIgnoreCase);
                return true;
            }
        }

        setParam2Used = false;
        return false;
    }
    public void BuildExpressionNormal(string line, Action<(int, bool, string)> actAdd, int subToken = 0, bool param2Used = false)
    {
        var startPos = line.GetNextPlaceHolder();
        string nonSysPlaceholder = string.Empty;
        while (startPos < line.Length && nonSysPlaceholder == string.Empty)
        {
            var endPos = line.IndexOf('>', startPos);
            string candidate = line.Substring(startPos, endPos - startPos + 1);
            if (IsSystemPlaceholder(candidate, out bool setParam2Used))
            {
                param2Used |= setParam2Used;
            }
            else
            {
                nonSysPlaceholder = candidate;
            }
            startPos = line.GetNextPlaceHolder(endPos + 1);
        }

        if (nonSysPlaceholder == string.Empty)
        {
            actAdd?.Invoke((subToken, param2Used, line));
            return;
        }

        foreach (var substitution in PlaceHolderSubst)
        {
            if (substitution.PlaceHolder == nonSysPlaceholder)
            {
                BuildExpressionNormal(line.Replace(nonSysPlaceholder, substitution.text), actAdd, subToken + substitution.Number, param2Used);
            }
        }
    }

    private static bool DefinitionUsesExpression(string? text)
        => !string.IsNullOrEmpty(text) && text.IndexOf(CExpression, StringComparison.OrdinalIgnoreCase) >= 0;

}
