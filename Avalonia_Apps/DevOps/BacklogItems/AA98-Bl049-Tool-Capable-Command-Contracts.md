# AA98-Bl049 Tool-Capable Command Contracts

## Parent
- Feature: `../Features/AA98-F40-Copilot-Assisted-Workflow.md`
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`

## Value
AA98 commands can be invoked both by UI users and by AI components through explicit machine-readable contracts.

## Scope
- Define metadata for tool-capable commands.
- Describe command parameters, results, context requirements, and safety requirements.
- Keep shared reusable metadata separate from AA98-specific routing semantics.
- Align command invocation with consent and context-sharing policies.

## Acceptance Criteria
- Tool-capable command metadata has a clear shared versus AA98-specific boundary.
- Commands can describe parameters and results without provider-specific AI assumptions.
- Tests validate descriptor normalization and safety metadata.

## Implementation Tasks
- `AA98-T076 Define Tool-Capable Command Metadata`
- `AA98-T077 Implement Tool Command Descriptor Contracts`
- `AA98-T078 Add Tool Command Contract Tests`

## Assumptions
- Existing context-sensitive command contracts are the foundation for this work.

## Open Questions
- Which metadata belongs in `AppKomponentBaseLib` versus `AA98_AvlnCodeStudio.Base`?

## Next Refinement Steps
1. Start with contract-only implementation.
2. Add invocation infrastructure only after descriptors are stable.

## Status
- Proposed
