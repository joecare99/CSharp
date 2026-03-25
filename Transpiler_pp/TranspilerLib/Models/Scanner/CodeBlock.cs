// ***********************************************************************
// Assembly         : TranspilerLib
// Author           : Mir
// Created          : 09-29-2023
//
// Last Modified By : Mir
// Last Modified On : 09-29-2023
// ***********************************************************************
// <copyright file="CodeBlock.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using BaseLib.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TranspilerLib.Data;
using TranspilerLib.Interfaces.Code;

namespace TranspilerLib.Models.Scanner;


/// <summary>
/// Represents a hierarchical unit of parsed source code.
/// A <c>CodeBlock</c> can contain nested sub blocks forming a tree structure
/// (e.g. namespaces, classes, methods, statements, labels, etc.).
/// It also supports weak references to control-flow destinations and sources
/// (e.g. for Goto/Label relationships) without preventing garbage collection.
/// </summary>
/// <remarks>
/// Main responsibilities:
/// 1. Maintain parent/child relationships among code blocks.
/// 2. Track positional metadata (<see cref="SourcePos"/>) and structural metadata (<see cref="Level"/>, <see cref="Index"/>).
/// 3. Represent semantic type information via <see cref="Type"/>.
/// 4. Manage directed weak links for flow navigation (<see cref="Destination"/> and <see cref="Sources"/>).
/// 5. Provide helper operations to move or delete sub blocks while keeping references consistent.
/// 6. Support serialization of destination/source references through index paths (<see cref="DestinationIndex"/>, <see cref="SourcesIndex"/>).
/// </remarks>
public class CodeBlock : ICodeBlock
{
    #region Properties

    /// <summary>
    /// Backing field for <see cref="Parent"/>.
    /// </summary>
    private ICodeBlock? _parent = null;

    /// <summary>
    /// Gets the depth level of this block within the root code tree.
    /// Root level is 0; each nested level increments by 1.
    /// </summary>
    /// <value>Zero-based depth level.</value>
    public int Level => Parent?.Level + 1 ?? 0;

    /// <summary>
    /// Gets the zero-based index of this block among its siblings in <see cref="Parent"/>'s <see cref="SubBlocks"/> list.
    /// Returns 0 if this block has no parent.
    /// </summary>
    /// <value>The sibling index or 0 if no parent.</value>
    public int Index => Parent?.SubBlocks.IndexOf(this) ?? 0;

    /// <summary>
    /// Gets or sets the logical name of this code block (e.g. identifier, label, method name).
    /// May be empty if not applicable for the <see cref="Type"/>.
    /// </summary>
    /// <value>Human readable or identifier name.</value>
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the raw code text fragment represented by this block.
    /// This is typically the original token(s) or reconstructed code for output.
    /// </summary>
    /// <value>Source code fragment.</value>
    public string Code { get; set; } = "";

    /// <summary>
    /// Gets or sets the semantic classification for this block.
    /// Used to distinguish structural, declarative, and operational elements.
    /// </summary>
    /// <value>A value of <see cref="CodeBlockType"/>.</value>
    public CodeBlockType Type { get; set; }

    /// <summary>
    /// Gets the list of child code blocks nested inside this block.
    /// Children maintain their own parent reference automatically.
    /// </summary>
    /// <value>Mutable list of sub blocks.</value>
    public IList<ICodeBlock> SubBlocks { get; init; }

    /// <summary>
    /// Gets the next sibling block under the same parent, if any.
    /// </summary>
    /// <value>Next sibling or <c>null</c>.</value>
    public ICodeBlock? Next => Parent is ICodeBlock pcb && pcb.SubBlocks.Count > Index + 1 ? pcb.SubBlocks[Index + 1] : null;

    /// <summary>
    /// Gets the previous sibling block under the same parent, if any.
    /// </summary>
    /// <value>Previous sibling or <c>null</c>.</value>
    public ICodeBlock? Prev => Parent is ICodeBlock pcb && 0 < Index ? pcb.SubBlocks[Index - 1] : null;

