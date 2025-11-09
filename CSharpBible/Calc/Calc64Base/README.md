# Calc64Base

Library containing extended 64?bit calculator / operation logic. UI-agnostic; supplies core APIs for various front ends (WinForms, WPF, Console).

## Project
- `Calc64Base.csproj` – Multi-target: net462; net472; net48; net481; net6.0; net7.0; net8.0; net9.0

## Core Components
- `Calc64.cs`  High-level facade / orchestrating class.
- `Models/Calc64Model.cs`  State data (registers, current operator, memory values).
- `CalcOperation.cs` + `StandardOperations.cs`  Abstractions and implementations for binary / unary operations.
- `EOpMode.cs`  Enumeration of operation / input modes.
- `Models/Interfaces/ICalculator.cs`  Contract for implementations (testability / replaceability).

## Extensibility
Add a new operation:
1. Create subclass of `CalcOperation` (or suitable interface implementation).
2. Register in `StandardOperations`.
3. Add tests (see `Calc64BaseTests`).

## Dependencies
- `BaseLib` (utilities / notification / IoC helpers)
- Conditional reference `System.ValueTuple` only for net462 (missing BCL type backfill).

## Design Principles
- Separation of operator definition and execution logic.
- Immutable / defensive copies for critical state transitions (where sensible).
- Focus on deterministic, thread-safe calculations.

## Tests
See folder `Calc64BaseTests` for extensive coverage (binary, unary, memory, error cases).
