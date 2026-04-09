# Repository instructions for GitHub Copilot

Apply these defaults when working in this repository unless the user explicitly asks otherwise:

## General Guidelines
- Document code thoroughly in English.
- Validate changes with relevant builds and tests before finishing.
- If requirements are unclear, ask clarifying questions before starting implementation.
- Avoid UI text strings in core services. Use Enumerations instead, and keep UI-facing strings in the ViewModel/UI layer.
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
- UI-facing strings and summary formatting should stay in the ViewModel/UI layer, not in extracted application logic services.

## Naming Conventions
- Use PascalCase for class names, method names, and properties.
- Use _camelCase for local/private variables and parameters.
- Use 1 letter prefixes for type of variable, e.g. in model classes, use: 
	- `s` for string, 
	- `i` for all int (8-128bit), 
	- `u` for Unsigned Int (8-128 bit), 
	- `x` for bool,  
	- `f` for float/double,  
- Use 3 letter prefixes for UI elements, use:
	- `lst` for list, 
	- `btn` for all kind of buttons, 
	- `edt` for all kind of text-inputs,
	- `chk` for checkboxes,
	- `lbl` for all kind of text-displaying elements,

## Nullability
- Use strict nullable reference types to indicate when a variable can be null, and handle nullability appropriately in code.