    /// <summary>
    /// Gets or sets the weak reference to a destination block (e.g. target of a Goto or jump).
    /// This forms a single forward link; the reciprocal sources are recorded in <see cref="Sources"/>.
    /// </summary>
    /// <remarks>
    /// Weak reference is used to avoid ownership cycles and permit GC if blocks become detached.
    /// </remarks>
    /// <value>Weak reference to destination or <c>null</c>.</value>
    [IgnoreDataMember]
    public virtual WeakReference<ICodeBlock>? Destination { get; set; }

    /// <summary>
    /// Gets or sets a serialized index-path representation of <see cref="Destination"/>.
    /// The path is a sequence of indexes from root to the referenced block.
    /// Setting this reconstructs the weak destination link and updates the target's <see cref="Sources"/>.
    /// </summary>
    /// <value>Index path list; empty if no destination.</value>
    [DataMember(EmitDefaultValue = false)]
    public List<int> DestinationIndex { get => GetItemIdx(Destination); set => Destination = SetDestByIdx(this, value); }

    /// <summary>
    /// Gets the collection of weak references to blocks that point to this block as their <see cref="Destination"/>.
    /// </summary>
    /// <remarks>
    /// Each entry is a weak reference; entries may become invalid if the source is GC collected.
    /// </remarks>
    /// <value>List of weak references to source blocks.</value>
    [IgnoreDataMember]
    public virtual IList<WeakReference<ICodeBlock>> Sources { get; private set; } = new List<WeakReference<ICodeBlock>>();

    /// <summary>
    /// Gets or sets the index-paths for each source referring to this block.
    /// Setting reconstructs the <see cref="Sources"/> list and updates each source's <see cref="Destination"/>.
    /// </summary>
    /// <value>Collection of index paths; each path identifies a source block.</value>
    [DataMember(EmitDefaultValue = false)]
    public List<List<int>> SourcesIndex
    {
        get => Sources.Select((s) => GetItemIdx(s)).ToList();
        set => Sources = value.Select((s) => SetSourceByIdx(this, s)).Where((s) => s != null).Select(s=>s!).ToList();
    }

    /// <summary>
    /// Gets or sets the parent block in the hierarchical tree.
    /// Setting handles removal from the previous parent and insertion into the new parent's <see cref="SubBlocks"/>.
    /// Passing <c>null</c> detaches the block from the tree.
    /// </summary>
    /// <remarks>
    /// Insertion happens at the end of the new parent's children list.
    /// </remarks>
    /// <value>Parent block or <c>null</c>.</value>
    [IgnoreDataMember]
    public virtual ICodeBlock? Parent
    {
        get => _parent;
        set
        {
            if (_parent != value)
            {
                _ = (_parent?.SubBlocks.Remove(this));
                _parent = value;
                _parent?.SubBlocks.Add(this);
            }
        }
    }

