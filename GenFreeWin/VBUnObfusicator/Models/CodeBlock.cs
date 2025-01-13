// ***********************************************************************
// Assembly         : VBUnObfusicator
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
using static VBUnObfusicator.Interfaces.Code.ICSCode;

/// <summary>
/// The Models namespace.
/// </summary>
namespace VBUnObfusicator.Models
{
    /// <summary>
    /// Class CSCode.
    /// Implements the <see cref="Interfaces.Code.ICSCode" />
    /// </summary>
    /// <seealso cref="Interfaces.Code.ICSCode" />
    public partial class CSCode
    {
        /// <summary>
        /// Class CodeBlock.
        /// Implements the <see cref="ICodeBlock" />
        /// </summary>
        /// <seealso cref="ICodeBlock" />
        public class CodeBlock : ICodeBlock
        {
            /// <summary>
            /// The parent
            /// </summary>
            private ICodeBlock? _parent = null;
            /// <summary>
            /// Gets the level.
            /// </summary>
            /// <value>The level.</value>
            public int Level => Parent?.Level + 1 ?? 0;
            /// <summary>
            /// Gets the index.
            /// </summary>
            /// <value>The index.</value>
            public int Index => Parent?.SubBlocks.IndexOf(this) ?? 0;
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; } = "";
            /// <summary>
            /// Gets or sets the code.
            /// </summary>
            /// <value>The code.</value>
            public string Code { get; set; } = "";
            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>The type.</value>
            public CodeBlockType Type { get; set; }
            /// <summary>
            /// Gets the sub blocks.
            /// </summary>
            /// <value>The sub blocks.</value>
            public IList<ICodeBlock> SubBlocks { get; init; }
            /// <summary>
            /// Gets the next.
            /// </summary>
            /// <value>The next.</value>
            public ICodeBlock? Next => Parent is ICodeBlock pcb && pcb.SubBlocks.Count > Index + 1 ? pcb.SubBlocks[Index + 1] : null;
            /// <summary>
            /// Gets the previous.
            /// </summary>
            /// <value>The previous.</value>
            public ICodeBlock? Prev => Parent is ICodeBlock pcb && 0 < Index ? pcb.SubBlocks[Index - 1] : null;

            /// <summary>
            /// Initializes a new instance of the <see cref="CodeBlock"/> class.
            /// </summary>
            public CodeBlock()
            {
                SubBlocks = new ParentedItemsList<ICodeBlock>(this);
            }

            /// <summary>
            /// Gets or sets the destination.
            /// </summary>
            /// <value>The destination.</value>
            [IgnoreDataMember]
            public virtual WeakReference<ICodeBlock>? Destination { get; set; }

            /// <summary>
            /// Gets or sets the index of the destination.
            /// </summary>
            /// <value>The index of the destination.</value>
            [DataMember(EmitDefaultValue = false)]
            public List<int> DestinationIndex { get => GetItemIdx(Destination); set => Destination = SetDestByIdx(this, value); }

            /// <summary>
            /// Sets the index of the DST.
            /// </summary>
            /// <param name="item">The item.</param>
            /// <param name="value">The value.</param>
            /// <returns>System.WeakReference&lt;VBUnObfusicator.Models.ICSCode.ICodeBlock&gt;?.</returns>
            static private WeakReference<ICodeBlock>? SetDestByIdx(ICodeBlock item, List<int> value)
            {
                // Initialize a new WeakReference to the destination
                WeakReference<ICodeBlock>? result = null;
                // Get the root of the item
                ICodeBlock? dst = GetItemByIndex(item, value);
                if (dst != null)
                {
                    result = new(dst);
                    dst.Sources.Add(new(item));
                }
                return result;
            }

