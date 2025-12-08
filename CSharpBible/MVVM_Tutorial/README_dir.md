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
| [MVVM_00_Template](MVVM_00_Template/README_dir.md) | Baseline MVVM starter template. | MVVM_00_TemplateTests |
| [MVVM_00a_CTTemplate](MVVM_00a_CTTemplate/README_dir.md) | Baseline template using CommunityToolkit.Mvvm. | MVVM_00a_CTTemplateTests |
| [MVVM_00_IoCTemplate](MVVM_00_IoCTemplate/README_dir.md) | Adds lightweight IoC / DI to the base template. | MVVM_00_IoCTemplateTests |
| [MVVM_03_NotifyChange](MVVM_03_NotifyChange/README_dir.md) | Deeper dive into change notification patterns. | MVVM_03_NotifyChangeTests |
| [MVVM_03a_CTNotifyChange](MVVM_03a_CTNotifyChange/README_dir.md) | CommunityToolkit variant of change notification. | MVVM_03a_CTNotifyChangeTests |
| [MVVM_04_DelegateCommand](MVVM_04_DelegateCommand/README_dir.md) | Implements a reusable DelegateCommand. | MVVM_04_DelegateCommandTests |
| [MVVM_04a_CTRelayCommand](MVVM_04a_CTRelayCommand/README_dir.md) | RelayCommand via CommunityToolkit. | MVVM_04a_CTRelayCommandTests |
| [MVVM_05_CommandParCalculator](MVVM_05_CommandParCalculator/README_dir.md) | Passing parameters into commands. | MVVM_05_CommandParCalculatorTests |
| [MVVM_05a_CTCommandParCalc](MVVM_05a_CTCommandParCalc/README_dir.md) | Toolkit-based parameter command sample. | MVVM_05a_CTCommandParCalcTests |
| [MVVM_06_Converters](MVVM_06_Converters/README_dir.md) | Value converters introduction. | MVVM_06_ConvertersTests |
| [MVVM_06_Converters_2](MVVM_06_Converters_2/README_dir.md) | Expanded converter catalog. | MVVM_06_Converters_4Tests |
| [MVVM_06_Converters_3](MVVM_06_Converters_3/README_dir.md) | Multi-value conversions. | MVVM_06_Converters_3Tests |
| [MVVM_06_Converters_4](MVVM_06_Converters_4/README_dir.md) | Advanced / performance notes for converters. | MVVM_06_Converters_4Tests |
| [MVVM_09_DialogBoxes](MVVM_09_DialogBoxes/README_dir.md) | Dialog service abstraction. | MVVM_09_DialogBoxesTest |
| [MVVM_09a_CTDialogBoxes](MVVM_09a_CTDialogBoxes/README_dir.md) | Toolkit-friendly dialog pattern. | MVVM_09a_CTDialogBoxesTests |
| [MVVM_16_UserControl1](MVVM_16_Usercontrol1/README_dir.md) | Composing UI with UserControls. | MVVM_16_UserControl1Tests |
| [MVVM_16_UserControl2](MVVM_16_Usercontrol2/README_dir.md) | Advanced UserControl patterns. | MVVM_16_UserControl1Tests |
| [MVVM_17_1_CSV_Laden](MVVM_17_1_CSV_Laden/README_dir.md) | Loading CSV data as a model source. |  |
| [MVVM_18_MultiConverters](MVVM_18_MultiConverters/README_dir.md) | Combining multiple converter concepts. | MVVM_18_MultiConvertersTests |
| [MVVM_19_FilterLists](MVVM_19_FilterLists/README_dir.md) | Collection filtering & sorting. |  |
| [MVVM_20_Sysdialogs](MVVM_20_Sysdialogs/README_dir.md) | Wrapping system dialogs. | MVVM_20_SysdialogsTests |
| [MVVM_20a_CTSysdialogs](MVVM_20a_CTSysdialogs/README_dir.md) | Toolkit variant for system dialogs. | MVVM_20a_CTSysdialogsTests |
| [MVVM_21_Buttons](MVVM_21_Buttons/README_dir.md) | Command binding showcase (various button styles). |  |
| [MVVM_22_CTWpfCap](MVVM_22_CTWpfCap/README_dir.md) | CommunityToolkit + WPF capabilities sample. | MVVM_22_CTWpfCapTests |
| [MVVM_22_WpfCap](MVVM_22_WpfCap/README_dir.md) | WPF capabilities without Toolkit. | MVVM_22_WpfCapTests |
| [MVVM_24_UserControl](MVVM_24_UserControl/README_dir.md) | Reusable simple UserControl pattern. | MVVM_24_UserControlTests |
| [MVVM_24a_CTUserControl](MVVM_24a_CTUserControl/README_dir.md) | Toolkit-enhanced UserControl. | MVVM_24a_CTUserControlTests |
| [MVVM_24b_UserControl](MVVM_24b_UserControl/README_dir.md) | Intermediate composite UserControl. | MVVM_24b_UserControlTests |
| [MVVM_24c_CTUserControl](MVVM_24c_CTUserControl/README_dir.md) | Advanced Toolkit UserControl sample. | MVVM_24c_CTUserControlTests |
| [MVVM_25_RichTextEdit](MVVM_25_RichTextEdit/README_dir.md) | Binding & manipulating RichTextBox content. | MVVM_25_RichTextEditTests |
| [MVVM_26_CTBindingGroupExp](MVVM_26_CTBindingGroupExp/README_dir.md) | BindingGroup experimentation (Toolkit). |  |
| [MVVM_26_BindingGroupExp](MVVM_26_BindingGroupExp/README_dir.md) | BindingGroup exploration (pure WPF). |  |
| [MVVM_27_DataGrid](MVVM_27_DataGrid/README_dir.md) | DataGrid fundamentals. | MVVM_27_DataGridTests |
| [MVVM_28_DataGrid](MVVM_28_DataGrid/README_dir.md) | Advanced DataGrid scenarios. | MVVM_28_DataGridTests |
| [MVVM_28_1_DataGridExt](MVVM_28_1_DataGridExt/README_dir.md) | Extended DataGrid behaviors. | MVVM_28_1_DataGridExtTests |
| [MVVM_28_1_CTDataGridExt](MVVM_28_1_CTDataGridExt/README_dir.md) | Toolkit variant of extended DataGrid. | MVVM_28_1_CTDataGridExtTests |
| [MVVM_31_Validation1](MVVM_31_Validation1/README_dir.md) | Property-level validation patterns. | MVVM_31_Validation1Tests |
| [MVVM_31_Validation2](MVVM_31_Validation2/README_dir.md) | Cross-field & async validation. | MVVM_31_Validation2Tests |
| [MVVM_31a_CTValidation1](MVVM_31a_CTValidation1/README_dir.md) | Toolkit simplified validation (basic). | MVVM_31a_CTValidation1Tests |
| [MVVM_31a_CTValidation2](MVVM_31a_CTValidation2/README_dir.md) | Toolkit validation (intermediate). | MVVM_31a_CTValidation2Tests |
| [MVVM_31a_CTValidation3](MVVM_31a_CTValidation3/README_dir.md) | Toolkit validation (advanced). | MVVM_31a_CTValidation3Tests |
| [MVVM_33_Events_to_Commands](MVVM_33_Events_to_Commands/README_dir.md) | Event-to-command bridging. | MVVM_33_Events_to_CommandsTests |
| [MVVM_33a_CTEvents_To_Commands](MVVM_33a_CTEvents_To_Commands/README_dir.md) | Toolkit event-to-command patterns. | MVVM_33a_CTEvents_To_CommandsTests |
| [MVVM_34_BindingEventArgs](MVVM_34_BindingEventArgs/README_dir.md) | Inspecting binding event args. | MVVM_34_BindingEventArgsTests |
| [MVVM_34a_CTBindingEventArgs](MVVM_34a_CTBindingEventArgs/README_dir.md) | Toolkit binding event insights. | MVVM_34a_CTBindingEventArgsTests |
| [MVVM_35_CommunityToolkit](MVVM_35_CommunityToolkit/README_dir.md) | Broader CommunityToolkit feature tour. | MVVM_35_CommunityToolkitTests |
| [MVVM_36_ComToolKtSavesWork](MVVM_36_ComToolKtSavesWork/README_dir.md) | Productivity boosters with Toolkit. | MVVM_36_ComToolKtSavesWorkTests |
| [MVVM_37_TreeView](MVVM_37_TreeView/README_dir.md) | Hierarchical data & TreeView. | MVVM_37_TreeViewTests |
| [MVVM_38_CTDependencyInjection](MVVM_38_CTDependencyInjection/README_dir.md) | DI patterns with Toolkit integration. | MVVM_38_CTDependencyInjectionTests |
| [MVVM_39_MultiModelTest](MVVM_39_MultiModelTest/README_dir.md) | Coordinating multiple model types. | MVVM_39_MultiModelTestTests |
| [MVVM_40_Wizzard](MVVM_40_Wizzard/README_dir.md) | Multi-step wizard workflow. | MVVM_40_WizzardTests |
| [MVVM_41_Sudoku](MVVM_41_Sudoku/README_dir.md) | MVVM applied to a Sudoku game. | MVVM_41_SudokuTests |
| [MVVM_99_SomeIssue](MVVM_99_SomeIssue/README_dir.md) | Sandbox for reproducing / isolating issues. | MVVM_99_SomeIssueTests |
| [MVVM_AllExamples](MVVM_AllExamples/README_dir.md) | Aggregate launcher for all examples. |  |

