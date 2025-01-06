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

public record struct TokenData(string Code, CodeBlockType type, int Level,int Pos=0)
{
    /* 
       public static implicit operator (string, ICSCode.CodeBlockType, int)(TokenData value)
       {
           return (value.Code, value.type, value.Level);
       }
    */
    public static implicit operator TokenData((string, CodeBlockType, int) value)
    {
        return new TokenData(value.Item1, value.Item2, value.Item3);
    }

}