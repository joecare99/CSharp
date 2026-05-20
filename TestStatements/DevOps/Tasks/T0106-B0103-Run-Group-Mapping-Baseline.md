# T0106A - Run-Group Mapping Baseline

## Parent Backlog Item

- B0103 - Define Grouped Execution Profiles for Examples

## Summary

Map the current example areas of the repository into coherent execution groups that can later be used in documentation, launcher filtering, smoke validation, and UI-based browsing.

## Proposed Run Groups

### `LanguageBasics`

Purpose:

- Cover foundational C# syntax and control-flow learning.

Mapped areas:

- `TestStatements.Anweisungen`
- `TestStatements.Constants`
- selected parts of `TestStatements.CS_Concepts`

Representative examples:

- `Declarations.cs`
- `ConditionalStatement.cs`
- `LoopStatements.cs`
- `ProgramFlow.cs`
- `ReturnStatement.cs`
- `UsingStatement.cs`
- `YieldStatement.cs`
- `Constants.cs`

### `TypesAndFormatting`

Purpose:

- Cover type understanding, strings, enums, and visible formatting behavior.

Mapped areas:

- `TestStatements.DataTypes`
- selected parts of `TestStatements.CS_Concepts`

Representative examples:

- `EnumTest.cs`
- `IntegratedTypes.cs`
- `StringEx.cs`
- `Formating.cs`
- `TypeSystem.cs`
- `PatternComparison.cs`

### `CollectionsAndQueries`

Purpose:

- Cover collections, lookup behavior, ordering, and LINQ-oriented query thinking.

Mapped areas:

- `TestStatements.Collection.Generic`
- `TestStatements.Linq`

Representative examples:

- `ComparerExample.cs`
- `DictionaryExample.cs`
- `SortedListExample.cs`
- `TestHashSet.cs`
- `TestList.cs`
- `LinqLookup.cs`

### `ObjectModelAndReuse`

Purpose:

- Cover interface-driven structure, members, and helper or extension-based reuse.

Mapped areas:

- `TestStatements.ClassesAndObjects`
- `TestStatements.Helper`

Representative examples:

- `Interface.cs`
- `Members.cs`
- `Extensons.cs`
- `ExtensionExample.cs`

### `DiagnosticsAndReflection`

Purpose:

- Cover runtime inspection, diagnostic output, and timing-oriented observation.

Mapped areas:

- `TestStatements.Diagnostics`
- `TestStatements.Reflection`

Representative examples:

- `DebugExample.cs`
- `StopWatchExample.cs`
- `AssemblyExample.cs`
- `ReflectionExample.cs`

### `RuntimeAndDynamic`

Purpose:

- Cover loader behavior, dynamic assembly scenarios, and runtime-generated behavior.

Mapped areas:

- `TestStatements.Runtime.Loader`
- `TestStatements.Runtime.Dynamic`
- related comparison work in `DynamicSample`

Representative examples:

- `RTLoaderExample.cs`
- `DynamicAssembly.cs`

### `SystemIntegration`

Purpose:

- Cover practical framework APIs such as serialization, XML, and printing-oriented examples.

Mapped areas:

- `TestStatements.SystemNS`

Representative examples:

- `System_Namespace.cs`
- `XmlNS.cs`
- `Printing_Ex.cs`
- `SerializeBasic.cs`
- `SerializeToFile.cs`
- `SerializeToFileAsync.cs`

### `AsyncAndTasks`

Purpose:

- Cover task orchestration, async sequencing, and concurrency-adjacent learning.

Mapped areas:

- `TestStatements.Threading.Tasks`
- selected statement examples related to locking where later needed

Representative examples:

- `TaskExample.cs`
- `AsyncBreakFast1.cs`
- `IPing.cs`
- `PingProxy.cs`
- `Locking.cs`

### `ModernArchitecture`

Purpose:

- Cover modern service composition, dependency injection, and architecture-oriented examples.

Mapped areas:

- `TestStatements.DependencyInjection`

Representative examples:

- `DIExample.cs`
- `DIExample2.cs`
- `IMessageWriter.cs`
- `LoggingMessageWriter.cs`
- `Worker.cs`
- `Worker2.cs`

## Mapping Notes

- Some example areas appear in more than one conceptual path when viewed from a learning perspective.
- `CS_Concepts` is split across foundational and type-oriented groups because its content supports more than one learning stage.
- `Locking.cs` belongs structurally to `Anweisungen` but is operationally relevant to async and concurrency-oriented runs.
- `DynamicSample` is not part of the main `TestStatements` namespace tree but should be associated with the `RuntimeAndDynamic` group for grouped execution planning.

## Usage Intent

These run groups are intended to support:

- future `CallAllExamples` filtering
- smoke-style execution by theme
- user-facing learning paths
- UI grouping in a future example browser
- validation mapping between examples and tests

## Done Criteria

- Coherent run groups are defined.
- Existing example areas are mapped to them.
- Overlaps and special cases are documented.
