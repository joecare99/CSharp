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
/// <summary>
/// The Models namespace.
/// </summary>
namespace TranspilerLib.Data;

/// <summary>
/// Enum CodeBlockType
/// </summary>
public enum CodeBlockType
{
    /// <summary>
    /// The main block
    /// </summary>
    MainBlock,
    /// <summary>
    /// The sub block
    /// </summary>
    SubBlock,
    /// <summary>
    /// The label
    /// </summary>
    Label,
    /// <summary>
    /// The variable
    /// </summary>
    Variable,
    /// <summary>
    /// The function
    /// </summary>
    Function,
    /// <summary>
    /// The declaration
    /// </summary>
    Declaration,
    /// <summary>
    /// The class
    /// </summary>
    Class,
    /// <summary>
    /// The instruction
    /// </summary>
    Operation,
    /// <summary>
    /// The parameter
    /// </summary>
    Parameter,
    /// <summary>
    /// The namespace
    /// </summary>
    Namespace,
    /// <summary>
    /// The using
    /// </summary>
    Using,
    /// <summary>
    /// The goto
    /// </summary>
    Goto,
    /// <summary>
    /// The Linecomment (starts with '//', usually after the element) 
    /// </summary>
    LComment,
    /// <summary>
    /// The full Linecomment (starts with '//', usually before the element)
    /// </summary>
    FLComment,
    /// <summary>
    /// The comment
    /// </summary>
    Comment,
    /// <summary>
    /// The unknown
    /// </summary>
    Unknown,
    /// <summary>
    /// The string(-constant)
    /// </summary>
    String,
    /// <summary>
    /// The block 
    /// </summary>
    Block,
    /// <summary>
    /// A number-constant
    /// </summary> 
    Number,
    /// <summary>
    /// A bracket (e.g. '(' or ')')
    /// </summary> 
    Bracket,
    /// <summary>
    /// An Assignment-Operation 
    /// </summary> 
    Assignment,
}
