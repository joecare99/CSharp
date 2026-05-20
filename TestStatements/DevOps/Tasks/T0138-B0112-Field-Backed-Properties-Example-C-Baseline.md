# T0138A - Field-Backed Properties Example C Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the third concrete field-backed-properties comparison example that highlights the naming and migration caveat when a symbol named `field` already exists.

## Example Intent

Demonstrate that the `field` contextual keyword reduces boilerplate but introduces a potential migration consideration when existing code already uses `field` as an identifier.

## Modern Example Shape

- property accessor using `field`
- nearby identifier or member naming scenario that can cause ambiguity or confusion
- explicit note about disambiguation or renaming

## Alternative Example Shape

- explicit private backing field implementation without the contextual keyword
- naming situation remains unambiguous in the older form

## Comparison Type

- Exact comparison with migration caveat

## Teaching Focus

- identical intent with different readability risks
- migration awareness
- responsible adoption of the new syntax in real code bases

## Expected Documentation Notes

- make the caveat visible without overstating it
- explain that the modern form is still useful, but naming clarity matters
- present the older form as a compatibility-safe fallback when naming conflicts make the newer form less clear

## Expected Output Guidance

- readability-focused or runtime-equivalent comparison
- output is secondary to source-code clarity
- one shared behavior statement is enough if runtime behavior is unchanged

## Candidate Scenario Direction

Use a small property example where an existing symbol or member named `field` creates a realistic migration discussion.

## Done Criteria

- Example scope is defined.
- Migration caveat is explicit.
- The example is ready for later concrete code planning.
