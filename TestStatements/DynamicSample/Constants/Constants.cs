﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSample.Constants
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public static class Constants
    {
        private const string LineEnding = "\r\n";
        /// <summary>
        /// The header
        /// </summary>
        public const string Header = "======================================================================" + LineEnding +
            "## {0}" + LineEnding +
            "======================================================================";

        /// <summary>
        /// The header2
        /// </summary>
        public const string Header2 = LineEnding+"+----------------------------------------------------------" +LineEnding+
            "| {0}" + LineEnding+
            "+----------------------------------------------------------";
    }
}
