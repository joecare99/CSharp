// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Constants.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestStatements.Constants
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    class Constants
    {
        /// <summary>
        /// The line ending
        /// </summary>
        private const string LineEnding = "\r\n";
        /// <summary>
        /// The header
        /// </summary>
        public const string Header = "======================================================================" + LineEnding +
            "## %s " + LineEnding +
            "======================================================================";

        /// <summary>
        /// The header2
        /// </summary>
        public const string Header2 = LineEnding+"+----------------------------------------------------------" +LineEnding+
            "| {0}" + LineEnding+
            "+----------------------------------------------------------";
    }
}
