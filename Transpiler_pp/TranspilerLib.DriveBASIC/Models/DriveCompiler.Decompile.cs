using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BaseLib.Helper;
using TranspilerLib.DriveBASIC.Data;
using TranspilerLib.DriveBASIC.Data.Interfaces;
using static TranspilerLib.DriveBASIC.Data.ParseDefinitions;

namespace TranspilerLib.DriveBASIC.Models;

public partial class DriveBasic
{
    public void Decompile()
      => new Decompiler(this).Decompile();

    public class Decompiler(DriveBasic parent)
    {
        public void Decompile()
        {
            var i = 0;
            var indent = 2;
            var hasLabelPrefix = false;
            string sourceLine = string.Empty;

            IList<string> result = [];
            foreach (var cmd in parent.TokenCode)
            {
                if (cmd.Token is EDriveToken.tt_end or EDriveToken.tt_else)
                {
                    indent -= 4;
                }
                if (cmd.Token is EDriveToken.tte_if && cmd.SubToken % 64 == 1)
                {
                    indent += 4;
                }

                sourceLine = DecompileTC(cmd, hasLabelPrefix, i);
                if (hasLabelPrefix || sourceLine.StartsWith("//", StringComparison.Ordinal))
                {
                    result.Add(new string(' ', indent) + sourceLine);
                }
                else
                {
                    result.Add(new string(' ', indent + 4) + sourceLine);
                }

                if (cmd.Token is EDriveToken.tt_else or EDriveToken.tte_if or EDriveToken.tte_while
                    or EDriveToken.tt_for || cmd.Token == EDriveToken.tt_Nop && cmd.SubToken == 5)
                {
                    indent += 4;
                }

                i++;
            }
            parent.SourceCode = result;
        }

        private string DecompileTC(IDriveCommand cmd, bool hasLabelPrefix, int index)
        {
            ParseDef? found = null;
            foreach (var definition in parent.parseDefs.Where(o => o.Token == cmd.Token))
            {
                if ((DefinitionUsesExpression(definition.text) ? cmd.SubToken & 0b11000000 : cmd.SubToken) == definition.SubToken)
                {
                    found = definition;
                    break;
                }
            }

            string result;
            if (found != null && found.text != null)
            {
                if (cmd.Token == EDriveToken.tt_Nop && cmd.SubToken == 5)
                {
                    cmd.Par1 = index;
                }
                result = found.text!;
            }
            else
            {
                result = string.Empty;
            }

            int nextPlaceholder = result.GetNextPlaceHolder();
            while (nextPlaceholder < result.Length)
            {
                result = SubstitutePlaceHolder(result, cmd);
                nextPlaceholder = result.GetNextPlaceHolder();
            }

            if (cmd.Token != EDriveToken.tt_Nop || cmd.SubToken != 5)
            {
                for (int k = 0; k < parent.Labels?.Count; k++)
                {
                    if (parent.Labels[k].Index == index)
                    {
                        result = $"{parent.Labels[k].Name}: {result}";
                    }
                }
            }

            return result;
        }

        private string SubstitutePlaceHolder(string line, IDriveCommand cmd)
        {
            int placeholderPos = line.GetNextPlaceHolder();
            int placeholderEnd = line.IndexOf('>', placeholderPos);
            if (placeholderPos >= 0 && placeholderEnd > placeholderPos)
            {
                var placeholderToken = line.Substring(placeholderPos, placeholderEnd - placeholderPos + 1);
                if (parent.IsSystemPlaceholder(placeholderToken, out _))
                {
                    switch (parent.ParsePlaceholderIndex(placeholderToken))
                    {
                        case 0:
                            return line.Replace(placeholderToken, GetLabelText(cmd.Par1));
                        case 1:
                            return line.Replace(placeholderToken, GetMessageText(cmd.Par1));
                        case 2:
                            return line.Replace(placeholderToken, GetVariableText(cmd.Par1));
                        case 3:
                            return line.Replace(placeholderToken, GetVariableText(cmd.Par2));
                        case 4:
                            return line.Replace(placeholderToken, GetVariableText((int)Math.Round(cmd.Par3)));
                        case 5:
                            return line.Replace(placeholderToken, cmd.Par3.ToString(CultureInfo.InvariantCulture));
                        case 6:
                            return line.Replace(placeholderToken, cmd.Par1.ToString(CultureInfo.InvariantCulture));
                        case 7:
                            return line.Replace(placeholderToken, cmd.Par2.ToString(CultureInfo.InvariantCulture));
                        case 8:
                            return line.Replace(placeholderToken, ((int)Math.Round(cmd.Par3)).ToString(CultureInfo.InvariantCulture));
                        case 9:
                            return line.Replace(placeholderToken, parent.GetAxisName(cmd.Par1));
                        case 10:
                            return line.Replace(placeholderToken, " ");
                        default:
                            return line;
                    }
                }
                else if (placeholderToken.Equals(CExpression, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var expression in parent.expressionNormals)
                    {
                        if ((cmd.SubToken & 63) == expression.Item1 && expression.Item2 == (cmd.Par2 != 0))
                        {
                            return line.Replace(placeholderToken, expression.Item3);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < parent.PlaceHolderSubst.Length; i++)
                    {
                        if (parent.PlaceHolderSubst[i].PlaceHolder.Equals(placeholderToken, StringComparison.OrdinalIgnoreCase))
                        {
                            return line.Replace(placeholderToken, parent.PlaceHolderSubst[i].text);
                        }
                    }
                }
            }

            return line;
        }

        private string GetLabelText(int par1)
        {
            var label = parent.Labels.FirstOrDefault(l => l.Index == par1);
            return label != null ? label.Name ?? par1.ToString() : par1.ToString();
        }

        private string GetMessageText(int par1)
        {
            var message = par1 >= 0 && par1 < parent.Messages.Count ? parent.Messages[par1] : null;
            return message ?? $"#{par1}";
        }

        private string GetVariableText(int value)
        {
            int labelIndex;
            int axis;
            if (value >= DimensionBase)
            {
                labelIndex = ((value - DimensionBase) / 6) + PointBase;
                axis = ((value - DimensionBase) % 6) + 1;
            }
            else
            {
                labelIndex = value;
                axis = 0;
            }

            var variable = parent.Variables.FirstOrDefault(var => var.Index == labelIndex);
            var name = variable != null ? variable.Name ?? $"#{labelIndex}" : $"#{labelIndex}";
            if (axis > 0)
            {
                name += parent.GetAxisName(axis);
            }
            return name;
        }

    }
}