## Reverse Concept Index
Each concept lists all example projects demonstrating it.

### [ObservableProperty]
  - [MVVM_03a_CTNotifyChange](MVVM_03a_CTNotifyChange/README_dir.md)

### Advanced behaviors
  - [MVVM_33a_CTEvents_To_Commands](MVVM_33a_CTEvents_To_Commands/README_dir.md)

### Aggregate coordination
  - [MVVM_39_MultiModelTest](MVVM_39_MultiModelTest/README_dir.md)

### Aggregate validation
  - [MVVM_31a_CTValidation2](MVVM_31a_CTValidation2/README_dir.md)

### Aggregation
  - [MVVM_06_Converters_3](MVVM_06_Converters_3/README_dir.md)

### Alternatives to converters
  - [MVVM_06_Converters_2](MVVM_06_Converters_2/README_dir.md)

### Async dialogs
  - [MVVM_09a_CTDialogBoxes](MVVM_09a_CTDialogBoxes/README_dir.md)

### Async patterns
  - [MVVM_31a_CTValidation3](MVVM_31a_CTValidation3/README_dir.md)
  - [MVVM_36_ComToolKtSavesWork](MVVM_36_ComToolKtSavesWork/README_dir.md)

### Async validation
  - [MVVM_31_Validation2](MVVM_31_Validation2/README_dir.md)

