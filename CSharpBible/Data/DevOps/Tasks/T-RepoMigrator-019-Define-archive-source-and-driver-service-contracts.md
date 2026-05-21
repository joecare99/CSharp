# Task T-RepoMigrator-019 - Define archive source and driver service contracts

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-009` - `Define archive snapshot source detection and ordering contracts`
- Backlog Item `BI-RepoMigrator-010` - `Plan driver-based archive extraction and metadata inspection`

## Goal

Define the service interfaces for archive discovery, archive-driver resolution, inspection, download, and extraction.

## Scope

- Define `IMigrationSourceProvider` responsibilities for repository-backed and archive-backed sources
- Define archive-source discovery contracts for HTTP(S) and local directory sources
- Define driver contracts for inspection and extraction
- Define registry or selector responsibilities for supported archive formats
- Record where download and local-cache behavior belong in the service model
- Identify interactions with destination providers and the existing repository-target provider contracts

## Deliverables

- Proposed service-interface list and responsibilities
- Notes about dependency boundaries between core and provider implementations
- Identified impact on dependency injection and orchestration setup
- A staged service map for first-slice local-directory support and later HTTP-source expansion

## Open Questions

- Should HTTP download behavior belong to the source abstraction or to a shared archive-content acquisition service?
- Should inspection and extraction share one driver contract or be split into specialized roles?

## Detailed Work Packages

1. Source-provider contract draft
   - define the minimum responsibilities for `IMigrationSourceProvider`
   - describe how repository-backed and archive-backed source providers coexist
   - define selection and failure behavior in `IMigrationSourceProviderFactory`
2. Archive discovery contract draft
   - define local-directory discovery behavior for the first slice
   - define the later HTTP-index discovery seam without forcing implementation now
   - define normalized discovered-item metadata
3. Driver contract draft
   - define inspection, extraction, and metadata contracts
   - define registry selection behavior and unsupported-format failures
   - define how compound extensions are handled during driver matching
4. Service-boundary mapping
   - map source providers to archive planners and destination providers
   - define DI registration responsibilities and extension points
   - identify test seams for discovery, driver selection, and metadata inspection

## Acceptance Criteria

- The task documents `IMigrationSourceProvider` and archive-specific service boundaries clearly
- The contract supports first-slice local-directory discovery and later HTTP expansion
- Driver selection and failure behavior are specified clearly
- Interactions with destination providers and target repository providers are identified
