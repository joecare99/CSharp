# Task: Document.Base Contracts Wave 01

## Parent
- Related note: `DevOps/BacklogItems/PBI-12-DocumentBaseCanonicalModel.md`

## Goal
Finalize the minimal shared contract layer in `Document.Base` so the higher-level document model and import pipeline can rely on stable interfaces, enums, and rudimentary base types.

## Status
Draft

## Scope
- review and refine the new IDocumentXXX interfaces
- document each contract in English
- keep the base layer minimal and renderer-neutral
- avoid PDF-specific types in the shared contracts
- validate that the base layer supports the planned model and importer layers

## Recommended Order
1. Review the current interface set.
2. Refine naming and comments.
3. Verify the contracts stay document-type agnostic.
4. Validate the contract surface against the planned model layer.
5. Confirm the base layer does not introduce PDF-specific dependencies.

## Assumptions
- the shared contract layer should remain small
- concrete document behavior belongs in higher-level projects
- PDF-specific implementation details must stay out of the base layer

## Exit Criteria
- the contract layer is documented and coherent
- the interfaces are agnostic to document type
- the model layer can consume the contracts cleanly