### AsyncRelayCommand
  - [MVVM_04a_CTRelayCommand](MVVM_04a_CTRelayCommand/README_dir.md)

### Attached behaviors
  - [MVVM_28_1_DataGridExt](MVVM_28_1_DataGridExt/README_dir.md)

### Base ViewModel patterns
  - [MVVM_03_NotifyChange](MVVM_03_NotifyChange/README_dir.md)

### Behaviors
  - [MVVM_33_Events_to_Commands](MVVM_33_Events_to_Commands/README_dir.md)

### Binding diagnostics
  - [MVVM_34_BindingEventArgs](MVVM_34_BindingEventArgs/README_dir.md)

### Binding forwarding
  - [MVVM_24_UserControl](MVVM_24_UserControl/README_dir.md)

### BindingGroup
  - [MVVM_26_CTBindingGroupExp](MVVM_26_CTBindingGroupExp/README_dir.md)

### Boilerplate reduction
  - [MVVM_36_ComToolKtSavesWork](MVVM_36_ComToolKtSavesWork/README_dir.md)

### Button commanding
  - [MVVM_21_Buttons](MVVM_21_Buttons/README_dir.md)

### CanExecute management
  - [MVVM_04_DelegateCommand](MVVM_04_DelegateCommand/README_dir.md)

