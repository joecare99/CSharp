# T0153A - Partial Members Example B Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second concrete partial-members comparison example that contrasts a partial event with explicit `add` and `remove` accessors against an older non-partial custom-event implementation.

## Example Intent

Demonstrate how C# 14 allows an event surface to be declared in one partial declaration and implemented in another, which is useful for generated code or source-generator-backed event patterns.

## Modern Example Shape

- partial type with a defining partial event declaration
- implementing partial event declaration with explicit `add` and `remove` accessors
- clear separation between public event surface and backing logic

## Alternative Example Shape

- custom event implemented as a normal non-partial event
- manual backing storage and accessors in one place
- same approximate subscription semantics without first-class split declarations

## Comparison Type

- Approximate comparison

## Teaching Focus

- declaration-versus-implementation separation for events
- custom accessor requirement on the implementing declaration
- usefulness for generated event infrastructure or weak-event-style patterns

## Expected Documentation Notes

- explain that partial events are not field-like at runtime
- note that the implementing declaration must define `add` and `remove`
- keep the example compact and source-organization focused

## Expected Output Guidance

- readability-focused or behavior-aware comparison
- output is secondary to API-shape and source-structure explanation
- one shared subscription-behavior statement is enough if any output is shown

## Candidate Scenario Direction

Use a small notification or callback surface where custom event accessors are easy to understand without requiring broader UI or framework context.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
