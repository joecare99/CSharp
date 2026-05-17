# Task T-RepoMigrator-022 - Define test strategy for archive import planning and resume

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-006` - `Expand automated test baseline for migration orchestration and tools`
- Backlog Item `BI-RepoMigrator-012` - `Define DevOps-backed archive import status persistence and resume`

## Goal

Define the automated test strategy for archive planning, naming, ordering, persistence, and resume behavior.

## Scope

- Identify unit-test coverage for source discrimination, ordering, and naming rules
- Identify persistence tests for manifest read/write and schema evolution handling
- Identify orchestration tests for interruption and resume scenarios
- Define test seams needed for HTTP acquisition, archive inspection, and target-commit operations
- Record which behaviors require integration-style tests versus isolated unit tests
- Identify test coverage needs for symmetric source-provider and destination-provider abstractions

## Deliverables

- A proposed test matrix for archive-import behavior
- Identified missing seams in the current codebase
- Follow-up implementation tasks for the highest-risk coverage gaps
- A prioritized list of first-slice tests versus deferred coverage for later archive formats and destination types

## Open Questions

- Which resume scenarios need deterministic integration tests first?
- How should sample archives and persisted-state fixtures be stored for test readability?

## Detailed Work Packages

1. Unit-test inventory
   - identify tests for source normalization, ordering precedence, naming rules, and manifest serialization
   - identify tests for source-provider and destination-provider factory selection
   - identify tests for archive-driver registry behavior and compound-extension handling
2. Resume-test inventory
   - identify deterministic tests for interruption after commit, after tag, and after branch creation
   - identify manifest roundtrip tests across plan and state files
   - identify target-divergence and unsafe-resume stop-condition tests
3. Integration-test boundary definition
   - separate isolated planner/state-store tests from end-to-end archive-to-target flow tests
   - identify which scenarios require a real Git-backed destination versus substitutes
   - identify fixture and temp-directory requirements for portable tests
4. Prioritization and seams
   - define first-slice mandatory coverage for local-directory plus `.zip`
   - list deferred coverage for HTTP sources, additional formats, and sequential archive destinations
   - identify missing seams in providers and orchestration that must be added before testing

## Acceptance Criteria

- The test strategy covers source providers, destination providers, planning, persistence, and resume behavior
- First-slice mandatory tests are distinguished from later-slice coverage clearly
- Required test seams are identified explicitly
- The strategy remains compatible with the existing MSTest and NSubstitute baseline
