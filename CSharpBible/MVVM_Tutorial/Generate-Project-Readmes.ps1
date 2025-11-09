<#!
.SYNOPSIS
 Generates (or updates) README.md files for all MVVM tutorial example folders (excluding Libraries and Games) and a central root README with a reverse concept index.

.DESCRIPTION
 The script contains metadata for each example project in the MVVM_Tutorial solution. It writes a README.md
 into the root of each corresponding directory (only if that directory exists relative to the script location).
 Existing README.md files can optionally be overwritten by using -Force.

 Additionally, it now generates a central README.md in the script root containing:
   * Overview of all projects (name, summary, test project)
   * Reverse concept index (concept -> projects)
   * Coverage instructions (global)
   * Regeneration instructions

 Each per-project README contains:
   * Title & short elevator pitch
   * Detailed description / learning goals
   * Implemented MVVM / WPF concepts
   * Project structure notes
   * Build & run instructions (classic .NET Framework + modern multi-target variants)
   * Testing & coverage status placeholders (line, branch, and complexity coverage)
   * How to locally collect coverage using coverlet + vstest / dotnet test
   * Link back to central index

.PARAMETER Force
 Overwrite existing README.md files if they already exist.

.EXAMPLE
  ./Generate-Project-Readmes.ps1
  Generates README files, skipping ones that already exist.

.EXAMPLE
  ./Generate-Project-Readmes.ps1 -Force
  Regenerates all README files.

.NOTES
  You can refine coverage numbers after running the provided commands. The script inserts placeholders
  (e.g. TBD or 0.00%) which you should manually replace once metrics are known.
