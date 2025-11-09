using System;
using System.Linq;
using System.Collections.Generic;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;
using TranspilerLib.Models.Scanner;

namespace TranspilerLib.Pascal.Models.Scanner;

/// <summary>
/// Pascal-spezifischer Builder ohne Rückgriff auf <c>base.OnToken</c>.
/// Entfernt Level-basierte Logik der Basisklasse und baut ausschließlich einen
/// expliziten Pascal-Strukturbaum:
/// - PROGRAM/UNIT Marker am Root
/// - Deklarationssektionen (var/const/type) vor begin
/// - Body Block für begin..end
/// - Assignment-Reparenting: x := a + b -> Assignment[x, Operation('+')[a,b]]
/// </summary>
public class PasCodeBuilder : CodeBuilder
{
    private const string DECL_KEY = "__DECL";
    private const string DECL_COMMA_KEY = "__DECL_COMMA";
    private const string DECL_COLON_KEY = "__DECL_COLON";

    public PasCodeBuilder()
    {
        NewCodeBlock = (name, type, code, parent, pos)
            => new PasCodeBlock() { Name = name, Type = type, Code = code, Parent = parent, SourcePos = pos };
    }

    public override void OnToken(TokenData tokenData, ICodeBuilderData data)
    {
        switch (tokenData.type)
        {
            case CodeBlockType.Block:
                BuildBlock(tokenData, data);
                break;
            case CodeBlockType.Assignment:
                BuildAssignment(tokenData, data);
                break;
            case CodeBlockType.Operation:
                BuildOperation(tokenData, data);
                break;
            case CodeBlockType.Variable:
            case CodeBlockType.Number:
                BuildValue(tokenData, data);
                break;
            case CodeBlockType.Separator:
                // Semikolons werden im Kontext behandelt
                break;
            default:
                AddRawToken(tokenData, data);
                break;
        }
        data.cbtLast = tokenData.type;
    }

    private static bool IsBegin(string code) => string.Equals(code, "begin", StringComparison.OrdinalIgnoreCase);
    private static bool IsEnd(string code) => string.Equals(code, "end", StringComparison.OrdinalIgnoreCase);

    private void BuildBlock(TokenData tokenData, ICodeBuilderData data)
    {
        if (IsBegin(tokenData.Code))
        {
            var isMetaRoot = data.actualBlock.Type == CodeBlockType.MainBlock || data.actualBlock.Type == CodeBlockType.Function;
            var name = isMetaRoot ? "Body" : "Block";
            data.actualBlock = NewCodeBlock(name, CodeBlockType.Block, "begin", data.actualBlock, tokenData.Pos);
            ClearDeclState(data);
        }
        else if (IsEnd(tokenData.Code))
        {
            if (data.actualBlock.Parent != null)
                data.actualBlock = data.actualBlock.Parent;
        }
        else
        {
            AddRawToken(tokenData, data);
        }
    }

    private void BuildAssignment(TokenData tokenData, ICodeBuilderData data)
    {
        var left = data.actualBlock;
        if (left == null) return;

        var parent = left.Parent ?? left;
        int leftIndex = left.Parent != null ? left.Parent.SubBlocks.IndexOf(left) : -1;

        var assign = NewCodeBlock("Assignment", CodeBlockType.Assignment, tokenData.Code, parent, tokenData.Pos);

        if (left.Parent != null && leftIndex >= 0)
        {
            var siblings = left.Parent.SubBlocks;
            int idxAssign = siblings.IndexOf(assign);
            if (idxAssign >= 0)
                siblings.RemoveAt(idxAssign);
            siblings[leftIndex] = assign;
        }

        if (!left.MoveToSub(assign))
        {
            if (left.Parent != null)
            {
                var siblings = left.Parent.SubBlocks;
                int idx = siblings.IndexOf(left);
                if (idx >= 0) siblings.RemoveAt(idx);
            }
            assign.SubBlocks.Insert(0, left);
        }
        else
        {
            var idxL = assign.SubBlocks.IndexOf(left);
            if (idxL > 0)
            {
                assign.SubBlocks.RemoveAt(idxL);
                assign.SubBlocks.Insert(0, left);
            }
        }

        data.actualBlock = assign;
    }

