# B0002-IEC-Symbol-Binding

## Parent
- Feature: F0001-IEC-to-CSharp-Testability

## Description
Introduce declaration binding and scope-aware symbol resolution for IEC code so identifiers, parameters, and declared variables can be resolved deterministically during interpretation and code generation.

## Value
Typed symbol binding separates syntax from semantics and enables robust validation, clearer diagnostics, and reliable downstream generation.

## Scope
- Model declaration categories such as `VAR`, `VAR_INPUT`, `VAR_OUTPUT`, `VAR_IN_OUT`, and `VAR_TEMP`
- Build scope-aware symbol lookup for the supported subset
- Provide test coverage for duplicate names, missing identifiers, and simple shadowing rules where applicable

## Out of Scope
- Full type inference across all IEC constructs
- Hardware-specific symbol sources

## Acceptance Criteria
- Supported declarations are collected into a symbol table
- Identifier references in the supported subset can be resolved against the correct scope
- Tests validate successful binding and expected failures

## Assumptions
- B0001 provides the typed nodes needed for declaration and identifier references

## Open Questions
- How much of the IEC declaration syntax is already captured by the current builder output
- Which shadowing and redeclaration rules should be enforced in the first increment
