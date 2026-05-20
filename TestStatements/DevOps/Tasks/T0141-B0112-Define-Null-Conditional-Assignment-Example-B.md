# T0141 - Define Null-Conditional Assignment Example B

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second null-conditional assignment comparison example for deferred right-side evaluation.

## Scope

- modern form using null-conditional assignment
- older alternative using an explicit null check around an evaluated right side
- exact comparison with observable guarded evaluation behavior

## Example Direction

- use a helper method or calculation on the right side
- show that the right side is evaluated only when the receiver is non-null
- keep the scenario deterministic and easy to observe

## Done Criteria

- The comparison focus is fixed.
- The behavior-aware angle is explicit.
- The example is ready for a detailed baseline.
