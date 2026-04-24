# Feature F-RepoMigrator-05 - Target projection and branch transformation tools

## Status

Draft

## Parent

- Epic `E-RepoMigrator-001 - RepoMigrator migration workbench`

## Goal

Provide path-based target projection and dedicated branch transformation tooling so migrated content can be reshaped into practical branch layouts.

## Summary

This feature documents the already implemented baseline for subdirectory-based target projection during migration and the separate Git branch splitting tool for repository reshaping. The current implementation already supports configurable branch split depth, generated projection plans, and a focused command-line flow for creating path-based snapshot branches.

## In Scope

- Subdirectory-based branch projection during snapshot migration
- Shared planning logic for path-to-branch mapping
- Configurable branch split depth within the supported range
- Dedicated Git branch splitting tool and its option parsing
- Automated coverage around projection planning and tool option behavior

## Out of Scope

- Arbitrary branch-layout transformations beyond the current path-based model
- Full topology reconstruction from complex repository histories
- UI-heavy branch-visualization work
- Server-side branch governance or remote publishing policy

## Acceptance Criteria

- Snapshot migration can project content into target branches based on directory structure
- Projection planning supports the documented branch split depth range
- Generated branch names are normalized into valid branch segments
- A dedicated command-line tool can create path-based split branches from a local Git branch
- Option parsing and projection planning are covered by automated tests
- The relationship between in-migration projection and post-processing tooling stays explicit in planning

## Related Backlog Items

- `BI-RepoMigrator-005` - `Define branch projection and repository reshaping scenarios`
- `BI-RepoMigrator-008` - `Avoid Git ref collisions in projected sub-branches`
- `BI-RepoMigrator-006` - `Expand automated test baseline for migration orchestration and tools`

## Risks

- Path-based branch projection may not match all repository conventions in real migrations
- Branch naming rules may collide with existing target naming policies
- Post-processing tools may be misused as substitutes for unsupported core migration scenarios

## Open Questions

- Which projection scenarios belong inside the main migration flow versus the branch split tool?
- Is the current split-depth range sufficient for expected repository structures?
- Which overwrite and collision rules should become mandatory defaults for production use?

## Next Refinement Steps

1. Confirm the migration scenarios that require branch projection as a first-class option
2. Clarify the product boundary between target projection and post-processing tools
3. Add backlog items for branch naming, collision handling, and tool safety rules
4. Revisit whether deeper projection modes are needed after initial usage feedback
