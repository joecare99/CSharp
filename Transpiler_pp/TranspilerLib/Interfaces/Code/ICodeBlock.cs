// ***********************************************************************
// Assembly         : TranspilerLib
// Author           : Mir
// Created          : 09-26-2023
//
// Last Modified By : Mir
// Last Modified On : 09-29-2023
// ***********************************************************************
// <copyright file="ICSCode.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using TranspilerLib.Data;

/// <summary>
/// The Models namespace.
/// </summary>
namespace TranspilerLib.Interfaces.Code;

/// <summary>
/// Interface ICodeBlock
/// Extends the <see cref="IHasParents{ICodeBlock}" />
/// Extends the <see cref="IEquatable{ICodeBlock}" />
/// </summary>
/// <seealso cref="Models.IHasParents{ICodeBlock}" />
/// <seealso cref="IEquatable{ICodeBlock}" />
public interface ICodeBlock : IHasParents<ICodeBlock>, IEquatable<ICodeBlock>
{
    /// <summary>
    /// Gets the sub blocks.
    /// </summary>
    /// <value>The sub blocks.</value>
    IList<ICodeBlock> SubBlocks { get; }
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    CodeBlockType Type { get; set; }
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    string Name { get; set; }
    /// <summary>
    /// Gets or sets the code.
    /// </summary>
    /// <value>The code.</value>
    string Code { get; set; }
    /// <summary>
    /// Gets or sets the destination.
    /// </summary>
    /// <value>The destination.</value>
    WeakReference<ICodeBlock>? Destination { get; set; }
    /// <summary>
    /// Gets the level.
    /// </summary>
    /// <value>The level.</value>
    int Level { get; }
    /// <summary>
    /// Gets the index.
    /// </summary>
    /// <value>The index.</value>
    int Index { get; }
    /// <summary>
    /// Gets the sources.
    /// </summary>
    /// <value>The sources.</value>
    IList<WeakReference<ICodeBlock>?> Sources { get; }
    //  new ICodeBlock? Parent { get; set; }
    /// <summary>
    /// Gets the next.
    /// </summary>
    /// <value>The next.</value>
    ICodeBlock? Next { get; }
    /// <summary>
    /// Gets the previous.
    /// </summary>
    /// <value>The previous.</value>
    ICodeBlock? Prev { get; }
    /// <summary>
    /// Gets the position in the original source from which this block was created.
    /// </summary>
    /// <value>The zero-based character index in the source text.</value>
    int SourcePos { get; init; }

    /// <summary>
    /// Deletes the sub blocks.
    /// </summary>
    /// <param name="iSrc">The index of the first sub block to delete.</param>
    /// <param name="cnt">The number of sub blocks to delete.</param>
    /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
    bool DeleteSubBlocks(int iSrc, int cnt);
    /// <summary>
    /// Moves the sub blocks.
    /// </summary>
    /// <param name="iSrc">The index of the first sub block to move.</param>
    /// <param name="iDst">The destination index within the same parent.</param>
    /// <param name="cnt">The number of sub blocks to move.</param>
    /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
    bool MoveSubBlocks(int iSrc, int iDst, int cnt);
    /// <summary>
    /// Moves the sub blocks to a different destination block.
    /// </summary>
    /// <param name="iSrc">The index of the first sub block to move.</param>
    /// <param name="cDst">The destination block reference.</param>
    /// <param name="cnt">The number of sub blocks to move.</param>
    /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
    bool MoveSubBlocks(int iSrc, ICodeBlock cDst, int cnt);
    /// <summary>
    /// Moves this block into the sub blocks of the specified destination.
    /// </summary>
    /// <param name="cDst">The destination block to move under.</param>
    /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
    bool MoveToSub(ICodeBlock cDst);
    /// <summary>
    /// Converts to code.
    /// </summary>
    /// <param name="indent">The indent.</param>
    /// <returns>string.</returns>
    string ToCode(int indent = 4);
}

