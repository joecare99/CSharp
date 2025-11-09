# TestStatementsTest Directory

This folder hosts multiple MSTest-based test projects that validate the sample/demo code in the solution.

## Projects

### TestStatements_netTest.csproj
Multi-targeted test project (net6.0; net7.0; net8.0; net9.0) exercising the modern .NET builds of the core sample assemblies:
- References: DynamicSample_net, TestStatements_net
- Tooling Packages: Microsoft.NET.Test.Sdk, MSTest, coverlet.collector (code coverage)
- Versioning: AssemblyVersion pattern 1.0.* for automatic build/revision increments.

### TestStatementsTest.csproj
Full framework test project (net462; net472; net48; net481) targeting legacy runtime compatibility.
- References: DynamicSample (full framework), TestStatements (full framework)
- Mirrors the intent of the multi-target test project to ensure feature parity against desktop frameworks.

## Purpose
Provides regression coverage for:
- Reflection and assembly metadata demos
- Collections, LINQ, threading & async samples
- Formatting, type system, language construct demonstrations
- Plugin & dynamic loading helper classes (indirect via referenced projects)

## Structure Notes
Linked helper files (TestHelper.cs, ClassHelper.cs) originate from a shared external library (CSharpBible/BaseLib) to reduce duplication.

## Running Tests
Use: `dotnet test TestStatementsTest/TestStatements_netTest.csproj` or `dotnet test TestStatementsTest/TestStatementsTest.csproj`.

## Coverage
The coverlet.collector package enables `dotnet test /p:CollectCoverage=true` for coverage gathering on supported TFMs.

## Key Design Points
- Separation between modern and legacy frameworks ensures broad API surface validation.
- Deterministic flags disabled to allow wildcard versioning for illustrative assembly build numbers.
