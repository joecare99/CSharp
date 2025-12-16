// ***********************************************************************
// Assembly         : VBUnObfusicator
// Author           : Mir
// Created          : 09-26-2023
//
// Last Modified By : Mir
// Last Modified On : 09-29-2023
// ***********************************************************************
// <copyright file="TokenData.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// The Models namespace.
/// </summary>
namespace VBUnObfusicator.Data;

public record struct TokenData(string Code, CodeBlockType type, int Level)
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