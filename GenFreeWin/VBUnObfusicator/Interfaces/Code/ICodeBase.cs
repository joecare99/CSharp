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
using static VBUnObfusicator.Interfaces.Code.ICSCode;

namespace VBUnObfusicator.Interfaces.Code;

public interface ICodeBase
{
    public delegate void TokenDelegate(TokenData data);

    string OriginalCode { get; set; }

    /// <summary>
    /// Parses the specified values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <returns>VBUnObfusicator.Models.ICSCode.ICodeBlock.</returns>
    ICodeBlock Parse(IEnumerable<TokenData>? values = null);
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
    IEnumerable<TokenData> Tokenize();
    void Tokenize(TokenDelegate? token);
}