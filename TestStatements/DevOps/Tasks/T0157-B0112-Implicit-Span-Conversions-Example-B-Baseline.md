# T0157A - Implicit Span Conversions Example B Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second concrete implicit-span-conversions comparison example that contrasts passing a `string` directly to a `ReadOnlySpan<char>` API with an older more explicit alternative.

## Example Intent

Demonstrate that C# 14 makes text-oriented span APIs easier to call by allowing `string` inputs to flow into `ReadOnlySpan<char>` parameters more naturally.

## Modern Example Shape

- string input
- API consuming `ReadOnlySpan<char>`
- direct call site using the implicit conversion

## Alternative Example Shape

- same text-processing intent expressed through more explicit conversion or helper calls
- same observable behavior with a less concise call site
- source pattern that reflects older span-consumption friction

## Comparison Type

- Behavior-aware comparison

## Teaching Focus

- simpler text-processing call sites
- clearer demonstration of span-based API ergonomics
- preserved observable behavior despite reduced syntax noise

## Expected Documentation Notes

- keep the example deterministic and small
- emphasize source-level simplification at the API boundary
- avoid overstating the comparison as a runtime-performance claim

## Expected Output Guidance

- behavior-aware comparison
- small deterministic output or outcome statement
- one shared behavior description is enough

## Candidate Scenario Direction

Use a small parser, prefix check, trim-like scanner, or character-inspection API where `ReadOnlySpan<char>` is realistic and easy to understand.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
