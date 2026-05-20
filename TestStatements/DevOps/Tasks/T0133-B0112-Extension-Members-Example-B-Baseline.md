# T0133A - Extension Members Example B Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second concrete extension-members comparison example that contrasts an extension property with an older helper-method style alternative.

## Example Intent

Demonstrate that extension members can change the apparent shape of an API by exposing property-like access, while older-language alternatives usually require a helper method instead.

## Modern Example Shape

- one extension block
- one extension property that reads naturally from the receiver instance
- minimal supporting members only where necessary

## Alternative Example Shape

- helper method with a verb-like name that returns the same information
- optional classic extension method form if that improves fairness of comparison

## Comparison Type

- Approximate comparison

## Teaching Focus

- difference between property-like and method-like usage
- API shape and readability
- limits of older-language alternatives when no direct extension-property syntax exists

## Expected Documentation Notes

- explain clearly why the comparison is approximate
- keep the semantic purpose of property and helper method closely aligned
- focus on source ergonomics rather than output detail

## Expected Output Guidance

- readability-focused comparison
- if output is shown, one shared result is enough
- avoid pretending the helper method is a direct syntactic equivalent when it is only semantically similar

## Candidate Scenario Direction

Use a receiver type where a computed property-like concept is easy to understand, such as a simple state or classification value.

## Done Criteria

- Example scope is defined.
- Approximate comparison is explicit.
- The example is ready for later concrete code planning.
