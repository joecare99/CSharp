# T0148A - Lambda Modifiers Example A Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first concrete lambda-modifiers comparison example that contrasts a simple `out`-parameter lambda in C# 14 with the older explicitly typed lambda form.

## Example Intent

Demonstrate that C# 14 allows parameter modifiers such as `out` in a lambda without repeating explicit parameter types, while preserving the same delegate behavior.

## Modern Example Shape

- small delegate such as a parsing-style delegate
- lambda parameter list with an `out` modifier
- omitted parameter types where inference is available

## Alternative Example Shape

- same delegate target
- explicitly typed lambda parameter list
- identical parsing or assignment behavior

## Comparison Type

- Exact comparison

## Teaching Focus

- reduced syntax noise
- unchanged delegate semantics
- clear first example for the feature

## Expected Documentation Notes

- emphasize that both versions bind to the same delegate shape
- describe one shared expected behavior
- note that the gain is readability and brevity, not changed runtime behavior

## Expected Output Guidance

- runtime-equivalent comparison
- deterministic behavior
- small output or behavior statement is sufficient

## Candidate Scenario Direction

Use a simple `TryParse`-style delegate where `out` is realistic and easy to recognize.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
