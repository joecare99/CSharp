# B0001-Typed-IEC-AST

## Parent
- Feature: F0001-IEC-to-CSharp-Testability

## Description
Define a typed abstract syntax tree for IEC code that replaces string-based execution assumptions with explicit semantic nodes for declarations, expressions, and statements.

## Value
A typed AST creates the foundation for deterministic interpretation, reliable code generation, and focused unit testing without depending on fragile string parsing at execution time.

## Scope
- Introduce explicit node types for declarations, literals, identifiers, function calls, unary expressions, binary expressions, and assignments
- Define traversal or access patterns needed by interpretation and later generation steps
- Keep the initial node set limited to the smallest subset needed for the next semantic increment

## Out of Scope
- Full semantic binding of identifiers
- Complete control-flow modeling for all IEC constructs
- Direct C# code generation

## Acceptance Criteria
- IEC expressions and simple assignments can be represented without reparsing raw code strings
- Node types are structured so later interpreter and generator steps can consume them directly
- Tests cover the initial node shapes and representative sample expressions

## Assumptions
- Existing code blocks can temporarily coexist with the typed AST during migration
- The first AST slice should support the currently implemented interpreter scenarios

## Open Questions
- Should typed AST nodes extend the existing code block model or be introduced as a parallel semantic model
- Which existing scanner outputs can be reused without broad refactoring

## Next Refinement Steps
- Define the concrete first-pass node inventory
- Describe the migration path from `IECCodeBuilder` output to typed nodes
- Add implementation tasks for model design and tests
