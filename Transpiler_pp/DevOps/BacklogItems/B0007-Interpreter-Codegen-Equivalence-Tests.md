# B0007-Interpreter-Codegen-Equivalence-Tests

## Parent
- Feature: F0001-IEC-to-CSharp-Testability

## Description
Create semantic equivalence tests that compare supported IEC behavior between the interpreter path and the generated C# path.

## Value
Equivalence testing provides a controlled way to grow language coverage while protecting behavior during generator evolution.

## Scope
- Define shared test fixtures for supported IEC samples
- Compare outputs and state transitions across interpreter and generated code execution
- Cover representative expressions, assignments, and cycle-based scenarios

## Out of Scope
- Full conformance testing for all IEC dialect variants
- Performance benchmarking

## Acceptance Criteria
- Shared scenarios can be executed through both execution paths
- Divergences are surfaced through automated tests
- The initial suite covers the supported subset defined by earlier roadmap items

## Assumptions
- Both the interpreter and generated C# paths exist for the supported subset

## Open Questions
- Whether generated code should be compiled at test time or pre-generated into fixtures
- How to keep expected behavior readable as the supported subset grows
