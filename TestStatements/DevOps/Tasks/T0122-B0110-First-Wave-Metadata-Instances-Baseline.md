# T0122A - First-Wave Metadata Pilot Instances Baseline

## Parent Backlog Item

- B0110 - Map Metadata to Current Example Groups and Execution Profiles

## Summary

Define a first set of concrete metadata pilot instances for high-value example areas to validate the metadata shape against real repository content.

## Pilot Metadata Instances

### `TS-REF-ASM-001`

- `Id`: `TS-REF-ASM-001`
- `Title`: `Assembly Metadata Inspection`
- `Project`: `TestStatements`
- `NamespaceGroup`: `Reflection`
- `PrimarySource`: `TestStatements/Reflection/AssemblyExample.cs`
- `ExecutionProfile`: `DiagnosticsAndReflection`
- `Summary`: `Shows how the current assembly can be inspected, enumerated, and invoked through reflection.`
- `LearningIntent`: `Teach assembly metadata discovery, reflected method enumeration, and late-bound invocation.`
- `ExpectedObservation`: `Assembly identity information, discovered types and public static methods, and a reflected invocation result are printed.`
- `OutputSensitivity`: `High`
- `Determinism`: `Illustrative`
- `TargetFrameworkScope`: `CrossTarget`
- `RepresentativeMethods`: `ExampleMain`, `SampleMethod`
- `Tags`: `reflection`, `assembly`, `metadata`, `invocation`
- `Prerequisites`: `Basic understanding of classes, methods, and assemblies`
- `RelatedExamples`: `TS-DIA-STW-001`, `TS-ASY-BRK-001`
- `Notes`: `Method and type ordering may vary across runtimes; the important outcome is the visible metadata categories and reflected invocation flow.`

### `TS-ASY-BRK-001`

- `Id`: `TS-ASY-BRK-001`
- `Title`: `Async Breakfast Orchestration`
- `Project`: `TestStatements`
- `NamespaceGroup`: `Threading.Tasks`
- `PrimarySource`: `TestStatements/Threading/Tasks/AsyncBreakFast1.cs`
- `ExecutionProfile`: `AsyncAndTasks`
- `Summary`: `Compares sequential and asynchronous breakfast workflows to show different orchestration patterns.`
- `LearningIntent`: `Teach how async and task-based composition changes execution order, overlap, and completion behavior.`
- `ExpectedObservation`: `Console output shows preparation steps, overlapping operations in async variants, and different completion ordering between variants.`
- `OutputSensitivity`: `High`
- `Determinism`: `Illustrative`
- `TargetFrameworkScope`: `CrossTarget`
- `RepresentativeMethods`: `AsyncBreakfast_Main`, `AsyncBreakfast_Main2`, `AsyncBreakfast_Main3`, `AsyncBreakfast_Main4`
- `Tags`: `async`, `tasks`, `orchestration`, `sequencing`, `breakfast`
- `Prerequisites`: `Basic knowledge of methods, timing, and task concepts`
- `RelatedExamples`: `TS-DIA-STW-001`, `TS-REF-ASM-001`
- `Notes`: `Exact ordering can vary slightly in the asynchronous variants. The key learning result is overlap and orchestration style, not exact timestamps.`

### `TS-DIA-STW-001`

