# T0149A - Lambda Modifiers Example B Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second concrete lambda-modifiers comparison example that contrasts `ref`, `in`, or `ref readonly` lambda parameters in C# 14 with the older explicitly typed lambda form.

## Example Intent

Demonstrate that by-reference parameter intent can stay visible in a lambda while C# 14 removes the need to restate obvious parameter types.

## Modern Example Shape

- small delegate with a by-reference modifier such as `ref`, `in`, or `ref readonly`
- lambda parameter list with modifier and omitted types where allowed
- compact example focused on the signature difference

## Alternative Example Shape

- same delegate target
- explicitly typed lambda parameter list
- same by-reference semantics and observable result

## Comparison Type

- Exact comparison

## Teaching Focus

- clearer emphasis on the modifier itself
- less ceremony in the lambda signature
- same semantics despite shorter syntax

## Expected Documentation Notes

- emphasize that both versions express the same by-reference contract
- keep the example small and deterministic
- explain that the main gain is source readability in advanced delegate scenarios

## Expected Output Guidance

- runtime-equivalent comparison
- deterministic behavior
- shared result description is enough

## Candidate Scenario Direction

Use a small delegate that validates, compares, or updates a value where the by-reference modifier is visible but the surrounding logic stays minimal.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
