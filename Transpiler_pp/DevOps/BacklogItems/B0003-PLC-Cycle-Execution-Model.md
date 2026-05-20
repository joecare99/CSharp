# B0003-PLC-Cycle-Execution-Model

## Parent
- Feature: F0001-IEC-to-CSharp-Testability

## Description
Define a deterministic execution model for `PROGRAM` and `FUNCTION_BLOCK` processing that preserves state across cycles and makes behavior directly testable from C# unit tests.

## Value
A clear cycle model is the core requirement for turning PLC logic into deterministic, repeatable tests.

## Scope
- Define execution entry points such as `ExecuteCycle`
- Separate inputs, outputs, and persistent internal state
- Document how one execution cycle consumes inputs and produces outputs

## Out of Scope
- Full runtime scheduling
- Hardware integration details

## Acceptance Criteria
- The supported program model exposes a deterministic cycle-based execution contract
- State persistence rules are documented and testable
- Tests can drive multiple cycles with explicit input and output assertions

## Assumptions
- Symbol and declaration handling are available for the supported subset

## Open Questions
- Whether generated C# should expose a state container, instance fields, or both
- How to represent retained values in a way that remains simple for tests