- `Id`: `TS-DIA-STW-001`
- `Title`: `Stopwatch Timing and Measurement`
- `Project`: `TestStatements`
- `NamespaceGroup`: `Diagnostics`
- `PrimarySource`: `TestStatements/Diagnostics/StopWatchExample.cs`
- `ExecutionProfile`: `DiagnosticsAndReflection`
- `Summary`: `Measures elapsed time and compares timing-related operations using stopwatch-based diagnostics.`
- `LearningIntent`: `Teach how runtime measurement works, how stopwatch values are formatted, and how repeated operations can be timed comparatively.`
- `ExpectedObservation`: `Runtime durations, timer properties, and comparative timing information are printed to the console.`
- `OutputSensitivity`: `High`
- `Determinism`: `EnvironmentSensitive`
- `TargetFrameworkScope`: `CrossTarget`
- `RepresentativeMethods`: `ExampleMain`, `ExampleMain1`, `ExampleMain2`, `DisplayTimerProperties`, `TimeOperations`
- `Tags`: `diagnostics`, `stopwatch`, `timing`, `measurement`, `performance`
- `Prerequisites`: `Basic understanding of elapsed time and console output`
- `RelatedExamples`: `TS-ASY-BRK-001`, `TS-REF-ASM-001`
- `Notes`: `Exact timing values depend on environment and runtime conditions. The important observation is the measurement pattern, not the precise numbers.`

### `TS-DAT-ENU-001`

- `Id`: `TS-DAT-ENU-001`
- `Title`: `Enumeration Values and Flags`
- `Project`: `TestStatements`
- `NamespaceGroup`: `DataTypes`
- `PrimarySource`: `TestStatements/DataTypes/EnumTest.cs`
- `ExecutionProfile`: `TypesAndFormatting`
- `Summary`: `Demonstrates enumeration names, numeric values, and flag-based combinations.`
- `LearningIntent`: `Teach how enums represent symbolic values, explicit numeric meanings, and combinable flags.`
- `ExpectedObservation`: `The console shows enum names with numeric values and a combined flags result for color values.`
- `OutputSensitivity`: `Medium`
- `Determinism`: `Deterministic`
- `TargetFrameworkScope`: `CrossTarget`
- `RepresentativeMethods`: `MainTest`
- `Tags`: `enum`, `flags`, `datatypes`, `values`, `formatting`
- `Prerequisites`: `Basic C# syntax and console output understanding`
- `RelatedExamples`: `TS-DAT-STR-001`
- `Notes`: `This is a strong beginner-friendly metadata candidate because the visible result is stable and easy to compare.`

### `TS-DAT-STR-001`

- `Id`: `TS-DAT-STR-001`
- `Title`: `String Construction and Unicode Behavior`
- `Project`: `TestStatements`
- `NamespaceGroup`: `DataTypes`
- `PrimarySource`: `TestStatements/DataTypes/StringEx.cs`
- `ExecutionProfile`: `TypesAndFormatting`
- `Summary`: `Demonstrates multiple ways to build strings, format values, and observe Unicode-related behavior.`
- `LearningIntent`: `Teach string construction, formatting, substring extraction, Unicode normalization, and surrogate-pair behavior.`
- `ExpectedObservation`: `The console shows created strings, formatted output, extracted content, and surrogate-pair information; one method also writes Unicode comparison data to a text file.`
- `OutputSensitivity`: `High`
- `Determinism`: `Mixed`
- `TargetFrameworkScope`: `CrossTarget`
- `RepresentativeMethods`: `AllTests`, `StringEx1`, `StringEx2`, `StringEx3`, `StringEx4`, `StringEx5`, `UnicodeEx1`, `StringSurogarteEx1`
- `Tags`: `string`, `unicode`, `formatting`, `surrogate`, `normalization`
- `Prerequisites`: `Basic string and console concepts`
- `RelatedExamples`: `TS-DAT-ENU-001`
- `OutputArtifacts`: `graphemes.txt`
- `Notes`: `Most output is stable, but date-based formatting and file output introduce environment or culture sensitivity. UnicodeEx1 creates a text artifact in the working directory.`

## Pilot Set Notes

- The pilot set intentionally covers both deterministic and non-deterministic examples.
- The selected examples exercise output-sensitive, reflection-based, timing-based, and beginner-friendly metadata cases.
- The pilot set is suitable for validating metadata reuse in documentation before wider rollout.

## Done Criteria

- Concrete metadata instances are documented.
- The instances reuse the agreed metadata shape.
- The first-wave rollout targets are covered with representative examples.
