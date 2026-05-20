# T0010-B0008-Physical-Semantics-Source-Ownership

## Parent
- Backlog Item: B0008-Shared-Transpiler-Semantics

## Description
Move the shared semantic AST and metadata source files so they physically live in `TranspilerLib.Semantics` instead of being linked from the IEC frontend project.

## Goal
Make the shared semantics project the physical and logical owner of reusable semantic model code to reduce maintenance friction and avoid duplicate-compilation regressions.

## Scope
- Move shared AST and semantic model source files into `TranspilerLib.Semantics\Models\Ast`
- Replace linked compile items in the semantics project with normal local compilation
- Keep only IEC-specific mapping and scanning code in `TranspilerLib.IEC`
- Preserve namespaces and current consumer behavior during the relocation
- Validate the relocation with focused builds and tests

## Out of Scope
- Renaming IEC-prefixed semantic types
- Reworking semantic namespaces
- Broad parser or backend behavior changes

## Assumptions
- Physical source ownership should match semantic project ownership for maintainability
- Stable namespaces are preferable to a broader rename during this relocation step

## Open Questions
- When the shared semantic types should be renamed away from IEC-prefixed names
- Whether the semantics project should later use a more neutral namespace root than `TranspilerLib.IEC.Models.Ast`

## Implementation Notes
1. Move only reusable semantic model files in this step
2. Leave `IecAstMapper` in the IEC frontend project
3. Keep validation focused on the semantic and backend test surface

## Acceptance Criteria
- Shared AST and metadata files physically live under `TranspilerLib.Semantics`
- `TranspilerLib.Semantics.csproj` compiles local files instead of linked IEC files
- `TranspilerLib.IEC.csproj` keeps only IEC-specific AST mapping ownership
- Existing focused semantic and backend tests still pass
