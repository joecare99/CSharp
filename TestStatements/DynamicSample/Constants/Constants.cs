﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicSample.Constants
{
    public static class Constants
    {
        private const string LineEnding = "\r\n";
        public const string Header = "======================================================================" + LineEnding +
            "## {0}" + LineEnding +
            "======================================================================";

        public const string Header2 = LineEnding+"+----------------------------------------------------------" +LineEnding+
            "| {0}" + LineEnding+
            "+----------------------------------------------------------";
    }
}
