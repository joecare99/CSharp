# T0153 - Define Partial Members Example B

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second partial-members comparison example for a partial event with explicit `add` and `remove` accessors.

## Scope

- modern form using a partial event in a partial type
- older alternative using a manually implemented custom event without partial split support
- approximate comparison with emphasis on declaration-versus-implementation coordination

## Example Direction

- use a small event surface with clear subscription semantics
- show that the defining declaration is field-like while the implementing declaration provides accessors
- keep the scenario compact and source-organization focused

## Done Criteria

- The comparison focus is fixed.
- The example is ready for a detailed baseline.
