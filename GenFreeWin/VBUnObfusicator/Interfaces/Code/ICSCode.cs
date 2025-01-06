// ***********************************************************************
// Assembly         : VBUnObfusicator
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

/// <summary>
/// The Models namespace.
/// </summary>
namespace VBUnObfusicator.Interfaces.Code
{
    /// <summary>
    /// Interface ICSCode
    /// </summary>
    public interface ICSCode : ICodeBase
    {
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
            List<WeakReference<ICodeBlock>> Sources { get; }
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
            /// Converts to code.
            /// </summary>
            /// <param name="indent">The indent.</param>
            /// <returns>string.</returns>
            string ToCode(int indent = 4);
        }
        /// <summary>
        /// Gets or sets the original code.
        /// </summary>
        /// <value>The original code.</value>
        string OriginalCode { get; set; }
        bool DoWhile { get; set; }

        /// <summary>
        /// Parses the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns>VBUnObfusicator.Models.ICSCode.ICodeBlock.</returns>
        ICodeBlock Parse(IEnumerable<TokenData>? values = null);
        /// <summary>
        /// Removes the single source labels1.
        /// </summary>
        /// <param name="cStruct">The c structure.</param>
        void RemoveSingleSourceLabels1(ICodeBlock cStruct);
        /// <summary>
        /// Reorders the labels.
        /// </summary>
        /// <param name="cStruct">The c structure.</param>
        void ReorderLabels(ICodeBlock cStruct);
        /// <summary>
        /// Converts to code.
        /// </summary>
        /// <param name="cStruct">The c structure.</param>
        /// <param name="indent">The indent.</param>
        /// <returns>string.</returns>
        string ToCode(ICodeBlock cStruct, int indent = 4);
        /// <summary>
        /// Tokenizes the specified token.
        /// </summary>
        /// <param name="token">The token.</param>
        void Tokenize(TokenDelegate? token);
        IEnumerable<TokenData> Tokenize();
    }

    public record struct TokenData(string Code, ICSCode.CodeBlockType type, int Level)
    {
        /* 
           public static implicit operator (string, ICSCode.CodeBlockType, int)(TokenData value)
           {
               return (value.Code, value.type, value.Level);
           }
        */
        public static implicit operator TokenData((string, ICSCode.CodeBlockType, int) value)
        {
            return new TokenData(value.Item1, value.Item2, value.Item3);
        }

    }
}