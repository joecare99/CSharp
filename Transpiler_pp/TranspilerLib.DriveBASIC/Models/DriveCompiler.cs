using System;
using System.Collections.Generic;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.DriveBASIC.Data;
using TranspilerLib.DriveBASIC.Data.Interfaces;
using static TranspilerLib.DriveBASIC.Data.ParseDefinitions;

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

    ParseDef[] parseDefs = ParseDefBase;
    ParseDef2[] PlaceHolderSubst = PlaceHolderDefBase;
    IList<(int, bool, string)> expressionNormals = [];
    public IReadOnlyList<(int, bool, string)> ExpressionNormals => expressionNormals.ToList();
    string[] PlaceHolders = ParseStrings;
    Dictionary<EDriveToken, int> TextsPerToken = [];
    Dictionary<EDriveToken, int> Sindex = [];
    char[] replace = ['§', 'm', '¹', '²', '³', 'f', '1', '2', '3', 'x', ' ', 't', 'a', '¹', '²', '³'];

    private readonly HashSet<EDriveToken> _expressionTokens = new();

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

    private readonly Dictionary<string, CompilerVariable> _variablesByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, CompilerLabel> _labelsByName = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _messageNumbers = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<EVarType, int> _varMax = new();
    private int _maxMessage;

    private const int VarTypeStride = 0x4000;
    private const int PointBase = 0x8000;
    private const int DimensionBase = 0xC000;
    private const int MaxRecursionDepth = 15;

    public DriveCompiler()
    {
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

        foreach (EVarType type in Enum.GetValues(typeof(EVarType)))
        {
            _varMax[type] = 0;
        }

        InitializeExpressionTokens();
        BuildExpressionNormal(CExpression, expressionNormals.Add);
    }

    private void InitializeExpressionTokens()
    {
        _expressionTokens.Clear();
        foreach (var def in parseDefs)
        {
            if (DefinitionUsesExpression(def.text))
            {
                _expressionTokens.Add(def.Token);
            }
        }
    }

    private static bool DefinitionUsesExpression(string? text)
        => !string.IsNullOrEmpty(text) && text.IndexOf(CExpression, StringComparison.OrdinalIgnoreCase) >= 0;

    private bool IsExpressionToken(EDriveToken token) => _expressionTokens.Contains(token);
}
