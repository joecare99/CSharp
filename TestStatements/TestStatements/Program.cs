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

namespace TestStatements
{
     class Program
    {
        static void Main(string[] args)
        {
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
            EnumTest.MainTest();
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
            Console.WriteLine("===============================================");
            Console.Write("<Enter> to continue");
            // Linq
            LinqLookup.LookupExample();

            Console.ReadLine();
        }
    }
}
