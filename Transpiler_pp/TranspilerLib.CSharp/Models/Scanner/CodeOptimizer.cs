using BaseLib.Helper;
using System.Collections.Generic;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;


public class CodeOptimizer : ICodeOptimizer
{
    public bool _noWhile { get; set; } = false;

    private ICodeBlock? RemoveGotoAndLabel(ICodeBlock item, ICodeBlock source)
    {
        var p = source.Parent;
        if (p != null
            && p == item.Parent
            && (item.Index < source.Index + 3)
                && (item.Index > source.Index))
        {
            var c = item;
            while (c.Next is ICodeBlock next2
                && IsCommentLike(next2))
                c = next2;
            if (c.Next is ICodeBlock next
                && IsNumAssignment(next)
                && !next.Code.Contains("(")
                && next.Code.EndsWith(";"))
                c = next;
            if (c.Next is ICodeBlock next3)
                c = next3;
            var deleteCount = c.Index - source.Index;
            if (deleteCount > 0)
                _ = p.DeleteSubBlocks(source.Index, deleteCount);
            if (FindLeadingLabel(c) is ICodeBlock labelItem
                && labelItem.Sources.Count >= 2)
                TestItem(labelItem);
        }
        return p;
    }

    private static void MoveItemBlockToSource(ICodeBlock item, ICodeBlock source)
    {
        if (item.Parent is ICodeBlock actParent
            && ((source.Parent != actParent
                && !(source.Parent?.Code.StartsWith("switch") ?? true))
            || ((source.Parent == actParent)
                && (source.Next != item)
                && (source.Next?.Next != item)
                 && FindLeadingLabel(source) is ICodeBlock lbl
                 && (lbl.Code != "default:")
                 && (!lbl.Code.StartsWith("case ")
                    || !actParent.Code.StartsWith("switch (try")) // VB-specific
                    )))
        {
            // Calculate Chunksize
            var l = CalcChunksize(item);
            if (l <= 0)
                return;
            var cDst = source;
            while (cDst.Type is CodeBlockType.Goto or CodeBlockType.LComment && cDst.Next is ICodeBlock next)
                cDst = next;
            _ = actParent.MoveSubBlocks(item.Index, cDst, l);
        }
    }

    private static void TestMoveToLowerLevel(ICodeBlock source)
    {
        if (source.Prev is ICodeBlock prev
            && prev.Type == CodeBlockType.Operation
            && prev.SubBlocks.Count == 0
            && ((prev.Code == "else")
            || prev.Code.StartsWith("if ")))
        {
            _ = new CodeBlock() { Name = "Start", Type = CodeBlockType.Block, Code = "{", Parent = prev };
            source.Parent = prev;
            _ = new CodeBlock() { Name = "End", Type = CodeBlockType.Block, Code = "}", Parent = prev };
        }
    }

    private void DeleteAndMoveGoto(ICodeBlock cIf, ICodeBlock cIfGoto, ICodeBlock cElse, ICodeBlock cElseGoto)
    {
        if (cIfGoto.Type == CodeBlockType.Goto
            && cElseGoto.Type == CodeBlockType.Goto
            && cElse.Next is ICodeBlock next)
        {
            var flag = false;
            ICodeBlock? destLabel = null;
            if (cElseGoto.Code == cIfGoto.Code)
            {
                flag = cIf.DeleteSubBlocks(cIfGoto.Index, 1);
                flag |= cElse.MoveSubBlocks(cElseGoto.Index, next, 1);
            }
            else if (cElseGoto.Destination is not null
                && cElseGoto.Destination.TryGetTarget(out var cEGTarg)
                && cIfGoto.Destination is not null
                && cIfGoto.Destination.TryGetTarget(out var cIGTarg)
                && cEGTarg.Sources.Count > 2
                && cIGTarg.Sources.Count > 2)
                flag = cElse.MoveSubBlocks(cElseGoto.Index, next, 1);
            // ======================
            if (cElse.SubBlocks.Count == 2
                && cElse.SubBlocks.All((c) => c.Type == CodeBlockType.Block))
                if (cElse.Parent is ICodeBlock elseParent)
                    _ = elseParent.DeleteSubBlocks(cElse.Index, 1);
            if (cElseGoto.Destination is not null
                && cElseGoto.Destination.TryGetTarget(out var resolvedDestLabel))
                destLabel = resolvedDestLabel;
            if (flag
                && destLabel?.Sources.Count is 1 or 2)
                TestItem(destLabel);
            else if (flag && destLabel != null && destLabel.Sources.Count > 1)
                //  foreach (var item in _testList)
                TestEndGotoUpLevel(next.Parent);

        }
    }