    /// <summary>
    /// Gets the zero-based character position in the original source text
    /// where parsing produced this block.
    /// Useful for diagnostics, mapping, and error reporting.
    /// </summary>
    /// <value>Character index into original source.</value>
    public int SourcePos { get; init; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeBlock"/> class.
    /// Creates an empty <see cref="SubBlocks"/> list bound to this instance as parent.
    /// </summary>
    public CodeBlock()
    {
        SubBlocks = new ParentedItemsList<ICodeBlock>(this);
    }

    /// <summary>
    /// Creates a destination weak reference from an index path and registers
    /// this block as a source of the destination.
    /// </summary>
    /// <param name="item">Current block initiating the link.</param>
    /// <param name="value">Index path to target block (root-first order).</param>
    /// <returns>Weak reference to destination or <c>null</c> if invalid path.</returns>
    /// <remarks>
    /// Adds a reciprocal source entry in the destination's <see cref="Sources"/>.
    /// </remarks>
    private static WeakReference<ICodeBlock>? SetDestByIdx(ICodeBlock item, List<int> value)
    {
        WeakReference<ICodeBlock>? result = null;
        ICodeBlock? dst = GetItemByIndex(item, value);
        if (dst != null)
        {
            result = new(dst);
            dst.Sources.Add(new(item));
        }
        return result;
    }

    /// <summary>
    /// Creates a source weak reference from an index path and sets the source's
    /// destination to this block.
    /// </summary>
    /// <param name="item">Current destination block.</param>
    /// <param name="value">Index path to source block.</param>
    /// <returns>Weak reference to source or <c>null</c> if invalid path.</returns>
    /// <remarks>
    /// Updates the source block's <see cref="Destination"/> property to point to <paramref name="item"/>.
    /// </remarks>
    private static WeakReference<ICodeBlock>? SetSourceByIdx(ICodeBlock item, List<int> value)
    {
        WeakReference<ICodeBlock>? result = null;
        ICodeBlock? src = GetItemByIndex(item, value);
        if (src != null)
        {
            result = new(src);
            src.Destination = new(item);
        }
        return result;
    }

    /// <summary>
    /// Resolves an index path into a block starting from the root ancestor of <paramref name="item"/>.
    /// </summary>
    /// <param name="item">Reference block from which the root is derived.</param>
    /// <param name="value">Sequence of child indexes from root to desired block.</param>
    /// <returns>The resolved block or <c>null</c> if any index is invalid.</returns>
    private static ICodeBlock? GetItemByIndex(ICodeBlock item, List<int> value)
    {
        ICodeBlock root = GetRoot(item);
        ICodeBlock? item2 = null;
        foreach (var v in value)
        {
            if (item2 == null)
                item2 = root;
            else if (item2 != null && v >= item2.SubBlocks.Count || v < 0)
            { item2 = null; break; }
            else
                item2 = item2!.SubBlocks[v];
        }
        return item2;
    }

    /// <summary>
    /// Traverses upward from a given block to obtain the root ancestor.
    /// </summary>
    /// <param name="item">Starting block.</param>
    /// <returns>Root block (topmost with no parent).</returns>
    private static ICodeBlock GetRoot(ICodeBlock item)
    {
        var root = item;
        while (root.Parent is ICodeBlock pcb)
            root = pcb;
        return root;
    }

    /// <summary>
    /// Converts a weak block reference into an index path suitable for serialization.
    /// </summary>
    /// <param name="Dst">Weak reference to a target block.</param>
    /// <returns>Index path from root to target or empty list if null/unresolved.</returns>
    private static List<int> GetItemIdx(WeakReference<ICodeBlock>? Dst)
    {
        List<int> result = new();
        if (Dst?.TryGetTarget(out var target) ?? false)
        {
            result.Add(target.Index);
            while (target.Parent is ICodeBlock pcb)
            {
                result.Insert(0, pcb.Index);
                target = pcb;
            }
        }
        return result;
    }

    /// <summary>
    /// Returns a string representation including metadata and recursively nested blocks.
    /// Intended for debugging; not guaranteed to be valid recompilable source.
    /// </summary>
    /// <returns>Multi-line string representation.</returns>
    public override string ToString()
    {
        return $"///{Name} {Type} {Level},{Index}{(Destination != null ? " Dest:OK" : "")}{(Sources.Count > 0 ? $" {Sources.Count}" : "")}\r\n{Code}{(SubBlocks.Count > 0 ? Environment.NewLine : string.Empty)}{string.Join(Environment.NewLine, SubBlocks)}";
    }

    /// <summary>
    /// Renders this block and its descendants into a formatted code string.
    /// </summary>
    /// <param name="indent">Current indentation level (spaces). First nested blocks increase indentation.</param>
    /// <returns>Formatted code snippet (not necessarily identical to original source).</returns>
    /// <remarks>
    /// Adds an inline comment annotation for label blocks receiving more than two incoming sources.
    /// </remarks>
    public virtual string ToCode(int indent = 4)
    {
        string codeComment = string.Empty;
        if (Type is CodeBlockType.Label && Sources.Count > 2)
            codeComment = $" // <========== {Sources.Count}";
        return $"{new string(' ', Type is CodeBlockType.Block or CodeBlockType.Label ? indent - 4 : indent)}{Code}{codeComment}{(SubBlocks.Count > 0 ? Environment.NewLine : string.Empty)}{string.Join(Environment.NewLine, SubBlocks.Select((c) => c.ToCode(indent + 4)))}";
    }

    /// <summary>
    /// Moves a contiguous range of child blocks within this block's <see cref="SubBlocks"/> list.
    /// </summary>
    /// <param name="iSrc">Index of the first block to move.</param>
    /// <param name="iDst">Destination index for insertion (after move).</param>
    /// <param name="cnt">Number of blocks to move.</param>
    /// <returns><c>true</c> if move succeeded; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// Rejects invalid indexes or no-op moves. Adjusts ordering while preserving parent references.
    /// </remarks>
    public bool MoveSubBlocks(int iSrc, int iDst, int cnt)
    {
        if (iSrc == iDst || iSrc < 0 || iSrc + cnt >= SubBlocks.Count || iDst < 0 || iDst >= SubBlocks.Count)
            return false;

        for (int i = 0; i < cnt; i++)
            if (iDst > iSrc)
            {
                SubBlocks.MoveItem(iSrc, iDst);
            }
            else
            {
                SubBlocks.MoveItem(iSrc++, iDst++);
            }
        return true;
    }

    /// <summary>
    /// Moves a contiguous range of child blocks possibly into another block's parent list.
    /// </summary>
    /// <param name="iSrc">Index of first block to move from this block.</param>
    /// <param name="cDst">Destination block whose parent list will receive moved blocks.</param>
    /// <param name="cnt">Number of blocks to move.</param>
    /// <returns><c>true</c> if move succeeded; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// If <paramref name="cDst"/> shares the same parent, delegates to the indexed overload.
    /// Otherwise reassigns each moved block's parent and reorders within the destination parent's list.
    /// </remarks>
    public bool MoveSubBlocks(int iSrc, ICodeBlock cDst, int cnt)
    {
        if (cDst == null
            || (cDst.Parent == this && iSrc == cDst.Index)
            || iSrc < 0
            || iSrc + cnt >= SubBlocks.Count
            || cDst.Parent == null
            )
            return false;
        if (cDst.Parent == this)
            return MoveSubBlocks(iSrc, cDst.Index, cnt);
        for (int i = 0; i < cnt; i++)
        {
            var c = SubBlocks[iSrc];
            c.Parent = cDst.Parent;
            cDst.Parent.SubBlocks.MoveItem(c.Index, cDst.Index);
        }
        return true;
    }

    /// <summary>
    /// Deletes a contiguous range of sub blocks, cleaning up destination/source link references.
    /// </summary>
    /// <param name="iSrc">Index of first sub block to delete.</param>
    /// <param name="cnt">Number of blocks to delete.</param>
    /// <returns><c>true</c> if deletion succeeded; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// For each removed block:
    /// 1. Detaches from parent.
    /// 2. Removes reciprocal source entries from its destination (if any).
    /// 3. Clears destination links from its sources.
    /// Does not recursively delete grandchildren beyond detaching chain references.
    /// </remarks>
    public bool DeleteSubBlocks(int iSrc, int cnt)
    {
        if (iSrc < 0 || iSrc + cnt >= SubBlocks.Count)
            return false;

        ICodeBlock c;
        for (int i = 0; i < cnt; i++)
        {
            (c = SubBlocks[iSrc]).Parent = null;
            if (c.Destination != null && c.Destination.TryGetTarget(out var target))
                foreach (var item in target!.Sources)
                    if (item.TryGetTarget(out var source))
                        if (source == c)
                        {
                            _ = target.Sources.Remove(item);
                            break;
                        }
            if (c.Sources.Count > 0)
                foreach (var item in c.Sources)
                    if (item.TryGetTarget(out var source))
                        source.Destination = null;
        }
        return true;
    }

    /// <summary>
    /// Moves this block to become a direct child of <paramref name="cDst"/>.
    /// </summary>
    /// <param name="cDst">Destination parent block.</param>
    /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// Fails if:
    /// - <paramref name="cDst"/> is <c>null</c>
    /// - This block has no current parent
    /// - Destination is a descendant making cyclic structure
    /// - Destination equals current parent (no-op)
    /// </remarks>
    public bool MoveToSub(ICodeBlock cDst)
    {
        if (cDst == null || Parent == null || cDst.Parent == this || Parent == cDst)
            return false;

        Parent = cDst;
        return true;
    }

    /// <summary>
    /// Determines whether this instance and another represent the same object reference.
    /// </summary>
    /// <param name="other">Other block to compare.</param>
    /// <returns><c>true</c> if same reference; otherwise <c>false</c>.</returns>
    public bool Equals(ICodeBlock? other)
    {
        return this == other;
    }
}

