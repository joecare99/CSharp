# T0145 - Define Unbound Generic Nameof Example B

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second unbound-generic-`nameof` comparison example for member access on an unbound generic type.

## Scope

- modern form using member access such as `nameof(Container<>.Member)`
- older alternative using a bound generic type such as `nameof(Container<int>.Member)`
- exact comparison with equivalent compile-time string result

## Example Direction

- use a small custom generic type with one clear member
- show that member naming does not require a dummy type argument anymore
- keep the example API-oriented and deterministic

## Done Criteria

- The comparison focus is fixed.
- The example is ready for a detailed baseline.
