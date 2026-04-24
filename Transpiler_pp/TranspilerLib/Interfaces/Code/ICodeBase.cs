// ***********************************************************************
// Assembly         : TranspilerLib
// Author           : Mir
// Created          : 09-26-2023
//
// Last Modified By : Mir
// Last Modified On : 09-29-2023
// ***********************************************************************
// <copyright file="ICodeBase.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************

// ***********************************************************************
// Assembly         : TranspilerLib
// Author           : Mir
// Created          : 09-26-2023
//
// Last Modified By : Mir
// Last Modified On : 09-29-2023
// ***********************************************************************
// <copyright file="ICodeBase.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using TranspilerLib.Data;

namespace TranspilerLib.Interfaces.Code;

/// <summary>
/// Defines the common lifecycle for code processors: accept raw input (<see cref="OriginalCode"/>),
/// tokenize it (<see cref="Tokenize()"/> or the streaming variant), build a structured representation
/// (<see cref="Parse(System.Collections.Generic.IEnumerable{TranspilerLib.Data.TokenData}?)"/>), and finally render
/// it back to formatted code (<see cref="ToCode(ICodeBlock, int)"/>).
/// </summary>
public interface ICodeBase
{
    /// <summary>
    /// Delegate used to stream tokens during tokenization. Implementations receive each produced <see cref="TokenData"/>.
    /// </summary>
    /// <param name="data">The token produced by the tokenizer.</param>
    public delegate void TokenDelegate(TokenData data);

    /// <summary>
    /// Gets or sets the raw source text (e.g., file contents, code fragment) to be processed.
    /// </summary>
    string OriginalCode { get; set; }

    /// <summary>
    /// Parses the specified token sequence or, when <paramref name="values"/> is <c>null</c>, the tokens produced from
    /// the current <see cref="OriginalCode"/>.
    /// </summary>
    /// <param name="values">Optional predetermined token sequence.</param>
    /// <returns>The root <see cref="ICodeBlock"/> of the generated syntax/structure tree.</returns>
    ICodeBlock Parse(IEnumerable<TokenData>? values = null);

    /// <summary>
    /// Converts a code block tree into formatted source code.
    /// </summary>
    /// <param name="cStruct">The root of the code block tree to emit.</param>
    /// <param name="indent">Base indentation width (default 4).</param>
    /// <returns>The formatted code as a string.</returns>
    string ToCode(ICodeBlock cStruct, int indent = 4);

    /// <summary>
    /// Performs a full tokenization of the currently set <see cref="OriginalCode"/> and returns the resulting sequence.
    /// </summary>
    /// <returns>An enumerable of all produced <see cref="TokenData"/> in lexical order.</returns>
    IEnumerable<TokenData> Tokenize();

    /// <summary>
    /// Tokenizes the source and invokes the optional callback for each produced token.
    /// </summary>
    /// <param name="token">Optional delegate for streaming processing of individual tokens; may be <c>null</c>.</param>
    void Tokenize(TokenDelegate? token);
}