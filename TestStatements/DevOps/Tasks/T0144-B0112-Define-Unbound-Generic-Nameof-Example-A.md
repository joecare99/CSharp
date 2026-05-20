# T0144 - Define Unbound Generic Nameof Example A

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first unbound-generic-`nameof` comparison example for naming a generic type without an arbitrary type argument.

## Scope

- modern form using `nameof(List<>)` or a similar unbound generic type
- older alternative using a bound generic type such as `nameof(List<int>)`
- exact comparison with equivalent compile-time string result

## Example Direction

- keep the scenario minimal and easy to read
- focus on removal of irrelevant type arguments
- emphasize that the produced name stays the same

## Done Criteria

- The comparison focus is fixed.
- The example is ready for a detailed baseline.
