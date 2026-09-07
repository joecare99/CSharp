# ConsoleApps Code Coverage Wave – 2026-08

## Backlog item

- **ID:** ConsoleApps-Bl001
- **Title:** Establish a solution-wide code coverage baseline
- **Status:** Analysis complete; implementation work not started
- **Coverage target:** No fixed percentage. Prioritize the largest untested production areas first.

## Measurement

Command used:

```powershell
dotnet test ConsoleApps.slnx --configuration Debug --no-restore --collect:"XPlat Code Coverage" --results-directory TestResults\Solution-Coverage
```

The solution contains multiple target frameworks, including .NET Framework and Windows UI targets. The run was stopped after approximately 100 seconds because the multi-target build/test graph continued to expand. At that point:

- 13,437 tests had been processed.
- 13,430 tests passed.
- 7 tests failed.
- The overall command returned exit code 1 because the interrupted run reported 19 build errors.
- 7 Cobertura reports were produced in `TestResults/Solution-Coverage`.

The reports are test-assembly/target-framework specific and must not be added together as if they were independent solution totals. The observed report line rates ranged from 15.7% to 95.4%; these are useful for locating gaps, but no single solution-wide percentage is claimed from this run.

## Largest observed production gaps

The following values are missing executable lines in the report in which the production class was measured. Test classes and generated/test-only paths were excluded from this list.

| Priority | Production area | Class | Missing lines | Source |
|---|---|---|---:|---|
| 1 | BaseLib helper | `BaseLib.Helper.StringUtils` | 230 | `Libraries/BaseLib/Helper/StringUtils.cs` |
| 2 | ConsoleLib controls | `ConsoleLib.CommonControls.ScrollBar` | 227 | `Libraries/ConsoleLib/CommonControls/ScrollBar.cs` |
| 3 | ConsoleLib controls | `ConsoleLib.CommonControls.Application` | 150 | `Libraries/ConsoleLib/CommonControls/Application.cs` |
| 4 | BaseLib model | `BaseLib.Models.ConsoleProxy` | 133 | `Libraries/BaseLib/Models/ConsoleProxy.cs` |
| 5 | ConsoleLib controls | `ConsoleLib.CommonControls.MenuBar` | 133 | `Libraries/ConsoleLib/CommonControls/MenuBar.cs` |
| 6 | ConsoleLib controls | `ConsoleLib.CommonControls.Terminal` | 148 | `Libraries/ConsoleLib/CommonControls/Terminal.cs` |
| 7 | ConsoleLib controls | `ConsoleLib.CommonControls.Grid` | 117 | `Libraries/ConsoleLib/CommonControls/Grid.cs` |
| 8 | BaseLib helper | `BaseLib.Helper.ObjectHelper` | 94 | `Libraries/BaseLib/Helper/ObjectHelper.cs` |
| 9 | BaseLib helper | `BaseLib.Helper.FileUtils` | 65 | `Libraries/BaseLib/Helper/FileUtils.cs` |
| 10 | BaseLib helper | `BaseLib.Helper.ByteUtils` | 53 | `Libraries/BaseLib/Helper/ByteUtils.cs` |

The class-level results are intentionally used for prioritization only. Before writing tests, each class must be checked against its source and public behavior so that dead code, platform-specific branches, and framework-generated paths are not converted into low-value tests.

## Prioritized work

### ConsoleApps-T001 – Cover `StringUtils` branches

- Add focused MSTest cases for the currently uncovered input categories and boundary conditions.
- Prefer `TestMethod` and `DataRow` only where the repository's current test conventions permit it.
- Keep tests in the dedicated `BaseLibTests` project.
- Re-run the BaseLib test project with Cobertura and record the delta.

### ConsoleApps-T002 – Cover `ScrollBar` behavior

- Map the uncovered lines to value changes, bounds, orientation, and rendering/input paths.
- Add behavior-focused tests in the appropriate ConsoleLib test project.
- Use existing test infrastructure and substitutes; do not add a new testing library.
- Re-run the ConsoleLib coverage measurement and record the delta.

### ConsoleApps-T003 – Cover high-value ConsoleLib controls

- Address `Application`, `Terminal`, `MenuBar`, and `Grid` after the first two areas.
- Separate platform/UI setup from deterministic control behavior.
- Avoid asserting implementation details when the existing control contracts provide a stable behavior surface.

### ConsoleApps-T004 – Cover remaining BaseLib helpers/models

- Address `ConsoleProxy`, `ObjectHelper`, `FileUtils`, and `ByteUtils`.
- Include exceptional and boundary paths where they represent supported behavior.
- Keep file-system and console dependencies isolated through the existing test doubles.

## Validation protocol

1. Run the focused test project without coverage and require all tests to pass.
2. Run the same project with `XPlat Code Coverage`.
3. Use `C:\Projekte\Cmd\Coverage\Get-CoberturaUncoveredLines.ps1` to list remaining uncovered lines.
4. Record test counts, line-rate delta, and any measurement limitation in this document.
5. Run a broader solution validation only after focused waves are complete; avoid treating a cancelled multi-target run as a clean baseline.

## Limitations and follow-up

- The solution-wide command is expensive because it expands several target frameworks per project.
- The current run was intentionally not used as a clean pass/fail gate because it was interrupted after producing actionable reports.
- The 7 failed tests need a separate failure-focused run to identify their exact test names; they are not assumed to be coverage regressions.
- No fixed coverage percentage is assigned. Completion is based on reducing the highest-value uncovered production behavior and maintaining passing focused tests.
