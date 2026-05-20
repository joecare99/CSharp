# T0129A - CS14 Extension Members Package Baseline

## Parent Backlog Item

- B0112 - Define C# 14 Feature Comparison Scenarios

## Summary

Define the first concrete example package for extension members in `TestStatements.CS14` and the older comparison patterns it should use.

## Package Intent

Show how C# 14 extension members group related extension behavior more naturally than older extension-method-only patterns.

## Modern Feature Focus

- extension blocks
- extension properties
- static extension members on the extended type surface

## Non-C# 14 Comparison Direction

- classic static extension methods in a helper class
- ordinary static helper methods or properties for functionality that cannot be expressed as older extension members

## Comparison Type

- Approximate comparison

## Planned Example Cuts

### Example A - Instance-Style Extension Group

Goal:

- Show how several related instance-style extensions can be grouped in one extension block.

Alternative:

- separate classic extension methods in a helper class

Teaching focus:

- grouping and discoverability of related members
- reduced conceptual fragmentation

### Example B - Extension Property versus Helper Method

Goal:

- Show how a property-like extension member changes the shape of the usage code.

Alternative:

- helper method with an equivalent semantic purpose

Teaching focus:

- source readability
- where the older alternative is only approximate

### Example C - Static Extension Surface

Goal:

- Show how extension members can extend the type surface and not only instances.

Alternative:

- separate helper utilities or factory-style methods

Teaching focus:

- conceptual difference between extending an instance and extending a type-oriented API surface

## Expected Documentation Style

- comparison-first
- emphasize structural approximation clearly
- keep runtime output secondary unless a minimal observable result helps understanding

## Output Guidance

- mostly readability-focused or behavior-aware comparison
- avoid inventing noisy console output
- use small observable results only where they support the explanation

## Migration Note Direction

- explain when extension blocks improve grouping and discoverability
- explain when older helper-class patterns remain necessary for compatibility

## Done Criteria

- Package scope is defined.
- Comparison cuts are outlined.
- The package is ready for concrete example implementation planning.
