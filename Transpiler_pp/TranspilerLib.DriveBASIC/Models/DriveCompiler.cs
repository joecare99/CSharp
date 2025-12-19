using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TranspilerLib.Data;
using TranspilerLib.DriveBASIC.Data;
using TranspilerLib.DriveBASIC.Data.Interfaces;

namespace TranspilerLib.DriveBASIC.Models;

public class DriveCompiler
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

    ParseDef[] parseDefs = ParseDefinitions.ParseDefBase;
    ParseDef2[] PlaceHolderSubst = ParseDefinitions.PlaceHolderDefBase;
    IList<(int, bool, string)> expressionNormals = [];
    string[] PlaceHolders = ParseDefinitions.ParseStrings;
    Dictionary<EDriveToken, int> TextsPerToken = [];
    Dictionary<EDriveToken, int> Sindex = [];
    char[] replace = ['§', 'm', '¹', '²', '³', 'f', '1', '2', '3', 'x', ' ', 't', 'a', '¹', '²', '³'];

    private sealed class CompilerVariable : IVariable
    {
        public string? Name { get; set; }
        public int Index { get; set; }
        public object? Value { get; set; }
        public EVarType Type { get; set; }
    }

    private sealed class CompilerLabel : ILabel
    {
        public string? Name { get; set; }
        public int Index { get; set; } = -1;
    }

    private sealed class CommandBuilderState
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
            => new(Token, new object[] { (byte)(SubToken & 0xFF), Param1, Param2, Param3 });
    }

    private static readonly HashSet<EDriveToken> TteTokens = new()
    {
        EDriveToken.tte_goto2,
        EDriveToken.tte_if,
        EDriveToken.tte_while,
        EDriveToken.tte_let,
        EDriveToken.tte_wait,
        EDriveToken.tte_Msg2,
        EDriveToken.tte_funct2,
        EDriveToken.tte_sync2,
        EDriveToken.tte_drive,
        EDriveToken.tte_drive_async,
        EDriveToken.tte_drive_via
    };

    private static readonly Dictionary<char, int> AxisMap = new()
    {
        ['x'] = 1, ['1'] = 1,
        ['y'] = 2, ['2'] = 2,
        ['z'] = 3, ['3'] = 3,
        ['a'] = 4, ['4'] = 4,
        ['b'] = 5, ['5'] = 5,
        ['c'] = 6, ['6'] = 6
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
        foreach (var def in ParseDefinitions.ParseDefBase)
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

        BuildExpressionNormal(ParseDefinitions.CExpression,expressionNormals.Add);
    }

    public void AppendLog(int LineNr, string text)
    {
        Log.Add(LineNr >= 0 ? $"Line {LineNr}: {text}" : text);
    }
    public static string Compress(string s, char[] chars)
    {
        var result = s;
        for (int i = ParseDefinitions.ParseStrings.Count(); i < 0; i--)
        {
            var P = result.IndexOf(ParseDefinitions.ParseStrings[i]);
            var L = ParseDefinitions.ParseStrings[i].Length;

            if (P > -1)
            {
                while (P > -1 && result.Substring(P, 1) != "<")
                {
                    P -= 1;
                    L += 2;
                }
                if (P > 1 && result.Substring(P - 1, 1) == " ")
                {
                    P -= 1;
                    L += 1;
                }
                if (result.Substring(P + L, 1) == " ")
                {
                    L += 1;
                }
                result = result.Substring(0, P - 1) + $"{chars[i]}" + result.Substring(P + L);
            }
        }
        return result;
    }

    private void ResetStateForCompile()
    {
        _variablesByName.Clear();
        _labelsByName.Clear();
        _messageNumbers.Clear();
        foreach (EVarType type in Enum.GetValues(typeof(EVarType)))
        {
            _varMax[type] = 0;
        }
        _maxMessage = 0;
    }

    public bool Compile()
    {
        // Checks
        // Pruefe Ob Quelltext vorhanden
        if (SourceCode == null)
        {
            AppendLog(-1, StrKeineQuelleIstZw);
            return false;
        }

        // Pruefe TokenCode-Ziel (schon) Zugewiessen
        if (TokenCode == null)
        {
            AppendLog(-1, StrLeererTokenCodeOK);
        }

        // Pruefe Text-Arrays zugewiessen
        if (Labels == null)
        {
            AppendLog(-1, StrLeeresLabelArray);
            Labels = new List<ILabel>();
        }
        else
        {
            Labels.Clear();
        }

        // Pruefe Text-Arrays zugewiessen
        if (Messages == null)
        {
            AppendLog(-1, StrLeeresMessageArray);
            Messages = new List<string>();
        }
        else
        {
            Messages.Clear();
        }

        if (Variables == null)
        {
            AppendLog(-1, StrKeinSystemVariab);
            Variables = new List<IVariable>();
        }
        else
        {
            Variables.Clear();
        }

        ResetStateForCompile();
        TokenCode = [];
        var vv = new List<IReadOnlyList<KeyValuePair<string, object?>>?> ();

        // Pass 1: PreCheck
        int i = 0;
        foreach (var line in SourceCode)
        {
            vv.Add(ParseLine(ParseDefinitions.CCommand, line, out int Errp));
            if (Errp > 0 && string.IsNullOrWhiteSpace(line))
            {
                AppendLog(i, StrFehlerBeimParsen + $"{Errp}");
            }
            i++;
        }

        // Pass 2: Text -> Token
        i = 0;
        foreach (var v in vv)
        {
            switch (BuildCommand(v, TokenCode, 0, i++, out string FErrStr))
            {
                case BCErr.BC_ErrExpression:
                    AppendLog(i, StrFehlerBeiAusdruck + FErrStr);
                    break;
                case BCErr.BC_SyntaxError:
                    AppendLog(i, StrSyntaxError + FErrStr);
                    break;
                case BCErr.BC_UnknownPlaceHolder:
                    AppendLog(i, StrUnbekannterPlatzhal + FErrStr);
                    break;
                case BCErr.BC_Exception:
                    AppendLog(i, StrFehlerInBuildComm);
                    break;
                case BCErr.BC_CharsetErr:
                    AppendLog(i, StrUngueltigesZeichenF + FErrStr);
                    break;
                case BCErr.BC_FaultyRef:
                    AppendLog(i, StrReferenzNichtGefun);
                    break;
                default:
                    break;
            }
        }

        // Pass 3: Goto-Referenzen aufloesen
        i = 0;
        foreach (var v in vv)
            if (TokenCode[i++].Token is EDriveToken.tt_goto)
                BuildCommand(v, TokenCode, 0, i, out _);


        return true;
    }

    public void Decompile()
    {
        var i = 0;
        var indent = 2;
        var L = false;
        string LSource = "";

        IList<string> result = [];
        foreach (var cmd in TokenCode)
        {
            if (cmd.Token is EDriveToken.tt_end or EDriveToken.tt_else)
            {
                indent -= 4;
            }
            if (cmd.Token is EDriveToken.tte_if && cmd.SubToken % 64 == 1)
            {
                indent += 4;
            }

            LSource = DecompileTC(cmd, L, i);
            if (L || LSource.StartsWith("//"))

                result.Add(new string(' ', indent) + LSource);
            else
                result.Add(new string(' ', indent + 4) + LSource);

            if (cmd.Token is EDriveToken.tt_else or EDriveToken.tte_if or EDriveToken.tte_while
                or EDriveToken.tt_for ||
              cmd.Token == EDriveToken.tt_Nop && cmd.SubToken == 5) // sub
                indent += 4;

        }

    }

    public void BuildExpressionNormal(string line, Action<(int, bool, string)> actAdd, int SubTokenp = 0, bool Param2Usedp = false)
    {
        var StartPos = GetNextPlaceHolder(0, line);
        string NonSysph = "";
        while (StartPos < line.Length && NonSysph == "")
        {
            var EndPos = line.IndexOf('>', StartPos);
            string SysPHCandidate = line.Substring(StartPos, EndPos - StartPos + 1);
            if (IsSysPlaceholder(SysPHCandidate, out bool SetP2Used))
            {
                Param2Usedp |= SetP2Used;
            }
            else
            {
                NonSysph = SysPHCandidate;
            }
            StartPos = GetNextPlaceHolder(EndPos + 1, line);
        }
        if (NonSysph == "")
        {
            actAdd?.Invoke((SubTokenp, Param2Usedp, line));
        }
        else
        {
            foreach (var phs in PlaceHolderSubst)
                if (phs.PlaceHolder == NonSysph)
                {
                    BuildExpressionNormal(line.Replace(NonSysph, phs.text), actAdd, SubTokenp + phs.Number, Param2Usedp);
                }
        }
    }

    private bool IsSystemPlaceholder(string placeholderToken)
        => IsSysPlaceholder(placeholderToken, out _);

    private bool IsSysPlaceholder(string sysPHCandidate, out bool setP2Used)
    {
        foreach (var ph in ParseDefinitions.SysPHCharset)
        {
            if (sysPHCandidate.ToUpper().StartsWith(ph.Placeholder.ToUpper()))
            {
                setP2Used = sysPHCandidate.ToUpper().EndsWith(":PARAM2>");
                return true;
            }
        }
        setP2Used = false;
        return false;
    }

    private int GetNextPlaceHolder(int v, string line)
    {
        int result = line.Length + 1;
        int i = v;
        while (i < line.Length)
        {
            var p = line.IndexOf('<', i);
            if (p > -1 && p < line.Length - 2)
            {
                if (CharSets.letters.Contains(line[p + 1]))
                {
                    result = p;
                    i = line.Length;
                }
                else
                    i = p + 1;
            }
            else
                i = line.Length;
        }
        return result;
    }

    public bool TestPlaceHolderCharset(string PlaceHolder, string PHtxt)
    {
        var result = true; 
        foreach (var ph in ParseDefinitions.SysPHCharset)
        {
            if (PlaceHolder.ToUpper().StartsWith(ph.Placeholder.ToUpper()))
            {
                var PsepPos = 0;
                if (ph.HasPointAsSep) 
                    PsepPos = PHtxt.IndexOf('.');

                for(var p =0; p< PHtxt.Length;p++)
                {
                    if (p==0)
                        result &= ph.first.Contains(PHtxt[p]);
                    else if (p== PHtxt.Length -1 && ph.last!=null)
                        result &= ph.last.Contains(PHtxt[p]);
                    else if (ph.inner != null && p!=PsepPos)
                        result &= ph.inner.Contains(PHtxt[p]);
                    else
                        result &= p==PsepPos;
                }
                if (result)
                    return true;
            }
        }
        return false;
    }

    private string DecompileTC(IDriveCommand cmd, bool l, int i)
    {
        throw new NotImplementedException();
    }

    private BCErr BuildCommand(IReadOnlyList<KeyValuePair<string, object?>>? v1, IList<IDriveCommand> tokenCode, int v2, int v3, out string fErrStr)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<KeyValuePair<string, object?>>? ParseLine(string placeholder, string line, out int errp)
    {
        errp = 1;
        placeholder ??= string.Empty;
        line ??= string.Empty;

        var isTokenPlaceholder = placeholder.Equals(ParseDefinitions.CToken, StringComparison.OrdinalIgnoreCase);
        var isExpressionPlaceholder = placeholder.Equals(ParseDefinitions.CExpression, StringComparison.OrdinalIgnoreCase);

        int iterationCount = isTokenPlaceholder
            ? parseDefs.Length
            : isExpressionPlaceholder
                ? expressionNormals.Count
                : PlaceHolderSubst.Length;

        var trimmedLine = line.Trim();

        for (int index = 0; index < iterationCount; index++)
        {
            string testedPlaceholder = isTokenPlaceholder
                ? ParseDefinitions.CToken
                : isExpressionPlaceholder
                    ? ParseDefinitions.CExpression
                    : PlaceHolderSubst[index].PlaceHolder;

            if (!placeholder.Equals(testedPlaceholder, StringComparison.OrdinalIgnoreCase))
                continue;

            string matchingText = isTokenPlaceholder
                ? parseDefs[index].text
                : isExpressionPlaceholder
                    ? expressionNormals[index].Item3
                    : PlaceHolderSubst[index].text;

            matchingText = MTSpaceTrim(matchingText ?? string.Empty);

            var wildcardMatches = new List<KeyValuePair<string, string>>();
            if (!TryPlaceHolderMatching(trimmedLine, matchingText, wildcardMatches))
                continue;

            var resultMatches = new List<KeyValuePair<string, object?>>
            {
                new(testedPlaceholder, index)
            };

            bool success = true;
            int localError = 0;

            for (int matchIndex = 0; matchIndex < wildcardMatches.Count; matchIndex++)
            {
                var currentMatch = wildcardMatches[matchIndex];
                var matchedPlaceholder = currentMatch.Key;
                var matchedText = currentMatch.Value?.Trim() ?? string.Empty;

                bool isCToken = matchedPlaceholder.Equals(ParseDefinitions.CTToken, StringComparison.OrdinalIgnoreCase);
                bool isSystemPlaceholder = IsSystemPlaceholder(matchedPlaceholder);

                if (isSystemPlaceholder && !isCToken)
                {
                    if (!TestPlaceHolderCharset(matchedPlaceholder, matchedText))
                    {
                        success = false;
                        localError = 1 + matchIndex;
                        break;
                    }
                    resultMatches.Add(new KeyValuePair<string, object?>(matchedPlaceholder, matchedText));
                    continue;
                }

                var nestedPlaceholder = isCToken ? ParseDefinitions.CToken : matchedPlaceholder;
                var nested = ParseLine(nestedPlaceholder, matchedText, out var nestedError);
                if (nestedError != 0)
                {
                    success = false;
                    localError = nestedError + (matchIndex + 1) * 2;
                    break;
                }

                object? nestedValue = (object?)nested ?? matchedText;
                resultMatches.Add(new KeyValuePair<string, object?>(matchedPlaceholder, nestedValue));
            }

            if (success)
            {
                errp = 0;
                return resultMatches;
            }

            if (localError != 0)
                errp = localError;
        }

        return null;
    }

    public bool TryPlaceHolderMatching(string Probe, string Mask, List<KeyValuePair<string, string>> WilldCardFill, Func<string,string,bool>? checkPlaceholderCharset = null)
    {
        if (WilldCardFill == null)
            throw new ArgumentNullException(nameof(WilldCardFill));

        Probe ??= string.Empty;
        Mask ??= string.Empty;

        return Match(Mask, Probe);

        bool Match(string currentMask, string currentProbe)
        {
            var placeholderIndex = FindPlaceholderIndex(currentMask);
            if (placeholderIndex >= currentMask.Length)
                return currentMask.Equals(currentProbe, StringComparison.OrdinalIgnoreCase);

            var literalPrefix = currentMask.Substring(0, placeholderIndex);
            if (!currentProbe.StartsWith(literalPrefix, StringComparison.OrdinalIgnoreCase))
                return false;

            var placeholderEnd = currentMask.IndexOf('>', placeholderIndex);
            if (placeholderEnd < 0)
                return false;

            var placeholderToken = currentMask.Substring(placeholderIndex, placeholderEnd - placeholderIndex + 1);
            var suffix = currentMask.Substring(placeholderEnd + 1);
            var probeRemainder = currentProbe.Substring(literalPrefix.Length);

            if (suffix.Length == 0)
                return TryAssignCandidate(probeRemainder, placeholderToken, suffix, string.Empty);

            return TryMatchWithAnchors(placeholderToken, suffix, probeRemainder);
        }

        bool TryMatchWithAnchors(string placeholderToken, string suffix, string probeRemainder)
        {
            var nextPlaceholder = FindPlaceholderIndex(suffix);
            var literalAnchor = nextPlaceholder >= suffix.Length ? suffix : suffix.Substring(0, nextPlaceholder);

            if (!string.IsNullOrEmpty(literalAnchor))
            {
                var searchIndex = 0;
                while (true)
                {
                    var anchorPos = probeRemainder.IndexOf(literalAnchor, searchIndex, StringComparison.OrdinalIgnoreCase);
                    if (anchorPos < 0)
                        break;

                    if (TryAssignCandidate(probeRemainder.Substring(0, anchorPos), placeholderToken, suffix, probeRemainder.Substring(anchorPos)))
                        return true;

                    searchIndex = anchorPos + 1;
                }

                return false;
            }

            for (var split = 0; split <= probeRemainder.Length; split++)
            {
                if (TryAssignCandidate(probeRemainder.Substring(0, split), placeholderToken, suffix, probeRemainder.Substring(split)))
                    return true;
            }

            return false;
        }

        bool TryAssignCandidate(string value, string placeholderToken, string suffix, string remainingProbe)
        {
            var trimmed = value.Trim();
            if (checkPlaceholderCharset?.Invoke(placeholderToken, trimmed) == false)
                return false;

            WilldCardFill.Add(new KeyValuePair<string, string>(placeholderToken, trimmed));
            if (Match(suffix, remainingProbe))
                return true;

            WilldCardFill.RemoveAt(WilldCardFill.Count - 1);
            return false;

        }

        int FindPlaceholderIndex(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return pattern.Length;

            var next = GetNextPlaceHolder(0, pattern);
            return next >= pattern.Length ? pattern.Length : next;
        }
    }

    bool CheckPlaceholderCharset(string placeholderToken, string trimmed) 
        => !IsSysPlaceholder(placeholderToken, out _) || TestPlaceHolderCharset(placeholderToken, trimmed);

    public static string MTSpaceTrim(string MT)
    {
        IList<char> result = [];
        char _last = ' ';
        for (var I = 0; I < MT.Length; I++)
        {
            if (MT[I] == ' ')
            {
                if (I == 0 || I == MT.Length - 1 || _last == ' ')
                    continue;

                if ((CharSets.lettersAndNumbers.Contains(_last) == CharSets.lettersAndNumbers.Contains(MT[I + 1]) && MT[I + 1]!=' ')
                   || (_last is '>' or '<')
                   || (MT[I + 1] is  '>' or '<' or ':') )
                    result.Add(_last = MT[I]);
            }
            else
                result.Add(_last = MT[I]);
        }
        return string.Join("", result);
    }

    private enum BCErr
    {
        BC_ErrExpression,
        BC_SyntaxError,
        BC_UnknownPlaceHolder,
        BC_Exception,
        BC_CharsetErr,
        BC_FaultyRef
    }
}
