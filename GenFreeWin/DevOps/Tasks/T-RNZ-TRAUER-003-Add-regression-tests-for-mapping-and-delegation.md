# Task T-RNZ-TRAUER-003: Add Regression Tests for Mapping and Delegation

## Parent

- Backlog Item `BI-RNZ-TRAUER-001` - Refactor DataHandler Responsibilities

## Objective

Protect the refactoring increment with focused tests around repository delegation and announcement-record mapping.

## Deliverables

- `DataHandlerTests` covering repository-backed index loading
- `DataHandlerTests` covering announcement record mapping before repository persistence
- Validation run for the touched RNZ test scope

## Outcome Snapshot

- Focused MSTest coverage was added for the new repository interaction seam.
- The targeted `RnzTrauer.Tests.DataHandlerTests` scope passed after the refactoring changes.
- A full workspace build was not clean because of unrelated pre-existing errors outside the RNZ projects.

## 2026-05-02 Update: Browser Selection (Firefox/Edge)

### Summary

- Added RNZ browser selection via configuration (`Browser`) with `Firefox` as default.
- Implemented dedicated Selenium factories for Firefox and Edge.
- Added a browser-selecting factory that resolves the active web driver from configuration.
- Updated console startup wiring to load config first and inject browser-aware driver selection.
- Updated `RNZ_Config.sample.json` to include an Edge example.

### Changed Artifacts

- `RnzTrauer.Core/Models/BrowserType.cs` (new)
- `RnzTrauer.Core/Models/RnzConfig.cs`
- `RnzTrauer.Core/Services/Interfaces/IFirefoxWebDriverFactory.cs` (new)
- `RnzTrauer.Core/Services/Interfaces/IEdgeWebDriverFactory.cs` (new)
- `RnzTrauer.Core/Services/FirefoxWebDriverFactory.cs`
- `RnzTrauer.Core/Services/EdgeWebDriverFactory.cs` (new)
- `RnzTrauer.Core/Services/BrowserWebDriverFactory.cs` (new)
- `RnzTrauer.Core/Services/ConfigLoader.cs`
- `RnzTrauer.Console/Program.cs`
- `RnzTrauer.Console/RNZ_Config.sample.json`
- `RnzTrauer.Console/RnzTrauer.Console.csproj`
- `Directory.Packages.props`
- `RnzTrauer.Tests/BrowserWebDriverFactoryTests.cs` (new)
- `RnzTrauer.Tests/ConfigLoaderTests.cs`

### Validation

- Targeted test run: `RnzTrauer.Tests.BrowserWebDriverFactoryTests`, `RnzTrauer.Tests.ConfigLoaderTests`
  - Status: **Succeeded**
  - Result: **8 passed, 0 failed, 0 skipped**

- Full RNZ test project run (`RnzTrauer.Tests`)
  - Status: **Partially failed due to unrelated pre-existing tests**
  - Result: **93 passed, 2 failed, 0 skipped**
  - Existing failing tests:
    - `BuildTrauerFallIndex_UsesQueryFactory`
    - `AppendTrauerFall_UsesAbstractCommand`

## 2026-05-02 Update: Coverage Wave 2

### Summary

- Ran the updated shared `TestCoverage` skill for RNZTrauer using full class output mode.
- Added targeted branch tests for `WebHandler`, `DataHandler`, and repository fallback behavior.
- Added testability seams for driver-factory defaults and Amtsblatt web-driver creation to avoid real browser startup in tests.

### Added/Extended Tests

- `RnzTrauer.Tests.WebHandlerTests`
  - unsupported `GetDictionaryList`/`GetStringList` value-type branches
  - empty title+body branch in `HasMeaningfulPageContent`
  - stale-body branch in `TryGetBodyText`
  - `WorkSubPage` branch for `data-original` media lookup and protected link-processing catch branch

- `RnzTrauer.Tests.DataHandlerTests`
  - profile-image branch where image source does not contain media marker

- `RnzTrauer.Tests.TrauerDataRepositoryTests`
  - additional update/insert fallback branch coverage (including `LastInsertedId` fallback scenarios)

### Validation

- Focused test run: `WebHandlerTests` + `DataHandlerTests`
  - Status: **Succeeded**
  - Result: **47 passed, 0 failed, 0 skipped**

- Coverage skill run (`RnzTrauer.Tests`, net10.0)
  - Test status: **GREEN**
  - Result: **126 passed, 0 failed, 0 skipped**
  - RNZ-only line coverage: **96.34%**

### Remaining Coverage Gaps (RNZ)

- `RnzTrauer.Core.WebHandler` — **90.6%**
- `RnzTrauer.Core.DataHandler` — **99.13%**
- `RnzTrauer.Core.PortedHelpers` — **99.51%**
