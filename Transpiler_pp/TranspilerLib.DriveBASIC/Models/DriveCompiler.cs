using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Labels = [];
        }

        // Pruefe Text-Arrays zugewiessen
        if (Messages == null)
        {
            AppendLog(-1, StrLeeresMessageArray);
            Messages = [];
        }

        if (Variables == null)
        {
            AppendLog(-1, StrKeinSystemVariab);
            Variables = [];
        }

        TokenCode = [];
        var vv = new List<object>();

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

            LSource = DeompileTC(cmd, L, i);
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

    private string DeompileTC(IDriveCommand cmd, bool l, int i)
    {
        throw new NotImplementedException();
    }

    private BCErr BuildCommand(object v1, IList<IDriveCommand> tokenCode, int v2, int v3, out string fErrStr)
    {
        throw new NotImplementedException();
    }

    private object ParseLine(string cCommand, string line, out int errp)
    {
        void AddFoundFktn(List<object> WilldCardFill_1, string PlaceHolder, string txt) {
            WilldCardFill_1.Add((PlaceHolder, txt.Trim()));
        }

        bool TryPlaceHolderMatching(string Probe, string Mask, List<object> WilldCardFill)
        {
            int IncGetProfileValue(List<object> WilldCardFill, string Fktnname, int Incr = 1)
            {
                int found = -1;
                int result = -1;
                for (int i = 0; i < WilldCardFill.Count/2; i++)
                {
                    if (WilldCardFill[i*2] == Fktnname)
                    {
                        found = i * 2 + 1;
                        break;
                    }
                    if (found >= 0)
                    {
                        result = ((int)WilldCardFill[found]) + Incr;
                        WilldCardFill[found] = result;
                        
                    }
                    else
                    {
                        AddFoundFktn(WilldCardFill,Fktnname,Incr.ToString());
                    }
                }
                return result;
            }

            var result = false;
            var start = 2;
            if (Mask != "")
             if (Mask.Length >= 2 && Mask[0] == '<' && CharSets.letters.Contains(Mask[2]))
                {
                    var apos1 = GetNextPlaceHolder(start, Mask);
                    start = Mask.Substring(0, apos1).IndexOf('>', start)+1;
                    if (Mask.Length > apos1)
                        if (start >0)
                    {
                        var submask = Mask.Substring(start);
                        var endpos = Probe.IndexOf(submask);
                        if (endpos > -1)
                        {
                            var phtext = Probe.Substring(0, endpos);
                            if (TestPlaceHolderCharset(Mask.Substring(0, apos1 + 1), phtext))
                            {
                                AddFoundFktn(WilldCardFill, Mask.Substring(0, apos1 + 1), phtext);
                                result = true;
                            }
                        }
                    }
                }
        }
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
