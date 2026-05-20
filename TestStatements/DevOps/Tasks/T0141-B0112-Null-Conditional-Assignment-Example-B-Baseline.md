# T0141A - Null-Conditional Assignment Example B Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second concrete null-conditional assignment comparison example that contrasts deferred right-side evaluation with an older explicit null-check alternative.

## Example Intent

Demonstrate that the right-hand side of a null-conditional assignment is evaluated only when the receiver is non-null, matching the behavior of an explicit guarded block.

## Modern Example Shape

- nullable reference receiver
- null-conditional assignment using `?.`
- helper method or calculation on the right side with an observable side effect or count

## Alternative Example Shape

- explicit `if (receiver is not null)` guard
- same helper method or calculation inside the guarded block
- same observable side effect behavior

## Comparison Type

- Exact comparison

## Teaching Focus

- behavior-aware equivalence
- short-circuiting of right-side evaluation
- confidence that the shorter syntax preserves guarded execution semantics

## Expected Documentation Notes

- explicitly state that the right side is not evaluated when the receiver is null
- keep the observable effect simple, such as a counter increment or logged marker
- make the equivalence statement more important than the transcript details

## Expected Output Guidance

- runtime-equivalent comparison
- deterministic observable behavior
- shared behavior description should state both assignment and skipped evaluation semantics

## Candidate Scenario Direction

Use a property assignment where the right side calls a deterministic helper like `CreateValue()` or `NextLabel()` and the example can show whether that helper ran.

## Done Criteria

- Example scope is defined.
- Guarded evaluation behavior is explicit.
- The example is ready for later concrete code planning.