### Centralized logging
  - [MVVM_34a_CTBindingEventArgs](MVVM_34a_CTBindingEventArgs/README_dir.md)

### Chaining converters
  - [MVVM_06_Converters_2](MVVM_06_Converters_2/README_dir.md)

### Command generation
  - [MVVM_04a_CTRelayCommand](MVVM_04a_CTRelayCommand/README_dir.md)

### Command orchestration
  - [MVVM_41_Sudoku](MVVM_41_Sudoku/README_dir.md)

### Command routing
  - [MVVM_24c_CTUserControl](MVVM_24c_CTUserControl/README_dir.md)

### CommandParameter binding
  - [MVVM_05_CommandParCalculator](MVVM_05_CommandParCalculator/README_dir.md)

### Commit/Cancel
  - [MVVM_26_CTBindingGroupExp](MVVM_26_CTBindingGroupExp/README_dir.md)

### CommunityToolkit.Mvvm
  - [MVVM_00a_CTTemplate](MVVM_00a_CTTemplate/README_dir.md)
  - [MVVM_03a_CTNotifyChange](MVVM_03a_CTNotifyChange/README_dir.md)

### Comparison approach
  - [MVVM_22_WpfCap](MVVM_22_WpfCap/README_dir.md)

### Component isolation
  - [MVVM_24c_CTUserControl](MVVM_24c_CTUserControl/README_dir.md)

### Conditional formatting
  - [MVVM_18_MultiConverters](MVVM_18_MultiConverters/README_dir.md)

### Converter orchestration
  - [MVVM_18_MultiConverters](MVVM_18_MultiConverters/README_dir.md)

### Cross-field rules
  - [MVVM_31_Validation2](MVVM_31_Validation2/README_dir.md)

### Custom sorting
  - [MVVM_28_DataGrid](MVVM_28_DataGrid/README_dir.md)

### Data formatting
  - [MVVM_06_Converters](MVVM_06_Converters/README_dir.md)

### Data parsing
  - [MVVM_17_1_CSV_Laden](MVVM_17_1_CSV_Laden/README_dir.md)

### DataBinding basics
  - [MVVM_00_Template](MVVM_00_Template/README_dir.md)

### DataGrid basics
  - [MVVM_27_DataGrid](MVVM_27_DataGrid/README_dir.md)

### Debouncing
  - [MVVM_31a_CTValidation3](MVVM_31a_CTValidation3/README_dir.md)

### Deferred commit
  - [MVVM_26_BindingGroupExp](MVVM_26_BindingGroupExp/README_dir.md)

### DelegateCommand pattern
  - [MVVM_04_DelegateCommand](MVVM_04_DelegateCommand/README_dir.md)

### Dependency Injection
  - [MVVM_00_IoCTemplate](MVVM_00_IoCTemplate/README_dir.md)

### DependencyProperty basics
  - [MVVM_16_UserControl1](MVVM_16_UserControl1/README_dir.md)

### DependencyProperty metadata
  - [MVVM_16_UserControl2](MVVM_16_UserControl2/README_dir.md)

### Design-time data
  - [MVVM_16_UserControl1](MVVM_16_UserControl1/README_dir.md)

### DI
  - [MVVM_22_CTWpfCap](MVVM_22_CTWpfCap/README_dir.md)

### DI patterns
  - [MVVM_20a_CTSysdialogs](MVVM_20a_CTSysdialogs/README_dir.md)

### DI refinement
  - [MVVM_35_CommunityToolkit](MVVM_35_CommunityToolkit/README_dir.md)

### Dialog service
  - [MVVM_09_DialogBoxes](MVVM_09_DialogBoxes/README_dir.md)

### Dialog wrappers
  - [MVVM_20a_CTSysdialogs](MVVM_20a_CTSysdialogs/README_dir.md)

### Document serialization
  - [MVVM_25_RichTextEdit](MVVM_25_RichTextEdit/README_dir.md)

### Error propagation
  - [MVVM_31a_CTValidation1](MVVM_31a_CTValidation1/README_dir.md)

### Error templates
  - [MVVM_26_BindingGroupExp](MVVM_26_BindingGroupExp/README_dir.md)

