// ***********************************************************************
// Assembly         : TestStatements
// Author           : Mir
// Created          : 07-12-2022
//
// Last Modified By : Mir
// Last Modified On : 09-09-2022
// ***********************************************************************
// <copyright file="Program.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.Anweisungen;
using TestStatements.DataTypes;
using TestStatements.Collection.Generic;
using TestStatements.Threading.Tasks;
using TestStatements.Diagnostics;
using TestStatements.Reflection;
using TestStatements.Linq;
using TestStatements.CS_Concepts;
using TestStatements.ClassesAndObjects;
using TestStatements.Helper;
using TestStatements.Runtime.Loader;

namespace TestStatements
{
    /// <summary>
    /// Class Program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
			foreach (var s in Properties.Resource1.Version.Split(new string[] {"\r\n"},StringSplitOptions.None))
			  Console.WriteLine(s.Substring(s.IndexOf(']')+1));
            Console.WriteLine();
            RTLoaderExample.Main(args);
            DebugExample.Main();
            Declarations.DoVarDeclarations(args);
            Declarations.DoConstantDeclarations(args);
            TypeSystem.All();          
            Expressions.DoExpressions(args);
            ConditionalStatement.DoIfStatement(args);
            ConditionalStatement.DoSwitchStatement(args);
            LoopStatements.DoWhileStatement(args);
            LoopStatements.DoDoStatement(args);
            LoopStatements.DoForStatement(args);
            LoopStatements.DoForEachStatement(args);
            ProgramFlow.DoBreakStatement(args);
            ProgramFlow.DoContinueStatement(args);
            ProgramFlow.DoGoToStatement(args);
            ReturnStatement.DoReturnStatement(args);
            YieldStatement.DoYieldStatement(args);
            ExceptionHandling.DoTryCatch(args);
            Checking.CheckedUnchecked(args);
            Locking.DoLockTest(args);
            UsingStatement.DoUsingStatement(args);
            // ClassesAndObjects
            InterfaceTest.Run();
            // Datatypes
            EnumTest.MainTest();
            StringEx.AllTests();
            Formating.CombinedFormating();
            Formating.IndexKomponent();
            Formating.IndexKomponent2();
            Formating.IndentationKomponent();
            Formating.EscapeSequence();
            Formating.CodeExamples1();
            Formating.CodeExamples2();
            Formating.CodeExamples3();
            Formating.CodeExamples4();
            SwitchStatement.SwitchExample1();
            SwitchStatement.SwitchExample2();
            SwitchStatement.SwitchExample3();
            SwitchStatement.SwitchExample4();
            SwitchStatement.SwitchExample5();
            SwitchStatement.SwitchExample6();
            SwitchStatement.SwitchExample7();
            SwitchStatement2.SwitchExample21();
            // Collection
            ComparerExample.ComparerExampleMain(args);
            TestList.ListMain();
            DinosaurExample.ListDinos();
            SortedListExample.SortedListMain();
            DictionaryExample.DictionaryExampleMain();
            TestHashSet.ShowHashSet();
            TaskExample.ExampleMain();
            StopWatchExample.ExampleMain();
            AssemblyExample.ExampleMain();
            ReflectionExample.ExampleMain();
            AsyncBreakfast.AsyncBreakfast_Main(args);
            AsyncBreakfast.AsyncBreakfast_Main2(args).Wait();
            AsyncBreakfast.AsyncBreakfast_Main3(args).Wait();
            AsyncBreakfast.AsyncBreakfast_Main4(args).Wait();
            Console.WriteLine("===============================================");
            Console.Write("<Enter> to continue");
            // Extension
            ExtensionExample.ShowExtensionEx1();
            // Linq
            LinqLookup.LookupExample();

            Console.ReadLine();
        }
    }
}
