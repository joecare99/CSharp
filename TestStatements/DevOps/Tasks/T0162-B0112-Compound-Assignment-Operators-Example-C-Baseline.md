# T0162A - Compound Assignment Operators Example C Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the third concrete compound-assignment-operators comparison example that documents fallback behavior, operator-shape constraints, and adoption caveats.

## Example Intent

Demonstrate that user-defined compound assignment operators offer a new direct customization point in C# 14, while older code still relies on compiler expansion when no explicit compound operator exists.

## Modern Example Shape

- explicit user-defined compound assignment operator or increment operator
- one focused rule demonstration such as single-parameter shape, instance-member requirement, or fallback to binary expansion when the explicit operator is absent
- example kept educational rather than compiler-error heavy

## Alternative Example Shape

- binary operator with compiler-expanded assignment behavior
- or helper-based reassignment pattern when explicit operator support is unavailable
- same broad intention with more indirect operator resolution semantics

## Comparison Type

- Approximate comparison with caveat notes

## Teaching Focus

- fallback behavior when no explicit compound operator exists
- required shape of explicit compound operators
- honest migration guidance for deciding when the new feature is worth using

## Expected Documentation Notes

- center the example on one main rule instead of a full rule catalog
- explain that the compiler still supports classic expansion as a fallback path
- keep caveats visible without overshadowing the core learning value

## Expected Output Guidance

- behavior-aware comparison
- output is secondary to operator-resolution and declaration-shape explanation
- caveat notes should describe the limitation or fallback precisely

## Candidate Scenario Direction

Use a compact type where one example can show an explicit `+=` overload and a nearby explanation can contrast it with the classic `x = x + y` fallback model.

## Done Criteria

- Example scope is defined.
- Boundary notes are explicit.
- The example is ready for later concrete code planning.