### Event args inspection
  - [MVVM_34_BindingEventArgs](MVVM_34_BindingEventArgs/README_dir.md)

### EventToCommand
  - [MVVM_33_Events_to_Commands](MVVM_33_Events_to_Commands/README_dir.md)

### Event-to-command
  - [MVVM_24b_UserControl](MVVM_24b_UserControl/README_dir.md)

### Extended behaviors
  - [MVVM_28_1_CTDataGridExt](MVVM_28_1_CTDataGridExt/README_dir.md)

### Factory pattern
  - [MVVM_38_CTDependencyInjection](MVVM_38_CTDependencyInjection/README_dir.md)

### File IO
  - [MVVM_17_1_CSV_Laden](MVVM_17_1_CSV_Laden/README_dir.md)

### Filtering
  - [MVVM_19_FilterLists](MVVM_19_FilterLists/README_dir.md)

### FlowDocument
  - [MVVM_25_RichTextEdit](MVVM_25_RichTextEdit/README_dir.md)

### Game state model
  - [MVVM_41_Sudoku](MVVM_41_Sudoku/README_dir.md)

### Grouping
  - [MVVM_19_FilterLists](MVVM_19_FilterLists/README_dir.md)
  - [MVVM_28_DataGrid](MVVM_28_DataGrid/README_dir.md)

### Hierarchical ViewModels
  - [MVVM_37_TreeView](MVVM_37_TreeView/README_dir.md)

### ICollectionView
  - [MVVM_19_FilterLists](MVVM_19_FilterLists/README_dir.md)

### ICommand
  - [MVVM_04_DelegateCommand](MVVM_04_DelegateCommand/README_dir.md)

### IDataErrorInfo
  - [MVVM_31_Validation1](MVVM_31_Validation1/README_dir.md)

### IMultiValueConverter
  - [MVVM_06_Converters_3](MVVM_06_Converters_3/README_dir.md)

### Inline editing
  - [MVVM_27_DataGrid](MVVM_27_DataGrid/README_dir.md)

### INotifyDataErrorInfo
  - [MVVM_31_Validation1](MVVM_31_Validation1/README_dir.md)

### INotifyPropertyChanged
  - [MVVM_00_Template](MVVM_00_Template/README_dir.md)

### INotifyPropertyChanged patterns
  - [MVVM_03_NotifyChange](MVVM_03_NotifyChange/README_dir.md)

### Input gestures
  - [MVVM_21_Buttons](MVVM_21_Buttons/README_dir.md)

### Isolation
  - [MVVM_99_SomeIssue](MVVM_99_SomeIssue/README_dir.md)

### Issue reproduction
  - [MVVM_99_SomeIssue](MVVM_99_SomeIssue/README_dir.md)

### IValueConverter
  - [MVVM_06_Converters](MVVM_06_Converters/README_dir.md)

### Lazy loading
  - [MVVM_37_TreeView](MVVM_37_TreeView/README_dir.md)

### Loose coupling
  - [MVVM_09a_CTDialogBoxes](MVVM_09a_CTDialogBoxes/README_dir.md)
  - [MVVM_33_Events_to_Commands](MVVM_33_Events_to_Commands/README_dir.md)

### Maintainability
  - [MVVM_06_Converters_2](MVVM_06_Converters_2/README_dir.md)
  - [MVVM_18_MultiConverters](MVVM_18_MultiConverters/README_dir.md)

### Messaging
  - [MVVM_31a_CTValidation2](MVVM_31a_CTValidation2/README_dir.md)

### Messenger
  - [MVVM_09a_CTDialogBoxes](MVVM_09a_CTDialogBoxes/README_dir.md)
  - [MVVM_22_CTWpfCap](MVVM_22_CTWpfCap/README_dir.md)
  - [MVVM_24c_CTUserControl](MVVM_24c_CTUserControl/README_dir.md)

### Messenger channels
  - [MVVM_36_ComToolKtSavesWork](MVVM_36_ComToolKtSavesWork/README_dir.md)

