# T0144A - Unbound Generic Nameof Example A Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first concrete unbound-generic-`nameof` comparison example that contrasts `nameof` on an unbound generic type with the older bound-generic workaround.

## Example Intent

Demonstrate that C# 14 allows a generic type name to be referenced in `nameof` without inventing an irrelevant type argument, while preserving the same compile-time string result.

## Modern Example Shape

- `nameof(List<>)` or comparable unbound generic type usage
- small example that focuses only on the type name result
- no unnecessary surrounding runtime logic

## Alternative Example Shape

- `nameof(List<int>)` or another closed generic workaround
- same compile-time resulting name
- same learning focus with more arbitrary syntax noise

## Comparison Type

- Exact comparison

## Teaching Focus

- removal of arbitrary type arguments
- same compile-time result with clearer intent
- direct demonstration of a small but useful language improvement

## Expected Documentation Notes

- emphasize that both versions produce the same type name
- explain that the improvement is source clarity rather than runtime behavior
- keep the example minimal and API-oriented

## Expected Output Guidance

- runtime-equivalent comparison with compile-time focus
- output is optional and secondary
- one shared expected name result is sufficient

## Candidate Scenario Direction

Use a familiar generic framework type or a small custom generic type where the type argument is obviously irrelevant to the name being requested.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
