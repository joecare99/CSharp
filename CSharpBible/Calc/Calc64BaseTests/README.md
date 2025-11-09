# Calc64BaseTests

Comprehensive test suite for the `Calc64Base` logic library. Covers all operation types, memory interactions and mode transitions.

## Key Test Files
- `Calc64Tests.cs`  Integration / main flow.
- `Calc64ModelTests.cs`  Model states & transitions.
- `BinaryOperationTests.cs`, `UnaryOperationTests.cs`  Arithmetic paths.
- `FromMemOperationTests.cs`, `ToMemOperationTests.cs`  Memory read/write scenarios.
- `CalcOperationTests.cs`  Base class / contract checks.
- `StandardOperationsTests.cs`  Registration & correctness of standard operations.

## Goals
- High coverage for critical computation branches.
- Early detection of regression bugs when extending `StandardOperations`.

## Extension
For a new operation:
1. Implement new class.
2. Add unit tests mirroring existing operation test patterns.
3. Extend integration test in `Calc64Tests`.
