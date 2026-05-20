# T0130A - CS14 Field-Backed Properties Package Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first concrete example package for field-backed properties in `TestStatements.CS14` and the older explicit-backing-field comparison patterns it should use.

## Package Intent

Show how the `field` contextual keyword reduces boilerplate in property accessors while preserving familiar property semantics.

## Modern Feature Focus

- `field` in property accessors
- validation logic inside concise property bodies
- transition from auto-property style to custom accessor behavior

## Non-C# 14 Comparison Direction

- explicit private backing field
- fully written get and set accessors with validation logic

## Comparison Type

- Exact comparison

## Planned Example Cuts

### Example A - Null-Rejecting Property

Goal:

- Show a property that rejects null values.

Alternative:

- explicit backing field with the same guard logic

Teaching focus:

- identical runtime behavior
- reduced ceremony in the modern version

### Example B - Normalizing Setter

Goal:

- Show a property that transforms or normalizes assigned values.

Alternative:

- explicit backing field with the same setter transformation

Teaching focus:

- concise accessor logic
- unchanged semantic result

### Example C - Naming Caveat

Goal:

- Show the ambiguity risk when a member or identifier named `field` already exists.

Alternative:

- explicit backing field version without the new keyword

Teaching focus:

- migration caveat
- readability and naming trade-offs

## Expected Documentation Style

- runtime-equivalent comparison
- shared output or shared expected behavior description
- clear note that the value lies mainly in lower boilerplate and clearer intent

## Output Guidance

- use one shared expected behavior description for both versions
- prefer deterministic examples
- avoid duplicate output sections for identical behavior

## Migration Note Direction

- explain when teams can simplify validated properties with the new syntax
- explain when naming clarity may require caution or explicit disambiguation

## Done Criteria

- Package scope is defined.
- Comparison cuts are outlined.
- The package is ready for concrete example implementation planning.
