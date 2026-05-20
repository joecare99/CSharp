# T0132A - Extension Members Example A Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first concrete extension-members comparison example that shows several related instance-style extensions grouped in one extension block.

## Example Intent

Demonstrate how C# 14 allows related instance-style extensions to live together in one coherent extension block instead of being scattered as separate static extension methods.

## Modern Example Shape

- one static container class
- one extension block for a single receiver type
- multiple instance-style extension members in that block

## Alternative Example Shape

- one static helper class
- multiple classic extension methods using the `this` parameter syntax
- same logical operations but with less structural grouping around the receiver concept

## Comparison Type

- Approximate comparison

## Teaching Focus

- grouped discoverability of related members
- lower conceptual fragmentation
- continuity with older extension-method concepts while showing the C# 14 grouping benefit

## Expected Documentation Notes

- the main difference is structural clarity, not dramatic runtime change
- a small shared usage example should be sufficient
- the learning value is mostly readability and organization

## Expected Output Guidance

- readability-focused comparison
- optional small observable result if it helps confirm that both versions support the same practical use
- avoid large output transcripts

## Candidate Scenario Direction

A simple receiver type with two or three closely related helper operations should be used so the grouping benefit is obvious.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
