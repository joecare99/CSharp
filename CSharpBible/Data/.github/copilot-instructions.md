# Repository instructions for GitHub Copilot

Apply these defaults when working in this repository unless the user explicitly asks otherwise:

## General Guidelines
- Document code thoroughly in English.
- Validate changes with relevant builds and tests before finishing.
- If requirements are unclear, ask clarifying questions before starting implementation.

## Testing
- Use `MSTest` in the latest practical version for new or updated tests.
- Use `NSubstitute` for mocks, stubs, and substitutes in tests.
- Prefer `DataRow` for parameterized single-test scenarios.

## Internationalization
- Keep I18N in mind when writing code, ensuring it can be easily adapted for different languages and regions.

## Architecture
- Use MVVM architecture for UI components to separate concerns and improve testability, using CommunityToolkit.Mvvm for MVVM implementation.
- Use Dependency Injection to manage dependencies and improve testability, using Microsoft.Extensions.DependencyInjection.

## Naming Conventions
- Use PascalCase for class names, method names, and properties.
- Use _camelCase for local/private variables and parameters.
- Use 1-3 letter prifixes for type of variable, e.g. 
	- `s` for string, 
	- `i` for all int (8-128bit), 
	- `ui` for Unsigned Int (8-128 bit), 
	- `x` for bool,  
	- `f` for float/double,  
	- `lst` for list, 
	- `btn` for button, etc.

## Nullability
- Use strict nullable reference types to indicate when a variable can be null, and handle nullability appropriately in code.
