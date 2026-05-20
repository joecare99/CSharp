# T0154A - Partial Members Example C Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the third concrete partial-members comparison example that documents signature rules, declaration coordination, and boundary notes for partial constructors or partial events.

## Example Intent

Demonstrate that partial constructors and partial events are powerful for split declarations, but they also require careful coordination of signatures, modifiers, and declaration responsibilities.

## Modern Example Shape

- partial member with one defining declaration and one implementing declaration
- focused rule demonstration such as matching signatures, constructor-initializer placement, or accessor requirements
- small example that keeps the language rule visible without turning into a compiler-error catalog

## Alternative Example Shape

- non-partial member pattern with manual coordination across files or helper members
- same conceptual intent but without compiler-recognized declaration pairing
- fallback explanation rather than forced one-to-one syntax equivalence

## Comparison Type

- Approximate comparison with caveat notes

## Teaching Focus

- signature-matching discipline across declarations
- visibility of declaration-role rules
- honest boundary documentation for adoption decisions

## Expected Documentation Notes

- document one main rule as the centerpiece
- keep other limitations as supporting notes
- recommend keeping parameter names aligned across declarations for clarity

## Expected Output Guidance

- readability-focused comparison
- output is optional and secondary
- caveat notes should explain constraints without overwhelming the main teaching point

## Candidate Scenario Direction

Use a compact constructor or event example where one declaration-rule constraint can be shown clearly, such as initializer placement or the need for matching parameter names and modifiers.

## Done Criteria

- Example scope is defined.
- Boundary notes are explicit.
- The example is ready for later concrete code planning.
