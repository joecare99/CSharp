using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStatements.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStatements.UnitTesting;

namespace TestStatements.Reflection.Tests
{
	/// <summary>
	/// Defines test class AssemblyExampleTests.
	/// Implements the <see cref="ConsoleTestsBase" />
	/// </summary>
	/// <seealso cref="ConsoleTestsBase" />
	[TestClass()]
    public class AssemblyExampleTests : ConsoleTestsBase
    {
#if NET9_0
	const string netversion = "net9.0";
#elif NET8_0
	const string netversion = "net8.0";
#elif NET7_0
	const string netversion = "net7.0";
#elif NET6_0
        const string netversion = "net6.0";
#else
	const string netversion = "net5.0";
#endif

        private readonly string cExpExampleMain =
          //"Assembly Full Name:\r\nTestStatements, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\r\n\nName: TestStatements\r\nVersion: 1.0\r\n\nAssembly CodeBase:\r\nfile:///C:/Projekte/CSharp/bin/Debug/TestStatements.EXE\r\nTestStatements.Program\r\nTestStatements.Threading.Tasks.TaskExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\n '-> .ExampleMain4()\r\nTestStatements.Reflection.AssemblyExample\r\n '-> .ExampleMain()\r\nTestStatements.Reflection.S\r\nTestStatements.Reflection.ReflectionExample\r\n '-> .ExampleMain()\r\nTestStatements.Linq.Package\r\nTestStatements.Linq.LinqLookup\r\n '-> .LookupExample()\r\n '-> .ShowContains()\r\n '-> .ShowIEnumerable()\r\n '-> .ShowCount()\r\n '-> .ShowGrouping()\r\nTestStatements.Diagnostics.StopWatchExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .DisplayTimerProperties()\r\nTestStatements.DataTypes.EnumTest\r\n '-> .MainTest()\r\nTestStatements.DataTypes.Formating\r\n '-> .CombinedFormating()\r\n '-> .IndexKomponent()\r\n '-> .IndexKomponent2()\r\n '-> .IndentationKomponent()\r\n '-> .EscapeSequence()\r\n '-> .CodeExamples1()\r\n '-> .CodeExamples2()\r\n '-> .CodeExamples3()\r\n '-> .CodeExamples4()\r\nTestStatements.Constants.Constants\r\nTestStatements.Collection.Generic.ComparerExample\r\n '-> .ComparerExampleMain(String[] args)\r\n '-> .ShowSortWithLengthFirstComparer()\r\n '-> .ShowSortwithDefaultComparer()\r\n '-> .ShowLengthFirstComparer()\r\nTestStatements.Collection.Generic.BoxLengthFirst\r\nTestStatements.Collection.Generic.BoxComp\r\nTestStatements.Collection.Generic.Box\r\nTestStatements.Collection.Generic.DictionaryExample\r\n '-> .DictionaryExampleMain()\r\n '-> .TryAddExisting()\r\n '-> .ShowIndex1()\r\n '-> .ShowIndex2()\r\n '-> .ShowIndex3()\r\n '-> .ShowIndex4()\r\n '-> .ShowTryGetValue()\r\n '-> .ShowContainsKey()\r\n '-> .ShowValueCollection()\r\n '-> .ShowKeyCollection()\r\n '-> .ShowRemove()\r\nTestStatements.Collection.Generic.SortedListExample\r\n '-> .SortedListMain()\r\n '-> .ShowValues1()\r\n '-> .ShowValues2()\r\n '-> .ShowKeys1()\r\n '-> .ShowKeys2()\r\n '-> .ShowRemove()\r\n '-> .ShowForEach()\r\n '-> .ShowContainsKey()\r\n '-> .ShowTryGetValue()\r\n '-> .TestIndexr()\r\n '-> .TestAddExisting()\r\nTestStatements.Collection.Generic.TestHashSet\r\n '-> .ShowHashSet()\r\nTestStatements.Collection.Generic.Part\r\nTestStatements.Collection.Generic.TestList\r\n '-> .ListMain()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowIndex()\r\n '-> .ShowRemove1()\r\n '-> .ShowRemove2()\r\nTestStatements.Collection.Generic.DinosaurExample\r\n '-> .ListDinos()\r\n '-> .ShowCreateData()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowItemProperty()\r\n '-> .ShowRemove()\r\n '-> .ShowTrimExcess()\r\n '-> .ShowClear()\r\nTestStatements.ClassesAndObjects.Members\r\n '-> .set_OnChange(EventHandler value)\r\n '-> .aMethod()\r\n '-> .set_aProperty(Int32 value)\r\nTestStatements.Anweisungen.Checking\r\n '-> .CheckedUnchecked(String[] args)\r\nTestStatements.Anweisungen.ExceptionHandling\r\n '-> .DoTryCatch(String[] args)\r\nTestStatements.Anweisungen.Account\r\nTestStatements.Anweisungen.Locking\r\n '-> .DoLockTest(String[] args)\r\nTestStatements.Anweisungen.ProgramFlow\r\n '-> .DoBreakStatement(String[] args)\r\n '-> .DoContinueStatement(String[] args)\r\n '-> .DoGoToStatement(String[] args)\r\nTestStatements.Anweisungen.Expressions\r\n '-> .DoExpressions(String[] args)\r\nTestStatements.Anweisungen.Declarations\r\n '-> .DoVarDeclarations(String[] args)\r\n '-> .DoConstantDeclarations(String[] args)\r\nTestStatements.Anweisungen.ConditionalStatement\r\n '-> .DoIfStatement(String[] args)\r\n '-> .DoSwitchStatement(String[] args)\r\nTestStatements.Anweisungen.LoopStatements\r\n '-> .DoWhileStatement(String[] args)\r\n '-> .DoDoStatement(String[] args)\r\n '-> .DoForStatement(String[] args)\r\n '-> .DoForEachStatement(String[] args)\r\nTestStatements.Anweisungen.RandomExample\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\nTestStatements.Anweisungen.ReturnStatement\r\n '-> .DoReturnStatement(String[] args)\r\nTestStatements.Anweisungen.SwitchStatement\r\n '-> .SwitchExample1()\r\n '-> .SwitchExample2()\r\n '-> .SwitchExample3()\r\n '-> .SwitchExample4()\r\n '-> .SwitchExample5()\r\n '-> .SwitchExample6()\r\n '-> .SwitchExample7()\r\nTestStatements.Anweisungen.DiceLibrary\r\nTestStatements.Anweisungen.Shape\r\nTestStatements.Anweisungen.Rectangle\r\nTestStatements.Anweisungen.Square\r\nTestStatements.Anweisungen.Circle\r\nTestStatements.Anweisungen.SwitchStatement2\r\n '-> .SwitchExample21()\r\nTestStatements.Anweisungen.YieldStatement\r\n '-> .DoYieldStatement(String[] args)\r\nTestStatements.Anweisungen.UsingStatement\r\n '-> .DoUsingStatement(String[] args)\r\n<PrivateImplementationDetails>\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_1\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_1\r\nTestStatements.Threading.Tasks.TaskExample+<ExampleMain2a>d__5\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0\r\nTestStatements.Reflection.ReflectionExample+NestedClass\r\nTestStatements.Reflection.ReflectionExample+INested\r\nTestStatements.Linq.LinqLookup+<>c\r\nTestStatements.DataTypes.EnumTest+Days\r\nTestStatements.DataTypes.EnumTest+BoilingPoints\r\nTestStatements.DataTypes.EnumTest+Colors\r\nTestStatements.DataTypes.Formating+<>c\r\nTestStatements.Anweisungen.SwitchStatement+Color\r\nTestStatements.Anweisungen.SwitchStatement+<>c\r\nTestStatements.Anweisungen.SwitchStatement+<>c__12`1\r\nTestStatements.Anweisungen.YieldStatement+<Range>d__0\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=20\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0+<<ExampleMain3>b__0>d\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0+<<ExampleMain4>b__0>d\r\n\nExample.SampleMethod(42) executes.\r\nSampleMethod returned 84.\r\n\nAssembly entry point:\r\nVoid Main(System.String[])";
          //"Assembly Full Name:\r\nTestStatements, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\r\n\nName: TestStatements\r\nVersion: 1.0\r\n\nAssembly CodeBase:\r\nfile:///C:/Users/DEROSCHR/Documents/Projekte/CSharp/bin/Debug/TestStatements.EXE\r\nTestStatements.Program\r\nTestStatements.Threading.Tasks.AsyncBreakfast\r\n '-> .AsyncBreakfast_Main(String[] args)\r\nTestStatements.Threading.Tasks.Juice\r\nTestStatements.Threading.Tasks.Toast\r\nTestStatements.Threading.Tasks.Bacon\r\nTestStatements.Threading.Tasks.Egg\r\nTestStatements.Threading.Tasks.Coffee\r\nTestStatements.Threading.Tasks.TaskExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\n '-> .ExampleMain4()\r\nTestStatements.SystemNS.System_Namespace\r\nTestStatements.Reflection.AssemblyExample\r\n '-> .ExampleMain()\r\nTestStatements.Reflection.S\r\nTestStatements.Reflection.ReflectionExample\r\n '-> .ExampleMain()\r\nTestStatements.Linq.Package\r\nTestStatements.Linq.LinqLookup\r\n '-> .LookupExample()\r\n '-> .ShowContains()\r\n '-> .ShowIEnumerable()\r\n '-> .ShowCount()\r\n '-> .ShowGrouping()\r\nTestStatements.Diagnostics.StopWatchExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .DisplayTimerProperties()\r\nTestStatements.CS_Concepts.TypeSystem\r\n '-> .All()\r\n '-> .UseOfTypes()\r\n '-> .DelareVariables()\r\n '-> .ValueTypes1()\r\n '-> .ValueTypes2()\r\n '-> .ValueTypes3()\r\n '-> .ValueTypes4()\r\nTestStatements.DataTypes.EnumTest\r\n '-> .MainTest()\r\nTestStatements.DataTypes.Formating\r\n '-> .CombinedFormating()\r\n '-> .IndexKomponent()\r\n '-> .IndexKomponent2()\r\n '-> .IndentationKomponent()\r\n '-> .EscapeSequence()\r\n '-> .CodeExamples1()\r\n '-> .CodeExamples2()\r\n '-> .CodeExamples3()\r\n '-> .CodeExamples4()\r\nTestStatements.DataTypes.IntegratedTypes\r\nTestStatements.DataTypes.StringEx\r\n '-> .AllTests()\r\n '-> .StringEx1()\r\n '-> .StringEx2()\r\n '-> .StringEx3()\r\n '-> .StringEx4()\r\n '-> .StringEx5()\r\n '-> .UnicodeEx1()\r\n '-> .StringSurogarteEx1()\r\nTestStatements.Constants.Constants\r\nTestStatements.Collection.Generic.ComparerExample\r\n '-> .ComparerExampleMain(String[] args)\r\n '-> .ShowSortWithLengthFirstComparer()\r\n '-> .ShowSortwithDefaultComparer()\r\n '-> .ShowLengthFirstComparer()\r\nTestStatements.Collection.Generic.BoxLengthFirst\r\nTestStatements.Collection.Generic.BoxComp\r\nTestStatements.Collection.Generic.Box\r\nTestStatements.Collection.Generic.DictionaryExample\r\n '-> .DictionaryExampleMain()\r\n '-> .TryAddExisting()\r\n '-> .ShowIndex1()\r\n '-> .ShowIndex2()\r\n '-> .ShowIndex3()\r\n '-> .ShowIndex4()\r\n '-> .ShowTryGetValue()\r\n '-> .ShowContainsKey()\r\n '-> .ShowValueCollection()\r\n '-> .ShowKeyCollection()\r\n '-> .ShowRemove()\r\nTestStatements.Collection.Generic.SortedListExample\r\n '-> .SortedListMain()\r\n '-> .ShowValues1()\r\n '-> .ShowValues2()\r\n '-> .ShowKeys1()\r\n '-> .ShowKeys2()\r\n '-> .ShowRemove()\r\n '-> .ShowForEach()\r\n '-> .ShowContainsKey()\r\n '-> .ShowTryGetValue()\r\n '-> .TestIndexr()\r\n '-> .TestAddExisting()\r\nTestStatements.Collection.Generic.TestHashSet\r\n '-> .ShowHashSet()\r\nTestStatements.Collection.Generic.Part\r\nTestStatements.Collection.Generic.TestList\r\n '-> .ListMain()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowIndex()\r\n '-> .ShowRemove1()\r\n '-> .ShowRemove2()\r\nTestStatements.Collection.Generic.DinosaurExample\r\n '-> .ListDinos()\r\n '-> .ShowCreateData()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowItemProperty()\r\n '-> .ShowRemove()\r\n '-> .ShowTrimExcess()\r\n '-> .ShowClear()\r\nTestStatements.ClassesAndObjects.Members\r\n '-> .set_OnChange(EventHandler value)\r\n '-> .aMethod()\r\n '-> .set_aProperty(Int32 value)\r\nTestStatements.Anweisungen.Checking\r\n '-> .CheckedUnchecked(String[] args)\r\nTestStatements.Anweisungen.ExceptionHandling\r\n '-> .DoTryCatch(String[] args)\r\nTestStatements.Anweisungen.Account\r\nTestStatements.Anweisungen.Locking\r\n '-> .DoLockTest(String[] args)\r\nTestStatements.Anweisungen.ProgramFlow\r\n '-> .DoBreakStatement(String[] args)\r\n '-> .DoContinueStatement(String[] args)\r\n '-> .DoGoToStatement(String[] args)\r\nTestStatements.Anweisungen.Expressions\r\n '-> .DoExpressions(String[] args)\r\nTestStatements.Anweisungen.Declarations\r\n '-> .DoVarDeclarations(String[] args)\r\n '-> .DoConstantDeclarations(String[] args)\r\nTestStatements.Anweisungen.ConditionalStatement\r\n '-> .DoIfStatement(String[] args)\r\n '-> .DoIfStatement2(String[] args)\r\n '-> .DoIfStatement3()\r\n '-> .DoSwitchStatement(String[] args)\r\n '-> .DoSwitchStatement1()\r\nTestStatements.Anweisungen.LoopStatements\r\n '-> .DoWhileStatement(String[] args)\r\n '-> .DoDoStatement(String[] args)\r\n '-> .DoDoStatement2(String[] args)\r\n '-> .DoDoStatement3(String[] args)\r\n '-> .DoDoDoStatement(String[] args)\r\n '-> .DoDoDoStatement2(String[] args)\r\n '-> .DoDoWhileNestedStatement2(String[] args)\r\n '-> .DoForStatement(String[] args)\r\n '-> .DoForStatement2(String[] args)\r\n '-> .DoForEachStatement(String[] args)\r\nTestStatements.Anweisungen.RandomExample\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\nTestStatements.Anweisungen.ReturnStatement\r\n '-> .DoReturnStatement(String[] args)\r\nTestStatements.Anweisungen.SwitchStatement\r\n '-> .SwitchExample1()\r\n '-> .SwitchExample2()\r\n '-> .SwitchExample3()\r\n '-> .SwitchExample4()\r\n '-> .SwitchExample5()\r\n '-> .SwitchExample6()\r\n '-> .SwitchExample7()\r\nTestStatements.Anweisungen.DiceLibrary\r\nTestStatements.Anweisungen.Shape\r\nTestStatements.Anweisungen.Rectangle\r\nTestStatements.Anweisungen.Square\r\nTestStatements.Anweisungen.Circle\r\nTestStatements.Anweisungen.SwitchStatement2\r\n '-> .SwitchExample21()\r\nTestStatements.Anweisungen.YieldStatement\r\n '-> .DoYieldStatement(String[] args)\r\nTestStatements.Anweisungen.UsingStatement\r\n '-> .DoUsingStatement(String[] args)\r\n<PrivateImplementationDetails>\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main2>d__1\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main3>d__2\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main4>d__3\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<MakeToastWithButterAndJamAsync>d__4\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<ToastBreadAsync>d__5\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<FryBaconAsync>d__6\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<FryEggsAsync>d__7\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_1\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_1\r\nTestStatements.Threading.Tasks.TaskExample+<ExampleMain2a>d__5\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0\r\nTestStatements.Reflection.ReflectionExample+NestedClass\r\nTestStatements.Reflection.ReflectionExample+INested\r\nTestStatements.Linq.LinqLookup+<>c\r\nTestStatements.CS_Concepts.TypeSystem+Coords\r\nTestStatements.CS_Concepts.TypeSystem+FileMode\r\nTestStatements.CS_Concepts.TypeSystem+MyClass\r\nTestStatements.CS_Concepts.TypeSystem+<>c__DisplayClass2_0\r\nTestStatements.CS_Concepts.TypeSystem+<>c\r\nTestStatements.CS_Concepts.TypeSystem+<FileModes>d__10\r\nTestStatements.DataTypes.EnumTest+Days\r\nTestStatements.DataTypes.EnumTest+BoilingPoints\r\nTestStatements.DataTypes.EnumTest+Colors\r\nTestStatements.DataTypes.Formating+<>c\r\nTestStatements.Anweisungen.SwitchStatement+Color\r\nTestStatements.Anweisungen.SwitchStatement+<>c\r\nTestStatements.Anweisungen.SwitchStatement+<>c__12`1\r\nTestStatements.Anweisungen.YieldStatement+<Range>d__0\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=6\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=20\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=24\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0+<<ExampleMain3>b__0>d\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0+<<ExampleMain4>b__0>d\r\n\nExample.SampleMethod(42) executes.\r\nSampleMethod returned 84.\r\n\nAssembly entry point:\r\nVoid Main(System.String[])"
#if NET5_0_OR_GREATER
          @"======================================================================
"+@"## Example for Reflection (AssemblyExample)
======================================================================

Assembly Full Name:
TestStatements_net, Version=1.0.1.1234, Culture=neutral, PublicKeyToken=null

Name: TestStatements_net
Version: 1.0

Assembly CodeBase:
C:\Projekte\CSharp\bin\TestStatements_netTest\Debug\" + netversion + @"\TestStatements_net.dll
" +
#if !NET8_0_OR_GREATER
@"Microsoft.CodeAnalysis.EmbeddedAttribute
System.Runtime.CompilerServices.NullableAttribute
System.Runtime.CompilerServices.NullableContextAttribute
System.Runtime.CompilerServices.RefSafetyRulesAttribute
" +
#endif
@"TestStatements.Program
TestStatements.Threading.Tasks.AsyncBreakfast
 '-> .AsyncBreakfast_Main(String[] args)
TestStatements.Threading.Tasks.Juice
TestStatements.Threading.Tasks.Toast
TestStatements.Threading.Tasks.Bacon
TestStatements.Threading.Tasks.Egg
TestStatements.Threading.Tasks.Coffee
TestStatements.Threading.Tasks.IPing
TestStatements.Threading.Tasks.PingProxy
TestStatements.Threading.Tasks.TaskExample
 '-> .ExampleMain()
 '-> .ExampleMain1()
 '-> .ExampleMain2()
 '-> .ExampleMain3()
 '-> .ExampleMain4()
TestStatements.SystemNS.System_Namespace
TestStatements.SystemNS.Xml.XmlNS
 '-> .XmlTest1()
TestStatements.SystemNS.Printing.Printing_Ex
 '-> .PrintDocument()
TestStatements.Runtime.Loader.RTLoaderExample
TestStatements.Runtime.Dynamic.DynamicAssembly
 '-> .CreateAndSaveAssembly()
TestStatements.Reflection.AssemblyExample
 '-> .ExampleMain()
TestStatements.Reflection.S
TestStatements.Reflection.ReflectionExample
 '-> .ExampleMain()
TestStatements.Properties.Resource1
 '-> .set_Culture(CultureInfo value)
TestStatements.Linq.Package
TestStatements.Linq.LinqLookup
 '-> .LookupExample()
 '-> .ShowContains()
 '-> .ShowIEnumerable()
 '-> .ShowCount()
 '-> .ShowGrouping()
TestStatements.Helper.ExtensionExample
 '-> .ShowExtensionEx1()
TestStatements.Helper.Extensions
TestStatements.Diagnostics.DebugExample
 '-> .Main()
 '-> .DebugWriteExample(String v)
TestStatements.Diagnostics.IStopwatch
TestStatements.Diagnostics.StopWatchExample
 '-> .set_GetStopwatch(Func`1 value)
 '-> .set_GetStopwatch2(Func`1 value)
 '-> .set_ThreadSleep(Action`1 value)
 '-> .ExampleMain()
 '-> .ExampleMain1()
 '-> .ExampleMain2()
 '-> .DisplayTimerProperties()
 '-> .TimeOperations()
TestStatements.Diagnostics.StopwatchProxy
TestStatements.DependencyInjection.DIExample
 '-> .Main(String[] args)
TestStatements.DependencyInjection.DIExample2
TestStatements.DependencyInjection.IItem
TestStatements.DependencyInjection.IMessageWriter
TestStatements.DependencyInjection.IObjectProcessor
TestStatements.DependencyInjection.IObjectRelay
TestStatements.DependencyInjection.IObjectStore
TestStatements.DependencyInjection.LoggingMessageWriter
TestStatements.DependencyInjection.MessageWriter
TestStatements.DependencyInjection.Worker
TestStatements.DependencyInjection.Worker2
TestStatements.DataTypes.EnumTest
 '-> .MainTest()
TestStatements.DataTypes.Formating
 '-> .CombinedFormating()
 '-> .IndexKomponent()
 '-> .IndexKomponent2()
 '-> .IndentationKomponent()
 '-> .EscapeSequence()
 '-> .CodeExamples1()
 '-> .CodeExamples2()
 '-> .CodeExamples3()
 '-> .CodeExamples4()
TestStatements.DataTypes.IntegratedTypes
TestStatements.DataTypes.StringEx
 '-> .AllTests()
 '-> .StringEx1()
 '-> .StringEx2()
 '-> .StringEx3()
 '-> .StringEx4()
 '-> .StringEx5()
 '-> .UnicodeEx1()
 '-> .StringSurogarteEx1()
TestStatements.CS_Concepts.TypeSystem
 '-> .All()
 '-> .UseOfTypes()
 '-> .DelareVariables()
 '-> .ValueTypes1()
 '-> .ValueTypes2()
 '-> .ValueTypes3()
 '-> .ValueTypes4()
TestStatements.Constants.Constants
TestStatements.Collection.Generic.ComparerExample
 '-> .ComparerExampleMain(String[] args)
 '-> .ShowSortWithLengthFirstComparer()
 '-> .ShowSortwithDefaultComparer()
 '-> .ShowLengthFirstComparer()
TestStatements.Collection.Generic.BoxLengthFirst
TestStatements.Collection.Generic.BoxComp
TestStatements.Collection.Generic.Box
TestStatements.Collection.Generic.DictionaryExample
 '-> .DictionaryExampleMain()
 '-> .TryAddExisting()
 '-> .ShowIndex1()
 '-> .ShowIndex2()
 '-> .AddValueWithDiffKeys()
 '-> .ShowIndex4()
 '-> .ShowTryGetValue()
 '-> .ShowContainsKey()
 '-> .ShowValueCollection()
 '-> .ShowKeyCollection()
 '-> .ShowRemove()
TestStatements.Collection.Generic.SortedListExample
 '-> .SortedListMain()
 '-> .ShowValues1()
 '-> .ShowValues2()
 '-> .ShowKeys1()
 '-> .ShowKeys2()
 '-> .ShowRemove()
 '-> .ShowForEach()
 '-> .ShowContainsKey()
 '-> .ShowTryGetValue()
 '-> .TestIndexr()
 '-> .TestAddExisting()
TestStatements.Collection.Generic.TestHashSet
 '-> .ShowHashSet()
TestStatements.Collection.Generic.Part
TestStatements.Collection.Generic.TestList
 '-> .ListMain()
 '-> .ShowContains()
 '-> .ShowInsert()
 '-> .ShowIndex()
 '-> .ShowRemove1()
 '-> .ShowRemove2()
TestStatements.Collection.Generic.DinosaurExample
 '-> .ListDinos()
 '-> .ShowCreateData()
 '-> .ShowContains()
 '-> .ShowInsert()
 '-> .ShowItemProperty()
 '-> .ShowRemove()
 '-> .ShowTrimExcess()
 '-> .ShowClear()
 '-> .CreateTestData()
 '-> .ShowList(List`1 dinosaurs)
 '-> .ShowStatus(List`1 dinosaurs)
TestStatements.ClassesAndObjects.IInterface1
TestStatements.ClassesAndObjects.IInterface2
TestStatements.ClassesAndObjects.IInterface3
TestStatements.ClassesAndObjects.Class1
TestStatements.ClassesAndObjects.Class2
TestStatements.ClassesAndObjects.Class3
TestStatements.ClassesAndObjects.Class4
TestStatements.ClassesAndObjects.Class5
TestStatements.ClassesAndObjects.InterfaceTest
 '-> .Run()
TestStatements.ClassesAndObjects.Members
 '-> .set_OnChange(EventHandler value)
 '-> .aMethod()
 '-> .set_aProperty(Int32 value)
TestStatements.Anweisungen.Checking
 '-> .CheckedUnchecked(String[] args)
TestStatements.Anweisungen.ConditionalStatement
 '-> .set_A(Boolean value)
 '-> .set_B(Boolean value)
 '-> .DoIfStatement(String[] args)
 '-> .DoIfStatement2(String[] args)
 '-> .DoIfStatement3()
 '-> .DoSwitchStatement(String[] args)
 '-> .DoSwitchStatement1(String[] args)
TestStatements.Anweisungen.Declarations
 '-> .DoVarDeclarations(String[] args)
 '-> .DoConstantDeclarations(String[] args)
TestStatements.Anweisungen.ExceptionHandling
 '-> .DoTryCatch(String[] args)
 '-> .DoTryFinally(String[] args)
TestStatements.Anweisungen.Expressions
 '-> .DoExpressions(String[] args)
TestStatements.Anweisungen.Account
TestStatements.Anweisungen.Locking
 '-> .DoLockTest(String[] args)
TestStatements.Anweisungen.LoopStatements
 '-> .DoWhileStatement(String[] args)
 '-> .DoDoStatement(String[] args)
 '-> .DoDoStatement2(String[] args)
 '-> .DoDoStatement3(String[] args)
 '-> .DoDoDoStatement(String[] args)
 '-> .DoDoDoStatement2(String[] args)
 '-> .DoDoWhileNestedStatement2(String[] args)
 '-> .DoForStatement(String[] args)
 '-> .DoForStatement2(String[] args)
 '-> .DoForEachStatement(String[] args)
TestStatements.Anweisungen.ProgramFlow
 '-> .DoBreakStatement(String[] args)
 '-> .DoContinueStatement(String[] args)
 '-> .DoGoToStatement(String[] args)
TestStatements.Anweisungen.RandomExample
 '-> .ExampleMain1()
 '-> .ExampleMain2()
 '-> .ExampleMain3()
TestStatements.Anweisungen.ReturnStatement
 '-> .DoReturnStatement(String[] args)
TestStatements.Anweisungen.SwitchStatement
 '-> .SwitchExample1()
 '-> .SwitchExample2()
 '-> .SwitchExample3()
 '-> .SwitchExample4()
 '-> .SwitchExample5()
 '-> .SwitchExample6()
 '-> .SwitchExample7()
TestStatements.Anweisungen.DiceLibrary
TestStatements.Anweisungen.Shape
TestStatements.Anweisungen.Rectangle
TestStatements.Anweisungen.Square
TestStatements.Anweisungen.Circle
TestStatements.Anweisungen.SwitchStatement2
 '-> .SwitchExample21()
 '-> .ShowShapeInfo(Shape sh)
TestStatements.Anweisungen.Vehicle
TestStatements.Anweisungen.Car
TestStatements.Anweisungen.Bus
TestStatements.Anweisungen.Truck
TestStatements.Anweisungen.Taxi
TestStatements.Anweisungen.TollCalculator
TestStatements.Anweisungen.SwitchStatement3
 '-> .ShowTollCollector()
TestStatements.Anweisungen.UsingStatement
 '-> .DoUsingStatement(String[] args)
TestStatements.Anweisungen.YieldStatement
 '-> .DoYieldStatement(String[] args)
<PrivateImplementationDetails>
" +
#if NET9_0_OR_GREATER
@"<>y__InlineArray4`1
" +
#endif
@"TestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main2>d__1
TestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main3>d__2
TestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main4>d__3
TestStatements.Threading.Tasks.AsyncBreakfast+<FryBaconAsync>d__6
TestStatements.Threading.Tasks.AsyncBreakfast+<FryEggsAsync>d__7
TestStatements.Threading.Tasks.AsyncBreakfast+<MakeToastWithButterAndJamAsync>d__4
TestStatements.Threading.Tasks.AsyncBreakfast+<ToastBreadAsync>d__5
TestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_0
TestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_1
TestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_0
TestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_1
TestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0
TestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0
TestStatements.Threading.Tasks.TaskExample+<ExampleMain2a>d__5
TestStatements.Runtime.Loader.RTLoaderExample+<Main>d__0
TestStatements.Reflection.ReflectionExample+NestedClass
TestStatements.Reflection.ReflectionExample+INested
TestStatements.Linq.LinqLookup+<>c
TestStatements.Helper.Extensions+<Convert>d__3`2
TestStatements.Diagnostics.StopWatchExample+<>c
TestStatements.Diagnostics.StopWatchExample+<>o__12
TestStatements.Diagnostics.StopWatchExample+<>o__9
TestStatements.DependencyInjection.Worker+<ExecuteAsync>d__2
TestStatements.DependencyInjection.Worker2+<ExecuteAsync>d__3
TestStatements.DataTypes.EnumTest+Days
TestStatements.DataTypes.EnumTest+BoilingPoints
TestStatements.DataTypes.EnumTest+Colors
TestStatements.DataTypes.Formating+<>c
TestStatements.DataTypes.StringEx+<<StringEx2>g__f|3_0>d
TestStatements.DataTypes.StringEx+<>c
TestStatements.CS_Concepts.TypeSystem+Coords
TestStatements.CS_Concepts.TypeSystem+FileMode
TestStatements.CS_Concepts.TypeSystem+MyClass
TestStatements.CS_Concepts.TypeSystem+<>c
TestStatements.CS_Concepts.TypeSystem+<>c__DisplayClass2_0
TestStatements.CS_Concepts.TypeSystem+<FileModes>d__10
TestStatements.Collection.Generic.TestHashSet+<>c__DisplayClass0_0
TestStatements.Anweisungen.SwitchStatement+Color
TestStatements.Anweisungen.SwitchStatement+<>c
TestStatements.Anweisungen.SwitchStatement+<>c__12`1
TestStatements.Anweisungen.SwitchStatement3+Caprio
TestStatements.Anweisungen.YieldStatement+<Range>d__0
<PrivateImplementationDetails>+__StaticArrayInitTypeSize=6
<PrivateImplementationDetails>+__StaticArrayInitTypeSize=16
<PrivateImplementationDetails>+__StaticArrayInitTypeSize=20
<PrivateImplementationDetails>+__StaticArrayInitTypeSize=24
<PrivateImplementationDetails>+__StaticArrayInitTypeSize=72
TestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0+<<ExampleMain3>b__0>d
TestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0+<<ExampleMain4>b__0>d

Example.SampleMethod(42) executes.
SampleMethod returned 84.

Assembly entry point:
Void Main(System.String[])";
		  //"Assembly Full Name:\r\nTestStatements, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\r\n\nName: TestStatements\r\nVersion: 1.0\r\n\nAssembly CodeBase:\r\nfile:///C:/Projekte/CSharp/bin/Debug/TestStatements.EXE\r\nTestStatements.Program\r\nTestStatements.Threading.Tasks.AsyncBreakfast\r\n '-> .AsyncBreakfast_Main(String[] args)\r\nTestStatements.Threading.Tasks.Juice\r\nTestStatements.Threading.Tasks.Toast\r\nTestStatements.Threading.Tasks.Bacon\r\nTestStatements.Threading.Tasks.Egg\r\nTestStatements.Threading.Tasks.Coffee\r\nTestStatements.Threading.Tasks.TaskExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\n '-> .ExampleMain4()\r\nTestStatements.SystemNS.System_Namespace\r\nTestStatements.Reflection.AssemblyExample\r\n '-> .ExampleMain()\r\nTestStatements.Reflection.S\r\nTestStatements.Reflection.ReflectionExample\r\n '-> .ExampleMain()\r\nTestStatements.Linq.Package\r\nTestStatements.Linq.LinqLookup\r\n '-> .LookupExample()\r\n '-> .ShowContains()\r\n '-> .ShowIEnumerable()\r\n '-> .ShowCount()\r\n '-> .ShowGrouping()\r\nTestStatements.Diagnostics.StopWatchExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .DisplayTimerProperties()\r\nTestStatements.CS_Concepts.TypeSystem\r\n '-> .All()\r\n '-> .UseOfTypes()\r\n '-> .DelareVariables()\r\n '-> .ValueTypes1()\r\n '-> .ValueTypes2()\r\n '-> .ValueTypes3()\r\n '-> .ValueTypes4()\r\nTestStatements.DataTypes.EnumTest\r\n '-> .MainTest()\r\nTestStatements.DataTypes.Formating\r\n '-> .CombinedFormating()\r\n '-> .IndexKomponent()\r\n '-> .IndexKomponent2()\r\n '-> .IndentationKomponent()\r\n '-> .EscapeSequence()\r\n '-> .CodeExamples1()\r\n '-> .CodeExamples2()\r\n '-> .CodeExamples3()\r\n '-> .CodeExamples4()\r\nTestStatements.DataTypes.IntegratedTypes\r\nTestStatements.DataTypes.StringEx\r\n '-> .AllTests()\r\n '-> .StringEx1()\r\n '-> .StringEx2()\r\n '-> .StringEx3()\r\n '-> .StringEx4()\r\n '-> .StringEx5()\r\n '-> .UnicodeEx1()\r\n '-> .StringSurogarteEx1()\r\nTestStatements.Constants.Constants\r\nTestStatements.Collection.Generic.ComparerExample\r\n '-> .ComparerExampleMain(String[] args)\r\n '-> .ShowSortWithLengthFirstComparer()\r\n '-> .ShowSortwithDefaultComparer()\r\n '-> .ShowLengthFirstComparer()\r\nTestStatements.Collection.Generic.BoxLengthFirst\r\nTestStatements.Collection.Generic.BoxComp\r\nTestStatements.Collection.Generic.Box\r\nTestStatements.Collection.Generic.DictionaryExample\r\n '-> .DictionaryExampleMain()\r\n '-> .TryAddExisting()\r\n '-> .ShowIndex1()\r\n '-> .ShowIndex2()\r\n '-> .ShowIndex3()\r\n '-> .ShowIndex4()\r\n '-> .ShowTryGetValue()\r\n '-> .ShowContainsKey()\r\n '-> .ShowValueCollection()\r\n '-> .ShowKeyCollection()\r\n '-> .ShowRemove()\r\nTestStatements.Collection.Generic.SortedListExample\r\n '-> .SortedListMain()\r\n '-> .ShowValues1()\r\n '-> .ShowValues2()\r\n '-> .ShowKeys1()\r\n '-> .ShowKeys2()\r\n '-> .ShowRemove()\r\n '-> .ShowForEach()\r\n '-> .ShowContainsKey()\r\n '-> .ShowTryGetValue()\r\n '-> .TestIndexr()\r\n '-> .TestAddExisting()\r\nTestStatements.Collection.Generic.TestHashSet\r\n '-> .ShowHashSet()\r\nTestStatements.Collection.Generic.Part\r\nTestStatements.Collection.Generic.TestList\r\n '-> .ListMain()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowIndex()\r\n '-> .ShowRemove1()\r\n '-> .ShowRemove2()\r\nTestStatements.Collection.Generic.DinosaurExample\r\n '-> .ListDinos()\r\n '-> .ShowCreateData()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowItemProperty()\r\n '-> .ShowRemove()\r\n '-> .ShowTrimExcess()\r\n '-> .ShowClear()\r\nTestStatements.ClassesAndObjects.Members\r\n '-> .set_OnChange(EventHandler value)\r\n '-> .aMethod()\r\n '-> .set_aProperty(Int32 value)\r\nTestStatements.Anweisungen.Checking\r\n '-> .CheckedUnchecked(String[] args)\r\nTestStatements.Anweisungen.ExceptionHandling\r\n '-> .DoTryCatch(String[] args)\r\nTestStatements.Anweisungen.Account\r\nTestStatements.Anweisungen.Locking\r\n '-> .DoLockTest(String[] args)\r\nTestStatements.Anweisungen.ProgramFlow\r\n '-> .DoBreakStatement(String[] args)\r\n '-> .DoContinueStatement(String[] args)\r\n '-> .DoGoToStatement(String[] args)\r\nTestStatements.Anweisungen.Expressions\r\n '-> .DoExpressions(String[] args)\r\nTestStatements.Anweisungen.Declarations\r\n '-> .DoVarDeclarations(String[] args)\r\n '-> .DoConstantDeclarations(String[] args)\r\nTestStatements.Anweisungen.ConditionalStatement\r\n '-> .DoIfStatement(String[] args)\r\n '-> .DoIfStatement2(String[] args)\r\n '-> .DoIfStatement3()\r\n '-> .DoSwitchStatement(String[] args)\r\n '-> .DoSwitchStatement1()\r\nTestStatements.Anweisungen.LoopStatements\r\n '-> .DoWhileStatement(String[] args)\r\n '-> .DoDoStatement(String[] args)\r\n '-> .DoDoStatement2(String[] args)\r\n '-> .DoDoStatement3(String[] args)\r\n '-> .DoDoDoStatement(String[] args)\r\n '-> .DoDoDoStatement2(String[] args)\r\n '-> .DoDoWhileNestedStatement2(String[] args)\r\n '-> .DoForStatement(String[] args)\r\n '-> .DoForStatement2(String[] args)\r\n '-> .DoForEachStatement(String[] args)\r\nTestStatements.Anweisungen.RandomExample\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\nTestStatements.Anweisungen.ReturnStatement\r\n '-> .DoReturnStatement(String[] args)\r\nTestStatements.Anweisungen.SwitchStatement\r\n '-> .SwitchExample1()\r\n '-> .SwitchExample2()\r\n '-> .SwitchExample3()\r\n '-> .SwitchExample4()\r\n '-> .SwitchExample5()\r\n '-> .SwitchExample6()\r\n '-> .SwitchExample7()\r\nTestStatements.Anweisungen.DiceLibrary\r\nTestStatements.Anweisungen.Shape\r\nTestStatements.Anweisungen.Rectangle\r\nTestStatements.Anweisungen.Square\r\nTestStatements.Anweisungen.Circle\r\nTestStatements.Anweisungen.SwitchStatement2\r\n '-> .SwitchExample21()\r\nTestStatements.Anweisungen.YieldStatement\r\n '-> .DoYieldStatement(String[] args)\r\nTestStatements.Anweisungen.UsingStatement\r\n '-> .DoUsingStatement(String[] args)\r\n<PrivateImplementationDetails>\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main2>d__1\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main3>d__2\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main4>d__3\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<MakeToastWithButterAndJamAsync>d__4\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<ToastBreadAsync>d__5\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<FryBaconAsync>d__6\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<FryEggsAsync>d__7\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_1\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_1\r\nTestStatements.Threading.Tasks.TaskExample+<ExampleMain2a>d__5\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0\r\nTestStatements.Reflection.ReflectionExample+NestedClass\r\nTestStatements.Reflection.ReflectionExample+INested\r\nTestStatements.Linq.LinqLookup+<>c\r\nTestStatements.CS_Concepts.TypeSystem+Coords\r\nTestStatements.CS_Concepts.TypeSystem+FileMode\r\nTestStatements.CS_Concepts.TypeSystem+MyClass\r\nTestStatements.CS_Concepts.TypeSystem+<>c__DisplayClass2_0\r\nTestStatements.CS_Concepts.TypeSystem+<>c\r\nTestStatements.CS_Concepts.TypeSystem+<FileModes>d__10\r\nTestStatements.DataTypes.EnumTest+Days\r\nTestStatements.DataTypes.EnumTest+BoilingPoints\r\nTestStatements.DataTypes.EnumTest+Colors\r\nTestStatements.DataTypes.Formating+<>c\r\nTestStatements.Anweisungen.SwitchStatement+Color\r\nTestStatements.Anweisungen.SwitchStatement+<>c\r\nTestStatements.Anweisungen.SwitchStatement+<>c__12`1\r\nTestStatements.Anweisungen.YieldStatement+<Range>d__0\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=6\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=20\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=24\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0+<<ExampleMain3>b__0>d\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0+<<ExampleMain4>b__0>d\r\n\nExample.SampleMethod(42) executes.\r\nSampleMethod returned 84.\r\n\nAssembly entry point:\r\nVoid Main(System.String[])";
#else
          //"Assembly Full Name:\r\nTestStatements, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\r\n\nName: TestStatements\r\nVersion: 1.0\r\n\nAssembly CodeBase:\r\nfile:///D:/Projekte/CSharp/bin/Debug/TestStatements.EXE\r\nTestStatements.Program\r\nTestStatements.Resource1\r\nTestStatements.Threading.Tasks.AsyncBreakfast\r\n '-> .AsyncBreakfast_Main(String[] args)\r\nTestStatements.Threading.Tasks.Juice\r\nTestStatements.Threading.Tasks.Toast\r\nTestStatements.Threading.Tasks.Bacon\r\nTestStatements.Threading.Tasks.Egg\r\nTestStatements.Threading.Tasks.Coffee\r\nTestStatements.Threading.Tasks.TaskExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\n '-> .ExampleMain4()\r\nTestStatements.SystemNS.System_Namespace\r\nTestStatements.SystemNS.Xml.XmlNS\r\n '-> .XmlTest1()\r\nTestStatements.Reflection.AssemblyExample\r\n '-> .ExampleMain()\r\nTestStatements.Reflection.S\r\nTestStatements.Reflection.ReflectionExample\r\n '-> .ExampleMain()\r\nTestStatements.Linq.Package\r\nTestStatements.Linq.LinqLookup\r\n '-> .LookupExample()\r\n '-> .ShowContains()\r\n '-> .ShowIEnumerable()\r\n '-> .ShowCount()\r\n '-> .ShowGrouping()\r\nTestStatements.Diagnostics.StopWatchExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .DisplayTimerProperties()\r\nTestStatements.CS_Concepts.TypeSystem\r\n '-> .All()\r\n '-> .UseOfTypes()\r\n '-> .DelareVariables()\r\n '-> .ValueTypes1()\r\n '-> .ValueTypes2()\r\n '-> .ValueTypes3()\r\n '-> .ValueTypes4()\r\nTestStatements.DataTypes.EnumTest\r\n '-> .MainTest()\r\nTestStatements.DataTypes.Formating\r\n '-> .CombinedFormating()\r\n '-> .IndexKomponent()\r\n '-> .IndexKomponent2()\r\n '-> .IndentationKomponent()\r\n '-> .EscapeSequence()\r\n '-> .CodeExamples1()\r\n '-> .CodeExamples2()\r\n '-> .CodeExamples3()\r\n '-> .CodeExamples4()\r\nTestStatements.DataTypes.IntegratedTypes\r\nTestStatements.DataTypes.StringEx\r\n '-> .AllTests()\r\n '-> .StringEx1()\r\n '-> .StringEx2()\r\n '-> .StringEx3()\r\n '-> .StringEx4()\r\n '-> .StringEx5()\r\n '-> .UnicodeEx1()\r\n '-> .StringSurogarteEx1()\r\nTestStatements.Constants.Constants\r\nTestStatements.Collection.Generic.ComparerExample\r\n '-> .ComparerExampleMain(String[] args)\r\n '-> .ShowSortWithLengthFirstComparer()\r\n '-> .ShowSortwithDefaultComparer()\r\n '-> .ShowLengthFirstComparer()\r\nTestStatements.Collection.Generic.BoxLengthFirst\r\nTestStatements.Collection.Generic.BoxComp\r\nTestStatements.Collection.Generic.Box\r\nTestStatements.Collection.Generic.DictionaryExample\r\n '-> .DictionaryExampleMain()\r\n '-> .TryAddExisting()\r\n '-> .ShowIndex1()\r\n '-> .ShowIndex2()\r\n '-> .ShowIndex3()\r\n '-> .ShowIndex4()\r\n '-> .ShowTryGetValue()\r\n '-> .ShowContainsKey()\r\n '-> .ShowValueCollection()\r\n '-> .ShowKeyCollection()\r\n '-> .ShowRemove()\r\nTestStatements.Collection.Generic.SortedListExample\r\n '-> .SortedListMain()\r\n '-> .ShowValues1()\r\n '-> .ShowValues2()\r\n '-> .ShowKeys1()\r\n '-> .ShowKeys2()\r\n '-> .ShowRemove()\r\n '-> .ShowForEach()\r\n '-> .ShowContainsKey()\r\n '-> .ShowTryGetValue()\r\n '-> .TestIndexr()\r\n '-> .TestAddExisting()\r\nTestStatements.Collection.Generic.TestHashSet\r\n '-> .ShowHashSet()\r\nTestStatements.Collection.Generic.Part\r\nTestStatements.Collection.Generic.TestList\r\n '-> .ListMain()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowIndex()\r\n '-> .ShowRemove1()\r\n '-> .ShowRemove2()\r\nTestStatements.Collection.Generic.DinosaurExample\r\n '-> .ListDinos()\r\n '-> .ShowCreateData()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowItemProperty()\r\n '-> .ShowRemove()\r\n '-> .ShowTrimExcess()\r\n '-> .ShowClear()\r\nTestStatements.ClassesAndObjects.Members\r\n '-> .set_OnChange(EventHandler value)\r\n '-> .aMethod()\r\n '-> .set_aProperty(Int32 value)\r\nTestStatements.Anweisungen.Checking\r\n '-> .CheckedUnchecked(String[] args)\r\nTestStatements.Anweisungen.ExceptionHandling\r\n '-> .DoTryCatch(String[] args)\r\nTestStatements.Anweisungen.Account\r\nTestStatements.Anweisungen.Locking\r\n '-> .DoLockTest(String[] args)\r\nTestStatements.Anweisungen.ProgramFlow\r\n '-> .DoBreakStatement(String[] args)\r\n '-> .DoContinueStatement(String[] args)\r\n '-> .DoGoToStatement(String[] args)\r\nTestStatements.Anweisungen.Expressions\r\n '-> .DoExpressions(String[] args)\r\nTestStatements.Anweisungen.Declarations\r\n '-> .DoVarDeclarations(String[] args)\r\n '-> .DoConstantDeclarations(String[] args)\r\nTestStatements.Anweisungen.ConditionalStatement\r\n '-> .DoIfStatement(String[] args)\r\n '-> .DoIfStatement2(String[] args)\r\n '-> .DoIfStatement3()\r\n '-> .DoSwitchStatement(String[] args)\r\n '-> .DoSwitchStatement1()\r\nTestStatements.Anweisungen.LoopStatements\r\n '-> .DoWhileStatement(String[] args)\r\n '-> .DoDoStatement(String[] args)\r\n '-> .DoDoStatement2(String[] args)\r\n '-> .DoDoStatement3(String[] args)\r\n '-> .DoDoDoStatement(String[] args)\r\n '-> .DoDoDoStatement2(String[] args)\r\n '-> .DoDoWhileNestedStatement2(String[] args)\r\n '-> .DoForStatement(String[] args)\r\n '-> .DoForStatement2(String[] args)\r\n '-> .DoForEachStatement(String[] args)\r\nTestStatements.Anweisungen.RandomExample\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\nTestStatements.Anweisungen.ReturnStatement\r\n '-> .DoReturnStatement(String[] args)\r\nTestStatements.Anweisungen.SwitchStatement\r\n '-> .SwitchExample1()\r\n '-> .SwitchExample2()\r\n '-> .SwitchExample3()\r\n '-> .SwitchExample4()\r\n '-> .SwitchExample5()\r\n '-> .SwitchExample6()\r\n '-> .SwitchExample7()\r\nTestStatements.Anweisungen.DiceLibrary\r\nTestStatements.Anweisungen.Shape\r\nTestStatements.Anweisungen.Rectangle\r\nTestStatements.Anweisungen.Square\r\nTestStatements.Anweisungen.Circle\r\nTestStatements.Anweisungen.SwitchStatement2\r\n '-> .SwitchExample21()\r\nTestStatements.Anweisungen.YieldStatement\r\n '-> .DoYieldStatement(String[] args)\r\nTestStatements.Anweisungen.UsingStatement\r\n '-> .DoUsingStatement(String[] args)\r\n<PrivateImplementationDetails>\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main2>d__1\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main3>d__2\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main4>d__3\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<FryBaconAsync>d__6\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<FryEggsAsync>d__7\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<MakeToastWithButterAndJamAsync>d__4\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<ToastBreadAsync>d__5\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_1\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_1\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0\r\nTestStatements.Threading.Tasks.TaskExample+<ExampleMain2a>d__5\r\nTestStatements.Reflection.ReflectionExample+NestedClass\r\nTestStatements.Reflection.ReflectionExample+INested\r\nTestStatements.Linq.LinqLookup+<>c\r\nTestStatements.CS_Concepts.TypeSystem+Coords\r\nTestStatements.CS_Concepts.TypeSystem+FileMode\r\nTestStatements.CS_Concepts.TypeSystem+MyClass\r\nTestStatements.CS_Concepts.TypeSystem+<>c\r\nTestStatements.CS_Concepts.TypeSystem+<>c__DisplayClass2_0\r\nTestStatements.CS_Concepts.TypeSystem+<FileModes>d__10\r\nTestStatements.DataTypes.EnumTest+Days\r\nTestStatements.DataTypes.EnumTest+BoilingPoints\r\nTestStatements.DataTypes.EnumTest+Colors\r\nTestStatements.DataTypes.Formating+<>c\r\nTestStatements.Anweisungen.SwitchStatement+Color\r\nTestStatements.Anweisungen.SwitchStatement+<>c\r\nTestStatements.Anweisungen.SwitchStatement+<>c__12`1\r\nTestStatements.Anweisungen.YieldStatement+<Range>d__0\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=6\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=20\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=24\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0+<<ExampleMain3>b__0>d\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0+<<ExampleMain4>b__0>d\r\n\nExample.SampleMethod(42) executes.\r\nSampleMethod returned 84.\r\n\nAssembly entry point:\r\nVoid Main(System.String[])";
          //"Assembly Full Name:\r\nTestStatements, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\r\n\nName: TestStatements\r\nVersion: 1.0\r\n\nAssembly CodeBase:\r\nfile:///C:/Projekte/CSharp/bin/Debug/TestStatements.EXE\r\nTestStatements.Program\r\nTestStatements.Threading.Tasks.AsyncBreakfast\r\n '-> .AsyncBreakfast_Main(String[] args)\r\nTestStatements.Threading.Tasks.Juice\r\nTestStatements.Threading.Tasks.Toast\r\nTestStatements.Threading.Tasks.Bacon\r\nTestStatements.Threading.Tasks.Egg\r\nTestStatements.Threading.Tasks.Coffee\r\nTestStatements.Threading.Tasks.TaskExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\n '-> .ExampleMain4()\r\nTestStatements.SystemNS.System_Namespace\r\nTestStatements.Reflection.AssemblyExample\r\n '-> .ExampleMain()\r\nTestStatements.Reflection.S\r\nTestStatements.Reflection.ReflectionExample\r\n '-> .ExampleMain()\r\nTestStatements.Linq.Package\r\nTestStatements.Linq.LinqLookup\r\n '-> .LookupExample()\r\n '-> .ShowContains()\r\n '-> .ShowIEnumerable()\r\n '-> .ShowCount()\r\n '-> .ShowGrouping()\r\nTestStatements.Diagnostics.StopWatchExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .DisplayTimerProperties()\r\nTestStatements.CS_Concepts.TypeSystem\r\n '-> .All()\r\n '-> .UseOfTypes()\r\n '-> .DelareVariables()\r\n '-> .ValueTypes1()\r\n '-> .ValueTypes2()\r\n '-> .ValueTypes3()\r\n '-> .ValueTypes4()\r\nTestStatements.DataTypes.EnumTest\r\n '-> .MainTest()\r\nTestStatements.DataTypes.Formating\r\n '-> .CombinedFormating()\r\n '-> .IndexKomponent()\r\n '-> .IndexKomponent2()\r\n '-> .IndentationKomponent()\r\n '-> .EscapeSequence()\r\n '-> .CodeExamples1()\r\n '-> .CodeExamples2()\r\n '-> .CodeExamples3()\r\n '-> .CodeExamples4()\r\nTestStatements.DataTypes.IntegratedTypes\r\nTestStatements.DataTypes.StringEx\r\n '-> .AllTests()\r\n '-> .StringEx1()\r\n '-> .StringEx2()\r\n '-> .StringEx3()\r\n '-> .StringEx4()\r\n '-> .StringEx5()\r\n '-> .UnicodeEx1()\r\n '-> .StringSurogarteEx1()\r\nTestStatements.Constants.Constants\r\nTestStatements.Collection.Generic.ComparerExample\r\n '-> .ComparerExampleMain(String[] args)\r\n '-> .ShowSortWithLengthFirstComparer()\r\n '-> .ShowSortwithDefaultComparer()\r\n '-> .ShowLengthFirstComparer()\r\nTestStatements.Collection.Generic.BoxLengthFirst\r\nTestStatements.Collection.Generic.BoxComp\r\nTestStatements.Collection.Generic.Box\r\nTestStatements.Collection.Generic.DictionaryExample\r\n '-> .DictionaryExampleMain()\r\n '-> .TryAddExisting()\r\n '-> .ShowIndex1()\r\n '-> .ShowIndex2()\r\n '-> .ShowIndex3()\r\n '-> .ShowIndex4()\r\n '-> .ShowTryGetValue()\r\n '-> .ShowContainsKey()\r\n '-> .ShowValueCollection()\r\n '-> .ShowKeyCollection()\r\n '-> .ShowRemove()\r\nTestStatements.Collection.Generic.SortedListExample\r\n '-> .SortedListMain()\r\n '-> .ShowValues1()\r\n '-> .ShowValues2()\r\n '-> .ShowKeys1()\r\n '-> .ShowKeys2()\r\n '-> .ShowRemove()\r\n '-> .ShowForEach()\r\n '-> .ShowContainsKey()\r\n '-> .ShowTryGetValue()\r\n '-> .TestIndexr()\r\n '-> .TestAddExisting()\r\nTestStatements.Collection.Generic.TestHashSet\r\n '-> .ShowHashSet()\r\nTestStatements.Collection.Generic.Part\r\nTestStatements.Collection.Generic.TestList\r\n '-> .ListMain()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowIndex()\r\n '-> .ShowRemove1()\r\n '-> .ShowRemove2()\r\nTestStatements.Collection.Generic.DinosaurExample\r\n '-> .ListDinos()\r\n '-> .ShowCreateData()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowItemProperty()\r\n '-> .ShowRemove()\r\n '-> .ShowTrimExcess()\r\n '-> .ShowClear()\r\nTestStatements.ClassesAndObjects.Members\r\n '-> .set_OnChange(EventHandler value)\r\n '-> .aMethod()\r\n '-> .set_aProperty(Int32 value)\r\nTestStatements.Anweisungen.Checking\r\n '-> .CheckedUnchecked(String[] args)\r\nTestStatements.Anweisungen.ExceptionHandling\r\n '-> .DoTryCatch(String[] args)\r\nTestStatements.Anweisungen.Account\r\nTestStatements.Anweisungen.Locking\r\n '-> .DoLockTest(String[] args)\r\nTestStatements.Anweisungen.ProgramFlow\r\n '-> .DoBreakStatement(String[] args)\r\n '-> .DoContinueStatement(String[] args)\r\n '-> .DoGoToStatement(String[] args)\r\nTestStatements.Anweisungen.Expressions\r\n '-> .DoExpressions(String[] args)\r\nTestStatements.Anweisungen.Declarations\r\n '-> .DoVarDeclarations(String[] args)\r\n '-> .DoConstantDeclarations(String[] args)\r\nTestStatements.Anweisungen.ConditionalStatement\r\n '-> .DoIfStatement(String[] args)\r\n '-> .DoIfStatement2(String[] args)\r\n '-> .DoIfStatement3()\r\n '-> .DoSwitchStatement(String[] args)\r\n '-> .DoSwitchStatement1()\r\nTestStatements.Anweisungen.LoopStatements\r\n '-> .DoWhileStatement(String[] args)\r\n '-> .DoDoStatement(String[] args)\r\n '-> .DoDoStatement2(String[] args)\r\n '-> .DoDoStatement3(String[] args)\r\n '-> .DoDoDoStatement(String[] args)\r\n '-> .DoDoDoStatement2(String[] args)\r\n '-> .DoDoWhileNestedStatement2(String[] args)\r\n '-> .DoForStatement(String[] args)\r\n '-> .DoForStatement2(String[] args)\r\n '-> .DoForEachStatement(String[] args)\r\nTestStatements.Anweisungen.RandomExample\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\nTestStatements.Anweisungen.ReturnStatement\r\n '-> .DoReturnStatement(String[] args)\r\nTestStatements.Anweisungen.SwitchStatement\r\n '-> .SwitchExample1()\r\n '-> .SwitchExample2()\r\n '-> .SwitchExample3()\r\n '-> .SwitchExample4()\r\n '-> .SwitchExample5()\r\n '-> .SwitchExample6()\r\n '-> .SwitchExample7()\r\nTestStatements.Anweisungen.DiceLibrary\r\nTestStatements.Anweisungen.Shape\r\nTestStatements.Anweisungen.Rectangle\r\nTestStatements.Anweisungen.Square\r\nTestStatements.Anweisungen.Circle\r\nTestStatements.Anweisungen.SwitchStatement2\r\n '-> .SwitchExample21()\r\nTestStatements.Anweisungen.YieldStatement\r\n '-> .DoYieldStatement(String[] args)\r\nTestStatements.Anweisungen.UsingStatement\r\n '-> .DoUsingStatement(String[] args)\r\n<PrivateImplementationDetails>\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main2>d__1\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main3>d__2\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main4>d__3\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<MakeToastWithButterAndJamAsync>d__4\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<ToastBreadAsync>d__5\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<FryBaconAsync>d__6\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<FryEggsAsync>d__7\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_1\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_1\r\nTestStatements.Threading.Tasks.TaskExample+<ExampleMain2a>d__5\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0\r\nTestStatements.Reflection.ReflectionExample+NestedClass\r\nTestStatements.Reflection.ReflectionExample+INested\r\nTestStatements.Linq.LinqLookup+<>c\r\nTestStatements.CS_Concepts.TypeSystem+Coords\r\nTestStatements.CS_Concepts.TypeSystem+FileMode\r\nTestStatements.CS_Concepts.TypeSystem+MyClass\r\nTestStatements.CS_Concepts.TypeSystem+<>c__DisplayClass2_0\r\nTestStatements.CS_Concepts.TypeSystem+<>c\r\nTestStatements.CS_Concepts.TypeSystem+<FileModes>d__10\r\nTestStatements.DataTypes.EnumTest+Days\r\nTestStatements.DataTypes.EnumTest+BoilingPoints\r\nTestStatements.DataTypes.EnumTest+Colors\r\nTestStatements.DataTypes.Formating+<>c\r\nTestStatements.Anweisungen.SwitchStatement+Color\r\nTestStatements.Anweisungen.SwitchStatement+<>c\r\nTestStatements.Anweisungen.SwitchStatement+<>c__12`1\r\nTestStatements.Anweisungen.YieldStatement+<Range>d__0\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=6\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=20\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=24\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0+<<ExampleMain3>b__0>d\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0+<<ExampleMain4>b__0>d\r\n\nExample.SampleMethod(42) executes.\r\nSampleMethod returned 84.\r\n\nAssembly entry point:\r\nVoid Main(System.String[])";
          "Assembly Full Name:\r\nTestStatements, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\r\n\nName: TestStatements\r\nVersion: 1.0\r\n\nAssembly CodeBase:\r\nfile:///C:/Projekte/CSharp/bin/TestStatementsTest/Debug/TestStatements.EXE\r\nTestStatements.Program\r\nTestStatements.Resource1\r\nTestStatements.Threading.Tasks.AsyncBreakfast\r\n '-> .AsyncBreakfast_Main(String[] args)\r\nTestStatements.Threading.Tasks.Juice\r\nTestStatements.Threading.Tasks.Toast\r\nTestStatements.Threading.Tasks.Bacon\r\nTestStatements.Threading.Tasks.Egg\r\nTestStatements.Threading.Tasks.Coffee\r\nTestStatements.Threading.Tasks.TaskExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\n '-> .ExampleMain4()\r\nTestStatements.SystemNS.System_Namespace\r\nTestStatements.SystemNS.Xml.XmlNS\r\n '-> .XmlTest1()\r\nTestStatements.Reflection.AssemblyExample\r\n '-> .ExampleMain()\r\nTestStatements.Reflection.S\r\nTestStatements.Reflection.ReflectionExample\r\n '-> .ExampleMain()\r\nTestStatements.Linq.Package\r\nTestStatements.Linq.LinqLookup\r\n '-> .LookupExample()\r\n '-> .ShowContains()\r\n '-> .ShowIEnumerable()\r\n '-> .ShowCount()\r\n '-> .ShowGrouping()\r\nTestStatements.Diagnostics.StopWatchExample\r\n '-> .ExampleMain()\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .DisplayTimerProperties()\r\nTestStatements.CS_Concepts.TypeSystem\r\n '-> .All()\r\n '-> .UseOfTypes()\r\n '-> .DelareVariables()\r\n '-> .ValueTypes1()\r\n '-> .ValueTypes2()\r\n '-> .ValueTypes3()\r\n '-> .ValueTypes4()\r\nTestStatements.DataTypes.EnumTest\r\n '-> .MainTest()\r\nTestStatements.DataTypes.Formating\r\n '-> .CombinedFormating()\r\n '-> .IndexKomponent()\r\n '-> .IndexKomponent2()\r\n '-> .IndentationKomponent()\r\n '-> .EscapeSequence()\r\n '-> .CodeExamples1()\r\n '-> .CodeExamples2()\r\n '-> .CodeExamples3()\r\n '-> .CodeExamples4()\r\nTestStatements.DataTypes.IntegratedTypes\r\nTestStatements.DataTypes.StringEx\r\n '-> .AllTests()\r\n '-> .StringEx1()\r\n '-> .StringEx2()\r\n '-> .StringEx3()\r\n '-> .StringEx4()\r\n '-> .StringEx5()\r\n '-> .UnicodeEx1()\r\n '-> .StringSurogarteEx1()\r\nTestStatements.Constants.Constants\r\nTestStatements.Collection.Generic.ComparerExample\r\n '-> .ComparerExampleMain(String[] args)\r\n '-> .ShowSortWithLengthFirstComparer()\r\n '-> .ShowSortwithDefaultComparer()\r\n '-> .ShowLengthFirstComparer()\r\nTestStatements.Collection.Generic.BoxLengthFirst\r\nTestStatements.Collection.Generic.BoxComp\r\nTestStatements.Collection.Generic.Box\r\nTestStatements.Collection.Generic.DictionaryExample\r\n '-> .DictionaryExampleMain()\r\n '-> .TryAddExisting()\r\n '-> .ShowIndex1()\r\n '-> .ShowIndex2()\r\n '-> .ShowIndex3()\r\n '-> .ShowIndex4()\r\n '-> .ShowTryGetValue()\r\n '-> .ShowContainsKey()\r\n '-> .ShowValueCollection()\r\n '-> .ShowKeyCollection()\r\n '-> .ShowRemove()\r\nTestStatements.Collection.Generic.SortedListExample\r\n '-> .SortedListMain()\r\n '-> .ShowValues1()\r\n '-> .ShowValues2()\r\n '-> .ShowKeys1()\r\n '-> .ShowKeys2()\r\n '-> .ShowRemove()\r\n '-> .ShowForEach()\r\n '-> .ShowContainsKey()\r\n '-> .ShowTryGetValue()\r\n '-> .TestIndexr()\r\n '-> .TestAddExisting()\r\nTestStatements.Collection.Generic.TestHashSet\r\n '-> .ShowHashSet()\r\nTestStatements.Collection.Generic.Part\r\nTestStatements.Collection.Generic.TestList\r\n '-> .ListMain()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowIndex()\r\n '-> .ShowRemove1()\r\n '-> .ShowRemove2()\r\nTestStatements.Collection.Generic.DinosaurExample\r\n '-> .ListDinos()\r\n '-> .ShowCreateData()\r\n '-> .ShowContains()\r\n '-> .ShowInsert()\r\n '-> .ShowItemProperty()\r\n '-> .ShowRemove()\r\n '-> .ShowTrimExcess()\r\n '-> .ShowClear()\r\nTestStatements.ClassesAndObjects.IInterface1\r\nTestStatements.ClassesAndObjects.IInterface2\r\nTestStatements.ClassesAndObjects.IInterface3\r\nTestStatements.ClassesAndObjects.Class1\r\nTestStatements.ClassesAndObjects.Class2\r\nTestStatements.ClassesAndObjects.Class3\r\nTestStatements.ClassesAndObjects.Class4\r\nTestStatements.ClassesAndObjects.Class5\r\nTestStatements.ClassesAndObjects.InterfaceTest\r\n '-> .Run()\r\nTestStatements.ClassesAndObjects.Members\r\n '-> .set_OnChange(EventHandler value)\r\n '-> .aMethod()\r\n '-> .set_aProperty(Int32 value)\r\nTestStatements.Anweisungen.Checking\r\n '-> .CheckedUnchecked(String[] args)\r\nTestStatements.Anweisungen.ExceptionHandling\r\n '-> .DoTryCatch(String[] args)\r\nTestStatements.Anweisungen.Account\r\nTestStatements.Anweisungen.Locking\r\n '-> .DoLockTest(String[] args)\r\nTestStatements.Anweisungen.ProgramFlow\r\n '-> .DoBreakStatement(String[] args)\r\n '-> .DoContinueStatement(String[] args)\r\n '-> .DoGoToStatement(String[] args)\r\nTestStatements.Anweisungen.Expressions\r\n '-> .DoExpressions(String[] args)\r\nTestStatements.Anweisungen.Declarations\r\n '-> .DoVarDeclarations(String[] args)\r\n '-> .DoConstantDeclarations(String[] args)\r\nTestStatements.Anweisungen.ConditionalStatement\r\n '-> .DoIfStatement(String[] args)\r\n '-> .DoIfStatement2(String[] args)\r\n '-> .DoIfStatement3()\r\n '-> .DoSwitchStatement(String[] args)\r\n '-> .DoSwitchStatement1()\r\nTestStatements.Anweisungen.LoopStatements\r\n '-> .DoWhileStatement(String[] args)\r\n '-> .DoDoStatement(String[] args)\r\n '-> .DoDoStatement2(String[] args)\r\n '-> .DoDoStatement3(String[] args)\r\n '-> .DoDoDoStatement(String[] args)\r\n '-> .DoDoDoStatement2(String[] args)\r\n '-> .DoDoWhileNestedStatement2(String[] args)\r\n '-> .DoForStatement(String[] args)\r\n '-> .DoForStatement2(String[] args)\r\n '-> .DoForEachStatement(String[] args)\r\nTestStatements.Anweisungen.RandomExample\r\n '-> .ExampleMain1()\r\n '-> .ExampleMain2()\r\n '-> .ExampleMain3()\r\nTestStatements.Anweisungen.ReturnStatement\r\n '-> .DoReturnStatement(String[] args)\r\nTestStatements.Anweisungen.SwitchStatement\r\n '-> .SwitchExample1()\r\n '-> .SwitchExample2()\r\n '-> .SwitchExample3()\r\n '-> .SwitchExample4()\r\n '-> .SwitchExample5()\r\n '-> .SwitchExample6()\r\n '-> .SwitchExample7()\r\nTestStatements.Anweisungen.DiceLibrary\r\nTestStatements.Anweisungen.Shape\r\nTestStatements.Anweisungen.Rectangle\r\nTestStatements.Anweisungen.Square\r\nTestStatements.Anweisungen.Circle\r\nTestStatements.Anweisungen.SwitchStatement2\r\n '-> .SwitchExample21()\r\nTestStatements.Anweisungen.YieldStatement\r\n '-> .DoYieldStatement(String[] args)\r\nTestStatements.Anweisungen.UsingStatement\r\n '-> .DoUsingStatement(String[] args)\r\n<PrivateImplementationDetails>\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main2>d__1\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main3>d__2\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<AsyncBreakfast_Main4>d__3\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<FryBaconAsync>d__6\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<FryEggsAsync>d__7\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<MakeToastWithButterAndJamAsync>d__4\r\nTestStatements.Threading.Tasks.AsyncBreakfast+<ToastBreadAsync>d__5\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass3_1\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass5_1\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0\r\nTestStatements.Threading.Tasks.TaskExample+<ExampleMain2a>d__5\r\nTestStatements.Reflection.ReflectionExample+NestedClass\r\nTestStatements.Reflection.ReflectionExample+INested\r\nTestStatements.Linq.LinqLookup+<>c\r\nTestStatements.CS_Concepts.TypeSystem+Coords\r\nTestStatements.CS_Concepts.TypeSystem+FileMode\r\nTestStatements.CS_Concepts.TypeSystem+MyClass\r\nTestStatements.CS_Concepts.TypeSystem+<>c\r\nTestStatements.CS_Concepts.TypeSystem+<>c__DisplayClass2_0\r\nTestStatements.CS_Concepts.TypeSystem+<FileModes>d__10\r\nTestStatements.DataTypes.EnumTest+Days\r\nTestStatements.DataTypes.EnumTest+BoilingPoints\r\nTestStatements.DataTypes.EnumTest+Colors\r\nTestStatements.DataTypes.Formating+<>c\r\nTestStatements.Anweisungen.SwitchStatement+Color\r\nTestStatements.Anweisungen.SwitchStatement+<>c\r\nTestStatements.Anweisungen.SwitchStatement+<>c__12`1\r\nTestStatements.Anweisungen.YieldStatement+<Range>d__0\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=6\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=20\r\n<PrivateImplementationDetails>+__StaticArrayInitTypeSize=24\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass6_0+<<ExampleMain3>b__0>d\r\nTestStatements.Threading.Tasks.TaskExample+<>c__DisplayClass7_0+<<ExampleMain4>b__0>d\r\n\nExample.SampleMethod(42) executes.\r\nSampleMethod returned 84.\r\n\nAssembly entry point:\r\nVoid Main(System.String[])";
#endif
		// ???

