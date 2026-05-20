# T0152A - Partial Members Example A Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first concrete partial-members comparison example that contrasts a partial constructor split across defining and implementing declarations with an older non-partial construction pattern.

## Example Intent

Demonstrate how C# 14 allows constructor declaration and constructor implementation to be separated across partial type parts, which supports generated or layered code scenarios that were previously forced into more manual coordination patterns.

## Modern Example Shape

- partial type with one defining constructor declaration
- separate implementing constructor declaration with the executable body
- optional constructor initializer placed only on the implementing declaration

## Alternative Example Shape

- ordinary non-partial constructor in one place
- or manual split between a normal constructor and extra helper initialization methods
- same final object intent but without first-class declaration-versus-implementation separation

## Comparison Type

- Approximate comparison

## Teaching Focus

- separation of constructor declaration from constructor implementation
- value for generated or layered source layouts
- limitation that the defining declaration cannot carry the constructor initializer

## Expected Documentation Notes

- explain that the older form cannot mirror the same split directly
- keep the example small and source-organization oriented
- document that the initializer belongs only on the implementing declaration

## Expected Output Guidance

- readability-focused or behavior-aware comparison
- runtime output is optional and secondary
- focus on the initialization story and source structure rather than transcript output

## Candidate Scenario Direction

Use a small partial type where one file represents a declaration-oriented surface and the other file represents generated or infrastructure implementation.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
