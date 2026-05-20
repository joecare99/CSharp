# T0100A - Namespace Inventory Baseline for TestStatements

## Parent Backlog Item

- B0100 - Create a Namespace Catalog for TestStatements

## Summary

Record the current namespace and folder groups of the main `TestStatements` project as the baseline result of the first implementation step.

## Inventory Result

### Core Namespace Groups

- `TestStatements.Anweisungen`
  - Focus: control flow, declarations, expressions, exception handling, locking, switching, using, yield, and related statement examples
  - Representative files: `Declarations.cs`, `ConditionalStatement.cs`, `LoopStatements.cs`, `ProgramFlow.cs`, `ExceptionHandling.cs`, `SwitchStatement.cs`, `UsingStatement.cs`, `YieldStatement.cs`
- `TestStatements.CS_Concepts`
  - Focus: type system and comparison-oriented concept samples
  - Representative files: `TypeSystem.cs`, `PatternComparison.cs`
- `TestStatements.ClassesAndObjects`
  - Focus: interfaces, members, and object-oriented language structure
  - Representative files: `Interface.cs`, `Members.cs`
- `TestStatements.Collection.Generic`
  - Focus: generic collection behavior and collection-oriented examples
  - Representative files: `ComparerExample.cs`, `DictionaryExample.cs`, `SortedListExample.cs`, `TestHashSet.cs`, `TestList.cs`
- `TestStatements.Constants`
  - Focus: constants and readonly-related examples
  - Representative files: `Constants.cs`
- `TestStatements.DataTypes`
  - Focus: enums, primitive or integrated types, strings, and formatting
  - Representative files: `EnumTest.cs`, `IntegratedTypes.cs`, `StringEx.cs`, `Formating.cs`
- `TestStatements.Diagnostics`
  - Focus: debug output, timing, and proxy-based stopwatch abstraction
  - Representative files: `DebugExample.cs`, `StopWatchExample.cs`, `IStopwatch.cs`, `StopwatchProxy.cs`
- `TestStatements.Helper`
  - Focus: extension methods and helper-oriented reusable examples
  - Representative files: `Extensons.cs`, `ExtensionExample.cs`
- `TestStatements.Linq`
  - Focus: lookup and enumeration examples with LINQ
  - Representative files: `LinqLookup.cs`
- `TestStatements.Reflection`
  - Focus: assembly metadata and reflection-based inspection
  - Representative files: `AssemblyExample.cs`, `ReflectionExample.cs`
- `TestStatements.Runtime.Loader`
  - Focus: runtime compile/load example behavior
  - Representative files: `RTLoaderExample.cs`
- `TestStatements.Runtime.Dynamic`
  - Focus: dynamic assembly generation and runtime behavior
  - Representative files: `DynamicAssembly.cs`
- `TestStatements.SystemNS`
  - Focus: broader `System` namespace examples including XML, printing, and JSON serialization
  - Representative files: `System_Namespace.cs`, `Xml/XmlNS.cs`, `Printing/Printing_Ex.cs`, `Text/Json/SerializeBasic.cs`, `SerializeToFile.cs`, `SerializeToFileAsync.cs`
- `TestStatements.Threading.Tasks`
  - Focus: tasks, async workflow sequencing, and related abstractions
  - Representative files: `TaskExample.cs`, `AsyncBreakFast1.cs`, `IPing.cs`, `PingProxy.cs`
- `TestStatements.DependencyInjection`
  - Focus: dependency injection and service wiring examples
  - Representative files: `DIExample.cs`, `DIExample2.cs`, `IMessageWriter.cs`, `LoggingMessageWriter.cs`, `Worker.cs`, `Worker2.cs`

## Modern-Only or Modern-Weighted Areas

- `DependencyInjection` is a modern-only or modern-weighted area in the current project inventory.
- JSON serialization examples in `SystemNS/Text/Json` are particularly relevant to the modern target set.
- The main executable flow in `Program.cs` is a practical aggregation point for many documented groups.

## Notes

- The inventory is derived from `TestStatements/TestStatements_net.csproj` and `TestStatements/Program.cs`.
- This baseline supports follow-up work for descriptions, expected-output notes, grouped execution, and future metadata modeling.

## Done Criteria

- The major namespace groups are documented.
- Representative files are named.
- Modern-only and modern-weighted areas are highlighted.
