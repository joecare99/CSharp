# T0150A - Lambda Modifiers Example C Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the third concrete lambda-modifiers comparison example that documents supported modifier simplification alongside boundary notes such as the continued explicit-type requirement for `params`.

## Example Intent

Demonstrate that C# 14 meaningfully simplifies many modifier-based lambda signatures, while still requiring explicit types in some cases and therefore benefiting from honest boundary documentation.

## Modern Example Shape

- supported modifier-focused lambda example with omitted parameter types
- boundary note showing a case that still needs explicit typing, especially `params`
- concise source-oriented comparison

## Alternative Example Shape

- explicitly typed lambda parameter list for the supported comparison
- explicit type retained for the boundary case where the language still requires it
- same intended behavior for the supported scenario

## Comparison Type

- Exact comparison with caveat notes

## Teaching Focus

- supported simplification path
- explicit visibility of remaining restrictions
- realistic adoption guidance instead of feature-only promotion

## Expected Documentation Notes

- document the supported case as the main comparison
- present the `params` restriction as a boundary note, not as a contradiction
- mention that mixed-experience teams may still prefer explicit types in some advanced signatures

## Expected Output Guidance

- runtime-equivalent comparison for the supported case
- output is secondary to source explanation
- caveat section should describe the boundary without overstating it

## Candidate Scenario Direction

Use a compact delegate example for the supported path and a short note or micro-example for the `params` limitation.

## Done Criteria

- Example scope is defined.
- Boundary notes are explicit.
- The example is ready for later concrete code planning.
