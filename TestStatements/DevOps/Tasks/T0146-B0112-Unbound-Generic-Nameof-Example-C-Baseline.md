# T0146A - Unbound Generic Nameof Example C Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the third concrete unbound-generic-`nameof` comparison example that highlights motivation, arbitrary type-argument noise, and unsupported boundary forms.

## Example Intent

Demonstrate that unbound generic `nameof` is especially valuable when older code would otherwise need an arbitrary, awkward, or constraint-driven type argument selection that adds no naming value.

## Modern Example Shape

- unbound generic `nameof` on a type where generic arguments are intentionally irrelevant to the resulting name
- small API, diagnostics, or validation-oriented context
- optional note about member access on the unbound type

## Alternative Example Shape

- bound generic `nameof` with a chosen sample type argument
- fallback string-based naming only if the scenario needs to illustrate awkwardness clearly
- same intended resulting name but with noisier or less robust source

## Comparison Type

- Exact comparison with caveat notes

## Teaching Focus

- motivation beyond brevity
- avoidance of misleading type-argument choices
- explicit awareness of unsupported partially unbound or nested-unbound forms

## Expected Documentation Notes

- explain why the old workaround can be arbitrary or brittle
- keep unsupported forms as boundary notes rather than mainline examples
- avoid overstating this as a runtime feature because the value is compile-time clarity

## Expected Output Guidance

- readability-focused runtime-equivalent comparison
- output is secondary to source explanation
- one shared expected-name statement is enough if any output is shown

## Candidate Scenario Direction

Use a constrained or documentation-oriented generic type name where selecting `int`, `object`, or another sample argument would distract from the actual naming intent.

## Done Criteria

- Example scope is defined.
- Boundary notes are explicit.
- The example is ready for later concrete code planning.
