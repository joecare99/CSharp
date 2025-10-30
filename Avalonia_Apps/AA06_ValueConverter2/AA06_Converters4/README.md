# AA06_Converters_4

> Advanced / performance notes for converters - Avalonia UI Edition

## Overview
Converted from WPF to Avalonia UI with AGV (Automated Guided Vehicle) simulation.

## Key Learning Goals
- Performance considerations
- Stateless design
- Testing converters
- Value converters in Avalonia
- Custom rendering with Canvas
- Dependency Injection with Microsoft.Extensions.DependencyInjection
- MVVM with CommunityToolkit.Mvvm

## Project Structure
- Views: Avalonia XAML views (axaml) illustrating bindings and styles
- ViewModels: Reactive presentation logic using CommunityToolkit.Mvvm
- Models: Plain data / domain classes with INotifyPropertyChanged
- ValueConverter: Custom value converters for data formatting
- Services: DI-based service configuration
- Behaviors / Helpers: Reusable interaction patterns

## Key Features
- **AGV Vehicle Simulation**: Interactive visualization of a 4-wheel automated guided vehicle
- **Real-time Calculations**: Computes vehicle velocity, rotation, and wheel dynamics
- **Value Converters**: Custom converters for units (mm, degrees, velocities)
- **Dependency Injection**: Full DI setup with constructor injection
- **Modern MVVM**: Using CommunityToolkit.Mvvm source generators

## Namespace Migration
This project was migrated from:
- **Old namespace**: `MVVM_06_Converters_4`
- **New namespace**: `AA06_Converters_4`
- **WPF → Avalonia**: All UI components converted to Avalonia equivalents

## Build & Run
```bash
dotnet build AA06_Converters_4
```

Run:
```bash
dotnet run --project AA06_Converters_4
```

## Testing
```bash
dotnet test AA06_Converters4Tests
```

## Test & Coverage Status

| Metric | Status |
|--------|--------|
| Unit Tests | Implemented (see AA06_Converters4Tests) |
| Line Coverage | TBD |
| Branch Coverage | TBD |
| Method Coverage | TBD |
| Complexity Coverage | TBD |

### Collecting Coverage Locally

```bash
dotnet test AA06_Converters4Tests -c Debug /p:CollectCoverage=true /p:CoverletOutputFormat=lcov \
  /p:Exclude="[xunit.*]*,[MSTest.*]*,[NUnit.*]*" --logger "trx" --results-directory ./TestResults
```

To merge coverage from several target frameworks:
```bash
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.info" -targetdir:CoverageReport -reporttypes:HtmlSummary;MarkdownSummaryGithub
```

## Extending This Example
- Complete the PlotFrame visualization with full rendering implementation
- Add additional vehicle types with different wheel configurations
- Introduce logging / diagnostics via ILogger abstractions
- Implement animation for vehicle movement
- Add export functionality for simulation data

## Technical Notes
- **Target Frameworks**: .NET 8.0, .NET 9.0
- **UI Framework**: Avalonia UI 11.2.2
- **MVVM Toolkit**: CommunityToolkit.Mvvm 8.4.0
- **DI Container**: Microsoft.Extensions.DependencyInjection 9.0.9
- **Math Library**: Custom MathLibrary for 2D vector calculations

## Migration Notes
The PlotFrame visualization uses a simplified Canvas implementation. The full WPF rendering logic
with `WindowPortToGridLines` converter requires additional work to port the complex shape rendering
to Avalonia's visual system. Current implementation provides the structure with placeholders for
the full rendering implementation.

## Notes
This README documents the Avalonia UI conversion of the original WPF project.
