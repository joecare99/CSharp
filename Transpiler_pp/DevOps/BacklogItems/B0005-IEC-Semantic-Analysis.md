# B0005-IEC-Semantic-Analysis

## Parent
- Feature: F0001-IEC-to-CSharp-Testability

## Description
Build a semantic analysis layer that validates supported IEC constructs, resolves types and identifiers, and prepares a stable intermediate representation for code generation.

## Value
A semantic layer reduces generator complexity and prevents runtime-only discovery of structural issues.

## Scope
- Resolve supported IEC primitive types to an internal semantic model
- Validate supported expressions and assignments
- Produce diagnostics for unsupported or invalid constructs in the supported subset

## Out of Scope
- Full code generation
- Full language coverage

## Acceptance Criteria
- Supported input can be validated before generation
- Semantic output contains enough information for deterministic interpretation and code generation
- Tests cover representative valid and invalid inputs

## Assumptions
- B0001 and B0002 provide typed nodes and symbol resolution

## Open Questions
- Which C# target types should be used for each IEC primitive in the first increment
- How diagnostics should be reported across parser, semantic analysis, and generator phases
