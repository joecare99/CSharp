# MVVM_BaseLibTests

Test suite for `MVVM_BaseLib`. Ensures that fundamental MVVM building blocks remain stable.

## Covered Areas
- View model base classes (notification, CancellationToken behavior).
- Commands (execution, CanExecute logic).
- Collections (change notification consistency, enumeration safety).
- Helpers (PropertyHelper reflection correctness, ValidationHelper paths, StreamUtilities behavior).
- Timer (`TimerProxy` tick frequency and dispose behavior).
- Converters (Bool2Visibility, DoubleValue).

## Test Strategy
- Focus on deterministic behavior tests (no random timings – use abstracted time / random interfaces from BaseLib when needed).
- Edge cases: null values, exceptions in property setters, multi-thread notifications (as far as reasonable without a UI thread).

## Extension
For new features create appropriate tests first (TDD recommended) – especially for stateful components.