		/// <summary>
		/// Defines the test method AssemblyExampleTest.
		/// </summary>
		[TestMethod()]
		public void AssemblyExampleTest() {
			Assert.IsNotNull(new AssemblyExample(5));
		}


		/// <summary>
		/// Samples the method test.
		/// </summary>
		/// <param name="iStart">The i start.</param>
		/// <param name="iTest1">The i test1.</param>
		/// <param name="iExp1">The i exp1.</param>
		/// <param name="iTest2">The i test2.</param>
		/// <param name="iExp2">The i exp2.</param>
		/// <param name="iTest3">The i test3.</param>
		/// <param name="iExp3">The i exp3.</param>
		[DataTestMethod()]
		[DataRow( 3, 2,  6, 1,  3, 3, 9)]
		[DataRow( 4, 2,  8, 1,  4, 3, 12)]
		[DataRow( 1, 2,  2, 1,  1, 3, 3)]
		[DataRow(-1, 2, -2, 1, -1, 3, -3)]
		[DataRow( 0, 2,  0, 1,  0, 3, 0)]
		public void SampleMethodTest(int iStart,int iTest1,int iExp1, int iTest2, int iExp2, int iTest3, int iExp3)
        {
            AssemblyExample l = new AssemblyExample(iStart) ;
            Assert.AreEqual(iExp1, l.SampleMethod(iTest1) );
            Assert.AreEqual(iExp2, l.SampleMethod(iTest2));
            Assert.AreEqual(iExp3, l.SampleMethod(iTest3));
        }

		/// <summary>
		/// Defines the test method SampleDataMethodTest.
		/// </summary>
		[TestMethod()]
		public void SampleDataMethodTest() {
			AssemblyExample l = new AssemblyExample(3);
			Assert.AreEqual(6, l.SampleMethod(2));
			Assert.AreEqual(3, l.SampleMethod(1));
			Assert.AreEqual(9, l.SampleMethod(3));
			l = new AssemblyExample(4);
			Assert.AreEqual(8, l.SampleMethod(2));
			Assert.AreEqual(4, l.SampleMethod(1));
			Assert.AreEqual(12, l.SampleMethod(3));
		}


		/// <summary>
		/// Defines the test method ExampleMainTest.
		/// </summary>
		[TestMethod()]
        public void ExampleMainTest()
        {
            AssertConsoleOutput(cExpExampleMain, AssemblyExample.ExampleMain);
        }
    }
}
