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
	- `btn` for buttons
	- `edt` for any keyboard input control
	- `lbl` for any text output control
- Do not use UI control prefixes for ViewModel properties or other non-UI members.

## Nullability
- Use strict nullable reference types to indicate when a variable can be null, and handle nullability appropriately in code.
