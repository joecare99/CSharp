# Feature F-RepoMigrator-04 - Advanced migration execution modes

## Status

Draft

## Parent

- Epic `E-RepoMigrator-001 - RepoMigrator migration workbench`

## Goal

Provide execution modes beyond the basic sequential flow so RepoMigrator can handle migrations with different operational and throughput requirements.

## Summary

This feature documents the already implemented execution-mode baseline for sequential snapshot migration, pipelined snapshot migration, and detailed progress reporting around export and commit handoff. The current implementation already includes execution-mode options, bounded pipeline coordination, ordered commits, and diagnostics for pipeline progress.

## In Scope

- Sequential snapshot migration behavior
- Pipelined snapshot migration behavior with bounded export coordination
- Execution-mode options and worker-tuning parameters
- Ordered commit processing after parallel export work
- Pipeline-oriented progress diagnostics and cleanup paths

## Out of Scope

- Benchmark-driven optimization of pipeline parameters
- Provider-independent scheduling frameworks beyond the current migration scope
- UI redesign of advanced tuning options
- Distributed or multi-machine execution models

## Acceptance Criteria

- Sequential execution remains available as the baseline migration mode
- Pipelined execution can prefetch and export snapshots before later commits finish
- Commit order remains deterministic even when export work is parallelized
- Pipeline tuning values are represented through shared migration options
- Progress reporting exposes meaningful pipeline state transitions
- Regression tests cover the most important ordering and diagnostics behaviors

## Related Backlog Items

- `BI-RepoMigrator-004` - `Productize pipelined migration execution`
- `BI-RepoMigrator-006` - `Expand automated test baseline for migration orchestration and tools`

## Risks

- Pipeline cleanup and temporary-directory handling may still be fragile under partial failure
- Tuning defaults may not scale across repository sizes and environments
- Provider-specific latency differences may reduce the expected pipeline benefit

## Open Questions

- Which execution mode should be the default for supported SVN-to-Git scenarios?
- Which telemetry or logs are required to troubleshoot pipeline failures reliably?
- Where should safe limits for worker counts and prefetch depth be enforced?

## Next Refinement Steps

1. Confirm the migration scenarios where pipelined execution is officially supported
2. Record operational constraints for temporary storage and cleanup
3. Add backlog items for fallback behavior and failure diagnostics
4. Revisit tuning defaults after real-world usage data becomes available
