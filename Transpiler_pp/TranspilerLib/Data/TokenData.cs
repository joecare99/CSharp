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
namespace TranspilerLib.Data;

/// <summary>Identifies a fine-grained C# lexical token.</summary>


/// <summary>
/// Represents a single token with its lexeme, token kind and nesting level information.
/// </summary>
/// <param name="Code">The token text as found in the source.</param>
/// <param name="type">The semantic kind of the token.</param>
/// <param name="Level">The block nesting level at which the token was observed.</param>
/// <param name="Pos">Optional absolute character position in the source.</param>
public record struct TokenData(string Code, CodeBlockType type = CodeBlockType.Unknown, int Level = -1, int Pos = 0)
{
    /// <summary>Gets or initializes the token's source length.</summary>
    public int Length { get; init; }

    /// <summary>Gets or initializes the whitespace preceding this token in the original source.</summary>
    public string LeadingTrivia { get; init; } = string.Empty;

    /// <summary>Gets the exclusive end offset of the token.</summary>
    public int End => Pos + Length;

    /* 
       public static implicit operator (string, ICSCode.CodeBlockType, int)(TokenData value)
       {
           return (value.Code, value.type, value.Level);
       }
    */
    /// <summary>
    /// Implicitly constructs a <see cref="TokenData"/> from a tuple of (code, type, level).
    /// </summary>
    /// <param name="value">Tuple containing the token fields.</param>
    public static implicit operator TokenData((string, CodeBlockType, int) value)
    {
        return new TokenData(value.Item1, value.Item2, value.Item3);
    }

}