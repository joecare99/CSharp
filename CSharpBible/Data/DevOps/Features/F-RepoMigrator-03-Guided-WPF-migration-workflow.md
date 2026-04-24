# Feature F-RepoMigrator-03 - Guided WPF migration workflow

## Status

Draft

## Parent

- Epic `E-RepoMigrator-001 - RepoMigrator migration workbench`

## Goal

Provide a guided desktop workflow that helps users configure migration endpoints, choose supported options, and monitor execution without dealing directly with low-level provider commands.

## Summary

This feature documents the already implemented WPF baseline around staged setup, option selection, repository-specific choices, progress reporting, and persisted input state. The current application already separates setup, options, and execution stages while keeping shared migration logic outside the UI layer.

## In Scope

- Step-based setup, options, and execution workflow in the WPF application
- ViewModel-based state transitions and validation hints
- Provider-aware branch, tag, and revision selection inputs
- Persisted app input state with protected credential storage
- Execution progress and log presentation for migrations

## Out of Scope

- Core migration logic that belongs in shared services
- Advanced UI redesign or visual-polish initiatives
- Packaging and deployment of the desktop application
- Cross-platform desktop support beyond the current WPF baseline

## Acceptance Criteria

- The desktop workflow separates setup, options, and execution into explicit stages
- UI logic stays bound to shared services and provider abstractions instead of embedding migration commands directly
- Provider-specific selections such as Git references and SVN revisions can be surfaced in the workflow
- Relevant input state can be saved and restored across sessions
- Credentials are not stored in plain text in the persisted state file
- The workflow can surface migration progress without moving UI strings into core services

## Related Backlog Items

- `BI-RepoMigrator-003` - `Harden guided WPF migration setup and execution flow`
- `BI-RepoMigrator-006` - `Expand automated test baseline for migration orchestration and tools`

## Risks

- Workflow complexity may grow as provider-specific options and edge cases increase
- Input-state persistence can become brittle if option sets evolve without migration rules
- Validation feedback may still be too technical for non-expert users in failure scenarios

## Open Questions

- Which configuration paths are used most often and should be optimized first?
- Which provider-specific details should remain visible to users versus hidden behind defaults?
- What level of troubleshooting detail belongs in the desktop workflow versus external logs?

## Next Refinement Steps

1. Review setup-stage validation and recovery paths with current provider capabilities
2. Clarify the preferred UX for provider-specific selections and unsupported options
3. Add backlog items for workflow hardening and state-migration scenarios
4. Keep UI-facing wording refinements separate from core-service changes