            /// <summary>
            /// Sets the index of the DST.
            /// </summary>
            /// <param name="item">The item.</param>
            /// <param name="value">The value.</param>
            /// <returns>System.WeakReference&lt;VBUnObfusicator.Models.ICSCode.ICodeBlock&gt;?.</returns>
            static private WeakReference<ICodeBlock>? SetSourceByIdx(ICodeBlock item, List<int> value)
            {
                // Initialize a new WeakReference to the destination
                WeakReference<ICodeBlock>? result = null;
                // Get the root of the item
                ICodeBlock? src = GetItemByIndex(item, value);
                if (src != null)
                {
                    result = new(src);
                    src.Destination = new(item);
                }
                return result;
            }

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
                        item2 = item2.SubBlocks[v];
                }
                return item2;
            }

            private static ICodeBlock GetRoot(ICodeBlock item)
            {
                var root = item;
                while (root.Parent is ICodeBlock pcb)
                    root = pcb;
                return root;
            }

            /// <summary>
            /// Gets the index of the DST.
            /// </summary>
            /// <param name="Dst">The DST.</param>
            /// <returns>System.Collections.Generic.List&lt;int&gt;.</returns>
            static private List<int> GetItemIdx(WeakReference<ICodeBlock>? Dst)
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
            /// Gets the sources.
            /// </summary>
            /// <value>The sources.</value>
            [IgnoreDataMember]
            public virtual List<WeakReference<ICodeBlock>> Sources { get; private set; } = new();

            [DataMember(EmitDefaultValue = false)]
            public List<List<int>> SourcesIndex 
            { 
                get => Sources.Select((s) => GetItemIdx(s)).ToList();
                set => Sources = value.Select((s) => SetSourceByIdx(this, s)).Where((s)=>s != null).ToList(); 
            }
            /// <summary>
            /// Gets or sets the parent.
            /// </summary>
            /// <value>The parent.</value>
            [IgnoreDataMember]
            public virtual ICodeBlock? Parent
            {
                get => _parent; set
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
            /// Converts to string.
            /// </summary>
            /// <returns>string.</returns>
            public override string ToString()
            {
                return $"///{Name} {Type} {Level},{Index}{(Destination != null ? " Dest:OK" : "")}{(Sources.Count > 0 ? $" {Sources.Count}" : "")}\r\n{Code}{(SubBlocks.Count > 0 ? Environment.NewLine : string.Empty)}{string.Join(Environment.NewLine, SubBlocks)}";
            }

            /// <summary>
            /// Converts to code.
            /// </summary>
            /// <param name="indent">The indent.</param>
            /// <returns>string.</returns>
            public string ToCode(int indent = 4)
            {
                string codeComment = string.Empty;
                if (Type is CodeBlockType.Label && Sources.Count > 2)
                    codeComment = $" // <========== {Sources.Count}";
                return $"{new string(' ', Type is CodeBlockType.Block or CodeBlockType.Label ? indent - 4 : indent)}{Code}{codeComment}{(SubBlocks.Count > 0 ? Environment.NewLine : string.Empty)}{string.Join(Environment.NewLine, SubBlocks.Select((c) => c.ToCode(indent + 4)))}";
            }
            /// <summary>
            /// Moves the sub blocks.
            /// </summary>
            /// <param name="iSrc">The i source.</param>
            /// <param name="iDst">The i DST.</param>
            /// <param name="cnt">The count.</param>
            /// <returns>bool.</returns>
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
            /// Moves the sub blocks.
            /// </summary>
            /// <param name="iSrc">The i source.</param>
            /// <param name="cDst">The c DST.</param>
            /// <param name="cnt">The count.</param>
            /// <returns>bool.</returns>
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
            /// Deletes the sub blocks.
            /// </summary>
            /// <param name="iSrc">The i source.</param>
            /// <param name="cnt">The count.</param>
            /// <returns>bool.</returns>
            public bool DeleteSubBlocks(int iSrc, int cnt)
            {
                if (iSrc < 0 || iSrc + cnt >= SubBlocks.Count)
                    return false;

                ICodeBlock c;
                for (int i = 0; i < cnt; i++)
                {
                    (c = SubBlocks[iSrc]).Parent = null;
                    if (c.Destination != null && c.Destination.TryGetTarget(out var target))
                        foreach (var item in target.Sources)
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
            /// Equalses the specified other.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns>bool.</returns>
            public bool Equals(ICodeBlock other)
            {
                return this == other;
            }
        }
    }
}