!#>
[CmdletBinding()]
param(
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function New-CoverageSection {
    param(
        [string]$TestProjectHint
    )
@"
## Test & Coverage Status

| Metric | Status |
|--------|--------|
| Unit Tests | Implemented (see associated *Tests* project: $TestProjectHint) |
| Line Coverage | TBD |
| Branch Coverage | TBD |
| Method Coverage | TBD |
| Complexity Coverage | TBD |

### Collecting Coverage Locally

Classic (.NET Framework targets):
```
dotnet test $TestProjectHint --framework net481 /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
```

Modern multi-target (.NET >= 6):
```
dotnet test $TestProjectHint -c Debug /p:CollectCoverage=true /p:CoverletOutputFormat=lcov \
  /p:Exclude="[xunit.*]*,[MSTest.*]*,[NUnit.*]*" --logger "trx" --results-directory ./TestResults
```

To merge coverage from several target frameworks:
```
dotnet tool install --global dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.info" -targetdir:CoverageReport -reporttypes:HtmlSummary;MarkdownSummaryGithub
```

> Replace TBD entries above with the real metrics after running the commands.
"@
}

# Region: Project metadata ---------------------------------------------------------------------------
# Each entry: Name, Folder, ShortSummary, Detailed, Concepts (array), TestProjectHint (best matching test csproj)
$projects = @(
    @{ Name='MVVM_00_Template'; Folder='MVVM_00_Template'; Summary='Baseline MVVM starter template.'; Detailed='Introduces the minimal structure for a WPF MVVM application: Models, ViewModels, Views, and basic INotifyPropertyChanged wiring. Serves as the foundational reference for subsequent incremental examples.'; Concepts=@('Solution layout','INotifyPropertyChanged','DataBinding basics'); Test='MVVM_00_TemplateTests'; },
    @{ Name='MVVM_00a_CTTemplate'; Folder='MVVM_00a_CTTemplate'; Summary='Baseline template using CommunityToolkit.Mvvm.'; Detailed='Shows how the CommunityToolkit.Mvvm source generators (ObservableObject, RelayCommand, etc.) reduce MVVM boilerplate compared with the classic template. Emphasizes attribute-driven property and command generation.'; Concepts=@('CommunityToolkit.Mvvm','ObservableObject','RelayCommand','Source Generators'); Test='MVVM_00a_CTTemplateTests'; },
    @{ Name='MVVM_00_IoCTemplate'; Folder='MVVM_00_IoCTemplate'; Summary='Adds lightweight IoC / DI to the base template.'; Detailed='Demonstrates integrating a simple dependency injection container to centralize service creation (e.g., navigation, data services). Establishes patterns for testability and loose coupling early.'; Concepts=@('Dependency Injection','Service Locator vs DI','Testability'); Test='MVVM_00_IoCTemplateTests'; },
    @{ Name='MVVM_03_NotifyChange'; Folder='MVVM_03_NotifyChange'; Summary='Deeper dive into change notification patterns.'; Detailed='Explores manual implementation details of INotifyPropertyChanged, reducing duplication through helper base classes, and discusses pitfalls (e.g., magic strings vs nameof). Builds on the base template to refine ViewModel ergonomics.'; Concepts=@('INotifyPropertyChanged patterns','nameof usage','Base ViewModel patterns'); Test='MVVM_03_NotifyChangeTests'; },
    @{ Name='MVVM_03a_CTNotifyChange'; Folder='MVVM_03a_CTNotifyChange'; Summary='CommunityToolkit variant of change notification.'; Detailed='Contrasts the handwritten notification logic with attribute/source-generator driven property change support from CommunityToolkit.Mvvm, highlighting maintainability gains.'; Concepts=@('CommunityToolkit.Mvvm','[ObservableProperty]','Partial methods for OnChanged'); Test='MVVM_03a_CTNotifyChangeTests'; },
    @{ Name='MVVM_04_DelegateCommand'; Folder='MVVM_04_DelegateCommand'; Summary='Implements a reusable DelegateCommand.'; Detailed='Introduces commanding in MVVM: enabling and disabling commands, raising CanExecuteChanged, and parameter passing. Sets the groundwork for richer user interactions while keeping logic in ViewModels.'; Concepts=@('ICommand','DelegateCommand pattern','CanExecute management'); Test='MVVM_04_DelegateCommandTests'; },
    @{ Name='MVVM_04a_CTRelayCommand'; Folder='MVVM_04a_CTRelayCommand'; Summary='RelayCommand via CommunityToolkit.'; Detailed='Shows Toolkit RelayCommand / AsyncRelayCommand usage vs manual DelegateCommand, reducing complexity while adding async support and automatic CanExecute notifications.'; Concepts=@('RelayCommand','AsyncRelayCommand','Command generation'); Test='MVVM_04a_CTRelayCommandTests'; },
    @{ Name='MVVM_05_CommandParCalculator'; Folder='MVVM_05_CommandParCalculator'; Summary='Passing parameters into commands.'; Detailed='Demonstrates command parameter binding scenarios (Buttons, MenuItems) and validation/conversion considerations when forwarding user inputs into ViewModel logic.'; Concepts=@('CommandParameter binding','Validation basics','UI to VM data flow'); Test='MVVM_05_CommandParCalculatorTests'; },
    @{ Name='MVVM_05a_CTCommandParCalc'; Folder='MVVM_05a_CTCommandParCalc'; Summary='Toolkit-based parameter command sample.'; Detailed='Replicates parameterized command patterns using CommunityToolkit attributes and RelayCommand to emphasize reduced boilerplate and cleaner handlers.'; Concepts=@('RelayCommand parameters','Source-generated commands'); Test='MVVM_05a_CTCommandParCalcTests'; },
    @{ Name='MVVM_06_Converters'; Folder='MVVM_06_Converters'; Summary='Value converters introduction.'; Detailed='Covers implementing IValueConverter and binding scenarios to adapt Model data to UI-friendly formats. Focus on single-value converters and reuse strategies.'; Concepts=@('IValueConverter','Data formatting','XAML binding'); Test='MVVM_06_ConvertersTests'; },
    @{ Name='MVVM_06_Converters_2'; Folder='MVVM_06_Converters_2'; Summary='Expanded converter catalog.'; Detailed='Adds more specialized converters (null/empty checks, boolean inversions, formatting) and discusses when to prefer DataTriggers or template selectors instead.'; Concepts=@('Chaining converters','Maintainability','Alternatives to converters'); Test='MVVM_06_Converters_4Tests'; },
    @{ Name='MVVM_06_Converters_3'; Folder='MVVM_06_Converters_3'; Summary='Multi-value conversions.'; Detailed='Introduces IMultiValueConverter, binding multiple sources, and combining state (e.g., validation + formatting) into a single presentation value.'; Concepts=@('IMultiValueConverter','MultiBinding','Aggregation'); Test='MVVM_06_Converters_3Tests'; },
    @{ Name='MVVM_06_Converters_4'; Folder='MVVM_06_Converters_4'; Summary='Advanced / performance notes for converters.'; Detailed='Explores converter performance, caching, stateless design, and ensuring thread-safety. Also contrasts converters vs computed ViewModel properties.'; Concepts=@('Performance considerations','Stateless design','Testing converters'); Test='MVVM_06_Converters_4Tests'; },
    @{ Name='MVVM_09_DialogBoxes'; Folder='MVVM_09_DialogBoxes'; Summary='Dialog service abstraction.'; Detailed='Shows decoupling modal dialogs from Views via an IDialogService abstraction, enabling testability and alternative UI shells.'; Concepts=@('Dialog service','Service abstraction','Test doubles'); Test='MVVM_09_DialogBoxesTest'; },
    @{ Name='MVVM_09a_CTDialogBoxes'; Folder='MVVM_09a_CTDialogBoxes'; Summary='Toolkit-friendly dialog pattern.'; Detailed='Adapts the dialog service concept using Toolkit features (e.g., Messenger) for looser coupling and optional asynchronous dialog flows.'; Concepts=@('Messenger','Async dialogs','Loose coupling'); Test='MVVM_09a_CTDialogBoxesTests'; },
    @{ Name='MVVM_16_UserControl1'; Folder='MVVM_16_Usercontrol1'; Summary='Composing UI with UserControls.'; Detailed='Introduces splitting complex Views into smaller UserControls with their own dependency properties and binding contexts.'; Concepts=@('UserControl composition','DependencyProperty basics','Design-time data'); Test='MVVM_16_UserControl1Tests'; },
    @{ Name='MVVM_16_UserControl2'; Folder='MVVM_16_Usercontrol2'; Summary='Advanced UserControl patterns.'; Detailed='Explores reusability, theming, and properly exposing dependency properties with validation callbacks and change notification.'; Concepts=@('DependencyProperty metadata','Theming','Reusability'); Test='MVVM_16_UserControl1Tests'; },
    @{ Name='MVVM_17_1_CSV_Laden'; Folder='MVVM_17_1_CSV_Laden'; Summary='Loading CSV data as a model source.'; Detailed='Demonstrates importing external CSV data, parsing to strongly typed models, and surfacing them through observable collections with sorting/filtering potential.'; Concepts=@('File IO','Data parsing','ObservableCollection'); Test='(none explicit)'; },
    @{ Name='MVVM_18_MultiConverters'; Folder='MVVM_18_MultiConverters'; Summary='Combining multiple converter concepts.'; Detailed='Integrates single & multi-value converters, conditional formatting, and introduces scenario-driven converter selection for complex UIs.'; Concepts=@('Converter orchestration','Conditional formatting','Maintainability'); Test='MVVM_18_MultiConvertersTests'; },
    @{ Name='MVVM_19_FilterLists'; Folder='MVVM_19_FilterLists'; Summary='Collection filtering & sorting.'; Detailed='Focuses on ICollectionView for dynamic filtering, sorting, and grouping without mutating the underlying collection.'; Concepts=@('ICollectionView','Filtering','Grouping'); Test='(none explicit)' },
    @{ Name='MVVM_20_Sysdialogs'; Folder='MVVM_20_Sysdialogs'; Summary='Wrapping system dialogs.'; Detailed='Abstracts common system dialogs (OpenFileDialog, SaveFileDialog) behind an interface to support testing and future replacement (e.g., custom dialogs).'; Concepts=@('System dialogs abstraction','Testability','Wrapper services'); Test='MVVM_20_SysdialogsTests' },
    @{ Name='MVVM_20a_CTSysdialogs'; Folder='MVVM_20a_CTSysdialogs'; Summary='Toolkit variant for system dialogs.'; Detailed='Uses Toolkit messaging / DI helpers to streamline dialog invocation while keeping ViewModels decoupled.'; Concepts=@('Messenger integration','Dialog wrappers','DI patterns'); Test='MVVM_20a_CTSysdialogsTests' },
    @{ Name='MVVM_21_Buttons'; Folder='MVVM_21_Buttons'; Summary='Command binding showcase (various button styles).'; Detailed='Explores styling, templating, and commanding for buttons, including enabling/disabling logic, icons, and keyboard gesture hints.'; Concepts=@('Button commanding','Styling','Input gestures'); Test='(none explicit)' },
    @{ Name='MVVM_22_CTWpfCap'; Folder='MVVM_22_CTWpfCap'; Summary='CommunityToolkit + WPF capabilities sample.'; Detailed='Aggregates several Toolkit features (ObservableObject, Messenger, DI) with WPF-specific capabilities to illustrate synergy.'; Concepts=@('Toolkit integration','Messenger','DI'); Test='MVVM_22_CTWpfCapTests' },
    @{ Name='MVVM_22_WpfCap'; Folder='MVVM_22_WpfCap'; Summary='WPF capabilities without Toolkit.'; Detailed='Parallel example to contrast pure WPF approaches vs Toolkit-enhanced version for educational comparison.'; Concepts=@('WPF core APIs','Comparison approach'); Test='MVVM_22_WpfCapTests' },
    @{ Name='MVVM_24_UserControl'; Folder='MVVM_24_UserControl'; Summary='Reusable simple UserControl pattern.'; Detailed='Foundational approach to exposing properties and events from a custom UserControl while keeping MVVM separation intact.'; Concepts=@('UserControl API design','Binding forwarding'); Test='MVVM_24_UserControlTests' },
    @{ Name='MVVM_24a_CTUserControl'; Folder='MVVM_24a_CTUserControl'; Summary='Toolkit-enhanced UserControl.'; Detailed='Uses source-generated ViewModels paired with UserControls to reduce notifier boilerplate and promote consistency.'; Concepts=@('Toolkit ViewModels','Simplified binding'); Test='MVVM_24a_CTUserControlTests' },
    @{ Name='MVVM_24b_UserControl'; Folder='MVVM_24b_UserControl'; Summary='Intermediate composite UserControl.'; Detailed='Adds nested controls, event-to-command wiring, and design-time data facilitation for richer composition.'; Concepts=@('Nested composition','Event-to-command'); Test='MVVM_24b_UserControlTests' },
    @{ Name='MVVM_24c_CTUserControl'; Folder='MVVM_24c_CTUserControl'; Summary='Advanced Toolkit UserControl sample.'; Detailed='Shows incremental sophistication: dependency properties + Toolkit messaging + command routing for decoupled component interaction.'; Concepts=@('Messenger','Command routing','Component isolation'); Test='MVVM_24c_CTUserControlTests' },
    @{ Name='MVVM_25_RichTextEdit'; Folder='MVVM_25_RichTextEdit'; Summary='Binding & manipulating RichTextBox content.'; Detailed='Demonstrates converting FlowDocument content to/from persistable formats (e.g., XAML, plain text) and exposing editing commands via MVVM.'; Concepts=@('RichTextBox','FlowDocument','Document serialization'); Test='MVVM_25_RichTextEditTests' },
    @{ Name='MVVM_26_CTBindingGroupExp'; Folder='MVVM_26_CTBindingGroupExp'; Summary='BindingGroup experimentation (Toolkit).'; Detailed='Explores BindingGroup for multi-field validation commits combined with Toolkit ViewModels for concise logic.'; Concepts=@('BindingGroup','Multi-field validation','Commit/Cancel'); Test='(none explicit)' },
    @{ Name='MVVM_26_BindingGroupExp'; Folder='MVVM_26_BindingGroupExp'; Summary='BindingGroup exploration (pure WPF).'; Detailed='Focuses on validation flows, staging edits, and error presentation without additional libraries.'; Concepts=@('Validation UX','Deferred commit','Error templates'); Test='(none explicit)' },
    @{ Name='MVVM_27_DataGrid'; Folder='MVVM_27_DataGrid'; Summary='DataGrid fundamentals.'; Detailed='Introduces binding collections to DataGrid, auto vs explicit columns, selection handling, and simple editing.'; Concepts=@('DataGrid basics','Selection','Inline editing'); Test='MVVM_27_DataGridTests' },
    @{ Name='MVVM_28_DataGrid'; Folder='MVVM_28_DataGrid'; Summary='Advanced DataGrid scenarios.'; Detailed='Adds sorting customization, grouping, styling rows/cells, and command integration for batch operations.'; Concepts=@('Custom sorting','Grouping','Styling'); Test='MVVM_28_DataGridTests' },
    @{ Name='MVVM_28_1_DataGridExt'; Folder='MVVM_28_1_DataGridExt'; Summary='Extended DataGrid behaviors.'; Detailed='Implements reusable attached behaviors (e.g., auto-scroll, selection sync) to keep ViewModels slim.'; Concepts=@('Attached behaviors','Separation of concerns'); Test='MVVM_28_1_DataGridExtTests' },
    @{ Name='MVVM_28_1_CTDataGridExt'; Folder='MVVM_28_1_CTDataGridExt'; Summary='Toolkit variant of extended DataGrid.'; Detailed='Combines attached behaviors with Toolkit messaging/commands for decoupled coordination and improved testability.'; Concepts=@('Messenger coordination','Extended behaviors'); Test='MVVM_28_1_CTDataGridExtTests' },
    @{ Name='MVVM_31_Validation1'; Folder='MVVM_31_Validation1'; Summary='Property-level validation patterns.'; Detailed='Implements IDataErrorInfo / INotifyDataErrorInfo patterns for immediate feedback and discusses UX trade-offs.'; Concepts=@('IDataErrorInfo','INotifyDataErrorInfo','Validation messages'); Test='MVVM_31_Validation1Tests' },
    @{ Name='MVVM_31_Validation2'; Folder='MVVM_31_Validation2'; Summary='Cross-field & async validation.'; Detailed='Adds cross-property rules and async validation flows (e.g., server uniqueness check) while keeping UI responsive.'; Concepts=@('Cross-field rules','Async validation'); Test='MVVM_31_Validation2Tests' },
    @{ Name='MVVM_31a_CTValidation1'; Folder='MVVM_31a_CTValidation1'; Summary='Toolkit simplified validation (basic).'; Detailed='Leverages Toolkit generated properties plus partial methods for inlined validation rule hooks.'; Concepts=@('Toolkit validation hooks','Error propagation'); Test='MVVM_31a_CTValidation1Tests' },
    @{ Name='MVVM_31a_CTValidation2'; Folder='MVVM_31a_CTValidation2'; Summary='Toolkit validation (intermediate).'; Detailed='Introduces aggregated validation results and messaging for global status banners.'; Concepts=@('Aggregate validation','Messaging'); Test='MVVM_31a_CTValidation2Tests' },
    @{ Name='MVVM_31a_CTValidation3'; Folder='MVVM_31a_CTValidation3'; Summary='Toolkit validation (advanced).'; Detailed='Adds staged validation, debounced async checks, and rule composition for maintainability.'; Concepts=@('Debouncing','Rule composition','Async patterns'); Test='MVVM_31a_CTValidation3Tests' },
    @{ Name='MVVM_33_Events_to_Commands'; Folder='MVVM_33_Events_to_Commands'; Summary='Event-to-command bridging.'; Detailed='Demonstrates converting UI events (e.g., selection changed) into ICommand invocations via behaviors, enabling full MVVM separation.'; Concepts=@('Behaviors','EventToCommand','Loose coupling'); Test='MVVM_33_Events_to_CommandsTests' },
    @{ Name='MVVM_33a_CTEvents_To_Commands'; Folder='MVVM_33a_CTEvents_To_Commands'; Summary='Toolkit event-to-command patterns.'; Detailed='Uses Toolkit + behaviors + messenger to route events with payload enrichment to listening ViewModels.'; Concepts=@('Messenger payloads','Advanced behaviors'); Test='MVVM_33a_CTEvents_To_CommandsTests' },
    @{ Name='MVVM_34_BindingEventArgs'; Folder='MVVM_34_BindingEventArgs'; Summary='Inspecting binding event args.'; Detailed='Shows interception / inspection of binding events for diagnostics, logging, or adaptive UX (e.g., dynamic validation severity).' ; Concepts=@('Binding diagnostics','Event args inspection'); Test='MVVM_34_BindingEventArgsTests' },
    @{ Name='MVVM_34a_CTBindingEventArgs'; Folder='MVVM_34a_CTBindingEventArgs'; Summary='Toolkit binding event insights.'; Detailed='Extends the diagnostics scenario with Toolkit services (logging, messaging) to centralize reporting.'; Concepts=@('Centralized logging','Toolkit messenger'); Test='MVVM_34a_CTBindingEventArgsTests' },
    @{ Name='MVVM_35_CommunityToolkit'; Folder='MVVM_35_CommunityToolkit'; Summary='Broader CommunityToolkit feature tour.'; Detailed='Explores additional Toolkit features: WeakReferenceMessenger, ObservableValidator, and DI patterns for scalable MVVM architectures.'; Concepts=@('WeakReferenceMessenger','ObservableValidator','DI refinement'); Test='MVVM_35_CommunityToolkitTests' },
    @{ Name='MVVM_36_ComToolKtSavesWork'; Folder='MVVM_36_ComToolKtSavesWork'; Summary='Productivity boosters with Toolkit.'; Detailed='Aggregates small productivity examples (async commands, generated properties, messenger channels) to highlight cumulative maintenance savings.'; Concepts=@('Async patterns','Messenger channels','Boilerplate reduction'); Test='MVVM_36_ComToolKtSavesWorkTests' },
    @{ Name='MVVM_37_TreeView'; Folder='MVVM_37_TreeView'; Summary='Hierarchical data & TreeView.'; Detailed='Presents hierarchical ViewModels, lazy loading children, and selection synchronization patterns.'; Concepts=@('Hierarchical ViewModels','Lazy loading','Selection sync'); Test='MVVM_37_TreeViewTests' },
    @{ Name='MVVM_38_CTDependencyInjection'; Folder='MVVM_38_CTDependencyInjection'; Summary='DI patterns with Toolkit integration.'; Detailed='Demonstrates structured registration of services, lifetime management, and consumption through constructor injection / factory patterns.'; Concepts=@('Service lifetimes','Factory pattern','ViewModel injection'); Test='MVVM_38_CTDependencyInjectionTests' },
    @{ Name='MVVM_39_MultiModelTest'; Folder='MVVM_39_MultiModelTest'; Summary='Coordinating multiple model types.'; Detailed='Illustrates handling several domain model aggregates in one UI, orchestrating interactions and consistency across them.'; Concepts=@('Aggregate coordination','State management'); Test='MVVM_39_MultiModelTestTests' },
    @{ Name='MVVM_40_Wizzard'; Folder='MVVM_40_Wizzard'; Summary='Multi-step wizard workflow.'; Detailed='Implements a step-based navigation manager, forward/back validation, and transient vs committed state handling.'; Concepts=@('Wizard navigation','State staging','Validation gating'); Test='MVVM_40_WizzardTests' },
    @{ Name='MVVM_41_Sudoku'; Folder='MVVM_41_Sudoku'; Summary='MVVM applied to a Sudoku game.'; Detailed='Applies MVVM patterns to a small game: board modeling, cell state validation, and solving assistance hints while keeping UI decoupled.'; Concepts=@('Game state model','Validation overlays','Command orchestration'); Test='MVVM_41_SudokuTests' },
    @{ Name='MVVM_99_SomeIssue'; Folder='MVVM_99_SomeIssue'; Summary='Sandbox for reproducing / isolating issues.'; Detailed='A scratchpad project used to reproduce edge cases, experiment with fixes, and document resolutions before integrating changes elsewhere.'; Concepts=@('Issue reproduction','Isolation','Rapid iteration'); Test='MVVM_99_SomeIssueTests' },
    @{ Name='MVVM_AllExamples'; Folder='MVVM_AllExamples'; Summary='Aggregate launcher for all examples.'; Detailed='Provides a consolidated shell / menu to navigate and launch individual example modules, useful for demonstrations and regression verification.'; Concepts=@('Module navigation','Shell aggregation','Sample discovery'); Test='(none explicit)' }
)

# ----------------------------------------------------------------------------------------------------
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root

# Build reverse concept index
$conceptIndex = @{}
foreach ($proj in $projects) {
    foreach ($c in $proj.Concepts) {
        if (-not $conceptIndex.ContainsKey($c)) { $conceptIndex[$c] = New-Object System.Collections.Generic.List[string] }
        $conceptIndex[$c].Add($proj.Name)
    }
}

# Generate per-project README files
foreach ($p in $projects) {
    $dir = Join-Path $root $p.Folder
    if (-not (Test-Path $dir)) { Write-Warning "Skip (missing directory): $($p.Folder)"; continue }
    $readme = Join-Path $dir 'README.md'
    if ((Test-Path $readme) -and -not $Force) {
        Write-Host "Skip existing: $($p.Folder)/README.md" -ForegroundColor Yellow
        continue
    }

    $conceptList = ($p.Concepts | ForEach-Object { "- $_" }) -join "`n"
    $coverage = New-CoverageSection -TestProjectHint $p.Test
    $content = @"
# $($p.Name)

> $($p.Summary)

[Back to central project index](../README.md)

## Overview
$p.Detailed

## Key Learning Goals
$conceptList

## Project Structure
- Views: WPF XAML views illustrating bindings and styles
- ViewModels: Reactive presentation logic using either classic or Toolkit paradigms
- Models: Plain data / domain classes kept free of UI concerns
- Services: (Where applicable) abstractions for dialogs, navigation, data access
- Behaviors / Helpers: Reusable interaction patterns extending XAML without code-behind

## Build & Run
```
dotnet build $($p.Folder)
```
If multiple target frameworks (e.g., net6.0-windows + net481) exist, you can specify one:
```
dotnet build $($p.Folder) -f net6.0-windows
```
Run (where an executable host exists):
```
dotnet run --project $($p.Folder) -f net6.0-windows
```

## Testing
If a companion test project exists it is listed below. Execute:
```
dotnet test $($p.Test)
```
(If '(none explicit)' the example is illustrated without dedicated automated tests yet.)

$coverage

## Extending This Example
- Add additional ViewModels to explore more states
- Introduce logging / diagnostics via ILogger abstractions
- Write property-based tests for complex transformations

## Notes
This README was auto-generated. You can safely edit and commit refinements (the generator will skip existing files unless -Force is used).
"@

    Set-Content -Path $readme -Value $content -Encoding UTF8
    Write-Host "Written: $($p.Folder)/README.md" -ForegroundColor Green
}

# Generate central README --------------------------------------------------------------
$centralReadmePath = Join-Path $root 'README_dir.md'
if ((Test-Path $centralReadmePath) -and -not $Force) {
    Write-Host 'Central README_dir.md exists - use -Force to overwrite.' -ForegroundColor Yellow
} else {
    # Project overview table
    $projRows = foreach ($p in $projects) {
        $test = if ($p.Test -and $p.Test -ne '(none explicit)') { $p.Test } else { '' }
        "| [$($p.Name)]($($p.Folder)/README_dir.md) | $($p.Summary) | $test |"
    }

    # Concept reverse index
    $conceptSections = foreach ($k in ($conceptIndex.Keys | Sort-Object)) {
        $plist = ($conceptIndex[$k] | Sort-Object | ForEach-Object { "`n  - [$_]($_/README_dir.md)" }) -join ''
        "### $k$plist"  # already includes newlines via backtick n
    }

    $central = @"
# MVVM Tutorial Collection

Comprehensive set of incremental MVVM + WPF examples (classic patterns and CommunityToolkit variants). Each project focuses on a narrowly scoped learning goal so concepts build progressively.

## How To Use This Repository
1. Start with baseline templates (00*) to understand structure.
2. Progress through notification, commands, converters, and dialog abstractions.
3. Explore advanced composition (UserControls, DataGrid, TreeView) and validation patterns.
4. Compare classic vs Toolkit implementations to evaluate trade-offs.
5. Use the reverse concept index below to jump directly to examples covering a topic.

## Project Overview
| Project | Summary | Test Project |
|---------|---------|--------------|
$($projRows -join "`n")

## Reverse Concept Index
Each concept lists all example projects demonstrating it.

$($conceptSections -join "`n`n")

## Global Testing & Coverage
Execute all tests (multi-target frameworks will run each target):
```
dotnet test --collect:"XPlat Code Coverage"
```
Custom coverage with coverlet (example):
```
dotnet test MVVM_27_DataGridTests -c Debug /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
```
Merge and report:
```
reportgenerator -reports:"**/coverage.info" -targetdir:CoverageReport -reporttypes:HtmlSummary;MarkdownSummaryGithub
```

## Regenerating READMEs
Run the script:
```
./Generate-Project-Readmes.ps1 -Force
```
This will overwrite all per-project README.md files and the central README.md.

## Contributing
- Add new example metadata to the `$projects` array in the script.
- Ensure concepts list is accurate (improves the reverse index).
- Add tests in parallel *Tests projects for higher confidence and coverage.

## License / Usage
Educational use; adapt freely. Keep attribution to the original authors where appropriate.
"@

    Set-Content -Path $centralReadmePath -Value $central -Encoding UTF8
    Write-Host 'Central README.md written.' -ForegroundColor Green
}

Pop-Location
Write-Host 'Done.' -ForegroundColor Cyan
