# TestStatements
inspired by [MS Walktroughs] 

## (DE) Überblick
Dies ist eine Sammlung von Beispielprojekten rund um C#, Sprachelemente, .NET-Laufzeiten, Desktop (WinForms/WPF), Reflection, Generics, Plugins, dynamisches Laden, asynchrone Programmierung, Geometrie/Algorithmik und Spezialthemen wie Unicode & GPU-Rechnen.

---
## (DE) Projekte & Inhalte

### [TestStatements](TestStatements)
Dies ist das **Hauptprojekt** mit den meisten Beispielen.

#### [Anweisungen](TestStatements/Anweisungen)
Beispiele für verschiedene Sprach-/System-Anweisungen:
- Checking
- Conditional statements (If ...)
- Declarations
- Exception handling
- Expressions
- Loop statements
- Program flow
- Random examples
- Return statement
- Switch statements (3 Parts)
- Using statement
- Yield statement

#### [CS-Concepts](TestStatements/CS_Concepts)
Beispiele für das C#-Typsystem.

#### [Classes and objects](TestStatements/ClassesAndObjects)
Beispiele für Interfaces & Klassen-Member.

#### [Collection/Generic](TestStatements/Collection/Generic)
Beispiele für den Namensraum `System.Collections.Generic`:
- Comparer
- Dictionary
- SortedList
- HashSet

#### [Constants](TestStatements/Constants)
Konstanten vs. readonly-Felder/Eigenschaften.

#### [DataTypes](TestStatements/DataTypes)
Beispiele für Datentypen:
- Enum
- String & Formatting
- Integrierte Typen
- Type Helpers / Erweiterungen

#### [Diagnostics](TestStatements/Diagnostics)
- Debug example
- Stopwatch example

#### [Helper](TestStatements/Helper)
- Extensions & Beispiele

#### [Linq](TestStatements/Linq)
- LINQ Lookup & Enumeration

#### [Reflection](TestStatements/Reflection)
- Assembly example
- Reflection example

#### [Runtime/Loader](TestStatements/Runtime/Loader)
- RT-Loader example (Kompilieren und Laden einer Bibliothek im Speicher)

#### [System Namespace](TestStatements/SystemNS)
Beispiele rund um `System` & XML.

#### [Threading/Tasks](TestStatements/Threading/Tasks)
Frühstücks-/Async-Beispiele.

---
## (DE) Weitere Projekte (Kurz)
- TestStatementTests – Tests für TestStatements.
- CallAllExamples – Starter für alle Beispiele.
- ctlClockLib / TestClockApp – WinForms Uhr-/Alarm-Controls & Host-App.
- TestNamespaces – Namespace-Experimente.
- AsyncExampleWPF – Async in WPF (Full Framework).
- Tutorials – Kuratierte Schritt-für-Schritt Beispiele.
- PluginBase / AppWithPlugin / AppWithPluginWpf / HelloPlugin / OtherPlugin – Plugin-Architektur & Demonstrationen.
- DynamicSample – Dynamik & Reflection (getrennt für .NET Framework & moderne .NET).
- TestStatementsNew – Moderne WPF-Variante der Beispiel-Sammlung.
- TestGJKAlg / TestGJKAlgTest – Geometrischer Algorithmus (GJK) + Tests.
- ZeroLengthChar – Unicode / Zero-width Zeichen.
- SOq1 – ILGPU / GPU-Rechnen Beispiel.

---
# (EN) TestStatements Suite
A multi-project educational & exploratory sandbox showcasing C# language constructs, .NET runtime differences, patterns, dynamic behaviors, plugins, reflection, diagnostics, async/await, geometry algorithms, Unicode nuances, and GPU acceleration.

## English Table of Contents
1. Core Example Catalog (`TestStatements`)
2. Aggregators & Launchers (`CallAllExamples`)
3. Dynamic & Reflection Samples (`DynamicSample`)
4. Modern WPF Variant (`TestStatementsNew`)
5. Tutorials Stream (`Tutorials`)
6. Plugin Architecture (Host + Base + Plugins + Tests)
7. Desktop UI Samples (WinForms + WPF + Controls)
8. Specialized Domains (GJK Geometry, ILGPU, Unicode edge cases)
9. Testing Infrastructure (Multi-target test suites)

---
## 1. Core Example Catalog: `TestStatements`
Two variants exist to contrast classic .NET Framework and modern .NET:
- `TestStatements.csproj` (net462; net472; net48; net481)
- `TestStatements_net.csproj` (net6.0; net7.0; net8.0; net9.0)

Coverage highlights:
- Control flow: if / switch (multi forms) / loops / goto / yield / lock / try-catch / using
- Data & Types: enums, strings (surrogates, Unicode), value/reference types, formatting
- Collections & Generics: List, Dictionary, SortedList, HashSet, custom comparers
- LINQ: lookup/grouping/enumeration patterns
- Async & Tasks: breakfast metaphors, manual orchestration, continuations
- Reflection: assembly metadata enumeration & type/member introspection
- Dynamic runtime code: (paired with `DynamicSample` + Roslyn usage in modern variant)
- Diagnostics: Stopwatch timing, Debug output
- Dependency Injection / Hosting (Extensions.* packages in modern variant)

Folder mapping (representative):
- `Anweisungen` (Statements) – language statements & control constructs
- `CS_Concepts` – type system exploration
- `ClassesAndObjects` – members, interfaces, nested types
- `Collection/Generic` – generic collections & algorithms
- `DataTypes` – primitives, formatting, Unicode handling
- `Threading/Tasks` – task orchestration examples
- `Reflection` – runtime inspection
- `Runtime/Loader` – dynamic compile & load
- `SystemNS` – system namespace & XML demos