### Messenger coordination
  - [MVVM_28_1_CTDataGridExt](MVVM_28_1_CTDataGridExt/README_dir.md)

### Messenger integration
  - [MVVM_20a_CTSysdialogs](MVVM_20a_CTSysdialogs/README_dir.md)

### Messenger payloads
  - [MVVM_33a_CTEvents_To_Commands](MVVM_33a_CTEvents_To_Commands/README_dir.md)

### Module navigation
  - [MVVM_AllExamples](MVVM_AllExamples/README_dir.md)

### MultiBinding
  - [MVVM_06_Converters_3](MVVM_06_Converters_3/README_dir.md)

### Multi-field validation
  - [MVVM_26_CTBindingGroupExp](MVVM_26_CTBindingGroupExp/README_dir.md)

### nameof usage
  - [MVVM_03_NotifyChange](MVVM_03_NotifyChange/README_dir.md)

### Nested composition
  - [MVVM_24b_UserControl](MVVM_24b_UserControl/README_dir.md)

### ObservableCollection
  - [MVVM_17_1_CSV_Laden](MVVM_17_1_CSV_Laden/README_dir.md)

### ObservableObject
  - [MVVM_00a_CTTemplate](MVVM_00a_CTTemplate/README_dir.md)

### ObservableValidator
  - [MVVM_35_CommunityToolkit](MVVM_35_CommunityToolkit/README_dir.md)

### Partial methods for OnChanged
  - [MVVM_03a_CTNotifyChange](MVVM_03a_CTNotifyChange/README_dir.md)

### Performance considerations
  - [MVVM_06_Converters_4](MVVM_06_Converters_4/README_dir.md)

### Rapid iteration
  - [MVVM_99_SomeIssue](MVVM_99_SomeIssue/README_dir.md)

### RelayCommand
  - [MVVM_00a_CTTemplate](MVVM_00a_CTTemplate/README_dir.md)
  - [MVVM_04a_CTRelayCommand](MVVM_04a_CTRelayCommand/README_dir.md)

### RelayCommand parameters
  - [MVVM_05a_CTCommandParCalc](MVVM_05a_CTCommandParCalc/README_dir.md)

### Reusability
  - [MVVM_16_UserControl2](MVVM_16_UserControl2/README_dir.md)

### RichTextBox
  - [MVVM_25_RichTextEdit](MVVM_25_RichTextEdit/README_dir.md)

### Rule composition
  - [MVVM_31a_CTValidation3](MVVM_31a_CTValidation3/README_dir.md)

### Sample discovery
  - [MVVM_AllExamples](MVVM_AllExamples/README_dir.md)

### Selection
  - [MVVM_27_DataGrid](MVVM_27_DataGrid/README_dir.md)

### Selection sync
  - [MVVM_37_TreeView](MVVM_37_TreeView/README_dir.md)

### Separation of concerns
  - [MVVM_28_1_DataGridExt](MVVM_28_1_DataGridExt/README_dir.md)

### Service abstraction
  - [MVVM_09_DialogBoxes](MVVM_09_DialogBoxes/README_dir.md)

### Service lifetimes
  - [MVVM_38_CTDependencyInjection](MVVM_38_CTDependencyInjection/README_dir.md)

### Service Locator vs DI
  - [MVVM_00_IoCTemplate](MVVM_00_IoCTemplate/README_dir.md)

### Shell aggregation
  - [MVVM_AllExamples](MVVM_AllExamples/README_dir.md)

### Simplified binding
  - [MVVM_24a_CTUserControl](MVVM_24a_CTUserControl/README_dir.md)

### Solution layout
  - [MVVM_00_Template](MVVM_00_Template/README_dir.md)

### Source Generators
  - [MVVM_00a_CTTemplate](MVVM_00a_CTTemplate/README_dir.md)

### Source-generated commands
  - [MVVM_05a_CTCommandParCalc](MVVM_05a_CTCommandParCalc/README_dir.md)

### State management
  - [MVVM_39_MultiModelTest](MVVM_39_MultiModelTest/README_dir.md)

### State staging
  - [MVVM_40_Wizzard](MVVM_40_Wizzard/README_dir.md)

