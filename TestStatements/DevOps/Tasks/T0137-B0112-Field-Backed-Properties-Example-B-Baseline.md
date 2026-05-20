# T0137A - Field-Backed Properties Example B Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the second concrete field-backed-properties comparison example that contrasts a normalizing setter using `field` with an older explicit-backing-field alternative.

## Example Intent

Demonstrate that field-backed properties can express setter-side transformation logic concisely while preserving the same semantic result as an explicit backing field implementation.

## Modern Example Shape

- property with `get;`
- `set` accessor using `field`
- value normalization inside the setter, such as trimming or range adjustment

## Alternative Example Shape

- explicit private backing field
- explicit accessor pair
- same normalization logic implemented in the setter body

## Comparison Type

- Exact comparison

## Teaching Focus

- concise setter logic
- unchanged semantic result between both versions
- readability gain without altering property meaning

## Expected Documentation Notes

- emphasize equivalence of resulting stored value
- keep the example small and deterministic
- note that this is a strong example of lower boilerplate rather than different behavior

## Expected Output Guidance

- runtime-equivalent comparison
- deterministic output or state description
- describe shared resulting property state once rather than duplicating it per version

## Candidate Scenario Direction

Use a property where normalization is easy to recognize, such as trimmed text, bounded numeric value, or canonicalized casing.

## Done Criteria

- Example scope is defined.
- Comparison shape is clear.
- The example is ready for later concrete code planning.
