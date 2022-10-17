﻿// ***********************************************************************
// Assembly         : Calc64Base
// Author           : Mir
// Created          : 08-27-2022
//
// Last Modified By : Mir
// Last Modified On : 08-31-2022
// ***********************************************************************
// <copyright file="StandardOperations.cs" company="Hewlett-Packard Company">
//     Copyright © Hewlett-Packard Company 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace Calc64Base
{
    /// <summary>
    /// Class StandardOperations.
    /// </summary>
    public class StandardOperations
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>IEnumerable&lt;CalcOperation&gt;.</returns>
        public static IEnumerable<CalcOperation> GetAll()
        {
            return new List<CalcOperation>()
            {
                new BinaryOperation("+",2, "plus", (a, r) => r + a),
                new BinaryOperation("-",3, "minus", (a, r) => r - a),
                new BinaryOperation("*",4, "multiply", (a, r) => r * a),
                new BinaryOperation("/",5, "divide", (a, r) => r / a),
                new BinaryOperation("^",10, "power", (a, r) =>(Int64)Math.Pow(r , a)),

                new BinaryOperation("&",6, "and", (a, r) => r & a),
                new BinaryOperation("|",7, "or", (a, r) => r | a),
                new BinaryOperation("x",8, "xor", (a, r) => r ^ a),
                new BinaryOperation("&~",13, "nand", (a, r) => r & ~a),
                new BinaryOperation("|~",14, "nor", (a, r) => r | ~a),
                new BinaryOperation("x~",15, "xnor", (a, r) => r ^ ~a),

                new UnaryOperation("~",9, "invert", (a) => ~a),
                new UnaryOperation("±",11, "negate", (a) =>  - a),

                new BinaryOperation("%",12, "modulo", (a, r) => r % a),
                new BinaryOperation("==",16, "equals", (a, r) => r ^ -a-1),

                new FromMemOperation("MR",20,"Memory Retreive",(a,m)=>m),
                new ToMemOperation("MS",21,"Memory Store",(a,m)=>a),
                new ToMemOperation("M+",22,"Memory Add",(a,m)=> m + a),
                new ToMemOperation("M-",23,"Memory Subtract",(a,m)=> m - a),
                new ToMemOperation("MC",24,"Memory Clear",(a,m)=>0),
            };
        }
    }
}