    private static bool IsDeclContext(ICodeBuilderData data)
        => data.labels != null && data.labels.ContainsKey(DECL_KEY);

    private static ICodeBlock? GetDecl(ICodeBuilderData data)
        => IsDeclContext(data) ? data.labels[DECL_KEY] : null;

    private static ICodeBlock? GetDeclComma(ICodeBuilderData data)
        => (data.labels != null && data.labels.ContainsKey(DECL_COMMA_KEY)) ? data.labels[DECL_COMMA_KEY] : null;

    private static bool DeclHasColon(ICodeBuilderData data)
        => data.labels != null && data.labels.ContainsKey(DECL_COLON_KEY);

    private static void SetDecl(ICodeBuilderData data, ICodeBlock decl)
    {
        data.labels[DECL_KEY] = decl;
        if (data.labels.ContainsKey(DECL_COMMA_KEY)) data.labels.Remove(DECL_COMMA_KEY);
        if (data.labels.ContainsKey(DECL_COLON_KEY)) data.labels.Remove(DECL_COLON_KEY);
    }

    private static void SetDeclComma(ICodeBuilderData data, ICodeBlock comma)
        => data.labels[DECL_COMMA_KEY] = comma;

    private static void SetDeclColon(ICodeBuilderData data)
        => data.labels[DECL_COLON_KEY] = data.actualBlock;

    private static void ClearDeclState(ICodeBuilderData data)
    {
        if (data.labels.ContainsKey(DECL_KEY)) data.labels.Remove(DECL_KEY);
        if (data.labels.ContainsKey(DECL_COMMA_KEY)) data.labels.Remove(DECL_COMMA_KEY);
        if (data.labels.ContainsKey(DECL_COLON_KEY)) data.labels.Remove(DECL_COLON_KEY);
    }

    private static bool IsOperator(string text)
        => text is "+" or "-";

    private void BuildOperation(TokenData tokenData, ICodeBuilderData data)
    {
        var text = tokenData.Code.Trim();
        var upper = text.ToUpperInvariant();

        if (upper == "PROGRAM" && data.actualBlock.Type == CodeBlockType.MainBlock)
        {
            data.actualBlock.Code = "Program";
            return;
        }
        if (upper == "UNIT" && data.actualBlock.Type == CodeBlockType.MainBlock)
        {
            data.actualBlock.Code = "Unit";
            return;
        }

        if (upper == "VAR")
        {
            var parent = data.actualBlock;
            var decl = NewCodeBlock("Declaration", CodeBlockType.Declaration, "var", parent, tokenData.Pos);
            data.actualBlock = decl;
            SetDecl(data, decl);
            return;
        }

        if (upper == "CONST")
        {
            var parent = data.actualBlock;
            var decl = NewCodeBlock("Declaration", CodeBlockType.Declaration, "const", parent, tokenData.Pos);
            data.actualBlock = decl;
            SetDecl(data, decl);
            return;
        }

        if (upper == "TYPE")
        {
            var parent = data.actualBlock;
            var decl = NewCodeBlock("Declaration", CodeBlockType.Declaration, "type", parent, tokenData.Pos);
            data.actualBlock = decl;
            SetDecl(data, decl);
            return;
        }

        if (IsDeclContext(data))
        {
            var decl = GetDecl(data)!;
            if (text == "," && !DeclHasColon(data))
            {
                var comma = GetDeclComma(data);
                if (comma == null)
                {
                    comma = NewCodeBlock("Operation", CodeBlockType.Operation, ",", decl, tokenData.Pos);
                    if (decl.SubBlocks.Count > 0)
                    {
                        var last = decl.SubBlocks[^1];
                        if (last.Type == CodeBlockType.Variable)
                            last.MoveToSub(comma);
                    }
                    SetDeclComma(data, comma);
                }
                data.actualBlock = comma;
                return;
            }
            if (text == ":")
            {
                var colon = NewCodeBlock("Operation", CodeBlockType.Operation, ":", decl, tokenData.Pos);
                var comma = GetDeclComma(data);
                if (comma != null)
                {
                    comma.MoveToSub(colon);
                }
                else if (decl.SubBlocks.Count > 0)
                {
                    var v = decl.SubBlocks[^1];
                    if (v.Type == CodeBlockType.Variable)
                        v.MoveToSub(colon);
                }
                SetDeclColon(data);
                data.actualBlock = colon;
                return;
            }
            if (text == ";")
            {
                data.actualBlock = decl.Parent ?? decl;
                ClearDeclState(data);
                return;
            }
        }

        if (IsOperator(text))
        {
            ICodeBlock? assign = data.actualBlock.Type == CodeBlockType.Assignment
                ? data.actualBlock
                : data.actualBlock.Parent?.Type == CodeBlockType.Assignment ? data.actualBlock.Parent : null;
            if (assign != null)
            {
                var op = NewCodeBlock("Operation", CodeBlockType.Operation, text, assign, tokenData.Pos);
                if (assign.SubBlocks.Count > 1)
                {
                    var last = assign.SubBlocks[^1];
                    if (last != op)
                        last.MoveToSub(op);
                }
                data.actualBlock = op;
                return;
            }
        }

        if (data.actualBlock.Type is not CodeBlockType.Operation and not CodeBlockType.MainBlock
            || (!string.IsNullOrEmpty(data.actualBlock.Code) && data.actualBlock.Code.EndsWith(";")))
        {
            var parent = (data.actualBlock.Type is CodeBlockType.Function or CodeBlockType.Block)
                ? data.actualBlock
                : data.actualBlock.Parent;
            data.actualBlock = NewCodeBlock("Operation", CodeBlockType.Operation, text, parent, tokenData.Pos);
        }
        else
        {
            var pad = (data.actualBlock.Code.EndsWith("\"") && text.StartsWith("+")) || text.StartsWith("(")
                ? " " : string.Empty;
            data.actualBlock.Code += pad + tokenData.Code;
        }
    }

