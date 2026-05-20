# T0160A - Compound Assignment Operators Example A Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first concrete compound-assignment-operators comparison example that contrasts an explicit user-defined `+=` operator with the older binary-operator-plus-reassignment pattern.

## Example Intent

Demonstrate that C# 14 allows a mutable custom type to express in-place update intent directly through a user-defined `+=` operator, instead of relying on replacement-style composition through `x = x + y`.

## Modern Example Shape

- small mutable custom type
- explicit instance `+=` operator with one right-side parameter
- observable state update performed in place

## Alternative Example Shape

- binary `+` operator returning a new value or replacement instance
- explicit reassignment pattern at the call site
- same broad domain intent with more indirect mutation semantics

## Comparison Type

- Approximate comparison

## Teaching Focus

- direct expression of in-place mutation intent
- difference between mutating an existing instance and replacing it with a computed result
- clearer understanding of when explicit compound operators are meaningful

## Expected Documentation Notes

- explain that the older form does not mirror the same operator shape directly
- keep the example small and deterministic
- avoid making unsupported performance claims while still noting allocation or replacement differences conceptually

## Expected Output Guidance

- behavior-aware comparison
- observable result should be simple and deterministic
- shared result description should focus on the resulting state plus the source-structure difference

## Candidate Scenario Direction

Use a compact mutable type such as a counter, tally, accumulator, or quantity bucket where `+=` naturally signals state growth.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
