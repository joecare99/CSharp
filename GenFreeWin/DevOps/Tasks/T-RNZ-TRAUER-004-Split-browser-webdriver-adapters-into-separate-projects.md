# Task T-RNZ-TRAUER-004: Split Browser WebDriver Adapters into Separate Projects

## Parent

- Backlog Item `BI-RNZ-TRAUER-001` - Refactor DataHandler Responsibilities

## Objective

Move browser-specific Selenium driver factory implementations out of `RnzTrauer.Core` into dedicated adapter projects, while keeping browser selection via configuration and DI in the composition root.

## Deliverables

- New adapter project `RnzTrauer.WebDriver.Firefox`
- New adapter project `RnzTrauer.WebDriver.Edge`
- Browser factory implementations moved from core to adapter projects
- DI extension methods for adapter registration
- Console host wiring updated to consume adapter projects

## Scope

- Keep `IWebDriverFactory`, `IFirefoxWebDriverFactory`, `IEdgeWebDriverFactory`, and `BrowserWebDriverFactory` in `RnzTrauer.Core`
- Keep browser selection logic (`BrowserType` in config) unchanged
- Register concrete browser adapters via extension methods in `RnzTrauer.Console`

## Changed Artifacts

- `RnzTrauer.WebDriver.Firefox/RnzTrauer.WebDriver.Firefox.csproj` (new)
- `RnzTrauer.WebDriver.Firefox/FirefoxWebDriverFactory.cs` (new)
- `RnzTrauer.WebDriver.Firefox/FirefoxWebDriverServiceCollectionExtensions.cs` (new)
- `RnzTrauer.WebDriver.Edge/RnzTrauer.WebDriver.Edge.csproj` (new)
- `RnzTrauer.WebDriver.Edge/EdgeWebDriverFactory.cs` (new)
- `RnzTrauer.WebDriver.Edge/EdgeWebDriverServiceCollectionExtensions.cs` (new)
- `RnzTrauer.Console/RnzTrauer.Console.csproj`
- `RnzTrauer.Console/Program.cs`
- `AmtsblattLoader.Console/AmtsblattLoader.Console.csproj`
- `AmtsblattLoader.Console/Program.cs`
- `RnzTrauer.Tests/RnzTrauer.Tests.csproj`
- `RnzTrauer.Core/Services/AmtsblattWebHandler.cs`
- `RnzTrauer.Core/Services/FirefoxWebDriverFactory.cs` (removed)
- `RnzTrauer.Core/Services/EdgeWebDriverFactory.cs` (removed)

## Risks and Notes

- Type names for concrete factories remain unchanged; only assembly location changed.
- Existing tests around `BrowserWebDriverFactory` remain valid because they mock interfaces from `RnzTrauer.Core`.
- `AmtsblattWebHandler` no longer creates a Firefox factory directly in core; the default provider is now configured in the host composition root.

## Validation

- Build: `dotnet build ..\WinAhnenNew\RnzTrauer\RnzTrauer.Console\RnzTrauer.Console.csproj -c Debug`
  - Status: **Succeeded**
  - Result: **0 failed, warnings only (pre-existing nullable warnings in `WebHandler`)**

- Tests: full `RnzTrauer.Tests` run
  - Status: **Succeeded (GREEN)**
  - Result: **128 passed, 0 failed, 0 skipped**
