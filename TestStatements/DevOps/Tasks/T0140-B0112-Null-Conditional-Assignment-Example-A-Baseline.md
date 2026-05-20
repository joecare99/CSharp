# T0140A - Null-Conditional Assignment Example A Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first concrete null-conditional assignment comparison example that contrasts a simple guarded property assignment with an older explicit null-check alternative.

## Example Intent

Demonstrate that null-conditional assignment can express the same guarded property update as an explicit `if` block with less control-flow noise.

## Modern Example Shape

- nullable reference receiver
- property assignment using `?.` on the left side
- small deterministic assigned value

## Alternative Example Shape

- explicit `if (receiver is not null)` guard
- identical property assignment inside the guarded block
- same assigned value and same observable result

## Comparison Type

- Exact comparison

## Teaching Focus

- reduced branching noise
- same semantic intent in the normal property-assignment case
- clear first example for the feature

## Expected Documentation Notes

- emphasize that both versions update the same property only when the receiver exists
- describe one shared expected outcome
- note that the main benefit is brevity and directness

## Expected Output Guidance

- runtime-equivalent comparison
- deterministic behavior
- minimal output is sufficient if any output is shown at all

## Candidate Scenario Direction

Use a small object such as a customer, order, settings, or view-model-like state holder where assigning one property is easy to observe.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
