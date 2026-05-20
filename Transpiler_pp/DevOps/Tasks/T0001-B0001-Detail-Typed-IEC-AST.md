# T0001-B0001-Detail-Typed-IEC-AST

## Parent
- Backlog Item: B0001-Typed-IEC-AST

## Description
Detail the first implementation increment for a typed IEC AST so the current string-oriented execution path can be migrated toward explicit semantic nodes that are easier to validate, interpret, and later transpile to C#.

## Goal
Define the first AST slice that is small enough to implement quickly and complete enough to replace the current interpreter assumptions for simple assignments and function-call expressions.

## Proposed First-Pass Node Set
- Compilation unit or root node for an IEC function or program fragment
- Declaration node for a named variable with a declared IEC type where available
- Assignment statement node
- Identifier expression node
- Literal expression nodes for numeric, string, and boolean values
- Function call expression node
- Unary expression node for leading sign and logical negation where supported
- Binary expression node for arithmetic and comparison operators used by the initial subset

## Proposed Migration Strategy
1. Keep the existing scanner token output unchanged for the first increment
2. Introduce typed AST model classes in `TranspilerLib.IEC`
3. Add a mapping layer from current builder output into typed nodes for the supported subset
4. Update `IECInterpreter` to prefer typed nodes for the supported scenarios
5. Keep unsupported constructs on the existing path temporarily to reduce migration risk

## Design Notes
- Prefer one class per file for AST nodes
- Model nullability explicitly
- Avoid coupling typed AST nodes to UI or output formatting concerns
- Keep node APIs simple enough for both interpretation and later code generation
- Preserve source-location information where already available so diagnostics can be added later

## Initial Implementation Candidates
- Add a common AST base abstraction or interface
- Add statement and expression abstractions
- Add `IecAssignmentStatement`
- Add `IecIdentifierExpression`
- Add `IecLiteralExpression`
- Add `IecFunctionCallExpression`
- Add `IecUnaryExpression`
- Add `IecBinaryExpression`
- Add tests that construct and validate representative nodes
- Add interpreter tests that execute typed assignments and function-call expressions

## Suggested Test Focus
- Assignment with identifier on both sides
- Assignment with a literal on the right-hand side
- Assignment with a single-argument function call
- Assignment with a multi-argument function call
- Unary minus on numeric literals or identifiers
- Binary arithmetic for the first supported operators
- Failure behavior when an unsupported construct cannot be mapped into typed nodes

## Acceptance Criteria
- The first typed AST nodes exist in the IEC project and follow repository naming and nullability conventions
- The supported subset can be represented without reparsing raw `Code` strings during execution
- Tests document the supported node shapes and the first interpreter integration path
- The migration path remains incremental and does not require full parser replacement in one step

## Assumptions
- The current `IECCodeBuilder` output contains enough structural information to bootstrap the first typed mapping layer
- The first increment should optimize for simple, testable semantics rather than broad syntax coverage

## Open Questions
- Whether declarations should be represented immediately or added in a follow-up task after expressions and assignments
- Whether the typed AST should attach directly to existing code blocks or live as a parallel model during migration

## Next Refinement Steps
- Split this task into implementation and test tasks if coding starts immediately
- Review existing `CodeBlockType` usage to identify the minimum mapping surface
- Decide on the concrete namespace and folder layout for new AST node files
