# Solution Project Index

This repository contains a large multi-target .NET codebase (classic .NET Framework + .NET 6–9, Windows specific parts). Below is an index of projects and a short purpose statement. Dedicated, more detailed `README.md` files exist (or will exist) inside key project folders. For projects not yet expanded you can copy the TEMPLATE section at the end.

> NOTE: Many projects multi-target several frameworks. Unless otherwise specified they compile for (a subset of): net462, net472, net48 / net481, and net6.0–net9.0 (windows flavored where UI / console color APIs are needed).

## Core Libraries
| Project | Path | Description |
|---------|------|-------------|
| BaseLib | CSharpBible/Libraries/BaseLib | General purpose foundational helpers / abstractions. |
| GenInterfaces | CSharpBible/Libraries/GenInterfaces | Shared domain interfaces (genealogy domain). |
| GenFreeBase | Gen_FreeWin/GenFreeBase | Base domain layer (entities, value objects, core services). |
| GenFreeData | Gen_FreeWin/GenFreeData | Data access layer (repositories, persistence helpers). |
| GenFreeBaseClasses | Gen_FreeWin/GenFreeBaseClasses | Extended base classes for domain / infrastructure. |
| GenFreeBaseClassesTests | Gen_FreeWin/GenFreeBaseClassesTests | Tests for base classes. |
| GenFreeDataTests | Gen_FreeWin/GenFreeDataTests | Data-layer test suite. |
| GenFreeBaseTests | Gen_FreeWin/GenFreeBaseTests | Domain tests. |
| GenFreeHelper | Gen_FreeWin/GenFreeHelper | Utility & bridging helpers for legacy interop. |
| GenFreeHelperTests | Gen_FreeWin/GenFreeHelperTests | Tests for helper layer. |
| GenFreeBrowser | Gen_FreeWin/GenFreeBrowser | WPF map + place search components. |
| PlaceAuthorityConsoleDemo | Gen_FreeWin/PlaceAuthorityConsoleDemo | Console demo for place authority search (MVVM + ConsoleLib). |
| PlaceAuthorityDemo | Gen_FreeWin/PlaceAuthorityDemo | GUI (future) demo for place search (placeholder). |
| MapDemo | Gen_FreeWin/MapDemo | WPF sample for map viewer integration. |
| ConsoleLib | CSharpBible/Libraries/ConsoleLib | Rich console UI toolkit (controls, layout, binding). |
| ExtendedConsole | CSharpBible/Libraries/ExtendedConsole | Additional console abstractions (IExtendedConsole). |
| MVVM_BaseLib | CSharpBible/Libraries/MVVM_BaseLib | MVVM infrastructure (observable base, validation, commands). |
| MVVM_BaseLibTests | CSharpBible/Libraries/MVVM_BaseLibTests | Test suite for MVVM_BaseLib. |
| CommonDialogs | CSharpBible/Libraries/CommonDialogs | Cross-framework dialog abstractions (WinForms/WPF bridging). |
| CommonDialogs_net | CSharpBible/Libraries/CommonDialogs | .NET (Core) variant of dialogs. |
| WFSystem.Windows.Data | CSharpBible/Libraries/WFSystem.Data | WinForms data-binding style helpers. |
| GenFreeUIItfs | Gen_FreeWin/GenFreeUIItfs | UI contracts (interfaces) for GenFree apps. |
| GenFreeWinForms | Gen_FreeWin/GenFreeWinForms | WinForms UI implementation (legacy modernization). |
| GenFreeWinFormsTests | Gen_FreeWin/GenFreeWinFormsTests | WinForms layer tests. |
| GenFreeWin | Gen_FreeWin/GenFreeWin | Legacy WinForms entry host / migration area. |
| GenFreeWin2 / GenFreeWin3 | Gen_FreeWin/... | Successive refactor / transition shells. |
| MSQBrowser | Gen_FreeWin/MSQBrowser | Microsoft SQL / data browsing utilities. |
| MdbBrowser | Gen_FreeWin/MdbBrowser | Access/Jet (MDB) database browser. |
| MdbBrowserTests | Gen_FreeWin/MdbBrowserTests | Tests for MDB browser. |
| mdbTool | Gen_FreeWin/mdbTool | CLI utilities for MDB conversion / inspection. |
| DBTest1 | CSharpBible/DB/DBTest1 | Database experiment project. |
| GenDBImplOLEDB | Gen_FreeWin/GenDBImplOLEDB | OLE DB implementation of Gen database interfaces. |
| GenDBImplOLEDBTests | Gen_FreeWin/GenDBImplOLEDBTests | Tests for OLE DB impl. |
| Document.* (Base, Html, Odf, OdtBoldTextExample) | CSharpBible/Data/DocumentUtils/... | Document formatting / generation utilities & samples. |
| WinAhnen* (Gen_BaseItf, BaseGenClasses, WinAhnenCls, etc.) | WinAhnenNew | Alternative / next generation code line (shared genealogy logic). |

(Additional test projects follow the naming *Tests.)

## Representative READMEs
Detailed READMEs have been added for these core / exemplar projects:
- ConsoleLib
- ExtendedConsole
- MVVM_BaseLib
- GenFreeBrowser
- PlaceAuthorityConsoleDemo
- MapDemo
- GenFreeData
- GenFreeBase

## Conventions
- One public type per file.
- Namespace mirrors folder path.
- Multi-target settings unify APIs across classic & modern .NET.
- Nullable enabled everywhere for new code.
- Async suffix enforced for Task-returning methods.
- DI registration lives in `*Module.cs` or Program/App startup.

## Build
```
dotnet build -c Release
```
All projects restore without extra feeds.

## Test
```
dotnet test --no-build
```

## Run Samples
```
# Console place search demo
cd Gen_FreeWin/PlaceAuthorityConsoleDemo
 dotnet run

# Map WPF demo
cd Gen_FreeWin/MapDemo
 dotnet run
```

## Code Quality
- Roslyn analyzers + (optional) StyleCop recommended via root Directory.Build.props (not yet committed here — see AI guidelines plan).
- Consider `dotnet format` pre-commit.

## Template (Copy into project README)
```markdown
# {ProjectName}

Short one-line summary.

## Purpose
Explain the domain problem the project solves and why it exists separately.

## Key Features
- Feature 1
- Feature 2

## Targets
`netX.Y` [... add others]

## Getting Started
```bash
dotnet add package <dependencies-if-any>
```

## Usage
Show a minimal code snippet.

## Architecture
Explain layers / patterns (MVVM, repository, etc.).

## Contributing
Describe PR / branching expectations.

## License
(Insert license statement.)
```

---
If you would like autogenerated READMEs for every remaining project, request a "full expansion" (will generate placeholder text for each).