# T0136A - Field-Backed Properties Example A Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first concrete field-backed-properties comparison example that contrasts a null-rejecting property using `field` with an older explicit-backing-field alternative.

## Example Intent

Demonstrate that a field-backed property can keep the familiar property model while reducing boilerplate in a validated setter.

## Modern Example Shape

- auto-style property with `get;`
- `set` accessor using `field`
- null-check or guard logic directly in the accessor body

## Alternative Example Shape

- explicit private backing field
- full `get` and `set` accessors
- identical validation logic in the setter

## Comparison Type

- Exact comparison

## Teaching Focus

- same runtime behavior
- less ceremony in the modern syntax
- easier migration from auto property to guarded property logic

## Expected Documentation Notes

- emphasize that both versions should behave the same
- use one shared expected behavior description
- explain that the improvement is primarily in source brevity and clarity

## Expected Output Guidance

- runtime-equivalent comparison
- deterministic behavior
- console output is optional and only needed if it helps demonstrate the guard result

## Candidate Scenario Direction

Use a simple reference-type property where assigning `null` should be rejected with a clear exception or fallback behavior.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
