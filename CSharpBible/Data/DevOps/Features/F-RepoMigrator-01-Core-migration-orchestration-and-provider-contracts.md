# Feature F-RepoMigrator-01 - Core migration orchestration and provider contracts

## Status

Draft

## Parent

- Epic `E-RepoMigrator-001 - RepoMigrator migration workbench`

## Goal

Provide the shared migration contracts and orchestration flow that allow repository migrations to run independently from provider-specific command details.

## Summary

This feature documents the already implemented RepoMigrator baseline around shared repository models, provider selection, migration options, and the central migration service. It is the functional backbone that lets Git, SVN, the WPF application, and command-line tools operate on the same migration concepts.

## In Scope

- Shared endpoint, query, capability, and metadata models
- Provider factory and provider-selection abstractions
- Snapshot-based and native-history migration orchestration
- Shared progress reporting contracts for UI and command-line flows
- Extensibility points for additional repository providers

## Out of Scope

- Provider-specific Git or SVN command behavior details
- UI-specific validation and visual workflow design
- Specialized performance tuning for pipelined execution
- Post-processing tools that reshape repositories after migration

## Acceptance Criteria

- Shared repository models cover endpoints, changesets, capabilities, and migration options
- A central migration service selects the correct execution path for native-history and snapshot migration
- Provider implementations can be resolved through a common factory abstraction
- Progress reporting is available through shared contracts instead of UI-specific logic
- Additional providers can be added without rewriting the orchestration entry point

## Related Backlog Items

- `BI-RepoMigrator-001` - `Define supported migration matrix and capability baseline`
- `BI-RepoMigrator-002` - `Stabilize provider contract for migration and selection workflows`
- `BI-RepoMigrator-006` - `Expand automated test baseline for migration orchestration and tools`

## Risks

- The shared provider abstraction may hide edge cases that only appear in concrete repository systems
- Transfer-mode growth may increase orchestration complexity faster than expected
- Capability flags may become ambiguous if provider behavior is not documented precisely

## Open Questions

- Which provider behaviors must remain mandatory across all future implementations?
- How far should capability reporting go before it becomes too detailed for the shared model?
- Which additional repository types are realistic enough to influence contract design now?

## Next Refinement Steps

1. Confirm the supported migration matrix that the shared contracts must cover first
2. Review optional versus mandatory provider operations
3. Split contract hardening work from provider-specific work in backlog refinement
4. Add focused follow-up items for orchestration edge cases and regression coverage
