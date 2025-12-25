# Contributing Guidelines

## Coding Standards & Patterns

### Architecture
- **MVVM (Model-View-ViewModel):** We strictly follow the MVVM pattern for UI applications.
- **Dependency Injection (DI):** Use DI for loose coupling and testability.
- **File Structure:** Adhere to the "One Class per File" rule.
- **Strict Separation of Concerns:** Each class should have a single responsibility.
- **Strict Separation of Code and generated Stuff:** The code you write should be separate from any generated code.
- **I18n/L10n:** All user-facing strings must be localized.
- Prefer short methods (max about. 20 lines). If a method exceeds this, consider refactoring.
- Try to achieve high code-coverage with unit tests.
- Detailed XML documentation is required for all public members. 
  - Use `<summary>`, `<param>`, and `<returns>` tags where appropriate.
  - Also document Methods, longer than 5 lines, with remarks about their behavior.

	
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
