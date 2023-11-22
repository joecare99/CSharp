using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using static VBUnObfusicator.Models.ICSCode;

namespace VBUnObfusicator.Models
{
    public partial class CSCode
    {
        private static class CodeOptimizer
        {
            private static ICodeBlock? RemoveGotoAndLabel(ICodeBlock item, ICodeBlock source)
            {
                var p = source.Parent;
                if (source.Parent == item.Parent
                    && (item.Index < source.Index + 3)
                        && (item.Index > source.Index))
                {
                    var c = item;
                    while (c.Next is ICodeBlock next2
                        && next2.Type is CodeBlockType.LComment or CodeBlockType.Comment)
                        c = next2;
                    if (c.Next is ICodeBlock next
                        && next.Code.StartsWith("num =")
                        && !next.Code.Contains("(")
                        && next.Code.EndsWith(";"))
                        c = next;
                    if (c.Next is ICodeBlock next3)
                        c = next3;
                    _ = source.Parent!.DeleteSubBlocks(source.Index, c.Index - source.Index);
                    if (FindLeadingLabel(c) is ICodeBlock labelItem 
                        && labelItem.Sources.Count > 2)                        
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
                    var cDst = source;
                    while (cDst.Type is CodeBlockType.Goto or CodeBlockType.LComment && cDst.Next is ICodeBlock next)
                        cDst = next;
                    _ = actParent.MoveSubBlocks(item.Index, cDst, l);
                }
            }

            private static void TestMoveToLowerLevel(ICodeBlock source)
            {
                if (source.Prev is ICodeBlock prev
                    && prev.Type == CodeBlockType.Instruction
                    && prev.SubBlocks.Count == 0
                    && ((prev.Code == "else")
                    || prev.Code.StartsWith("if ")))
                {
                    _ = new CodeBlock() { Name = "Start", Type = CodeBlockType.Block, Code = "{", Parent = prev };
                    source.Parent = prev;
                    _ = new CodeBlock() { Name = "End", Type = CodeBlockType.Block, Code = "}", Parent = prev };
                }
            }

            private static void DeleteAndMoveGoto(ICodeBlock cIf, ICodeBlock cIfGoto, ICodeBlock cElse, ICodeBlock cElseGoto)
            {
                if (cIfGoto.Type == CodeBlockType.Goto
                    && cIfGoto.Type == CodeBlockType.Goto
                    && cElse.Next is ICodeBlock next)
                {
                    var flag = false;
                    if (cElseGoto.Code == cIfGoto.Code)
                    {
                        flag = cIf.DeleteSubBlocks(cIfGoto.Index, 1);
                        flag |= cElse.MoveSubBlocks(cElseGoto.Index, next, 1);
                    }
                    else if (cElseGoto.Destination!.TryGetTarget(out var cEGTarg)
                        && cIfGoto.Destination!.TryGetTarget(out var cIGTarg)
                        && cEGTarg.Sources.Count > 2
                        && cIGTarg.Sources.Count > 2)
                        flag = cElse.MoveSubBlocks(cElseGoto.Index, next, 1);
                    // ======================
                    if (cElse.SubBlocks.Count == 2
                        && cElse.SubBlocks.All((c) => c.Type == CodeBlockType.Block))
                        _ = cElse.Parent!.DeleteSubBlocks(cElse.Index, 1);
                    if (cElseGoto.Destination!.TryGetTarget(out var destLabel)
                        && flag
                        && destLabel.Sources.Count is 1 or 2)
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
                    while (c.Type is CodeBlockType.Block or CodeBlockType.Comment or CodeBlockType.LComment && c.Prev is ICodeBlock prev)
                        c = prev;
                    return c.Type == CodeBlockType.Goto ? c : null;
                }
                else
                    return null!;
            }

            private static void TestEndGotoUpLevel(ICodeBlock? actParent)
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
                return a.Type== CodeBlockType.Instruction 
                    && (a.Code.StartsWith("if ")
                        || a.Code.StartsWith("if("));
            }

            public static void TestItem(ICodeBlock item)
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

            private static void TestForEasyWhileLoops(ICodeBlock item)
            {
                foreach(var wrSource in item.Sources)
                    if (wrSource.TryGetTarget(out var source)
                        && source.Parent is ICodeBlock ifItem
                        && IsIfStatement(ifItem)
                        && FindLeadingLabel(ifItem) == item)
                    {
                        // Find while-pattern
                        ReplaceIfVar(ifItem);
                        ReplaceIfVar(ifItem);
                        var c = item.Next;
                        while (c?.Next is ICodeBlock next
                            && c.Type is CodeBlockType.LComment or CodeBlockType.Comment)
                            c = next;
                        if (c.Next is ICodeBlock next2 
                            && c.Code.StartsWith("num ="))
                            c = next2;
                        // move Instructions to While-Goto
                        if (item.Parent.MoveSubBlocks(c.Index, source, ifItem.Index - c.Index))
                        {
                            ifItem.Code = ifItem.Code.Replace("if", "while");
                            // Delete goto
                            DeleteItem(source);
                            if (ifItem.Next is ICodeBlock elseItem
                               && elseItem.Type == CodeBlockType.Instruction
                               && elseItem.Code == "else")
                            {
                                elseItem.MoveSubBlocks(1, elseItem.Next, elseItem.SubBlocks.Count-2);
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
                        if (IsIdentifyer(var1) && asItem1.Code.Contains(var1))
                        {
                            ifItem.Code = $"{ifItem.Code.Substring(0, 2)}{ifItem.Code.Substring(2).Replace(var1, var2)}";
                            DeleteItem(asItem1);
                        }
                    }
                }
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
                foreach(char c in var2)
                    if (!char.IsLetterOrDigit(c) && c!='_' && c != '.')
                        return false;
                return true;
            }

            private static void RemoveResumeNextCode(ICodeBlock item)
            {
                if (item.Sources.Count != 2) return;
                foreach (var item2 in item.Sources)
                    if (item2.TryGetTarget(out var source))
                        if (source.Parent?.Code is string s
                            && s.StartsWith("switch (num")
                            && s.EndswithAny("(num4)", "(num5)", "(num6)", "(num7)", "(num7)", "(num8)", "(num9)"))
                        {
                            var l = 1;
                            while (source.Parent.SubBlocks[source.Index - l].Type == CodeBlockType.Label)
                                l++;
                            _ = source.Parent.DeleteSubBlocks(source.Index - l + 1, l);
                            break;
                        }
            }

            public static int CalcChunksize(ICodeBlock item)
            {
                if (item.Parent is ICodeBlock codeBlock)
                {
                    int l = 1;
                    while (item.Index + l < codeBlock.SubBlocks.Count - 1
                        && codeBlock.SubBlocks[item.Index + l].Type is not CodeBlockType.Label and not CodeBlockType.Block)
                        l++;
                    return l;
                }
                else return 0;
            }

        }

    }
}