    private static ICodeBlock? FindLeadingLabel(ICodeBlock item)
    {
        ICodeBlock c = item;
        while (c.Type is not CodeBlockType.Label && c.Prev is ICodeBlock prev)
            c = prev;
        return c.Type == CodeBlockType.Label ? c : null;
    }

    private static ICodeBlock? FindTailingGoto(IList<ICodeBlock> codeBlocks)
    {
        if (codeBlocks.Count > 0)
        {
            ICodeBlock c = codeBlocks[codeBlocks.Count - 1];
            while ((IsCommentLike(c) || c.Type == CodeBlockType.Block)
                && c.Prev is ICodeBlock prev)
                c = prev;
            return c.Type == CodeBlockType.Goto ? c : null;
        }
        else
            return null;
    }

    private void TestEndGotoUpLevel(ICodeBlock? actParent)
    {
        if (actParent is ICodeBlock a
            && FindTailingGoto(a.SubBlocks) is ICodeBlock ag)
        {
            if (a.Code == "else"
                && a.Prev is ICodeBlock b1
                && (b1.Code.StartsWith("if ")
                    || b1.Code.StartsWith("if(")))
            {
                if (b1.SubBlocks.Count > 0
                  && FindTailingGoto(b1.SubBlocks) is ICodeBlock bg)
                {
                    DeleteAndMoveGoto(b1, bg, a, ag);
                }
            }
            else if ((a.Code.StartsWith("if ")
                  || a.Code.StartsWith("if("))
                && a.Next is ICodeBlock b2
                && b2.Code == "else")
            {
                if (b2.SubBlocks.Count > 0
                    && FindTailingGoto(b2.SubBlocks) is ICodeBlock bg)
                {
                    DeleteAndMoveGoto(a, ag, b2, bg);
                }
            }
            else if (IsIfStatement(a)
                && a.Next is ICodeBlock b3
                && b3.Type == CodeBlockType.Goto)
            {
                if (ag.Code == b3.Code)
                {
                    _ = a.DeleteSubBlocks(ag.Index, 1);
                }
            }
        }

    }

    private static bool IsIfStatement(ICodeBlock a)
    {
        return a.Type == CodeBlockType.Operation
            && (a.Code.StartsWith("if ")
                || a.Code.StartsWith("if("));
    }

    private static bool IsCommentLike(ICodeBlock item)
    {
        return item.Type is CodeBlockType.Comment or CodeBlockType.LComment;
    }

    private static bool IsElseStatement(ICodeBlock item)
    {
        return item.Type == CodeBlockType.Operation
            && item.Code == "else";
    }

    private static bool IsNumAssignment(ICodeBlock item)
    {
        return item.Type == CodeBlockType.Operation
            && item.Code.StartsWith("num =");
    }

    private static string ReplaceLeadingIfWithWhile(string code)
    {
        if (code.StartsWith("if("))
            return $"while{code.Substring(2)}";
        if (code.StartsWith("if "))
            return $"while{code.Substring(2)}";
        return code;
    }

