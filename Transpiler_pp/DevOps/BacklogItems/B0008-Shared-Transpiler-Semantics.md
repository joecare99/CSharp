# B0008-Shared-Transpiler-Semantics

## Parent
- Epic: E0001-Transpiler

## Description
Extract the typed AST, declaration analysis, identifier binding, type binding, and deterministic generation-facing semantic model into a shared project so multiple language frontends and backends can depend on one semantic foundation.

## Value
A shared semantic layer keeps IEC-specific parsing separate from reusable transpiler semantics and prepares the solution for Pascal, DriveBASIC, Basic, C++, and dedicated backend or optimization projects.

## Scope
- Introduce a shared semantics project for typed transpiler model components
- Move reusable IEC semantic model files out of the frontend compilation boundary
- Move reusable semantic model files into the shared project as their physical source owner instead of linking them from the IEC frontend project
- Move reusable semantic unit tests into a dedicated shared semantics test project
- Keep frontend-specific mapping and scanning logic in the IEC project
- Preserve current testable IEC-to-C# behavior during the split
- Establish `TranspilerLib.CSharp` as the first backend-facing consumer of the shared semantics layer on modern .NET targets
- Move language-independent artifact metadata such as artifact kind and accessibility into the shared semantic model
- Move wrapper-level traits such as static and partial intent into the shared semantic model
- Consolidate compilation-unit artifact traits into a shared metadata object to keep semantic growth structured
- Consolidate declaration-level traits into a shared metadata object to support future language-independent declaration modeling
- Introduce a general shared metadata base so specialized metadata objects align structurally before broader generalization

## Out of Scope
- Full rename of the semantic model away from IEC naming
- Introduction of a dedicated optimization project
- New frontend or backend implementations

## Acceptance Criteria
- A shared semantics project exists in the solution workspace
- IEC frontend references the shared semantics project
- Shared typed AST and semantic analysis files compile from the shared project
- Shared typed AST and semantic analysis files physically live under the shared semantics project
- Shared semantic unit tests run from a dedicated semantics test project
- Existing focused IEC semantic and generation tests still pass
- `TranspilerLib.CSharp` exposes a backend entry point that consumes the shared semantics layer on supported target frameworks
- Shared semantics own artifact kind and accessibility metadata for compilation units
- Shared semantics own wrapper-level static and partial traits for compilation units
- Compilation units expose a shared artifact metadata object that backends can consume directly
- Variable declarations expose a shared declaration metadata object that semantic consumers can use directly
- Specialized metadata objects align to a general shared metadata base

## Assumptions
- The current typed model is still IEC-shaped, but reusable enough to serve as the first shared semantic layer
- C# optimization can remain in the existing C# project until semantic boundaries stabilize

## Open Questions
- Whether the shared project should later be renamed to Core or IR
- When the semantic model should be generalized beyond IEC-prefixed type names
- Whether deterministic code emission belongs in semantics or in a dedicated backend layer
- When the shared semantics project should be broadened beyond net8+ so legacy C# backend targets can consume it directly
- Which additional language-independent declaration traits should follow artifact metadata into shared semantics next
- How wrapper-level traits should later relate to member-level traits and file partitioning rules
- Whether later declaration-level metadata should reuse the compilation-unit metadata object shape or introduce a parallel model
- Which additional declaration traits should join section and initializer text in the shared declaration metadata next
- Which common metadata concerns should graduate from the shared base into first-class reusable abstractions next
