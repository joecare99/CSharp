using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using TranspilerLib.DriveBASIC.Data;
using TranspilerLib.DriveBASIC.Data.Interfaces;

namespace TranspilerLib.DriveBASIC.Models;

public partial class DriveBasic
{
    public Compiler.BCErr BuildCommand(IReadOnlyList<KeyValuePair<string, object?>>? parseTree, IList<IDriveCommand> tokenBuffer, int level, int pc, out string errorText)=>
        new Compiler(this).BuildCommand(parseTree, tokenBuffer, level, pc, out errorText);


    public partial class Compiler
    {

        private IList<(EDriveToken Token, int SubToken, bool xExprToken, EDriveToken ReferencingToken, int ReferencingSubtoken, bool Backward, bool xRefExpr)> ReferencingToken;

        private void InitCompilerEmit(IEnumerable<(EDriveToken Token, int SubToken, EDriveToken ReferencingToken, int ReferencingSubtoken, bool Backward)> refTokenBase)
        {
            bool b, b2;
            ReferencingToken = refTokenBase.Select(
                o => (o.Token, (b = TokenUsesExpression(o.Token)) && o.SubToken != -1 ? o.SubToken * 64 : o.SubToken, b,
                      o.ReferencingToken, (b2 = TokenUsesExpression(o.ReferencingToken)) && o.ReferencingSubtoken != -1 ? o.ReferencingSubtoken * 64 : o.ReferencingSubtoken,
                      o.Backward, b2)).ToList();
        }
        public BCErr BuildCommand(IReadOnlyList<KeyValuePair<string, object?>>? parseTree, IList<IDriveCommand> tokenBuffer, int level, int pc, out string errorText)
        {
            errorText = string.Empty;
            if (parseTree == null)
            {
                errorText = StrSyntaxError;
                return BCErr.BC_SyntaxError;
            }

            var builder = new CommandBuilderState();

            try
            {
                var result = ProcessNode(parseTree, builder, tokenBuffer, level, pc, out errorText);
                if (result != BCErr.BC_OK)
                    return result;

                if (!builder.HasToken)
                {
                    errorText = StrSyntaxError;
                    return BCErr.BC_SyntaxError;
                }

                EnsureTokenSlot(tokenBuffer, pc);
                tokenBuffer[pc] = builder.ToDriveCommand();
                return BCErr.BC_OK;
            }
            catch (Exception ex)
            {
                errorText = ex.Message;
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
            var isTokenNode = placeholder.Equals(_parent.CToken, StringComparison.OrdinalIgnoreCase);
            var isExpressionNode = placeholder.Equals(CExpression, StringComparison.OrdinalIgnoreCase);
            var isCommandNode = placeholder.Equals(_parent.CCommand, StringComparison.OrdinalIgnoreCase);

            if (isTokenNode)
            {
                if (ruleIndex < 0 || ruleIndex >= _parent.parseDefs.Length)
                {
                    error = StrSyntaxError;
                    return BCErr.BC_SyntaxError;
                }
                var def = _parent.parseDefs[ruleIndex];
                builder.Initialize(def.Token, def.SubToken);
            }
            else if (isExpressionNode)
            {
                if (ruleIndex < 0 || ruleIndex >= _parent.expressionNormals.Count)
                {
                    error = StrSyntaxError;
                    return BCErr.BC_SyntaxError;
                }
                builder.AddSubToken(_parent.expressionNormals[ruleIndex].Item1);
            }
            else if (isCommandNode)
            {
                RegisterInlineLabels(nodes, pc);
            }
            else
            {
                if (ruleIndex < 0 || ruleIndex >= _parent.PlaceHolderSubst.Length)
                {
                    error = StrSyntaxError;
                    return BCErr.BC_SyntaxError;
                }
                builder.AddSubToken(_parent.PlaceHolderSubst[ruleIndex].Number);
            }

            foreach (var child in nodes)
            {
                if (_parent.IsSystemPlaceholder(child.Key, out _)
                    && !child.Key.Equals(_parent.CToken, StringComparison.OrdinalIgnoreCase)
                    && !child.Key.Equals(ParseDefinitions.CTToken, StringComparison.OrdinalIgnoreCase))
                {
                    var placeholderIndex = _parent.ParsePlaceholderIndex(child.Key);
                    if (placeholderIndex < 0)
                    {
                        error = child.Key ?? string.Empty;
                        return BCErr.BC_UnknownPlaceHolder;
                    }

                    var sysResult = HandleSystemPlaceholder(placeholderIndex, child.Value, builder, isCommandNode, level, pc, out error);
                    if (sysResult != BCErr.BC_OK)
                        return sysResult;
                    continue;
                }

                if (child.Value is IReadOnlyList<KeyValuePair<string, object?>> nested)
                {
                    var nestedResult = ProcessNode(nested, builder, tokenBuffer, level + 1, pc, out error);
                    if (nestedResult != BCErr.BC_OK)
                        return nestedResult;
                }
                else
                {
                    var literal = (child.Value as string)?.Trim() ?? string.Empty;
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
               && _parent.IsSystemPlaceholder(placeholder!, out _)
               && placeholder!.EndsWith("Label>", StringComparison.OrdinalIgnoreCase);

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

        private CompilerLabel EnsureLabel(string labelName)
        {
            var normalized = NormalizeIdentifier(labelName);
            if (!_labelsByName.TryGetValue(normalized, out var label))
            {
                label = new CompilerLabel { Name = normalized };
                _labelsByName[normalized] = label;
                _parent.Labels.Add(label);
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
                _parent.Messages.Add(text.Trim());
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
            var list = _parent.Variables ??= new List<IVariable>();
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
            if (name.Length >= 2 && name.LastIndexOf('.') == name.Length - 2)
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
                    && (rule.SubToken == -1 || GetComparableSubToken(builder.Token, builder.SubToken, rule.xExprToken) == rule.SubToken);

                bool matchesForward = !rule.Backward
                    && builder.Token == rule.ReferencingToken
                    && (rule.ReferencingSubtoken == -1 || GetComparableSubToken(builder.Token, builder.SubToken, rule.xRefExpr) == rule.ReferencingSubtoken);

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

        private int GetComparableSubToken(EDriveToken token, int subToken, bool xExpr)
            => xExpr ? subToken & 0b1100_0000 : subToken;

        private int FindReferenceTarget(IList<IDriveCommand> tokenBuffer, int startIndex, EDriveToken searchToken, int searchSubToken)
        {
            var index = startIndex;
            while (index >= 0 && index < tokenBuffer.Count)
            {
                var jumped = false;
                var candidate = tokenBuffer[index];
                foreach (var rule in ReferencingToken.Where(r => r.Backward && r.Token == candidate.Token))
                {
                    if ((rule.SubToken == -1 || GetComparableSubToken(candidate.Token, candidate.SubToken, rule.xExprToken) == rule.SubToken)
                        && candidate.Par1 <= index)
                    {
                        index = candidate.Par1 - 1;
                        jumped = true;
                        break;
                    }
                }

                if (!jumped)
                {
                    if (candidate.Token == searchToken
                        && (searchSubToken == -1 || GetComparableSubToken(candidate.Token, candidate.SubToken, TokenUsesExpression(candidate.Token)) == searchSubToken))
                    {
                        return index;
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
            tokenBuffer[index].Par1 = newValue;
        }

        private bool TokenUsesExpression(EDriveToken token)
        => _parent.parseDefs.Any(def => def.Token == token && DefinitionUsesExpression(def.text));

        public enum BCErr
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
}