### Assembly Versioning
Modern variant pins `AssemblyVersion` to a static semantic (+ illustrative build number). Classic variant mirrors for parity.

---
## 2. Aggregators & Launchers: `CallAllExamples`
Launches multiple demonstrations in sequence. Exists in framework & modern variants to verify behavior parity. Ideal for quick smoke validation.

---
## 3. Dynamic & Reflection Samples: `DynamicSample`
Dual project layout (`DynamicSample.csproj` vs `DynamicSample_net.csproj`) illustrates API differences for dynamic invocation, possible runtime code generation, and reflection semantics.

---
## 4. Modern WPF Variant: `TestStatementsNew`
Re-imagines the console examples with a GUI front-end. Multi-targets net6+ Windows TFMs; uses C# 13 preview for forward-looking language demonstrations. Good candidate for MVVM enrichment.

---
## 5. Tutorials: `Tutorials`
A curated linear path distinct from the broad, exploratory catalog. Focuses on clarity over breadth.

---
## 6. Plugin Architecture
Projects:
- `PluginBase` – shared contracts & MVVM helpers (CommunityToolkit.Mvvm)
- `AppWithPlugin` (console host)
- `AppWithPluginWpf` (WPF host)
- `HelloPlugin`, `OtherPlugin` – plugin implementations (localized resources, dynamic loading enabled)
- `AppWithPluginTest`, `HelloPluginTest` – contract & behavior verification with MSTest + NSubstitute

Features:
- Dynamic assembly discovery (content-copied plugin DLLs + satellite resource assemblies)
- Dependency injection integration (Microsoft.Extensions.DependencyInjection)
- Logging abstraction (Microsoft.Extensions.Logging.Abstractions)
- Conditional signing & user secrets support

Extension ideas:
- Plugin manifest schema
- Hot reload via filesystem watching
- Structured plugin lifecycle logging

---
## 7. Desktop UI Samples
- `ctlClockLib` – reusable WinForms clock/alarm UserControls
- `TestClockApp` – host application embedding the controls
- `AsyncExampleWPF` – async responsiveness patterns in WPF (legacy frameworks)
- `AppWithPluginWpf` – plugin-enabled GUI host

---
## 8. Specialized Domains
- `TestGJKAlg` & `TestGJKAlgTest` – GJK collision detection visualization + algorithm tests
- `SOq1` – ILGPU sample (GPU kernel & accelerator usage)
- `ZeroLengthChar` – Unicode / zero-width character exploration

---
## 9. Testing Infrastructure
Multi-target test suites ensure cross-runtime consistency:
- `TestStatements_netTest` (net6–net9)
- `TestStatementsTest` (full framework set)
- Plugin & geometry focused test projects

Shared helpers are linked from an external library (CSharpBible/BaseLib) to reduce duplication.

Coverage tooling: `coverlet.collector` supports `dotnet test /p:CollectCoverage=true`.

---
## Build & Run Quick Start
Clone (or update SVN metadata if present). Then:

```bash
# Build all
 dotnet build

# Run core modern examples
 dotnet run --project TestStatements/TestStatements_net.csproj

# Run aggregated launcher
 dotnet run --project CallAllExamples/CallAllExamples_net.csproj

# Execute tests (modern)
 dotnet test TestStatementsTest/TestStatements_netTest.csproj

# Execute tests (framework)
 dotnet test TestStatementsTest/TestStatementsTest.csproj
```

For WPF/WinForms GUI apps add `--framework` if needed to disambiguate.

---
## Multi-Targeting Strategy
Purpose:
- Demonstrate evolution of APIs across framework generations
- Ensure educational examples remain relevant for legacy maintenance scenarios
- Provide contrast of hosting/DI/reflection behaviors between runtimes

---
## Plugin Demo Usage
1. Build `PluginBase`, `HelloPlugin`, `OtherPlugin`, and host (`AppWithPlugin` or `AppWithPluginWpf`).
2. Run host – it probes the `PlugIns` directory (content items copied during build).
3. Observe loaded plugin list & invoked behaviors.

Optional enhancements: add command-line filtering or interactive reload.

---
## Suggested Learning Path
1. Start with `Tutorials`
2. Explore `TestStatements` (control flow ? data ? collections ? async ? reflection)
3. Examine `DynamicSample` & `Runtime/Loader` for advanced runtime topics
4. Dive into plugin architecture
5. Finish with specialized geometry (`TestGJKAlg`) & GPU (`SOq1`) samples

---
## Contributing / Extending
Add new thematic folders under `TestStatements` following existing naming. Provide targeted test coverage in the appropriate test project. For plugin additions, implement interfaces in `PluginBase` and drop build artifacts into the PlugIns directory (or augment discovery logic).

---
## License / Attribution
Educational sample set inspired by Microsoft walkthroughs & community patterns. (Add explicit license file if distribution is intended.)

---
## Current Limitations / Ideas
- Unify duplicate README generation (auto-index script)
- Introduce CI pipeline (GitHub Actions) for matrix builds
- Add benchmark project (BenchmarkDotNet) for performance comparisons (collections, async patterns)
- Provide markdown diagrams for plugin lifecycle & GJK algorithm

---
## German ? English Mapping
Original German section retained for native reference; English expansion supplies broader architectural context.

---
Happy exploring! ??

