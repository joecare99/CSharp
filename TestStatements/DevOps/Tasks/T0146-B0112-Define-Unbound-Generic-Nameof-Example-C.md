# T0146 - Define Unbound Generic Nameof Example C

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the third unbound-generic-`nameof` comparison example for constraints, arbitrary type arguments, and unsupported boundary notes.

## Scope

- modern form using unbound generic `nameof` to avoid arbitrary or awkward type argument choices
- older alternative using a bound generic type or fallback string-based naming depending on the scenario
- exact comparison with caveat notes for unsupported forms

## Example Direction

- use a generic type where choosing a sample type argument is noisy or misleading
- explain why the new syntax is cleaner for API, diagnostics, or documentation scenarios
- document unsupported forms such as partially unbound or nested-unbound cases as boundaries

## Done Criteria

- The comparison focus is fixed.
- The boundary notes are explicit.
- The example is ready for a detailed baseline.
