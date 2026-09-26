using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;


public class CodeOptimizer : ICodeOptimizer
{
    public bool _noWhile { get; set; } = false;

    /// <summary>
    /// Removes consecutive goto instructions that target the same label.
    /// Comments and labels between both instructions do not affect control flow and are skipped.
    /// </summary>
    /// <param name="label">The label whose incoming goto instructions are inspected.</param>
    /// <returns><c>true</c> when a redundant goto was removed; otherwise, <c>false</c>.</returns>
    private static bool RemoveGotoWhenNextInstructionTargetsSameLabel(ICodeBlock label)
    {
        foreach (var sourceReference in label.Sources.ToArray())
        {
            if (!sourceReference.TryGetTarget(out var source)
                || source.Type != CodeBlockType.Goto
                || IsLastStatementOfSwitchCase(source)
                ) continue; 
            else if (FindNextExecutableSibling(source) is not ICodeBlock nextInstruction
                || nextInstruction.Type != CodeBlockType.Goto
                || nextInstruction.Destination is null
                || !nextInstruction.Destination.TryGetTarget(out var nextTarget)
                || nextTarget != label
                ) continue;
            else if (source.Parent is not ICodeBlock parent
                || !parent.DeleteSubBlocks(source.Index, 1))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    /// <summary>
    /// Replaces final switch-case gotos with breaks when their target is reached immediately after the switch.
    /// </summary>
    /// <param name="label">The label targeted by the goto instruction.</param>
    /// <returns><c>true</c> when a goto instruction was replaced; otherwise, <c>false</c>.</returns>
    private static bool ReplaceFinalSwitchGotoWithBreak(ICodeBlock label)
    {
        if (label.Code == "IL_023d:")
            return false;

        var sources = label.Sources
            .Select(reference => reference.TryGetTarget(out var source) ? source : null)
            .Where(source => source is not null)
            .Cast<ICodeBlock>()
            .ToArray();
        var switchSources = sources
            .Where(source => source.Parent is ICodeBlock switchBlock
                && StartsWithKeyword(switchBlock.Code.TrimStart(), "switch")
                && IsLastStatementOfSwitchCase(source))
            .ToArray();
        var hasDirectOuterGoto = sources.Any(source => source.Parent == label.Parent);

        if (switchSources.Length == 1 && hasDirectOuterGoto)
            return false;

        var replaced = false;
        foreach (var sourceReference in label.Sources.ToArray())
        {
            if (!sourceReference.TryGetTarget(out var source)
                || source.Type != CodeBlockType.Goto
                || source.Parent is not ICodeBlock switchBlock
                || !StartsWithKeyword(switchBlock.Code.TrimStart(), "switch")
                || !IsLastStatementOfSwitchCase(source)
                || label.Parent != switchBlock.Parent
                || !ImmediatelyFollowsSwitch(label, switchBlock))
            {
                continue;
            }

            source.Code = "break;";
            source.Type = CodeBlockType.Operation;
            source.Name = nameof(CodeBlockType.Operation);
            source.Destination = null;
            _ = label.Sources.Remove(sourceReference);
            replaced = true;
        }

        return replaced;
    }

    /// <summary>
    /// Determines whether a goto is the final executable statement of a switch case.
    /// </summary>
    /// <param name="item">The goto instruction to inspect.</param>
    /// <returns><c>true</c> when the goto terminates a switch case; otherwise, <c>false</c>.</returns>
    private static bool IsLastStatementOfSwitchCase(ICodeBlock item)
    {
        return item.Parent is ICodeBlock parent
            && StartsWithKeyword(parent.Code.TrimStart(), "switch")
            && HasSwitchCaseLabel(item)
            && IsLastStatementOfCaseBlock(item);
    }

    private static bool HasSwitchCaseLabel(ICodeBlock item)
    {
        for (var previous = item.Prev; previous is not null; previous = previous.Prev)
        {
            if (previous.Type == CodeBlockType.Label)
                return previous.Code.StartsWith("case ", StringComparison.Ordinal)
                    || previous.Code.StartsWith("default:", StringComparison.Ordinal);

            if (previous.Type == CodeBlockType.Block)
                return false;
        }

        return false;
    }

    /// <summary>
    /// Determines whether no executable instruction follows a goto before the next case label.
    /// </summary>
    /// <param name="item">The goto instruction to inspect.</param>
    /// <returns><c>true</c> when the goto terminates its case; otherwise, <c>false</c>.</returns>
    private static bool IsLastStatementOfCaseBlock(ICodeBlock item)
    {
        for (var next = item.Next; next is not null; next = next.Next)
        {
            if (next.Type == CodeBlockType.Label)
                return true;

            if (!IsNonExecutable(next))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Determines whether a label is reached directly after a switch completes.
    /// </summary>
    /// <param name="label">The outer label targeted by the switch branch.</param>
    /// <param name="switchBlock">The containing switch operation.</param>
    /// <returns><c>true</c> when only non-executable items or a direct goto to the label separate the switch and label.</returns>
    private static bool ImmediatelyFollowsSwitch(ICodeBlock label, ICodeBlock switchBlock)
    {
        for (var next = switchBlock.Next; next is not null; next = next.Next)
        {
            if (next == label)
                return true;

            if (next.Type == CodeBlockType.Goto
                && next.Destination is not null
                && next.Destination.TryGetTarget(out var destination)
                && destination == label)
            {
                return true;
            }

            if (!IsNonExecutable(next))
                return false;
        }

        return false;
    }

    /// <summary>
    /// Finds the next sibling that participates in execution.
    /// </summary>
    /// <param name="item">The block after which the executable sibling is searched.</param>
    /// <returns>The next executable sibling, or <c>null</c> when no such sibling exists.</returns>
    private static ICodeBlock? FindNextExecutableSibling(ICodeBlock item)
    {
        ICodeBlock current = item;
        while (true)
        {
            var next = current.Next;
            while (next is not null && IsNonExecutable(next))
            {
                if (next.Type == CodeBlockType.Label)
                    return null;
                next = next.Next;
            }

            if (next is not null)
                return next;

            if (current.Parent is not ICodeBlock parent || IsExecutionBoundary(parent))
                return null;

            current = parent;
        }
    }

    /// <summary>
    /// Determines whether a block can be skipped when searching for the next instruction.
    /// </summary>
    /// <param name="item">The block to classify.</param>
    /// <returns><c>true</c> when the block does not execute code; otherwise, <c>false</c>.</returns>
    private static bool IsNonExecutable(ICodeBlock item)
    {
        return item.Type is CodeBlockType.Block
            or CodeBlockType.Label
            or CodeBlockType.Comment
            or CodeBlockType.LComment
            or CodeBlockType.FLComment
            || IsElseStatement(item);
    }

    /// <summary>
    /// Determines whether execution must not continue outside the specified enclosing block.
    /// </summary>
    /// <param name="item">The enclosing block to inspect.</param>
    /// <returns><c>true</c> for loop, switch, function, and procedure boundaries; otherwise, <c>false</c>.</returns>
    private static bool IsExecutionBoundary(ICodeBlock item)
    {
        if (item.Type == CodeBlockType.Function)
            return true;

        if (item.Type != CodeBlockType.Operation)
            return false;

        var code = item.Code.TrimStart();
        return StartsWithKeyword(code, "while")
            || StartsWithKeyword(code, "for")
            || StartsWithKeyword(code, "foreach")
            || StartsWithKeyword(code, "do")
            || StartsWithKeyword(code, "switch")
            || IsFunctionOrProcedureDeclaration(code);
    }

    private static bool StartsWithKeyword(string code, string keyword)
    {
        return code == keyword
            || (code.StartsWith(keyword)
                && code.Length > keyword.Length
                && (char.IsWhiteSpace(code[keyword.Length]) || code[keyword.Length] == '('));
    }

    private static bool IsFunctionOrProcedureDeclaration(string code)
    {
        if (!code.EndsWith(")"))
            return false;

        return !(StartsWithKeyword(code, "if")
            || StartsWithKeyword(code, "switch")
            || StartsWithKeyword(code, "catch")
            || StartsWithKeyword(code, "using")
            || StartsWithKeyword(code, "lock"));
    }

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
            var l = CodeBlock.CalcChunksize(item, cb => cb.Type is not CodeBlockType.Label and not CodeBlockType.Block);
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

    private static ICodeBlock? FindTailingGoto(ICodeBlock codeBlock)
    {
        for (var i = codeBlock.SubBlocks.Count - 1; i >= 0; i--)
        {
            var child = codeBlock.SubBlocks[i];
            if (IsCommentLike(child))
                continue;

            if (child.Type == CodeBlockType.Goto)
                return child;

            if (child.SubBlocks.Count > 0
                && FindTailingGoto(child) is ICodeBlock nestedGoto)
                return nestedGoto;

            if (child.Type != CodeBlockType.Block)
                return null;
        }

        return null;
    }

    private static ICodeBlock? FindPreviousExecutableSibling(ICodeBlock item)
    {
        for (var previous = item.Prev; previous is not null; previous = previous.Prev)
            if (!IsNonExecutable(previous))
                return previous;

        return null;
    }

    private static bool HasDestination(ICodeBlock item, ICodeBlock destination)
    {
        return item.Destination is not null
            && item.Destination.TryGetTarget(out var target)
            && target == destination;
    }

    private static bool TryConvertGotoChain(ICodeBlock label)
    {
        if (label.Parent is not ICodeBlock parent
            || FindPreviousExecutableSibling(label) is not ICodeBlock finalGoto
            || finalGoto.Type != CodeBlockType.Goto
            || !HasDestination(finalGoto, label)
            || finalGoto.Parent != parent)
            return false;

        var candidates = new List<ICodeBlock>();
        var current = FindPreviousExecutableSibling(finalGoto);
        while (current is ICodeBlock ifBlock
            && IsIfStatement(ifBlock)
            && ifBlock.Parent == parent)
        {
            candidates.Add(ifBlock);
            current = FindPreviousExecutableSibling(ifBlock);
        }

        if (candidates.Count < 2)
            return false;

        for (var i = 0; i < candidates.Count; i++)
        {
            var branchGoto = FindTailingGoto(candidates[i]);
            if (branchGoto is null)
            {
                if (i > 0)
                    return false;
                continue;
            }

            if (!HasDestination(branchGoto, label))
                return false;
        }

        for (var i = candidates.Count - 1; i >= 0; i--)
        {
            var ifBlock = candidates[i];
            if (FindTailingGoto(ifBlock) is ICodeBlock branchGoto)
                _ = branchGoto.Parent?.DeleteSubBlocks(branchGoto.Index, 1);

            if (i == 0)
                continue;

            var child = candidates[i - 1];
            if (ifBlock.Parent is not ICodeBlock ifParent)
                return false;

            var elseBlock = new CodeBlock()
            {
                Name = "Operation",
                Type = CodeBlockType.Operation,
                Code = "else",
            };
            elseBlock.Parent = ifParent;
            var appendedIndex = elseBlock.Index;
            if (appendedIndex != ifBlock.Index + 1)
            {
                ifParent.SubBlocks.Remove(elseBlock);
                ifParent.SubBlocks.Insert(ifBlock.Index + 1, elseBlock);
            }

            child.Parent = elseBlock;
        }

        return true;
    }

    /// <summary>
    /// Converts consecutive conditional branches that all leave for the same label into an
    /// <c>if</c>/<c>else if</c> chain. Once the branch-local gotos are removed, execution
    /// naturally continues with the instruction after the chain.
    /// </summary>
    /// <param name="label">The common destination of the trailing branch gotos.</param>
    /// <returns><c>true</c> when at least one conditional chain was converted.</returns>
    private static bool TryConvertConsecutiveConditionalGotoBranches(ICodeBlock label)
    {
        if (label.Sources.Count < 3)
            return false;

        if (label.Parent is ICodeBlock labelParent
            && StartsWithKeyword(labelParent.Code.TrimStart(), "switch")
            && !HasMultipleConditionalGotoSources(label, labelParent))
        {
            return false;
        }

        var converted = false;
        var convertedInPass = false;
        do
        {
            convertedInPass = false;
            if (TryConvertIfGotoWithEquivalentElseStatement(label))
            {
                converted = true;
                convertedInPass = true;
            }

            var parents = new HashSet<ICodeBlock>();
            foreach (var sourceReference in label.Sources.ToArray())
            {
                if (!sourceReference.TryGetTarget(out var source))
                    continue;

                for (var current = source.Parent as ICodeBlock; current is not null; current = current.Parent as ICodeBlock)
                    parents.Add(current);
            }

            foreach (var parent in parents.OrderByDescending(GetDepth))
            {
                for (var index = 0; index < parent.SubBlocks.Count;)
                {
                    var branches = new List<ICodeBlock>();
                    for (var currentIndex = index; currentIndex < parent.SubBlocks.Count; currentIndex++)
                    {
                        var branch = parent.SubBlocks[currentIndex];
                        if (!IsIfStatement(branch)
                            || FindDirectTailingGoto(branch) is not ICodeBlock branchGoto
                            || !HasDestination(branchGoto, label))
                        {
                            break;
                        }

                        branches.Add(branch);
                    }

                    if (branches.Count == 0
                        || !TryFindFollowingGoto(branches[^1], label, out var finalGoto)
                        || finalGoto is null)
                    {
                        index++;
                        continue;
                    }

                    if (branches.Count == 1
                        && (label.Parent != parent
                            || !StartsWithKeyword(parent.Code.TrimStart(), "switch")))
                    {
                        index++;
                        continue;
                    }

                    if (finalGoto.Parent != parent
                        || (branches.Count == 1 && branches[0].Next == finalGoto))
                    {
                        index++;
                        continue;
                    }

                    var tail = GetTailBeforeGoto(branches[^1], finalGoto);
                    if (branches.Count == 1 && tail.Length == 0)
                    {
                        index++;
                        continue;
                    }

                    foreach (var branch in branches)
                    {
                        if (FindDirectTailingGoto(branch) is ICodeBlock branchGoto)
                            _ = branchGoto.Parent?.DeleteSubBlocks(branchGoto.Index, 1);
                    }

                    var finalElse = CreateElseBlock(parent, branches[^1]);
                    if (tail.Length > 1)
                    {
                        var elseBody = new CodeBlock()
                        {
                            Name = nameof(CodeBlockType.Block),
                            Type = CodeBlockType.Block,
                            Code = "{",
                            Parent = finalElse
                        };
                        foreach (var tailItem in tail)
                            tailItem.Parent = elseBody;
                        _ = new CodeBlock()
                        {
                            Name = "End",
                            Type = CodeBlockType.Block,
                            Code = "}",
                            Parent = finalElse
                        };
                    }
                    else
                    {
                        foreach (var tailItem in tail)
                            tailItem.Parent = finalElse;
                    }

                    for (var branchIndex = branches.Count - 1; branchIndex > 0; branchIndex--)
                    {
                        var precedingBranch = branches[branchIndex - 1];
                        var nestedBranch = branches[branchIndex];
                        var elseBlock = CreateElseBlock(parent, precedingBranch);
                        nestedBranch.Parent = elseBlock;
                    }

                    converted = true;
                    convertedInPass = true;
                    index = branches[0].Index + 1;
                }
            }
        }
        while (convertedInPass);

        return converted;
    }

    private static bool TryConvertIfGotoWithEquivalentElseStatement(ICodeBlock label)
    {
        if (label.Parent is not ICodeBlock switchBlock
            || !StartsWithKeyword(switchBlock.Code.TrimStart(), "switch")
            || !HasMultipleConditionalGotoSources(label, switchBlock))
        {
            return false;
        }

        foreach (var sourceReference in label.Sources.ToArray())
        {
            if (!sourceReference.TryGetTarget(out var source)
                || source.Type != CodeBlockType.Goto
                || FindPreviousExecutableSibling(source) is not ICodeBlock branchStatement
                || branchStatement.Type != CodeBlockType.Operation)
            {
                continue;
            }

            for (var branch = source.Parent as ICodeBlock; branch is not null; branch = branch.Parent as ICodeBlock)
            {
                if (!IsIfStatement(branch)
                    || FindDirectTailingGoto(branch) != source
                    || branch.Parent is not ICodeBlock parent
                    || branch.Next is ICodeBlock existingElse && IsElseStatement(existingElse)
                    || FindNextExecutableSibling(branch) is not ICodeBlock continuation
                    || continuation.Parent != parent
                    || continuation.Type != CodeBlockType.Operation
                    || continuation.Code != branchStatement.Code
                    || FindNextExecutableSibling(continuation) is not ICodeBlock continuationGoto
                    || continuationGoto.Type != CodeBlockType.Goto
                    || !HasDestination(continuationGoto, label))
                {
                    continue;
                }

                var elseBlock = CreateElseBlock(parent, branch);
                var elseBody = new CodeBlock()
                {
                    Name = nameof(CodeBlockType.Block),
                    Type = CodeBlockType.Block,
                    Code = "{",
                    Parent = elseBlock
                };
                continuation.Parent = elseBody;
                _ = new CodeBlock()
                {
                    Name = "End",
                    Type = CodeBlockType.Block,
                    Code = "}",
                    Parent = elseBlock
                };
                if (source.Parent is ICodeBlock sourceParent)
                    _ = sourceParent.DeleteSubBlocks(source.Index, 1);
                return true;
            }
        }

        return false;
    }

    private static bool HasMultipleConditionalGotoSources(ICodeBlock label, ICodeBlock scope)
    {
        var conditionalSources = 0;
        foreach (var sourceReference in label.Sources)
        {
            if (!sourceReference.TryGetTarget(out var source)
                || source.Type != CodeBlockType.Goto)
            {
                continue;
            }

            for (var current = source.Parent as ICodeBlock;
                 current is not null && current != scope;
                 current = current.Parent as ICodeBlock)
            {
                if (IsIfStatement(current))
                {
                    conditionalSources++;
                    break;
                }
            }
        }

        return conditionalSources >= 2;
    }

    private static ICodeBlock CreateElseBlock(ICodeBlock parent, ICodeBlock precedingBranch)
    {
        var elseBlock = new CodeBlock()
        {
            Name = nameof(CodeBlockType.Operation),
            Type = CodeBlockType.Operation,
            Code = "else",
            Parent = parent
        };
        parent.SubBlocks.Remove(elseBlock);
        parent.SubBlocks.Insert(precedingBranch.Index + 1, elseBlock);
        return elseBlock;
    }

    private static bool TryFindFollowingGoto(
        ICodeBlock finalBranch,
        ICodeBlock label,
        out ICodeBlock? finalGoto)
    {
        for (var scope = finalBranch; ;)
        {
            for (var current = scope.Next; current is not null; current = current.Next)
            {
                if (current.Type == CodeBlockType.Label
                    || (current.Type == CodeBlockType.Goto && !HasDestination(current, label)))
                {
                    finalGoto = null;
                    return false;
                }

                if (current.Type == CodeBlockType.Goto
                    && HasDestination(current, label))
                {
                    finalGoto = current;
                    return true;
                }
            }

            if (scope.Parent is not ICodeBlock parent || IsExecutionBoundary(parent))
                break;

            scope = parent;
        }

        finalGoto = null;
        return false;
    }

    private static ICodeBlock[] GetTailBeforeGoto(
        ICodeBlock finalBranch,
        ICodeBlock finalGoto)
    {
        var tail = new List<ICodeBlock>();
        for (var current = finalBranch.Next;
             current is not null && current != finalGoto;
             current = current.Next)
        {
            tail.Add(current);
        }

        return tail.ToArray();
    }

    private static ICodeBlock? FindDirectTailingGoto(ICodeBlock codeBlock)
    {
        for (var index = codeBlock.SubBlocks.Count - 1; index >= 0; index--)
        {
            var child = codeBlock.SubBlocks[index];
            if (IsCommentLike(child) || child.Type == CodeBlockType.Block)
                continue;

            return child.Type == CodeBlockType.Goto ? child : null;
        }

        return null;
    }

    private static int GetDepth(ICodeBlock block)
    {
        var depth = 0;
        for (var current = block.Parent as ICodeBlock; current is not null; current = current.Parent as ICodeBlock)
            depth++;

        return depth;
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
        return item.Type is CodeBlockType.Comment or CodeBlockType.LComment or CodeBlockType.FLComment;
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
        if (ReplaceFinalSwitchGotoWithBreak(item))
            return;

        var redundantGotoRemoved = false;
        while (RemoveGotoWhenNextInstructionTargetsSameLabel(item))
            redundantGotoRemoved = true;

        if (redundantGotoRemoved && item.Code != "IL_023d:")
            return;

        if (TryConvertGotoChain(item))
            return;

        if (TryConvertConsecutiveConditionalGotoBranches(item))
            return;

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

    /// <summary>
    /// Simplifies conservative two-condition if chains while retaining short-circuit evaluation order.
    /// </summary>
    /// <param name="root">The root block whose conditional statements are inspected.</param>
    public void OptimizeIfChains(ICodeBlock root)
    {
        foreach (var child in root.SubBlocks.ToArray())
            OptimizeIfChains(child);

        for (var index = 0; index < root.SubBlocks.Count;)
        {
            if (TryFlattenConditionalBlock(root.SubBlocks[index]))
            {
                index = 0;
                continue;
            }

            if (TryMergeNestedIfWithAnd(root.SubBlocks[index]))
                continue;

            if (TryMergeElseIfWithOr(root, index))
                continue;

            index++;
        }
    }

    private static bool TryFlattenConditionalBlock(ICodeBlock conditional)
    {
        if ((!IsIfStatement(conditional) && !IsElseStatement(conditional))
            || conditional.Parent is not ICodeBlock parent
            || conditional.SubBlocks.Count < 2
            || !IsOpeningBlock(conditional.SubBlocks[0])
            || !IsClosingBlock(conditional.SubBlocks[^1]))
        {
            return false;
        }

        var openingBlock = conditional.SubBlocks[0];
        var closingBlock = conditional.SubBlocks[^1];
        var bodyIsNested = openingBlock.SubBlocks.Count > 0;
        var body = bodyIsNested
            ? openingBlock.SubBlocks.ToArray()
            : conditional.SubBlocks.Skip(1).Take(conditional.SubBlocks.Count - 2).ToArray();
        if (body.Length == 0)
            return false;

        var bodyStatements = body.Where(child => !IsCommentLike(child)).ToArray();
        var isNestedBlock = IsEntireBlock(bodyStatements);
        if (!isNestedBlock
            && (bodyStatements.Length == 0
                || !IsIfStatement(bodyStatements[0])
                || bodyStatements.Skip(1).Any(statement => !IsElseStatement(statement))
                || body.Any(comment => IsCommentLike(comment) && comment.Type != CodeBlockType.LComment)))
        {
            return false;
        }

        var leadingLineComments = body.TakeWhile(child => child.Type == CodeBlockType.LComment).ToArray();
        foreach (var comment in Enumerable.Reverse(leadingLineComments))
        {
            comment.Parent = parent;
            _ = parent.SubBlocks.Remove(comment);
            parent.SubBlocks.Insert(conditional.Index, comment);
        }

        if (bodyIsNested)
        {
            closingBlock.Parent = null;
            openingBlock.Parent = null;
            var insertionIndex = 0;
            foreach (var child in body)
            {
                if (leadingLineComments.Contains(child))
                    continue;

                child.Parent = conditional;
                _ = conditional.SubBlocks.Remove(child);
                conditional.SubBlocks.Insert(insertionIndex++, child);
            }
        }
        else
        {
            closingBlock.Parent = null;
            openingBlock.Parent = null;
        }

        return true;
    }

    private static bool IsEntireBlock(IReadOnlyList<ICodeBlock> blocks)
    {
        if (blocks.Count < 2 || !IsOpeningBlock(blocks[0]) || !IsClosingBlock(blocks[^1]))
            return false;

        var depth = 0;
        for (var index = 0; index < blocks.Count; index++)
        {
            if (IsOpeningBlock(blocks[index]))
                depth++;
            else if (IsClosingBlock(blocks[index]))
                depth--;

            if (depth == 0 && index < blocks.Count - 1)
                return false;
        }

        return depth == 0;
    }

    private static bool IsOpeningBlock(ICodeBlock block)
    {
        return block.Type == CodeBlockType.Block && block.Code == "{";
    }

    private static bool IsClosingBlock(ICodeBlock block)
    {
        return block.Type == CodeBlockType.Block && block.Code == "}";
    }

    private static bool TryMergeNestedIfWithAnd(ICodeBlock outerIf)
    {
        if (!IsIfStatement(outerIf)
            || outerIf.Next is ICodeBlock outerElse && IsElseStatement(outerElse)
            || !TryGetCondition(outerIf.Code, out var outerCondition)
            || !IsSafeBooleanCondition(outerCondition, outerIf))
        {
            return false;
        }

        var outerStatements = GetBodyStatements(outerIf);
        if (outerStatements.Length != 1
            || !IsIfStatement(outerStatements[0]))
        {
            return false;
        }

        var innerIf = outerStatements[0];
        if (!TryGetCondition(innerIf.Code, out var innerCondition)
            || !IsSafeBooleanCondition(innerCondition, innerIf))
        {
            return false;
        }

        var innerStatements = GetBodyStatements(innerIf);
        var innerBodyBlocks = innerIf.SubBlocks.ToArray();
        if (innerStatements.Any(statement => ContainsUnsafeControlFlow(statement, allowGoto: true)))
            return false;

        var outerEnd = FindBlockEnd(outerIf);

        outerIf.Code = $"if ({CombineNestedAndConditions(outerCondition, innerCondition)})";
        if (outerEnd is not null)
            outerEnd.Parent = null;
        innerIf.Parent = null;
        foreach (var bodyBlock in innerBodyBlocks)
            bodyBlock.Parent = outerIf;
        if (outerEnd is not null)
            outerEnd.Parent = outerIf;
        _ = TryRemoveSingleOperationBlock(outerIf);

        return true;
    }

    private static string CombineNestedAndConditions(string outerCondition, string innerCondition)
    {
        var innerConditions = GetGeneratedAndOperands(innerCondition);
        var combinedCondition = $"({outerCondition.Trim()}) && ({innerConditions[0]})";
        for (var index = 1; index < innerConditions.Count; index++)
            combinedCondition = $"({combinedCondition}) && ({innerConditions[index]})";

        return combinedCondition;
    }

    private static IReadOnlyList<string> GetGeneratedAndOperands(string condition)
    {
        condition = StripOuterParentheses(condition.Trim());
        if (!TrySplitTopLevelLogicalOperator(condition, out var operatorIndex, out var operatorLength)
            || condition[operatorIndex] != '&'
            || !IsWrappedInParentheses(condition[..operatorIndex])
            || !IsWrappedInParentheses(condition[(operatorIndex + operatorLength)..]))
        {
            return new[] { condition };
        }

        return GetGeneratedAndOperands(condition[..operatorIndex])
            .Concat(GetGeneratedAndOperands(condition[(operatorIndex + operatorLength)..]))
            .ToArray();
    }

    private static bool IsWrappedInParentheses(string condition)
    {
        var trimmedCondition = condition.Trim();
        return StripOuterParentheses(trimmedCondition) != trimmedCondition;
    }

    private static bool TryRemoveSingleOperationBlock(ICodeBlock conditional)
    {
        if (conditional.SubBlocks.Count < 3
            || !IsOpeningBlock(conditional.SubBlocks[0])
            || !IsClosingBlock(conditional.SubBlocks[^1]))
        {
            return false;
        }

        var body = conditional.SubBlocks.Skip(1).Take(conditional.SubBlocks.Count - 2).ToArray();
        if (body.Length != 1
            || body[0].Type != CodeBlockType.Operation
            || !IsSafeEquivalentBodyStatement(body[0]))
        {
            return false;
        }

        conditional.SubBlocks[^1].Parent = null;
        conditional.SubBlocks[0].Parent = null;
        return true;
    }

    private static bool TryMergeElseIfWithOr(ICodeBlock parent, int index)
    {
        if (index + 2 >= parent.SubBlocks.Count)
            return false;

        var firstIf = parent.SubBlocks[index];
        var elseBlock = parent.SubBlocks[index + 1];
        var secondIf = parent.SubBlocks[index + 2];
        if (!IsIfStatement(firstIf)
            || index > 0 && IsElseStatement(parent.SubBlocks[index - 1])
            || !IsElseStatement(elseBlock)
            || elseBlock.SubBlocks.Any(child => !IsCommentLike(child))
            || !IsIfStatement(secondIf)
            || !TryGetCondition(firstIf.Code, out var firstCondition)
            || !TryGetCondition(secondIf.Code, out var secondCondition)
            || !IsSafeBooleanCondition(firstCondition, firstIf)
            || !IsSafeBooleanCondition(secondCondition, secondIf)
            || !HaveEquivalentBodies(firstIf, secondIf))
        {
            return false;
        }

        MergeEquivalentBlocks(firstIf, elseBlock);
        MergeEquivalentBlocks(firstIf, secondIf);
        firstIf.Code = $"if (({firstCondition}) || ({secondCondition}))";
        secondIf.Parent = null;
        elseBlock.Parent = null;
        return true;
    }

    private static bool HaveEquivalentBodies(ICodeBlock first, ICodeBlock second)
    {
        var firstStatements = GetBodyStatements(first);
        var secondStatements = GetBodyStatements(second);
        if (firstStatements.Length == 0 || firstStatements.Length != secondStatements.Length)
            return false;

        for (var index = 0; index < firstStatements.Length; index++)
        {
            if (ContainsUnsafeControlFlow(firstStatements[index])
                || !IsSafeEquivalentBodyStatement(firstStatements[index])
                || !AreEquivalentBlocks(firstStatements[index], secondStatements[index]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsSafeEquivalentBodyStatement(ICodeBlock block)
    {
        if (block.Type != CodeBlockType.Operation)
            return false;

        var statement = block.Code.Trim();
        return (statement.EndsWith(");", StringComparison.Ordinal)
                && statement.Contains('(')
                && !statement.Contains(" = ", StringComparison.Ordinal))
            || System.Text.RegularExpressions.Regex.IsMatch(
                statement,
                @"^[A-Za-z_]\w*(?:\s*(?:\.\s*[A-Za-z_]\w*|\s*\[[^\]\r\n]+\]))*\s*=\s*.+;$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant);
    }

    private static bool AreEquivalentBlocks(ICodeBlock first, ICodeBlock second)
    {
        if (first.Type != second.Type || first.Code != second.Code)
            return false;

        var firstChildren = first.SubBlocks
            .Where(child => !IsBlockDelimiter(child) && !IsCommentLike(child))
            .ToArray();
        var secondChildren = second.SubBlocks
            .Where(child => !IsBlockDelimiter(child) && !IsCommentLike(child))
            .ToArray();
        if (firstChildren.Length != secondChildren.Length)
            return false;

        for (var index = 0; index < firstChildren.Length; index++)
        {
            if (!AreEquivalentBlocks(firstChildren[index], secondChildren[index]))
                return false;
        }

        return true;
    }

    private static ICodeBlock[] GetBodyStatements(ICodeBlock block)
    {
        return block.SubBlocks
            .Where(child => !IsBlockDelimiter(child) && !IsCommentLike(child))
            .ToArray();
    }

    private static void MergeEquivalentBlocks(ICodeBlock target, ICodeBlock source)
    {
        var targetStatements = GetBodyStatements(target);
        var statementIndex = 0;

        foreach (var sourceChild in source.SubBlocks.ToArray())
        {
            if (IsBlockDelimiter(sourceChild))
                continue;

            if (IsCommentLike(sourceChild))
            {
                var insertionIndex = statementIndex < targetStatements.Length
                    ? targetStatements[statementIndex].Index
                    : FindBlockEnd(target)?.Index ?? target.SubBlocks.Count;
                sourceChild.Parent = target;
                _ = target.SubBlocks.Remove(sourceChild);
                target.SubBlocks.Insert(insertionIndex, sourceChild);
                continue;
            }

            MergeEquivalentBlocks(targetStatements[statementIndex], sourceChild);
            statementIndex++;
        }
    }

    private static ICodeBlock? FindBlockEnd(ICodeBlock block)
    {
        return block.SubBlocks.LastOrDefault(child => child.Type == CodeBlockType.Block && child.Code == "}");
    }

    private static bool IsBlockDelimiter(ICodeBlock block)
    {
        return block.Type == CodeBlockType.Block && block.Code is "{" or "}";
    }

    private static bool TryGetCondition(string code, out string condition)
    {
        condition = string.Empty;
        var text = code.Trim();
        if (text.Length < 5)
        {
            return false;
        }

        var openIndex = text.IndexOf('(');
        if (openIndex < 0 || text[^1] != ')')
            return false;

        condition = text[(openIndex + 1)..^1].Trim();
        return condition.Length > 0;
    }

    private static bool IsSafeBooleanCondition(string condition, ICodeBlock conditional)
    {
        condition = StripOuterParentheses(condition.Trim());
        if (condition.Length == 0)
            return false;

        if (TrySplitTopLevelLogicalOperator(condition, out var operatorIndex, out var operatorLength))
        {
            return IsSafeBooleanCondition(condition[..operatorIndex], conditional)
                && IsSafeBooleanCondition(condition[(operatorIndex + operatorLength)..], conditional);
        }

        if (condition is "true" or "false")
            return true;

        if (IsIdentifierCondition(condition))
        {
            var parameterPattern = @"\bbool\s+" + System.Text.RegularExpressions.Regex.Escape(condition) + @"\b";
            for (var current = conditional; current is not null; current = current.Parent as ICodeBlock)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(
                    current.Code,
                    parameterPattern,
                    System.Text.RegularExpressions.RegexOptions.CultureInvariant))
                {
                    return true;
                }
            }

            return false;
        }

        return ContainsTopLevelEquality(condition);
    }

    private static string StripOuterParentheses(string condition)
    {
        while (condition.Length >= 2 && condition[0] == '(' && condition[^1] == ')')
        {
            var depth = 0;
            var wrapsEntireCondition = true;
            for (var index = 0; index < condition.Length; index++)
            {
                if (condition[index] == '(')
                    depth++;
                else if (condition[index] == ')')
                    depth--;

                if (depth == 0 && index < condition.Length - 1)
                {
                    wrapsEntireCondition = false;
                    break;
                }
            }

            if (!wrapsEntireCondition || depth != 0)
                break;

            condition = condition[1..^1].Trim();
        }

        return condition;
    }

    private static bool TrySplitTopLevelLogicalOperator(string condition, out int operatorIndex, out int operatorLength)
    {
        var depth = 0;
        for (var index = 0; index < condition.Length - 1; index++)
        {
            if (condition[index] == '(')
            {
                depth++;
                continue;
            }

            if (condition[index] == ')')
            {
                depth--;
                continue;
            }

            if (depth == 0
                && condition[index] is '&' or '|'
                && condition[index] == condition[index + 1])
            {
                operatorIndex = index;
                operatorLength = 2;
                return true;
            }
        }

        operatorIndex = -1;
        operatorLength = 0;
        return false;
    }

    private static bool IsIdentifierCondition(string condition)
    {
        if (!(char.IsLetter(condition[0]) || condition[0] == '_'))
            return false;

        for (var index = 1; index < condition.Length; index++)
        {
            if (!char.IsLetterOrDigit(condition[index]) && condition[index] != '_')
                return false;
        }

        return true;
    }

    private static bool ContainsTopLevelEquality(string condition)
    {
        var depth = 0;
        for (var index = 0; index < condition.Length; index++)
        {
            var character = condition[index];
            if (character == '(')
            {
                depth++;
                continue;
            }

            if (character == ')')
            {
                depth--;
                continue;
            }

            if (depth != 0)
                continue;

            if (index + 1 >= condition.Length)
                continue;

            var nextCharacter = condition[index + 1];
            if ((character == '=' && nextCharacter == '=')
                || (character == '!' && nextCharacter == '='))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsUnsafeControlFlow(ICodeBlock block, bool allowGoto = false)
    {
        if (block.Type == CodeBlockType.Label
            || !allowGoto && block.Type == CodeBlockType.Goto)
            return true;

        var code = block.Code.TrimStart();
        if (!allowGoto && StartsWithKeyword(code, "goto")
            || StartsWithKeyword(code, "if")
            || StartsWithKeyword(code, "else")
            || StartsWithKeyword(code, "switch")
            || StartsWithKeyword(code, "while")
            || StartsWithKeyword(code, "for")
            || StartsWithKeyword(code, "do")
            || StartsWithStatementKeyword(code, "return")
            || StartsWithKeyword(code, "throw")
            || StartsWithStatementKeyword(code, "break")
            || StartsWithStatementKeyword(code, "continue"))
        {
            return true;
        }

        return block.SubBlocks.Any(child => ContainsUnsafeControlFlow(child, allowGoto));
    }

    private static bool StartsWithStatementKeyword(string code, string keyword)
    {
        return StartsWithKeyword(code, keyword)
            || code.StartsWith(keyword + ";", StringComparison.Ordinal);
    }

    private void TestForEasyWhileLoops(ICodeBlock item)
    {
        if (_noWhile)
            return;
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
        if (var2.Length == 0)
            return false;
        if (!char.IsLetter(var2[0]) && var2[0] != '_')
            return false;
        foreach (char c in var2)
            if (!IsIdentifierChar(c))
                return false;
        return true;
    }

    private static void RemoveResumeNextCode(ICodeBlock item)
    {
        if (item.Sources.Count != 2)
            return;
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
                    if (deleteIndex >= 0 
                        && deleteIndex + l <= parent.SubBlocks.Count 
                        && parent.SubBlocks[deleteIndex].Code != "default:")
                        _ = parent.DeleteSubBlocks(deleteIndex, l);
                    break;
                }
    }   

}
