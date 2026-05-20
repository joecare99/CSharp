# Repository instructions for GitHub Copilot

Apply these defaults when working in this repository unless the user explicitly asks otherwise:

## General Guidelines
- Document code thoroughly in English.
- Validate changes with relevant builds and tests before finishing.
- If requirements are unclear, ask clarifying questions before starting implementation or planning refinement.
- When reviewing build warnings in this repository, ignore unsupported older framework warnings if the project also targets newer supported frameworks. Additionally, ignore `IDE0130` warnings in test projects.

## Process and Planning
- Document changes in a DevOps manner using Markdown, extrapolating bugs, tasks, backlogs, and features.
- Use `DevOps` as the planning directory in this workspace, and treat `.Info.md` as the general planning description file. Team terminology around Azure DevOps backlog items may differ from generic 'story' naming.
- Planning should be done step by step in the `DevOps` directory with a Scrum-oriented structure.

## Memory
- Keep project-specific decisions in the repository `DevOps` folder; do not store them in memory for this repository.
- Use memory for general solution-wide or project-overlapping working preferences rather than repository-local implementation decisions.

## Code Organization
- Prefer one class/interface/struct per file.

## Testing
- Use `MSTest` in the latest practical version for new or updated tests.
- Use `NSubstitute` for mocks, stubs, and substitutes in tests.
- Prefer `DataRow` for parameterized single-test scenarios.

## Internationalization
- Keep I18N in mind when writing code, ensuring it can be easily adapted for different languages and regions.

## Architecture
- Use MVVM architecture for UI components to separate concerns and improve testability, using CommunityToolkit.Mvvm for MVVM implementation.
- Prefer `NotifyPropertyChangedFor` over manual `OnPropertyChanged(nameof(...))` in CommunityToolkit.Mvvm observable properties where applicable.
- Use Dependency Injection to manage dependencies and improve testability, using Microsoft.Extensions.DependencyInjection.
- In UI applications, keep UI-facing strings and summary formatting in the ViewModel/UI layer rather than in extracted application logic services.
- In UI applications, separate UI concerns from models, services, and reusable application logic where practical.
- For larger UI applications, prefer at least a split between a UI-focused project and a non-UI project for models, services, and reusable orchestration; additional widget or controls projects are acceptable when they improve clarity.
- Extract complex UI elements into separate widgets/components to keep main windows clean and simple.
- When working in a project suite, prefer composing the suite from standalone apps or reusable modules instead of developing suite and standalone variants as separate duplicated products.
- When a workflow depends on external data, make acquisition of the data basis an explicit first step so the UI and services can derive structure such as columns, groups, formats, or time base from it.

## MVVM Requirements
- In WPF UI projects, ViewModels must use CommunityToolkit.Mvvm.
- Prefer inheriting from ObservableObject instead of custom base classes unless a documented exception exists.
- Use [ObservableProperty] for observable state where practical.
- Use [RelayCommand] or [AsyncRelayCommand] for commands where practical.
- Use [NotifyPropertyChangedFor] instead of manual dependent property notifications where applicable.
- Do not introduce new WPF ViewModels based on manual ICommand and manual property boilerplate unless required by framework constraints.

## Naming Conventions
- Distinguish between UI control naming and variable/field naming.
- Use PascalCase for class names, method names, and properties.
- Use `_camelCase` for private fields.
- Use `camelCase` for local variables and parameters.
- Use short 1-character prefixes for simple types only when they meaningfully disambiguate, e.g.
	- `s` for `string`
	- `i` for signed integer types
	- `u` for unsigned integer types
	- `x` for `bool`
	- `f` for `float`, `double`, or `decimal`
- Prefer meaningful domain names over type prefixes when the intent is already clear.
- In UI code, use short 3-character prefixes for actual controls in views and code-behind, e.g.
	- `lst` for list controls
	- `btn` for all kinds of buttons
	- `edt` for any keyboard input control
	- `lbl` for any text output control
- Do not use UI control prefixes for ViewModel properties or other non-UI members.

## Nullability
- Use strict nullable reference types to indicate when a variable can be null, and handle nullability appropriately in code.
- When fixing nullability issues in this repository, do not use null-forgiving assignments like 'null!' except for test in test-projects; if a variable can be assigned null, make the variable nullable instead.
