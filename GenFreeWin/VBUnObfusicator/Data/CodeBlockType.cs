// ***********************************************************************
// Assembly         : VBUnObfusicator
// Author           : Mir
// Created          : 09-26-2023
//
// Last Modified By : Mir
// Last Modified On : 09-29-2023
// ***********************************************************************
// <copyright file="CodeBlockType.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The Models namespace.
/// </summary>
namespace VBUnObfusicator.Data;

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
    /// The class
    /// </summary>
    Class,
    /// <summary>
    /// The instruction
    /// </summary>
    Instruction,
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
    /// The l comment
    /// </summary>
    LComment,
    /// <summary>
    /// The comment
    /// </summary>
    Comment,
    /// <summary>
    /// The unknown
    /// </summary>
    Unknown,
    /// <summary>
    /// The string
    /// </summary>
    String,
    /// <summary>
    /// The block
    /// </summary>
    Block
}