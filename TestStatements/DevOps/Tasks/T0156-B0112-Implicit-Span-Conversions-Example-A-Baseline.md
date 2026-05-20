# T0156A - Implicit Span Conversions Example A Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first concrete implicit-span-conversions comparison example that contrasts passing an array directly to a span-based API with an older more explicit alternative.

## Example Intent

Demonstrate that C# 14 allows a span-oriented API to be called with an array more naturally, reducing call-site ceremony while preserving the same practical intent.

## Modern Example Shape

- single-dimensional array source
- API consuming `Span<T>` or `ReadOnlySpan<T>`
- direct call site that relies on the implicit span conversion

## Alternative Example Shape

- same API intent expressed through more explicit conversion steps, overload selection, or helper code
- slightly more ceremony at the call site
- same conceptual outcome with less natural source expression

## Comparison Type

- Behavior-aware comparison

## Teaching Focus

- improved API ergonomics for span-oriented code
- reduced ceremony at the call site
- same core data flow with a more natural conversion path

## Expected Documentation Notes

- explain that the modern form improves how the API is consumed rather than changing the business logic
- keep the example compact and deterministic
- note that the value is source ergonomics, not a claimed performance result

## Expected Output Guidance

- behavior-aware comparison
- observable result can be small and deterministic
- shared result description should focus on equivalent processing intent

## Candidate Scenario Direction

Use a small API such as summing, formatting, scanning, or slicing values from an integer or character array.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
