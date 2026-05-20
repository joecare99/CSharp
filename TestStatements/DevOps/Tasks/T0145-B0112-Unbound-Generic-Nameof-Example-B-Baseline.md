# T0145A - Unbound Generic Nameof Example B Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second concrete unbound-generic-`nameof` comparison example that contrasts member access on an unbound generic type with the older bound-generic workaround.

## Example Intent

Demonstrate that C# 14 allows member naming such as `nameof(Container<>.Value)` without requiring a dummy generic argument, while preserving the same compile-time member name result.

## Modern Example Shape

- custom or framework-based generic type with one clear member
- `nameof(GenericType<>.Member)` usage
- compact example focused on member naming intent

## Alternative Example Shape

- `nameof(GenericType<int>.Member)` or another closed generic version
- same compile-time resulting member name
- same observable outcome with more arbitrary type syntax

## Comparison Type

- Exact comparison

## Teaching Focus

- clearer member naming on generic APIs
- reduced dummy-type noise
- useful documentation and diagnostics scenario

## Expected Documentation Notes

- emphasize equal compile-time result for the member name
- explain that this improves API-oriented source readability
- keep the example deterministic and small

## Expected Output Guidance

- runtime-equivalent comparison with source-level emphasis
- output is optional and can be replaced by an expected-name statement
- one shared result description is enough

## Candidate Scenario Direction

Use a small generic container, result wrapper, or descriptor type with one obvious property or method so the member access remains easy to scan.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
