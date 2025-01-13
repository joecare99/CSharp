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
/// Extends the <see cref="Models.IHasParents`1" />
/// Extends the <see cref="IEquatable`1" />
/// </summary>
/// <seealso cref="Models.IHasParents`1" />
/// <seealso cref="IEquatable`1" />
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
    int SourcePos { get; init; }

    /// <summary>
    /// Deletes the sub blocks.
    /// </summary>
    /// <param name="iSrc">The i source.</param>
    /// <param name="cnt">The count.</param>
    /// <returns>bool.</returns>
    bool DeleteSubBlocks(int iSrc, int cnt);
    /// <summary>
    /// Moves the sub blocks.
    /// </summary>
    /// <param name="iSrc">The i source.</param>
    /// <param name="iDst">The i DST.</param>
    /// <param name="cnt">The count.</param>
    /// <returns>bool.</returns>
    bool MoveSubBlocks(int iSrc, int iDst, int cnt);
    /// <summary>
    /// Moves the sub blocks.
    /// </summary>
    /// <param name="iSrc">The i source.</param>
    /// <param name="cDst">The c DST.</param>
    /// <param name="cnt">The count.</param>
    /// <returns>bool.</returns>
    bool MoveSubBlocks(int iSrc, ICodeBlock cDst, int cnt);
    /// <summary>
    /// Moves the block to the sub blocks of the destination.
    /// </summary>
    /// <param name="cDst">The destination</param>
    /// <param name="cnt">The count.</param>
    /// <returns>bool.</returns>
    bool MoveToSub(ICodeBlock cDst);
    /// <summary>
    /// Converts to code.
    /// </summary>
    /// <param name="indent">The indent.</param>
    /// <returns>string.</returns>
    string ToCode(int indent = 4);
}

