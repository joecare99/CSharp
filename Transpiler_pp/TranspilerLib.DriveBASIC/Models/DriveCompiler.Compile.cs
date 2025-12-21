using System;
using System.Collections.Generic;
using TranspilerLib.DriveBASIC.Data;
using TranspilerLib.DriveBASIC.Data.Interfaces;
using static TranspilerLib.DriveBASIC.Data.ParseDefinitions;

namespace TranspilerLib.DriveBASIC.Models;

public partial class DriveCompiler
{
    public void AppendLog(int lineNr, string text)
    {
        Log.Add(lineNr >= 0 ? $"Line {lineNr}: {text}" : text);
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
        if (SourceCode == null)
        {
            AppendLog(-1, StrKeineQuelleIstZw);
            return false;
        }

        if (TokenCode == null)
        {
            AppendLog(-1, StrLeererTokenCodeOK);
        }

        if (Labels == null)
        {
            AppendLog(-1, StrLeeresLabelArray);
            Labels = new List<ILabel>();
        }
        else
        {
            Labels.Clear();
        }

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
        var parseResults = new List<IReadOnlyList<KeyValuePair<string, object?>>?>();

        int lineIndex = 0;
        foreach (var line in SourceCode)
        {
            parseResults.Add(ParseLine(CCommand, line, out int parseError));
            if (parseError > 0 && string.IsNullOrWhiteSpace(line))
            {
                AppendLog(lineIndex, StrFehlerBeimParsen + $"{parseError}");
            }
            lineIndex++;
        }

        lineIndex = 0;
        foreach (var parseTree in parseResults)
        {
            switch (BuildCommand(parseTree, TokenCode, 0, lineIndex++, out string errorText))
            {
                case BCErr.BC_ErrExpression:
                    AppendLog(lineIndex, StrFehlerBeiAusdruck + errorText);
                    break;
                case BCErr.BC_SyntaxError:
                    AppendLog(lineIndex, StrSyntaxError + errorText);
                    break;
                case BCErr.BC_UnknownPlaceHolder:
                    AppendLog(lineIndex, StrUnbekannterPlatzhal + errorText);
                    break;
                case BCErr.BC_Exception:
                    AppendLog(lineIndex, StrFehlerInBuildComm);
                    break;
                case BCErr.BC_CharsetErr:
                    AppendLog(lineIndex, StrUngueltigesZeichenF + errorText);
                    break;
                case BCErr.BC_FaultyRef:
                    AppendLog(lineIndex, StrReferenzNichtGefun);
                    break;
            }
        }

        lineIndex = 0;
        foreach (var parseTree in parseResults)
        {
            if (TokenCode[lineIndex++].Token is EDriveToken.tt_goto)
            {
                BuildCommand(parseTree, TokenCode, 0, lineIndex - 1, out _);
            }
        }

        return true;
    }
}