### Stateless design
  - [MVVM_06_Converters_4](MVVM_06_Converters_4/README_dir.md)

### Styling
  - [MVVM_21_Buttons](MVVM_21_Buttons/README_dir.md)
  - [MVVM_28_DataGrid](MVVM_28_DataGrid/README_dir.md)

### System dialogs abstraction
  - [MVVM_20_Sysdialogs](MVVM_20_Sysdialogs/README_dir.md)

### Test doubles
  - [MVVM_09_DialogBoxes](MVVM_09_DialogBoxes/README_dir.md)

### Testability
  - [MVVM_00_IoCTemplate](MVVM_00_IoCTemplate/README_dir.md)
  - [MVVM_20_Sysdialogs](MVVM_20_Sysdialogs/README_dir.md)

### Testing converters
  - [MVVM_06_Converters_4](MVVM_06_Converters_4/README_dir.md)

### Theming
  - [MVVM_16_UserControl2](MVVM_16_UserControl2/README_dir.md)

### Toolkit integration
  - [MVVM_22_CTWpfCap](MVVM_22_CTWpfCap/README_dir.md)

### Toolkit messenger
  - [MVVM_34a_CTBindingEventArgs](MVVM_34a_CTBindingEventArgs/README_dir.md)

### Toolkit validation hooks
  - [MVVM_31a_CTValidation1](MVVM_31a_CTValidation1/README_dir.md)

### Toolkit ViewModels
  - [MVVM_24a_CTUserControl](MVVM_24a_CTUserControl/README_dir.md)

### UI to VM data flow
  - [MVVM_05_CommandParCalculator](MVVM_05_CommandParCalculator/README_dir.md)

### UserControl API design
  - [MVVM_24_UserControl](MVVM_24_UserControl/README_dir.md)

### UserControl composition
  - [MVVM_16_UserControl1](MVVM_16_UserControl1/README_dir.md)

### Validation basics
  - [MVVM_05_CommandParCalculator](MVVM_05_CommandParCalculator/README_dir.md)

### Validation gating
  - [MVVM_40_Wizzard](MVVM_40_Wizzard/README_dir.md)

### Validation messages
  - [MVVM_31_Validation1](MVVM_31_Validation1/README_dir.md)

### Validation overlays
  - [MVVM_41_Sudoku](MVVM_41_Sudoku/README_dir.md)

### Validation UX
  - [MVVM_26_BindingGroupExp](MVVM_26_BindingGroupExp/README_dir.md)

### ViewModel injection
  - [MVVM_38_CTDependencyInjection](MVVM_38_CTDependencyInjection/README_dir.md)

### WeakReferenceMessenger
  - [MVVM_35_CommunityToolkit](MVVM_35_CommunityToolkit/README_dir.md)

### Wizard navigation
  - [MVVM_40_Wizzard](MVVM_40_Wizzard/README_dir.md)

### WPF core APIs
  - [MVVM_22_WpfCap](MVVM_22_WpfCap/README_dir.md)

### Wrapper services
  - [MVVM_20_Sysdialogs](MVVM_20_Sysdialogs/README_dir.md)

### XAML binding
  - [MVVM_06_Converters](MVVM_06_Converters/README_dir.md)

## Global Testing & Coverage
Execute all tests (multi-target frameworks will run each target):
`
dotnet test --collect:"XPlat Code Coverage"
`
Custom coverage with coverlet (example):
`
dotnet test MVVM_27_DataGridTests -c Debug /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
`
Merge and report:
`
reportgenerator -reports:"**/coverage.info" -targetdir:CoverageReport -reporttypes:HtmlSummary;MarkdownSummaryGithub
`

## Regenerating READMEs
Run the script:
`
./Generate-Project-Readmes.ps1 -Force
`
This will overwrite all per-project README.md files and the central README.md.

## Contributing
- Add new example metadata to the $projects array in the script.
- Ensure concepts list is accurate (improves the reverse index).
- Add tests in parallel *Tests projects for higher confidence and coverage.

## License / Usage
Educational use; adapt freely. Keep attribution to the original authors where appropriate.
