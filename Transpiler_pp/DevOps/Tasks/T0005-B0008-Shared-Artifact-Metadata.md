# T0005-B0008-Shared-Artifact-Metadata

## Parent
- Backlog Item: B0008-Shared-Transpiler-Semantics

## Description
Move language-independent artifact metadata such as artifact kind and accessibility into the shared semantic layer so backend projects map common declarations instead of inventing them locally.

## Goal
Represent artifact-level intent in shared semantics before backend shaping so the framework stays AST-first and language-independent.

## Scope
- Add shared artifact kind metadata to the compilation unit model
- Add shared accessibility metadata to the compilation unit model
- Update the C# backend to map shared metadata instead of owning those concepts
- Add focused tests for shared metadata and backend mapping

## Out of Scope
- Full declaration-level accessibility across all semantic nodes
- Rich lifecycle semantics for function blocks and programs
- Broad renaming of existing IEC-prefixed semantic types

## Assumptions
- Artifact kind and accessibility are language-independent enough to belong in shared semantics
- Backend projects should map shared metadata to target-language syntax rather than define the metadata themselves

## Open Questions
- Whether accessibility should later apply to members, declarations, and namespaces in addition to compilation units
- Whether artifact kind should later carry more semantic execution metadata than wrapper-shape intent

## Implementation Notes
1. Keep the first shared metadata slice compilation-unit-scoped
2. Preserve current defaults so existing focused generation tests remain stable
3. Validate both shared semantics and backend mapping with focused MSTest coverage

## Acceptance Criteria
- Shared semantics define artifact kind and accessibility metadata
- The compilation unit model exposes the new metadata with stable defaults
- The C# backend consumes shared metadata instead of backend-local artifact role ownership
- Focused semantic and backend tests pass
