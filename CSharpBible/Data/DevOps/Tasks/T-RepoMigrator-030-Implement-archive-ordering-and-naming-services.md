# Task T-RepoMigrator-030 - Implement archive ordering and naming services

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-011` - `Define release ref naming and manual archive ordering workflow`

## Goal

Implement the first-slice services that derive deterministic archive order and default release names.

## Scope

- Implement ordering precedence using manual order, version cues, archive metadata, fallback timestamps, and stable tie-breakers
- Implement default tag-name derivation from archive file names without extensions
- Implement optional first-slice branch-name derivation for repository-backed destinations
- Keep regex and advanced prefix normalization out of this initial implementation unless a minimal hook is required
- Emit explainable ordering evidence for review and troubleshooting

## Detailed Work Packages

1. Implement `ArchiveOrderingService`
2. Implement `ArchiveRefNamingService`
3. Add evidence structures or helper logic needed by planners and UI review later
4. Add tests covering precedence, tie-breakers, compound extensions, and default branch-name derivation

## Deliverables

- Ordering service implementation
- Naming service implementation
- Unit tests covering the first-slice precedence and naming rules

## Dependencies

- `T-RepoMigrator-018` - `Define archive core models and persisted state schema`
- `T-RepoMigrator-027` - `Implement archive plan and state models`
- `T-RepoMigrator-029` - `Implement zip archive driver`

## Acceptance Criteria

- Archive ordering is deterministic and explainable
- Default tag naming works from the archive base name with compound-extension safety
- Optional first-slice branch naming is available for repository-backed destinations