    private static string ReplaceIdentifierToken(string text, string identifier, string replacement)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(identifier))
            return text;

        int startIndex = 0;
        while (startIndex < text.Length)
        {
            var matchIndex = text.IndexOf(identifier, startIndex, System.StringComparison.Ordinal);
            if (matchIndex < 0)
                break;

            var hasStartBoundary = matchIndex == 0 || !IsIdentifierChar(text[matchIndex - 1]);
            var endIndex = matchIndex + identifier.Length;
            var hasEndBoundary = endIndex >= text.Length || !IsIdentifierChar(text[endIndex]);
            if (hasStartBoundary && hasEndBoundary)
            {
                text = text.Substring(0, matchIndex) + replacement + text.Substring(endIndex);
                startIndex = matchIndex + replacement.Length;
            }
            else
            {
                startIndex = matchIndex + identifier.Length;
            }
        }

        return text;
    }

    private static bool IsIdentifierChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_' || c == '.';
    }

    public void TestItem(ICodeBlock item)
    {
        RemoveResumeNextCode(item);
        if (item.Sources.Count == 1
            && item.Sources[0].TryGetTarget(out var source))
        {
            TestMoveToLowerLevel(source);
            MoveItemBlockToSource(item, source);
            var actParent = RemoveGotoAndLabel(item, source);
            TestEndGotoUpLevel(actParent);
        }
        else if (item.Sources.Count > 1)
            TestForEasyWhileLoops(item);
    }

    private void TestForEasyWhileLoops(ICodeBlock item)
    {
        if (_noWhile) return;
        foreach (var wrSource in item.Sources)
            if (wrSource.TryGetTarget(out var source)
                && source.Parent is ICodeBlock ifItem
                && IsIfStatement(ifItem)
                && (IsFirstInstructon(source) || IsFirstInstructon(ifItem))
                && FindLeadingLabel(ifItem) == item)
            {
                // Find while-pattern
                ReplaceIfVar(ifItem);
                ReplaceIfVar(ifItem);
                var c = item.Next;
                while (c?.Next is ICodeBlock next
                    && IsCommentLike(c))
                    c = next;
                if (c?.Next is ICodeBlock next2
                    && IsNumAssignment(c))
                    c = next2;
                // move Instructions to While-Goto
                if (c != null
                    && item.Parent is ICodeBlock itemParent
                    && c.Index >= 0
                    && ifItem.Index > c.Index
                    && itemParent.MoveSubBlocks(c.Index, source, ifItem.Index - c.Index))
                {
                    ifItem.Code = ReplaceLeadingIfWithWhile(ifItem.Code);
                    // Delete goto
                    DeleteItem(source);
                    if (ifItem.Next is ICodeBlock elseItem
                       && IsElseStatement(elseItem)
                       && elseItem.Next != null)
                    {
                        elseItem.MoveSubBlocks(1, elseItem.Next, elseItem.SubBlocks.Count - 2);
                        DeleteItem(elseItem);
                    }
                    TestItem(item);
                    break;
                }
            }

        static void ReplaceIfVar(ICodeBlock ifItem)
        {
            if (ifItem.Prev is ICodeBlock asItem1
                && asItem1.Code.Contains(" = "))
            {
                var var1 = asItem1.Code.Substring(0, asItem1.Code.IndexOf(" = "));
                var var2 = asItem1.Code.Substring(asItem1.Code.IndexOf(" = ") + 3).TrimEnd(';');
                if (IsIdentifyer(var1) && asItem1.Code.Contains(var1) && !var2.Contains("+") && !var2.Contains("-"))
                {
                    ifItem.Code = $"{ifItem.Code.Substring(0, 2)}{ReplaceIdentifierToken(ifItem.Code.Substring(2), var1, var2)}";
                    DeleteItem(asItem1);
                }
            }
        }
    }

    private static bool IsFirstInstructon(ICodeBlock source)
    {
        bool result = true;
        var test = source.Prev;
        while (result && test is CodeBlock c && !(c.Type is CodeBlockType.Label))
        {
            result = IsCommentLike(c)
                || c.Type == CodeBlockType.Block
                || IsNumAssignment(c);
            test = c.Prev;
        }
        return result;
    }

    private static void DeleteItem(ICodeBlock asItem1)
    {
        if (asItem1.Parent is ICodeBlock p)
            _ = p.DeleteSubBlocks(asItem1.Index, 1);
    }

    private static bool IsIdentifyer(string var2)
    {
        if (var2.Length == 0) return false;
        if (!char.IsLetter(var2[0]) && var2[0] != '_') return false;
        foreach (char c in var2)
            if (!IsIdentifierChar(c))
                return false;
        return true;
    }

    private static void RemoveResumeNextCode(ICodeBlock item)
    {
        if (item.Sources.Count != 2) return;
        foreach (var item2 in item.Sources)
            if (item2.TryGetTarget(out var source))
                if (source.Parent is ICodeBlock parent
                    && source.Index >= 0
                    && source.Index < parent.SubBlocks.Count
                    && parent.Code is string s
                    && s.StartsWith("switch (num")
                    && s.EndswithAny("(num4)", "(num5)", "(num6)", "(num7)", "(num7)", "(num8)", "(num9)"))
                {
                    var l = 1;
                    while (source.Index - l >= 0
                        && parent.SubBlocks[source.Index - l].Type == CodeBlockType.Label)
                        l++;
                    var deleteIndex = source.Index - l + 1;
                    if (deleteIndex >= 0 && deleteIndex + l <= parent.SubBlocks.Count)
                        _ = parent.DeleteSubBlocks(deleteIndex, l);
                    break;
                }
    }

    public static int CalcChunksize(ICodeBlock item)
    {
        if (item.Parent is ICodeBlock codeBlock)
        {
            if (item.Index < 0 || item.Index >= codeBlock.SubBlocks.Count)
                return 0;

            int l = 1;
            while (item.Index + l < codeBlock.SubBlocks.Count - 1
                && codeBlock.SubBlocks[item.Index + l].Type is not CodeBlockType.Label and not CodeBlockType.Block)
                l++;
            return l;
        }
        else return 0;
    }

}


