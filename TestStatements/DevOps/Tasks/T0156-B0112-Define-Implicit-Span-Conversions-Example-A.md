# T0156 - Define Implicit Span Conversions Example A

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first implicit-span-conversions comparison example for passing an array directly to a span-based API.

## Scope

- modern form using an implicit conversion from an array to `Span<T>` or `ReadOnlySpan<T>`
- older alternative using explicit conversion, overload selection, or helper code
- behavior-aware comparison with emphasis on API ergonomics

## Example Direction

- use a small span-consuming API
- keep the call site compact and easy to read
- emphasize reduced ceremony at the call site

## Done Criteria

- The comparison focus is fixed.
- The example is ready for a detailed baseline.
