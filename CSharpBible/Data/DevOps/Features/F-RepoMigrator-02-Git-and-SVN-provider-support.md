# Feature F-RepoMigrator-02 - Git and SVN provider support

## Status

Draft

## Parent

- Epic `E-RepoMigrator-001 - RepoMigrator migration workbench`

## Goal

Provide practical repository-provider support for Git and SVN so RepoMigrator can probe repositories, enumerate selectable references or revisions, and execute supported migration flows.

## Summary

This feature documents the already implemented provider baseline for local and remote Git scenarios plus SVN CLI-based migration scenarios. The current implementation already exposes provider capabilities, repository probing, selection data, changeset enumeration, snapshot materialization, and supported commit or transfer operations.

## In Scope

- Git provider behavior for probing, selection data, snapshot export, and native history transfer
- SVN CLI provider behavior for probing, revision selection, log enumeration, export, and commit-based target updates
- Capability reporting differences between Git and SVN
- Provider-level credential usage for supported access modes
- Automated tests around provider-specific behaviors and error handling

## Out of Scope

- Support for repository systems beyond Git and SVN
- Full authentication coverage for every enterprise environment
- Performance tuning outside the provider responsibilities
- UI-specific presentation of provider options

## Acceptance Criteria

- Git provider supports capability discovery for local and remote repository paths
- Git provider exposes selectable branches and tags where supported
- SVN provider exposes selectable revisions for path-scoped migration flows
- Supported Git and SVN providers can enumerate changesets and materialize snapshots
- Native history transfer remains explicitly scoped to supported Git-to-Git scenarios
- Provider-specific regression tests cover key capability and validation behaviors

## Related Backlog Items

- `BI-RepoMigrator-001` - `Define supported migration matrix and capability baseline`
- `BI-RepoMigrator-002` - `Stabilize provider contract for migration and selection workflows`
- `BI-RepoMigrator-007` - `Fix SVN working-copy sync for SVN-to-SVN migration`
- `BI-RepoMigrator-006` - `Expand automated test baseline for migration orchestration and tools`

## Risks

- Remote Git and SVN behavior may differ across hosting environments and server policies
- SVN CLI availability and server revprop policies may limit real-world migrations
- Credential expectations may vary beyond the currently visible username and password flows

## Open Questions

- Which Git remote scenarios are required beyond the currently supported baseline?
- Which SVN server policies must be treated as blockers versus expected limitations?
- How should unsupported provider capabilities be surfaced consistently across application and tools?

## Next Refinement Steps

1. Confirm the MVP source and target combinations for Git and SVN
2. Record provider-specific constraints that affect production usage
3. Separate provider hardening work from new-provider expansion work
4. Add targeted backlog items for missing authentication and hosting scenarios
