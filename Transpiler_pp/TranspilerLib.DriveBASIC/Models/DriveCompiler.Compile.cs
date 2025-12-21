using System;
using System.Collections.Generic;
using System.Configuration;
using TranspilerLib.DriveBASIC.Data;
using TranspilerLib.DriveBASIC.Data.Interfaces;

namespace TranspilerLib.DriveBASIC.Models;

public partial class DriveCompiler
{

    public bool Compile()=>new Compiler(this).Compile();

    public partial class Compiler
    {
        private readonly Dictionary<string, CompilerVariable> _variablesByName = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, CompilerLabel> _labelsByName = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, int> _messageNumbers = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<EVarType, int> _varMax = new();
        private int _maxMessage;

        private DriveCompiler _parent;

        public Compiler(DriveCompiler parent)
        {
            _parent = parent;

            foreach (EVarType type in Enum.GetValues(typeof(EVarType)))
            {
                _varMax[type] = 0;
            }

            InitCompilerEmit(ParseDefinitions.ReferencingToken);

        }
        public void AppendLog(int lineNr, string text)
        {
            _parent.Log.Add(lineNr >= 0 ? $"Line {lineNr}: {text}" : text);
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
            if (_parent.SourceCode == null)
            {
                AppendLog(-1, StrKeineQuelleIstZw);
                return false;
            }

            if (_parent.TokenCode == null)
            {
                AppendLog(-1, StrLeererTokenCodeOK);
            }

            if (_parent.Labels == null)
            {
                AppendLog(-1, StrLeeresLabelArray);
                _parent.Labels = new List<ILabel>();
            }
            else
            {
                _parent.Labels.Clear();
            }

            if (_parent.Messages == null)
            {
                AppendLog(-1, StrLeeresMessageArray);
                _parent.Messages = new List<string>();
            }
            else
            {
                //??
                _parent.Messages.Clear();
            }

            if (_parent.Variables == null)
            {
                AppendLog(-1, StrKeinSystemVariab);
                _parent.Variables = new List<IVariable>();
            }

            ResetStateForCompile();
            _parent.TokenCode = [];
            var parseResults = new List<IReadOnlyList<KeyValuePair<string, object?>>?>();

            int lineIndex = 0;
            foreach (var line in _parent.SourceCode)
            {
                parseResults.Add(ParseLine(_parent.CCommand, line, out int parseError));
                if (parseError > 0 && string.IsNullOrWhiteSpace(line))
                {
                    AppendLog(lineIndex, StrFehlerBeimParsen + $"{parseError}");
                }
                lineIndex++;
            }

            lineIndex = 0;
            foreach (var parseTree in parseResults)
            {
                switch (BuildCommand(parseTree, _parent.TokenCode, 0, lineIndex++, out string errorText))
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
                if (_parent.TokenCode[lineIndex++].Token is EDriveToken.tt_goto)
                {
                    BuildCommand(parseTree, _parent.TokenCode, 0, lineIndex - 1, out _);
                }
            }

            return true;
        }
    }
}
