﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.Constants;

namespace TestStatements.Anweisungen
{
    public class Declarations
    {
        /// <summary>Does variable declarations.</summary>
        /// <param name="args">The arguments.</param>
        public static void DoVarDeclarations(string[] args)
        {
            const string Title = "Deklaration von Variablen";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));
            int a;
            int b = 2, c = 3;
            a = 1;
            Console.WriteLine(a + b + c);
        }

        /// <summary>Does constant declarations.</summary>
        /// <param name="args">The arguments.</param>
        public static void DoConstantDeclarations(string[] args)
        {
            const string Title = "Deklaration von Konstanten";
            Console.WriteLine(Constants.Constants.Header.Replace("%s", Title));
            const float pi = 3.1415927f;
            const int r = 25;
            Console.WriteLine(pi * r * r);
        }
    }
}
