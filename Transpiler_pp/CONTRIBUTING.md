# Contributing Guidelines

## Coding Standards & Patterns

### Architecture
- **MVVM (Model-View-ViewModel):** We strictly follow the MVVM pattern for UI applications.
- **Dependency Injection (DI):** Use DI for loose coupling and testability.
- **File Structure:** Adhere to the "One Class per File" rule.
- **Strict Separation of Concerns:** Each class should have a single responsibility.
- **Strict Separation of Code and generated Stuff:** The code you write should be separate from any generated code.
- **I18n/L10n:** All user-facing strings must be localized.

### Testing & TDD
- **Test-Driven Development (TDD):** Tests must be created *before* the implementation.
- **Frameworks:**
  - **Testing Framework:** MSTestv4
  - **Mocking:** NSubstitute
  - **MVVM Support:** CommunityToolkit.Mvvm

### Build Configuration
- **Warnings as Errors:** `TreatWarningsAsErrors` is enabled. Ensure your code compiles without warnings.

## Workflow
1. Create a failing test (Red).
2. Implement the minimal code to pass the test (Green).
3. Refactor.
