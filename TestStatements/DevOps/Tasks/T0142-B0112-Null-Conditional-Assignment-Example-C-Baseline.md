# T0142A - Null-Conditional Assignment Example C Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the third concrete null-conditional assignment comparison example that contrasts compound assignment with an older explicit guarded read-modify-write alternative and documents feature boundaries.

## Example Intent

Demonstrate that null-conditional compound assignment can reduce mutation ceremony in guarded updates while still requiring explicit caveat notes for unsupported increment and decrement forms.

## Modern Example Shape

- nullable reference receiver
- compound assignment such as `+=` on a property, event, or indexed element through `?.` or `?[]`
- small deterministic mutation

## Alternative Example Shape

- explicit null check
- guarded read-modify-write logic or guarded compound mutation in the block
- same resulting observable state

## Comparison Type

- Exact comparison with caveat notes

## Teaching Focus

- concise guarded mutation
- exact equivalence for supported compound assignments
- visible limitation awareness for unsupported `++` and `--`

## Expected Documentation Notes

- document the supported compound form as the main comparison
- mention increment and decrement only as a boundary note
- avoid inventing misleading pseudo-equivalents for unsupported syntax

## Expected Output Guidance

- runtime-equivalent comparison for the supported scenario
- deterministic state change
- caveat section should describe unsupported forms without overstating the scope

## Candidate Scenario Direction

Use a small numeric property, counter-like state, event subscription shape, or indexed collection mutation where the guarded compound update is easy to understand.

## Done Criteria

- Example scope is defined.
- Supported and unsupported boundaries are explicit.
- The example is ready for later concrete code planning.
