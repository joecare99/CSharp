using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading.Tasks;
using TranspilerLib.Data;
using TranspilerLib.DriveBASIC.Data;
using TranspilerLib.DriveBASIC.Data.Interfaces;
using TranspilerLib.Models;
using static System.Net.Mime.MediaTypeNames;
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
        foreach (var def in ParseDefBase)
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

        BuildExpressionNormal(CExpression, expressionNormals.Add);
    }

    public void AppendLog(int LineNr, string text)
    {
        Log.Add(LineNr >= 0 ? $"Line {LineNr}: {text}" : text);
    }
    public static string Compress(string s, char[] chars)
    {
        var result = s;
        for (int i = ParseStrings.Count(); i < 0; i--)
        {
            var P = result.IndexOf(ParseStrings[i]);
            var L = ParseStrings[i].Length;

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
        var vv = new List<IReadOnlyList<KeyValuePair<string, object?>>?>();

        // Pass 1: PreCheck
        int i = 0;
        foreach (var line in SourceCode)
        {
            vv.Add(ParseLine(CCommand, line, out int Errp));
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
                BuildCommand(v, TokenCode, 0, i-1, out _);


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
        var StartPos = StringUtils.GetNextPlaceHolder(0, line);
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
            StartPos = StringUtils.GetNextPlaceHolder(EndPos + 1, line);
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
        foreach (var ph in SysPHCharset)
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

    public bool TestPlaceHolderCharset(string PlaceHolder, string PHtxt)
    {
        foreach (var ph in SysPHCharset)
        {
            var result = true;
            if (PlaceHolder.ToUpper().StartsWith(ph.Placeholder.ToUpper()))
            {
                var PsepPos = 0;
                if (ph.HasPointAsSep)
                    PsepPos = PHtxt.IndexOf('.');

                for (var p = 0; p < PHtxt.Length; p++)
                {
                    if (p == 0)
                        result &= ph.first.Contains(PHtxt[p]);
                    else if (p == PHtxt.Length - 1 && ph.last != null)
                        result &= ph.last.Contains(PHtxt[p]);
                    else if (ph.inner != null && p != PsepPos)
                        result &= ph.inner.Contains(PHtxt[p]);
                    else
                        result &= p == PsepPos;
                }
                if (result)
                    return true;
            }
        }
        return false;
    }

    private string DecompileTC(IDriveCommand cmd, bool l, int i)
    {
        ParseDef? found = null; 
        foreach (var pdCand in parseDefs)
        {
            if (cmd.Token == pdCand.Token)
            {
                if (TteTokens.Contains(cmd.Token))
                {
                    if (cmd.SubToken / 64 == pdCand.SubToken)
                    {
                        found = pdCand;
                        break;
                    }
                }
                else
                {
                    if (cmd.SubToken == pdCand.SubToken)
                    {
                        found = pdCand;
                        break;
                    }
                }
            }
        }
        string result;
        if (found != null && found.text !=null)
        {
            // SUB
            if (cmd.Token == EDriveToken.tt_Nop && cmd.SubToken == 5)
            {
                cmd.Par1 = i; // Sich selbst als Sprungmarke setzen (nur lokal)
            }
            result = found.text!;
        }
        else
        {
            result = string.Empty;
        }
        int nextPh = StringUtils.GetNextPlaceHolder(0, result);
        while (nextPh < result.Length)
        {
            result = SubstitutePlaceHolder(result, cmd);
            nextPh = StringUtils.GetNextPlaceHolder(0, result);
        }
        bool hasLabel = false;
        if (cmd.Token != EDriveToken.tt_Nop || cmd.SubToken != 5)
        {
            for (int k = 0; k < Labels.Count; k++)
            {
                if (Labels[k].Index == i)
                {
                    result = $"{Labels[k].Name}: {result}";
                    hasLabel = true;
                }
            }
        }
        return result;
    }

    private string SubstitutePlaceHolder(string line, IDriveCommand cmd)
    {
        int phPos = StringUtils.GetNextPlaceHolder(0, line);
        int phEnd = line.IndexOf('>', phPos);
        if (phPos >= 0 && phEnd > phPos)
        {
            var phst = line.Substring(phPos, phEnd - phPos + 1);
            if (IsSystemPlaceholder(phst))
            {
                switch (ParsePlaceholderIndex(phst))
                {
                    case 0:
                        return line.Replace(phst, GetLabelText(cmd.Par1));
                    case 1:
                        return line.Replace(phst, GetMessageText(cmd.Par1));
                    case 2:
                        return line.Replace(phst, GetVariableText(cmd.Par1));
                    case 3:
                        return line.Replace(phst, GetVariableText(cmd.Par2));
                    case 4:
                        return line.Replace(phst, GetVariableText((int)Math.Round(cmd.Par3)));
                    case 5:
                        return line.Replace(phst, cmd.Par3.ToString(CultureInfo.InvariantCulture));
                    case 6:
                        return line.Replace(phst, cmd.Par1.ToString(CultureInfo.InvariantCulture));
                    case 7:
                        return line.Replace(phst, cmd.Par2.ToString(CultureInfo.InvariantCulture));
                    case 8:
                        return line.Replace(phst, ((int)Math.Round(cmd.Par3)).ToString(CultureInfo.InvariantCulture));
                    case 9:
                        return line.Replace(phst, GetAxisName(cmd.Par1));
                    case 10:
                        return line.Replace(phst, " ");
                    default:
                        return line;
                }
            }
            else if (phst.Equals(CExpression, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var en in expressionNormals)
                {
                    if ((cmd.SubToken & 63) == en.Item1 && en.Item2 == (cmd.Par2 != 0))
                    {
                        return line.Replace(phst, en.Item3);
                    }
                }
            }
            else
            {
                for (int i = 0; i < PlaceHolderSubst.Length; i++)
                {
                    if (PlaceHolderSubst[i].PlaceHolder.Equals(phst, StringComparison.OrdinalIgnoreCase))
                    {
                        return line.Replace(phst, PlaceHolderSubst[i].text);
                    }
                }
            }
        }
        return line;
    }

    private string GetLabelText(int par1)
    {
        var label = Labels.FirstOrDefault(l => l.Index == par1);
        return label != null ? label.Name ?? par1.ToString() : par1.ToString();
    }

    private string GetMessageText(int par1)
    {
        var message = par1 >= 0 && par1 < Messages.Count ? Messages[par1] : null;
        return message ?? $"#{par1}";
    }

    private string GetVariableText(int v)
    {
        int lid;
        int lax;
        if (v >= DimensionBase)
        {
            lid = ((v - DimensionBase) / 6) + PointBase;
            lax = ((v - DimensionBase) % 6) + 1;
        }
        else
        {
            lid = v;
            lax = 0;
        }
        var variable = Variables.FirstOrDefault(var => var.Index == lid);
        var varName = variable != null ? variable.Name ?? $"#{lid}" : $"#{lid}";
        if (lax > 0)
        {
            varName += GetAxisName(lax);
        }
        return varName;
    }

    private string GetAxisName(int par1) 
        => AxisMap.Values.Contains(par1) ? $"{AxisMap.Keys.ToList()[AxisMap.Values.IndexOf(par1)]}":par1.ToString();

    private BCErr BuildCommand(IReadOnlyList<KeyValuePair<string, object?>>? parseTree, IList<IDriveCommand> tokenBuffer, int level, int pc, out string fErrStr)
    {
        fErrStr = string.Empty;
        if (parseTree == null)
        {
            fErrStr = StrSyntaxError;
            return BCErr.BC_SyntaxError;
        }

        var builder = new CommandBuilderState();

        try
        {
            var result = ProcessNode(parseTree, builder, tokenBuffer, level, pc, out fErrStr);
            if (result != BCErr.BC_OK)
                return result;

            if (!builder.HasToken)
            {
                fErrStr = StrSyntaxError;
                return BCErr.BC_SyntaxError;
            }

            EnsureTokenSlot(tokenBuffer, pc);
            tokenBuffer[pc] = builder.ToDriveCommand();
            return BCErr.BC_OK;
        }
        catch (Exception ex)
        {
            fErrStr = ex.Message;
            return BCErr.BC_Exception;
        }
    }

    private BCErr ProcessNode(IReadOnlyList<KeyValuePair<string, object?>> nodes, CommandBuilderState builder, IList<IDriveCommand> tokenBuffer, int level, int pc, out string error)
    {
        error = string.Empty;
        if (nodes.Count == 0)
        {
            error = StrSyntaxError;
            return BCErr.BC_SyntaxError;
        }

        if (level >= MaxRecursionDepth)
        {
            error = StrFehlerInBuildComm;
            return BCErr.BC_SyntaxError;
        }

        if (nodes[0].Value == null)
        {
            error = StrSyntaxError;
            return BCErr.BC_SyntaxError;
        }

        int ruleIndex;
        try
        {
            ruleIndex = Convert.ToInt32(nodes[0].Value, CultureInfo.InvariantCulture);
        }
        catch
        {
            error = StrSyntaxError;
            return BCErr.BC_SyntaxError;
        }

        var placeholder = nodes[0].Key ?? string.Empty;
        var isTokenNode = placeholder.Equals(CToken, StringComparison.OrdinalIgnoreCase);
        var isExpressionNode = placeholder.Equals(CExpression, StringComparison.OrdinalIgnoreCase);
        var isCommandNode = placeholder.Equals(CCommand, StringComparison.OrdinalIgnoreCase);

        if (isTokenNode)
        {
            if (ruleIndex < 0 || ruleIndex >= parseDefs.Length)
            {
                error = StrSyntaxError;
                return BCErr.BC_SyntaxError;
            }
            var def = parseDefs[ruleIndex];
            builder.Initialize(def.Token, def.SubToken);
        }
        else if (isExpressionNode)
        {
            if (ruleIndex < 0 || ruleIndex >= expressionNormals.Count)
            {
                error = StrSyntaxError;
                return BCErr.BC_SyntaxError;
            }
            builder.AddSubToken(expressionNormals[ruleIndex].Item1);
        }
        else if (isCommandNode)
        {
            RegisterInlineLabels(nodes, pc);
        }
        else
        {
            if (ruleIndex < 0 || ruleIndex >= PlaceHolderSubst.Length)
            {
                error = StrSyntaxError;
                return BCErr.BC_SyntaxError;
            }
            builder.AddSubToken(PlaceHolderSubst[ruleIndex].Number);
        }

        foreach(var node2 in nodes)
        {

            if (IsSystemPlaceholder(node2.Key)
                && !node2.Key.ToUpper().Equals(CToken)
                && !node2.Key.ToUpper().Equals(CTToken))
            {
                var placeholderIndex = ParsePlaceholderIndex(node2.Key);
                if (placeholderIndex < 0)
                {
                    error = node2.Key;
                    return BCErr.BC_UnknownPlaceHolder;
                }

                var sysResult = HandleSystemPlaceholder(placeholderIndex, node2.Value, builder, isCommandNode, level, pc, out error);
                if (sysResult != BCErr.BC_OK)
                    return sysResult;
                continue;
            }

            if (node2.Value is IReadOnlyList<KeyValuePair<string, object?>> nested)
            {
                var nestedResult = ProcessNode(nested, builder, tokenBuffer, level + 1, pc, out error);
                if (nestedResult != BCErr.BC_OK)
                    return nestedResult;
            }
            else
            {
                var literal = (node2.Value as string)?.Trim() ?? string.Empty;
                if (!string.IsNullOrEmpty(literal))
                {
                    error = literal;
                    return BCErr.BC_UnknownPlaceHolder;
                }
            }
        }

        if (isTokenNode)
        {
            var refResult = ResolveReferences(builder, tokenBuffer, pc, out error);
            if (refResult != BCErr.BC_OK)
                return refResult;
        }

        return BCErr.BC_OK;
    }

    private void RegisterInlineLabels(IReadOnlyList<KeyValuePair<string, object?>> node, int pc)
    {
        for (int i = 1; i < node.Count; i++)
        {
            if (IsLabelPlaceholder(node[i].Key) && node[i].Value is string labelText && !string.IsNullOrWhiteSpace(labelText))
            {
                SetLabel(labelText, pc);
            }
        }
    }

    private bool IsLabelPlaceholder(string? placeholder)
        => !string.IsNullOrEmpty(placeholder)
           && IsSystemPlaceholder(placeholder)
           && placeholder.EndsWith("Label>", StringComparison.OrdinalIgnoreCase);

    private BCErr HandleSystemPlaceholder(int placeholderIndex, object? rawValue, CommandBuilderState builder, bool isCommandNode, int level, int pc, out string error)
    {
        error = string.Empty;
        var text = (rawValue as string)?.Trim() ?? rawValue?.ToString()?.Trim() ?? string.Empty;

        switch (placeholderIndex)
        {
            case 0:
                if (isCommandNode)
                    return BCErr.BC_OK;
                var definitionTarget = level < 1 || (builder.Token == EDriveToken.tt_Nop && builder.SubToken == 5) ? pc : -1;
                var labelIndex = GetLabelIndex(text, definitionTarget);
                builder.Param1 = labelIndex >= 0 ? labelIndex : ushort.MaxValue;
                return BCErr.BC_OK;
            case 1:
                builder.Param1 = GetMessageNumber(text);
                return BCErr.BC_OK;
            case 2:
                builder.Param1 = GetVariableNumber(text);
                return BCErr.BC_OK;
            case 3:
                builder.Param2 = GetVariableNumber(text);
                return BCErr.BC_OK;
            case 4:
                builder.Param3 = GetVariableNumber(text);
                return BCErr.BC_OK;
            case 5:
            case 8:
                if (!TryParseFloatValue(text, out var floatValue))
                {
                    error = text;
                    return BCErr.BC_SyntaxError;
                }
                builder.Param3 = floatValue;
                return BCErr.BC_OK;
            case 6:
                if (!TryParseIntValue(text, out var intValue1))
                {
                    error = text;
                    return BCErr.BC_SyntaxError;
                }
                builder.Param1 = intValue1;
                return BCErr.BC_OK;
            case 7:
                if (!TryParseIntValue(text, out var intValue2))
                {
                    error = text;
                    return BCErr.BC_SyntaxError;
                }
                builder.Param2 = intValue2;
                return BCErr.BC_OK;
            case 9:
                var axis = GetAxisNumber(text);
                if (axis <= 0)
                {
                    error = text;
                    return BCErr.BC_SyntaxError;
                }
                builder.Param1 = axis;
                return BCErr.BC_OK;
            default:
                if (!string.IsNullOrEmpty(text))
                {
                    error = text;
                    return BCErr.BC_UnknownPlaceHolder;
                }
                return BCErr.BC_OK;
        }
    }

    private int ParsePlaceholderIndex(string placeholderToken)
    {
        for (int i = 0; i < PlaceHolders.Length; i++)
        {
            if (placeholderToken.EndsWith(PlaceHolders[i], StringComparison.OrdinalIgnoreCase))
                return i;
        }
        return -1;
    }

    private CompilerLabel EnsureLabel(string labelName)
    {
        var normalized = NormalizeIdentifier(labelName);
        if (!_labelsByName.TryGetValue(normalized, out var label))
        {
            label = new CompilerLabel { Name = normalized };
            _labelsByName[normalized] = label;
            Labels.Add(label);
        }
        return label;
    }

    private void SetLabel(string labelName, int destinationPc)
    {
        if (destinationPc < 0 || string.IsNullOrWhiteSpace(labelName))
            return;
        var label = EnsureLabel(labelName);
        label.Index = destinationPc;
    }

    private int GetLabelIndex(string labelName, int destinationPc)
    {
        if (string.IsNullOrWhiteSpace(labelName))
            return -1;
        var label = EnsureLabel(labelName);
        if (destinationPc >= 0)
            label.Index = destinationPc;
        return label.Index;
    }

    private int GetMessageNumber(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;
        var normalized = NormalizeIdentifier(text);
        if (!_messageNumbers.TryGetValue(normalized, out var number))
        {
            number = ++_maxMessage;
            _messageNumbers[normalized] = number;
            Messages.Add(text.Trim());
        }
        return number;
    }

    private int GetVariableNumber(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return 0;

        var trimmedName = name.Trim();
        var normalized = NormalizeIdentifier(trimmedName);
        var type = DetectVariableType(normalized);

        if (type == EVarType.vt_dimension && normalized.Length >= 2)
        {
            var axisChar = trimmedName[trimmedName.Length - 1];
            var baseDisplayName = trimmedName.Substring(0, trimmedName.Length - 1);
            var baseVariable = EnsureVariableEntry(baseDisplayName, EVarType.vt_point);
            var axis = GetAxisNumber(axisChar.ToString());
            if (axis <= 0)
                return 0;
            return DimensionBase + (baseVariable.Index - PointBase) * 6 + axis - 1;
        }

        var resolvedType = type == EVarType.vt_dimension ? EVarType.vt_point : type;
        var variable = EnsureVariableEntry(trimmedName, resolvedType);
        return variable.Index;
    }

    private CompilerVariable EnsureVariableEntry(string displayName, EVarType type)
    {
        var normalized = NormalizeIdentifier(displayName);
        if (!_variablesByName.TryGetValue(normalized, out var variable))
        {
            variable = new CompilerVariable
            {
                Name = normalized,
                Type = type,
                Index = AllocateVarNo(type)
            };
            _variablesByName[normalized] = variable;
            InsertVariable(variable);
        }
        return variable;
    }

    private void InsertVariable(CompilerVariable variable)
    {
        var list = Variables ??= new List<IVariable>();
        var insertIndex = 0;
        while (insertIndex < list.Count && list[insertIndex].Index <= variable.Index)
        {
            insertIndex++;
        }
        list.Insert(insertIndex, variable);
    }

    private int AllocateVarNo(EVarType type)
    {
        var key = type switch
        {
            EVarType.vt_universal => EVarType.vt_real,
            EVarType.vt_dimension => EVarType.vt_point,
            _ => type
        };

        var current = _varMax.TryGetValue(key, out var max) ? max : 0;
        current++;
        _varMax[key] = current;
        return current + (int)key * VarTypeStride;
    }

    private static EVarType DetectVariableType(string name)
    {
        if (string.IsNullOrEmpty(name))
            return EVarType.vt_real;
        if (name.EndsWith("?", StringComparison.Ordinal))
            return EVarType.vt_universal;
        if (name.EndsWith("&", StringComparison.Ordinal))
            return EVarType.vt_bool;
        if (name.EndsWith("%", StringComparison.Ordinal))
            return EVarType.vt_real;
        if (name.EndsWith(".", StringComparison.Ordinal))
            return EVarType.vt_point;
        if (name.Length >= 2 && name[name.Length - 2] == '.')
            return EVarType.vt_dimension;
        return EVarType.vt_real;
    }

    private static string NormalizeIdentifier(string? text)
        => text?.Trim().ToUpperInvariant() ?? string.Empty;

    private static bool TryParseFloatValue(string text, out double value)
    {
        if (double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value))
            return true;
        var normalized = text.Replace(',', '.');
        if (double.TryParse(normalized, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value))
            return true;
        return double.TryParse(text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out value);
    }

    private static bool TryParseIntValue(string text, out int value)
        => int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

    private static int GetAxisNumber(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;
        var key = char.ToLowerInvariant(text.Trim()[0]);
        return AxisMap.TryGetValue(key, out var axis) ? axis : 0;
    }

    private static void EnsureTokenSlot(IList<IDriveCommand> tokenBuffer, int pc)
    {
        while (tokenBuffer.Count <= pc)
        {
            tokenBuffer.Add(new DriveCommand(EDriveToken.tt_Nop));
        }
    }

    private BCErr ResolveReferences(CommandBuilderState builder, IList<IDriveCommand> tokenBuffer, int pc, out string error)
    {
        error = string.Empty;
        bool backwardRuleHit = false;
        bool backwardResolved = false;
        bool forwardRuleHit = false;
        bool forwardResolved = false;
        int forwardTarget = -1;

        foreach (var rule in ReferencingToken)
        {
            bool matchesBackward = rule.Backward
                && builder.Token == rule.Token
                && (rule.SubToken == -1 || GetComparableSubToken(builder.Token, builder.SubToken) == rule.SubToken);

            bool matchesForward = !rule.Backward
                && builder.Token == rule.ReferencingToken
                && (rule.ReferencingSubtoken == -1 || GetComparableSubToken(builder.Token, builder.SubToken) == rule.ReferencingSubtoken);

            if (!matchesBackward && !matchesForward)
                continue;

            var searchToken = matchesBackward ? rule.ReferencingToken : rule.Token;
            var searchSubToken = matchesBackward ? rule.ReferencingSubtoken : rule.SubToken;
            var found = FindReferenceTarget(tokenBuffer, pc - 1, searchToken, searchSubToken);

            if (matchesBackward)
            {
                backwardRuleHit = true;
                if (found >= 0)
                {
                    builder.Param1 = found;
                    backwardResolved = true;
                }
            }
            else
            {
                forwardRuleHit = true;
                if (found > forwardTarget)
                {
                    forwardTarget = found;
                    forwardResolved = found >= 0;
                }
            }
        }

        if (forwardRuleHit)
        {
            if (forwardResolved && forwardTarget >= 0)
            {
                UpdateCommandParam1(tokenBuffer, forwardTarget, pc);
            }
            else
            {
                return BCErr.BC_FaultyRef;
            }
        }

        if (backwardRuleHit && !backwardResolved)
            return BCErr.BC_FaultyRef;

        return BCErr.BC_OK;
    }

    private static int GetComparableSubToken(EDriveToken token, int subToken)
        => TteTokens.Contains(token) ? subToken / 64 : subToken;

    private int FindReferenceTarget(IList<IDriveCommand> tokenBuffer, int startIndex, EDriveToken searchToken, int searchSubToken)
    {
        var index = startIndex;
        while (index >= 0)
        {
            var jumped = false;
            for (int k = 0; k < ReferencingToken.Length; k++)
            {
                var rule = ReferencingToken[k];
                if (!rule.Backward || index >= tokenBuffer.Count)
                    continue;

                var candidate = tokenBuffer[index];
                if (candidate.Token == rule.Token
                    && (rule.SubToken == -1 || GetComparableSubToken(candidate.Token, candidate.SubToken) == rule.SubToken)
                    && candidate.Par1 <= index)
                {
                    index = candidate.Par1 - 1;
                    jumped = true;
                    break;
                }
            }

            if (!jumped)
            {
                if (index < tokenBuffer.Count)
                {
                    var candidate = tokenBuffer[index];
                    if (candidate.Token == searchToken
                        && (searchSubToken == -1 || GetComparableSubToken(candidate.Token, candidate.SubToken) == searchSubToken))
                    {
                        return index;
                    }
                }
                index--;
            }
        }

        return -1;
    }

    private void UpdateCommandParam1(IList<IDriveCommand> tokenBuffer, int index, int newValue)
    {
        if (index < 0)
            return;
        EnsureTokenSlot(tokenBuffer, index);
        var existing = tokenBuffer[index];
         tokenBuffer[index].Par1 =  newValue ;
    }

    public IReadOnlyList<KeyValuePair<string, object?>>? ParseLine(string placeholder, string line, out int errp)
    {
        errp = 1;
        placeholder ??= string.Empty;
        line ??= string.Empty;

        var isTokenPlaceholder = placeholder.Equals(CToken, StringComparison.OrdinalIgnoreCase);
        var isExpressionPlaceholder = placeholder.Equals(CExpression, StringComparison.OrdinalIgnoreCase);

        int iterationCount = isTokenPlaceholder
            ? parseDefs.Length
            : isExpressionPlaceholder
                ? expressionNormals.Count
                : PlaceHolderSubst.Length;

        var trimmedLine = line.Trim();

        for (int index = 0; index < iterationCount; index++)
        {
            string testedPlaceholder = isTokenPlaceholder
                ? CToken
                : isExpressionPlaceholder
                    ? CExpression
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
            if (!StringUtils.TryPlaceHolderMatching(trimmedLine, matchingText, wildcardMatches))
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

                bool isCToken = matchedPlaceholder.Equals(CTToken, StringComparison.OrdinalIgnoreCase);
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

                var nestedPlaceholder = isCToken ? CToken : matchedPlaceholder;
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

                if ((CharSets.lettersAndNumbers.Contains(_last) == CharSets.lettersAndNumbers.Contains(MT[I + 1]) && MT[I + 1] != ' ')
                   || (_last is '>' or '<')
                   || (MT[I + 1] is '>' or '<' or ':'))
                    result.Add(_last = MT[I]);
            }
            else
                result.Add(_last = MT[I]);
        }
        return string.Join("", result);
    }

    private enum BCErr
    {
        BC_OK,
        BC_ErrExpression,
        BC_SyntaxError,
        BC_UnknownPlaceHolder,
        BC_Exception,
        BC_CharsetErr,
        BC_FaultyRef
    }
}
