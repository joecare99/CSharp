# Backlog Item BI-RepoMigrator-010 - Plan driver-based archive extraction and metadata inspection

## Status

Draft

## Parent

- Epic `E-RepoMigrator-001` - `RepoMigrator migration workbench`
- Feature `F-RepoMigrator-07` - `Archive and web snapshot source ingestion`

## Goal

Plan a driver-oriented archive handling model that can enumerate, inspect, and extract supported archive formats without format-specific orchestration code leakage.

## Description

Archive-backed migration needs format-agnostic orchestration with format-specific drivers underneath. The initial archive set should cover `.zip`, `.7z`, `.tar`, and `.tar.gz`, while leaving room for later expansion. The design should define which metadata a driver exposes, how extraction is requested, and how unsupported or partially supported formats fail safely.

Besides extraction, the drivers must expose metadata needed for ordering and diagnostics. That includes archive format identity, archive entry listing, candidate timestamp signals, and any confidence information that helps explain why a snapshot received a specific inferred date.

The reviewed direction should also distinguish archive-format drivers from the later general patch-driver direction. Archive drivers focus on archive inspection and extraction, while the patch driver is a separate format-oriented component that starts with read-only `.patch` ingestion and may later grow optional output support.

## Scope

- Define an archive-driver abstraction for capability detection, metadata inspection, and extraction
- Define how archive drivers integrate beneath `IMigrationSourceProvider` implementations instead of leaking into orchestration directly
- Describe how `.zip`, `.7z`, `.tar`, and `.tar.gz` plug into the shared abstraction
- Define metadata output needed for snapshot ordering and diagnostics
- Identify external-tool versus library dependency questions per format
- Specify failure handling for unsupported, corrupted, or partially readable archives
- Clarify how extraction should materialize a full snapshot for downstream commit processing
- Clarify the boundary between archive drivers and the separate read-first patch-driver direction

## Acceptance Criteria

- The archive handling design is driver-oriented rather than a single hardcoded format switch in orchestration code
- The first supported formats are explicitly recorded as `.zip`, `.7z`, `.tar`, and `.tar.gz`
- Driver metadata responsibilities include timestamp-oriented inspection support
- Failure behavior is planned for unsupported and damaged archives
- Driver integration responsibilities within source providers are documented clearly
- The design clearly separates archive-driver responsibilities from the later read-first patch-driver responsibilities
- The design keeps later format additions possible without rewriting migration orchestration

## Assumptions

- Full-snapshot materialization remains the baseline behavior for archive-derived commits
- Not every archive format will expose identical metadata quality, so confidence or fallback signaling is necessary
- Extraction should integrate into the existing temp-directory-based snapshot flow where practical

## Risks

- `.7z` support may require different dependencies or tool availability assumptions than `.zip` and `.tar`
- Archive libraries may vary across target frameworks in the solution
- A too-thin driver abstraction may hide important metadata differences that matter for ordering

## Open Questions

- Should `.7z` and `.tar.gz` support depend on external tools in the first iteration, or is a managed-library baseline required?
- Which timestamp fields should drivers expose separately versus pre-normalizing into one suggested snapshot date?
- Should driver diagnostics include a confidence level for inferred archive dates?

## Next Refinement Steps

1. Inventory viable library or tool options per archive format against the solution target frameworks
2. Draft the archive-driver contract with inspection and extraction methods plus source-provider integration points
3. Separate first-slice `.zip` implementation planning from later `.7z`, `.tar`, and `.tar.gz` follow-up slices
4. Add implementation and test tasks once dependency constraints are understood

## Planned Implementation Tasks

- `T-RepoMigrator-026` - `Implement source and destination provider abstractions`
- `T-RepoMigrator-029` - `Implement zip archive driver`
- `T-RepoMigrator-034` - `Test archive import first slice`
