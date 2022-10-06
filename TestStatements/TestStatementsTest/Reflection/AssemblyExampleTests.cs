using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.ConsoleAsserts;

namespace TestStatements.Reflection.Tests
{
    [TestClass()]
    public class AssemblyExampleTests : TestConsole
    {
        private readonly string cExpExampleMain =
            "Assembly Full Name:\r\nTestStatements, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\r\n\nName: TestStatements\r\nVersion: 1.0\r\n\nAssembly CodeBase:\r\nfile:///C:/Projekte/CSharp/bin/Debug/TestStatements.EXE\r\nTestStatements.Program\r\nTestStatements.Threading.Tasks.TaskExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\n '-> .ExampleMain4()\r\nTestStatements.Reflection.AssemblyExample\r\n '-> .ExampleMain()\r\nTestStatements.Reflection.S\r\nTestStatements.Reflection.ReflectionExample\r\n '-> .ExampleMain()\r\nTestStatements.Linq.Package\r\nTestStatements.Linq.LinqLookup\r\n '-> .LookupExample()\r\n '-> .ShowContains()\r\n '-> .ShowIEnumerable()\r\n '-> .ShowCount()\r\n '-> .ShowGrouping()\r\nTestStatements.Diagnostics.StopWatchExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .DisplayTimerProperties()\r\nTestStatements.DataTypes.EnumTest\r\n '-> .MainTest()\r\nTestStatements.DataTypes.Formating\r\n '-> .CombinedFormating()\r\n '-> .IndexKomponent()\r\n '-> .IndexKomponent2()\r\n '-> .IndentationKomponent()\r\n '-> .EscapeSequence()\r\n '-> .CodeExamples1()\r\n '-> .CodeExamples2()\r\n '-> .CodeExamples3()\r\n '-> .CodeExamples4()\r\nTestStatements.Constants.Constants\r\nTestStatements.Collection.Generic.ComparerExample\r\n '-> .ComparerExampleMain(String[] args)\r\n '-> .ShowSortWithLengthFirstComparer()\r\n '-> .ShowSortwithDefaultComparer()\r\n '-> .ShowLengthFirstComparer()\r\nTestStatements.Collection.Generic.BoxLengthFirst\r\nTestStatements.Collection.Generic.BoxComp\r\nTestStatements.Collection.Generic.Box\r\nTestStatements.Collection.Generic.DictionaryExample\r\n '-> .DictionaryExampleMain()\r\n '-> .TryAddExisting()\r\n '-> .ShowIndex1()\r\n '-> .ShowIndex2()\r\n '-> .ShowIndex3()\r\n '-> .ShowIndex4()\r\n '-> .ShowTryGetValue()\r\n '-> .ShowContainsKey()\r\n '-> .ShowValueCollection()\r\n '-> .ShowKeyCollection()\r\n '-> .ShowRemove()\r\nTestStatements.Collection.Generic.SortedListExample\r\n '-> .SortedListMain()\r\n '-> .ShowValues1()\r\n '-> .ShowValues2()\r\n '-> .ShowKeys1()\r\n '-> .ShowKeys2()\r\n '-> .ShowRemove()\r\n '-> .ShowForEach()\r\n '-> .ShowContainsKey()\r\n '-> .ShowTryGetValue()\r\n '-> .TestIndexr()\r\n '-> .TestAddExisting()\r\nTestStatements.Collection.Generic.TestHashSet\r\n '-> .ShowHashSet()\r\nTestStatements.Collection.Generic.Part\r\nTestStatements.Collection.Generic.TestList\r\n '-> .ListMain()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowIndex()\r\n '-> .ShowRemove1()\r\n '-> .ShowRemove2()\r\nTestStatements.Collection.Generic.DinosaurExample\r\n '-> .ListDinos()\r\n '-> .ShowCreateData()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowItemProperty()\r\n '-> .ShowRemove()\r\n '-> .ShowTrimExcess()\r\n '-> .ShowClear()\r\nTestStatements.ClassesAndObjects.Members\r\n '-> .set_OnChange(EventHandler value)\r\n '-> .aMethod()\r\n '-> .set_aProperty(Int32 value)\r\nTestStatements.Anweisungen.Checking\r\n '-> .CheckedUnchecked(String[] args)\r\nTestStatements.Anweisungen.ExceptionHandling\r\n '-> .DoTryCatch(String[] args)\r\nTestStatements.Anweisungen.Account\r\nTestStatements.Anweisungen.Locking\r\n '-> .DoLockTest(String[] args)\r\nTestStatements.Anweisungen.ProgramFlow\r\n '-> .DoBreakStatement(String[] args)\r\n '-> .DoContinueStatement(String[] args)\r\n '-> .DoGoToStatement(String[] args)\r\nTestStatements.Anweisungen.Expressions\r\n '-> .DoExpressions(String[] args)\r\nTestStatements.Anweisungen.Declarations\r\n '-> .DoVarDeclarations(String[] args)\r\n '-> .DoConstantDeclarations(String[] args)\r\nTestStatements.Anweisungen.ConditionalStatement\r\n '-> .DoIfStatement(String[] args)\r\n '-> .DoSwitchStatement(String[] args)\r\nTestStatements.Anweisungen.LoopStatements\r\n '-> .DoWhileStatement(String[] args)\r\n '-> .DoDoStatement(String[] args)\r\n '-> .DoForStatement(String[] args)\r\n '-> .DoForEachStatement(String[] args)\r\nTestStatements.Anweisungen.RandomExample\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\nTestStatements.Anweisungen.ReturnStatement\r\n '-> .DoReturnStatement(String[] args)\r\nTestStatements.Anweisungen.SwitchStatement\r\n '-> .SwitchExample1()\r\n '-> .SwitchExample2()\r\n '-> .SwitchExample3()\r\n '-> .SwitchExample4()\r\n '-> .SwitchExample5()\r\n '-> .SwitchExample6()\r\n '-> .SwitchExample7()\r\nTestStatements.Anweisungen.DiceLibrary\r\nTestStatements.Anweisungen.Shape\r\nTestStatements.Anweisungen.Rectangle\r\nTestStatements.Anweisungen.Square\r\nTestStatements.Anweisungen.Circle\r\nTestStatements.Anweisungen.SwitchStatement2\r\n '-> .SwitchExample21()\r\nTestStatements.Anweisungen.YieldStatement\r\n '-> .DoYieldStatement(String[] args)\r\nTestStatements.Anweisungen.UsingStatement\r\n '-> .DoUsingStatement(String[] args)\r\n<PrivateImplementationDetails>\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_1\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_1\r\nTestStatements.Threading.Tasks.TaskExample+<ExampleMain2a>d__5\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0\r\nTestStatements.Reflection.ReflectionExample+NestedClass\r\nTestStatements.Reflection.ReflectionExample+INested\r\nTestStatements.Linq.LinqLookup+<>c\r\nTestStatements.DataTypes.EnumTest+Days\r\nTestStatements.DataTypes.EnumTest+BoilingPoints\r\nTestStatements.DataTypes.EnumTest+Colors\r\nTestStatements.DataTypes.Formating+<>c\r\nTestStatements.Anweisungen.SwitchStatement+Color\r\nTestStatements.Anweisungen.SwitchStatement+<>c\r\nTestStatements.Anweisungen.SwitchStatement+<>c__12`1\r\nTestStatements.Anweisungen.YieldStatement+<Range>d__0\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=20\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0+<<ExampleMain3>b__0>d\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0+<<ExampleMain4>b__0>d\r\n\nExample.SampleMethod(42) executes.\r\nSampleMethod returned 84.\r\n\nAssembly entry point:\r\nVoid Main(System.String[])";

        [TestMethod()]
        public void AssemblyExampleTest()
        {
            Assert.IsNotNull(new AssemblyExample(5));
        }

        [TestMethod()]
        public void SampleMethodTest()
        {
            AssemblyExample l = new AssemblyExample(3) ;
            Assert.AreEqual(6,l.SampleMethod(2) );
            Assert.AreEqual(3, l.SampleMethod(1));
            Assert.AreEqual(9, l.SampleMethod(3));
            l = new AssemblyExample(4);
            Assert.AreEqual(8, l.SampleMethod(2));
            Assert.AreEqual(4, l.SampleMethod(1));
            Assert.AreEqual(12, l.SampleMethod(3));
        }

        [TestMethod()]
        public void ExampleMainTest()
        {
            AssertConsoleOutput(cExpExampleMain, AssemblyExample.ExampleMain);
        }
    }
}