# MVVM_31a_CTValidation1

> Toolkit simplified validation (basic).

## Overview
System.Collections.Hashtable.Detailed

## Key Learning Goals
- Toolkit validation hooks
- Error propagation

## Project Structure
- Views: WPF XAML views illustrating bindings and styles
- ViewModels: Reactive presentation logic using either classic or Toolkit paradigms
- Models: Plain data / domain classes kept free of UI concerns
- Services: (Where applicable) abstractions for dialogs, navigation, data access
- Behaviors / Helpers: Reusable interaction patterns extending XAML without code-behind

## Build & Run
`
dotnet build MVVM_31a_CTValidation1
`
If multiple target frameworks (e.g., net6.0-windows + net481) exist, you can specify one:
`
dotnet build MVVM_31a_CTValidation1 -f net6.0-windows
`
Run (where an executable host exists):
`
dotnet run --project MVVM_31a_CTValidation1 -f net6.0-windows
`

## Testing
If a companion test project exists it is listed below. Execute:
`
dotnet test MVVM_31a_CTValidation1Tests
`
(If '(none explicit)' the example is illustrated without dedicated automated tests yet.)

## Test & Coverage Status

| Metric | Status |
|--------|--------|
| Unit Tests | Implemented (see associated *Tests* project: MVVM_31a_CTValidation1Tests) |
| Line Coverage | TBD |
| Branch Coverage | TBD |
| Method Coverage | TBD |
| Complexity Coverage | TBD |

### Collecting Coverage Locally

Classic (.NET Framework targets):
`
dotnet test MVVM_31a_CTValidation1Tests --framework net481 /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
`

Modern multi-target (.NET >= 6):
`
dotnet test MVVM_31a_CTValidation1Tests -c Debug /p:CollectCoverage=true /p:CoverletOutputFormat=lcov \
  /p:Exclude="[xunit.*]*,[MSTest.*]*,[NUnit.*]*" --logger "trx" --results-directory ./TestResults
`

To merge coverage from several target frameworks:
`
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.info" -targetdir:CoverageReport -reporttypes:HtmlSummary;MarkdownSummaryGithub
`

> Replace TBD entries above with the real metrics after running the commands.

## Extending This Example
- Add additional ViewModels to explore more states
- Introduce logging / diagnostics via ILogger abstractions
- Write property-based tests for complex transformations

## Notes
This README was auto-generated. You can safely edit and commit refinements (the generator will skip existing files unless -Force is used).
