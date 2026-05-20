# T0134A - Extension Members Example C Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the third concrete extension-members comparison example that demonstrates static extension members on the extended type surface and contrasts them with helper-based alternatives.

## Example Intent

Demonstrate that C# 14 can extend the apparent static surface of a type, which older-language alternatives typically approximate through separate helper or factory methods.

## Modern Example Shape

- one extension block that extends the type surface rather than an instance
- one static extension member with clear discoverability value
- optional second member if it sharpens the contrast

## Alternative Example Shape

- helper utility or factory-style static method in a separate class
- explicit call through the helper type instead of the extended type surface

## Comparison Type

- Approximate comparison

## Teaching Focus

- distinction between extending instances and extending the type surface
- discoverability and API-shape benefits
- why helper-based alternatives feel less integrated

## Expected Documentation Notes

- make the type-surface difference explicit
- highlight the conceptual integration benefit of the C# 14 version
- avoid overclaiming equivalence when the helper pattern is only an approximation

## Expected Output Guidance

- usually readability-focused or behavior-aware comparison
- shared observable result can be minimal
- the main comparison artifact is API shape, not console output

## Candidate Scenario Direction

Use a small domain where a static value, factory-like member, or type-level combinator is easy to understand from one glance.

## Done Criteria

- Example scope is defined.
- Static type-surface comparison is explicit.
- The example is ready for later concrete code planning.
