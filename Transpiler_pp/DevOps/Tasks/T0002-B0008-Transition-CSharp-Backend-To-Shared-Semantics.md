# T0002-B0008-Transition-CSharp-Backend-To-Shared-Semantics

## Parent
- Backlog Item: B0008-Shared-Transpiler-Semantics

## Description
Introduce a first backend-facing API in `TranspilerLib.CSharp` that consumes the shared semantic model without disturbing the existing scanner and optimizer implementation.

## Goal
Make `TranspilerLib.CSharp` a direct consumer of `TranspilerLib.Semantics` so the C# project becomes the natural backend entry point for deterministic source generation.

## Architecture Alignment
- This task follows the repository-local AST-first direction documented in DevOps
- `TranspilerLib.CSharp` acts as a backend consumer of the shared semantic model instead of becoming the owner of AST definitions
- Shared semantic structures remain the language-independent contract between frontends and backends

## Scope
- Reference `TranspilerLib.Semantics` from `TranspilerLib.CSharp` for modern .NET targets
- Add a thin backend facade in the C# project
- Add focused tests for the backend bridge
- Document the target-framework boundary explicitly

## Out of Scope
- Replacing the existing C# scanner stack
- Moving the optimizer into a separate project
- Renaming IEC-shaped semantic types

## Assumptions
- The first backend bridge should remain a thin adapter over the deterministic semantic emitter
- .NET Framework targets in `TranspilerLib.CSharp` should remain buildable without requiring the semantics project yet

## Open Questions
- Whether the backend facade should later grow options for namespace, type layout, or syntax-tree output
- Whether a future optimization stage should operate on generated source, syntax trees, or the shared semantic model

## Implementation Notes
1. Keep the existing scanner and optimizer APIs unchanged
2. Add the semantics reference only where the target framework supports it
3. Validate the bridge with a focused MSTest instead of broad scanner test changes

## Acceptance Criteria
- `TranspilerLib.CSharp` references `TranspilerLib.Semantics` for modern targets
- A backend facade exists in `TranspilerLib.CSharp`
- Focused backend bridge tests pass
- Existing scanner work remains untouched by the transition
