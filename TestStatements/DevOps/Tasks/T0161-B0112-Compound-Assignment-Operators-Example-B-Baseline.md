# T0161A - Compound Assignment Operators Example B Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second concrete compound-assignment-operators comparison example that contrasts instance `++` or `--` operators with older replacement-style or helper-based update patterns.

## Example Intent

Demonstrate that C# 14 allows increment or decrement semantics to be expressed as direct instance mutation on a custom type, which can better match the intent of counter-like domain objects.

## Modern Example Shape

- small custom mutable type
- instance `++` or `--` operator with `void` return type
- clear observable mutation of one tracked value

## Alternative Example Shape

- static increment operator returning a new value
- or helper method and reassignment pattern
- same conceptual effect achieved through a less direct update model

## Comparison Type

- Approximate comparison

## Teaching Focus

- direct mutation semantics for increment or decrement
- distinction between instance mutation and replacement-style operator results
- practical readability trade-offs for custom counter-like types

## Expected Documentation Notes

- explain that one modern operator body can support familiar increment syntax while mutating the instance
- keep the scenario easy to observe
- note that the comparison is about expressiveness and shape, not just fewer characters

## Expected Output Guidance

- behavior-aware comparison
- deterministic state observation
- describe one shared end-state while noting that the underlying update path differs structurally

## Candidate Scenario Direction

Use a small attendance, progress, inventory, or score-tracking type where increment-like updates are realistic and easy to read.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