    private void BuildValue(TokenData tokenData, ICodeBuilderData data)
    {
        if (IsDeclContext(data) && !DeclHasColon(data))
        {
            var parent = GetDeclComma(data) ?? GetDecl(data)!;
            data.actualBlock = NewCodeBlock(
                tokenData.type == CodeBlockType.Variable ? "Variable" : "Number",
                tokenData.type,
                tokenData.Code,
                parent,
                tokenData.Pos);
            return;
        }

        if (IsDeclContext(data) && DeclHasColon(data))
        {
            var colon = data.actualBlock.Type == CodeBlockType.Operation && data.actualBlock.Code == ":"
                ? data.actualBlock
                : GetDecl(data)!.SubBlocks.LastOrDefault(sb => sb.Type == CodeBlockType.Operation && sb.Code == ":") ?? data.actualBlock;

            data.actualBlock = NewCodeBlock(
                tokenData.type == CodeBlockType.Variable ? "Type" : (tokenData.type == CodeBlockType.Number ? "Number" : "Value"),
                tokenData.type,
                tokenData.Code,
                colon,
                tokenData.Pos);
            return;
        }

        if (data.actualBlock.Type == CodeBlockType.Assignment)
        {
            data.actualBlock = NewCodeBlock(
                tokenData.type == CodeBlockType.Variable ? "Variable" : "Number",
                tokenData.type,
                tokenData.Code,
                data.actualBlock,
                tokenData.Pos);
            return;
        }
        if (data.actualBlock.Type == CodeBlockType.Operation && IsOperator(data.actualBlock.Code)
            && data.actualBlock.Parent?.Type == CodeBlockType.Assignment)
        {
            data.actualBlock = NewCodeBlock(
                tokenData.type == CodeBlockType.Variable ? "Variable" : "Number",
                tokenData.type,
                tokenData.Code,
                data.actualBlock,
                tokenData.Pos);
            return;
        }

        AddRawToken(tokenData, data);
    }

    /// <summary>
    /// Ersetzt alle früheren base.OnToken-Verwendungen: Fügt ein generisches Token
    /// als eigener Block unter dem aktuellen Kontext hinzu ohne Level-Logik.
    /// </summary>
    private void AddRawToken(TokenData tokenData, ICodeBuilderData data)
    {
        var parent = data.actualBlock;
        data.actualBlock = NewCodeBlock(
            tokenData.type.ToString(),
            tokenData.type,
            tokenData.Code,
            parent,
            tokenData.Pos);
    }
}
