# T0158A - Implicit Span Conversions Example C Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the third concrete implicit-span-conversions comparison example that documents conversion boundaries, overload-selection behavior, and notable limitations such as method-group conversion exclusion.

## Example Intent

Demonstrate that implicit span conversions improve everyday API usage, while still requiring careful documentation of where the language support does not apply automatically.

## Modern Example Shape

- supported implicit span conversion in a realistic API call
- one visible boundary note such as method-group conversion exclusion or overload nuance
- example kept small enough that the limitation remains understandable

## Alternative Example Shape

- explicit conversion or helper-based fallback when the newer language support does not apply directly
- explanation of how older code or boundary cases stay more manual
- comparison focused on honest adoption guidance rather than forced equivalence

## Comparison Type

- Behavior-aware comparison with caveat notes

## Teaching Focus

- clearer understanding of supported versus unsupported conversion contexts
- practical limits of the language feature
- honest ergonomics guidance for mixed scenarios

## Expected Documentation Notes

- center the example on one main boundary instead of many edge cases
- explain that some contexts still need explicit intervention
- keep limitations visible without overshadowing the main value of the feature

## Expected Output Guidance

- behavior-aware comparison
- output is secondary to API-shape and conversion-boundary explanation
- caveat notes should describe the limitation precisely

## Candidate Scenario Direction

Use a compact example where a direct API call benefits from implicit conversion but a nearby method-group or overload-resolution case needs explicit clarification.

## Done Criteria

- Example scope is defined.
- Boundary notes are explicit.
- The example is ready for later concrete code planning.
