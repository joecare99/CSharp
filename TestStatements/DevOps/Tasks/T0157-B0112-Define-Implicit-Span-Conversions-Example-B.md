# T0157 - Define Implicit Span Conversions Example B

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second implicit-span-conversions comparison example for using `string` as `ReadOnlySpan<char>` in a span-oriented API.

## Scope

- modern form using an implicit conversion from `string` to `ReadOnlySpan<char>`
- older alternative using explicit conversion or helper calls
- behavior-aware comparison with emphasis on source-level ergonomics

## Example Direction

- use a small text-processing API
- keep the example deterministic and easy to observe
- show the call-site simplification clearly

## Done Criteria

- The comparison focus is fixed.
- The example is ready for a detailed baseline.
