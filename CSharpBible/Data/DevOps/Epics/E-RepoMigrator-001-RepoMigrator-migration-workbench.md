# Epic E-RepoMigrator-001 - RepoMigrator migration workbench

## Status

Draft

## Goal

Create the planning baseline for RepoMigrator as a reusable migration workbench for repository transfers, so the application, providers, and supporting tools can be refined incrementally and later transferred into Azure DevOps.

## Summary

This epic captures the current RepoMigrator product space around provider-based repository migration, guided configuration, and supporting migration tools. The document establishes boundaries, assumptions, and candidate delivery slices without pretending that all operational and product requirements are finalized yet.

## Expected Outcomes

- Repository migrations can be configured through shared provider abstractions instead of source-control-specific hardcoding
- Guided workflows exist for repository selection, revision or reference selection, and execution monitoring
- RepoMigrator can support both interactive and command-line-driven migration scenarios
- Migration-specific helper tools can be refined as focused delivery slices instead of unstructured side work
- The work can be split into features and backlog items that are ready for staged refinement

## In Scope

- Planning the RepoMigrator product baseline and major work streams
- Defining feature candidates across core orchestration, providers, UI workflow, and tooling
- Capturing assumptions, risks, and open questions that affect migration feasibility
- Identifying initial backlog candidates for incremental refinement
- Keeping interactive and automation-driven migration paths visible in the same planning model

## Out of Scope

- Final production hardening for every migration scenario
- Detailed implementation tasks for all candidate work items
- Provider support for every possible version-control system
- High-confidence effort estimates for the full initiative
- Operational rollout decisions such as packaging, deployment, or organization-wide adoption policy

## Assumptions

- RepoMigrator will continue to use provider abstractions so source and target systems can evolve independently
- Git and SVN remain the first practical repository types for refinement because they already shape the current solution structure
- Migration execution needs both sequential and pipelined options because performance and operational safety may differ by source system
- A guided WPF workflow remains useful even when focused command-line tools exist for specialized scenarios
- Migration helper tools such as branch splitting and pipelined execution should stay aligned with the same core migration concepts instead of diverging into separate products
- Backlog refinement will happen iteratively because supported migration scenarios, credential handling, and operational constraints are still evolving

## Features

The following implemented RepoMigrator slices already have dedicated feature files. Additional scope can remain in candidate state until implementation or refinement justifies a separate artifact.

### Feature F-RepoMigrator-01 - Core migration orchestration and provider contracts

Planning file: [F-RepoMigrator-01-Core-migration-orchestration-and-provider-contracts.md](../Features/F-RepoMigrator-01-Core-migration-orchestration-and-provider-contracts.md)

- Define and refine the shared repository endpoint, query, capability, and migration option model
- Keep migration orchestration independent from provider-specific command execution details
- Clarify extension points for future repository types and transfer modes

### Feature F-RepoMigrator-02 - Git and SVN provider support

Planning file: [F-RepoMigrator-02-Git-and-SVN-provider-support.md](../Features/F-RepoMigrator-02-Git-and-SVN-provider-support.md)

- Stabilize Git-based source and target repository interactions
- Stabilize SVN CLI-based source interactions and revision selection flows
- Capture capability differences between providers in a visible and testable way

### Feature F-RepoMigrator-03 - Guided WPF migration workflow

Planning file: [F-RepoMigrator-03-Guided-WPF-migration-workflow.md](../Features/F-RepoMigrator-03-Guided-WPF-migration-workflow.md)

- Support step-by-step setup, option selection, and execution monitoring in the desktop application
- Persist relevant user input state and restore it safely across sessions
- Keep provider-specific choices visible without leaking low-level command concerns into the UI

### Feature F-RepoMigrator-04 - Advanced migration execution modes

Planning file: [F-RepoMigrator-04-Advanced-migration-execution-modes.md](../Features/F-RepoMigrator-04-Advanced-migration-execution-modes.md)

- Support sequential and pipelined snapshot migration flows
- Make execution tuning parameters observable and safe to configure
- Record operational constraints around temporary storage, ordering, and cleanup behavior

### Feature F-RepoMigrator-05 - Target projection and branch transformation tools

Planning file: [F-RepoMigrator-05-Target-projection-and-branch-transformation-tools.md](../Features/F-RepoMigrator-05-Target-projection-and-branch-transformation-tools.md)

- Support subdirectory-based projection into target branches during migration
- Provide a focused Git branch splitting tool for post-processing and repository reshaping scenarios
- Clarify when branch transformation belongs in the main migration workflow versus a dedicated helper tool

### Feature Candidate F-RepoMigrator-06 - Diagnostics, validation, and automated test coverage

Planning status: Candidate only. No dedicated feature file has been created yet.

- Improve validation feedback for configuration and execution failures
- Keep migration progress reporting understandable across UI and command-line flows
- Expand automated coverage around provider behavior, option parsing, and orchestration edge cases

## Prioritized First Increment

The first refinement slice focuses on a stable repository migration foundation across shared contracts, initial providers, and a guided execution workflow.

### Objective

Make RepoMigrator reliable enough to configure and execute an initial set of Git and SVN migration scenarios through shared provider contracts, observable progress reporting, and a consistent interactive workflow.

### Why this increment comes first

- It creates the product baseline that all specialized tools depend on
- It reduces coupling between provider-specific behavior and the user-facing workflow
- It makes later performance, projection, and helper-tool work easier to refine against a stable core
- It exposes unsupported scenarios earlier instead of hiding them inside specialized tooling

### Planned Features

- [`F-RepoMigrator-01`](../Features/F-RepoMigrator-01-Core-migration-orchestration-and-provider-contracts.md) - `Core migration orchestration and provider contracts`
- [`F-RepoMigrator-02`](../Features/F-RepoMigrator-02-Git-and-SVN-provider-support.md) - `Git and SVN provider support`
- [`F-RepoMigrator-03`](../Features/F-RepoMigrator-03-Guided-WPF-migration-workflow.md) - `Guided WPF migration workflow`

## Initial Backlog Candidates

### BI-RepoMigrator-001 - Define supported migration matrix and capability baseline

Parent: Epic `E-RepoMigrator-001`

- List supported source and target combinations for the first planned increment
- Describe which repository capabilities are available, partial, or unsupported per provider
- Record known constraints for references, revisions, tags, and native-history transfer

### BI-RepoMigrator-002 - Stabilize provider contract for migration and selection workflows

Parent: Epic `E-RepoMigrator-001`

- Review the shared provider abstractions used for probing, selection, migration, and transfer
- Clarify mandatory versus optional provider behaviors
- Record extension points needed for future repository types without over-designing them now

### BI-RepoMigrator-003 - Harden guided WPF migration setup and execution flow

Parent: Epic `E-RepoMigrator-001`

- Review the setup, options, and execution stages as one user-visible workflow
- Identify validation gaps, confusing inputs, and state restoration expectations
- Capture how provider-specific choices should appear in the UI without adding core-service UI strings

### BI-RepoMigrator-004 - Productize pipelined migration execution

Parent: Epic `E-RepoMigrator-001`

- Define when pipelined execution is supported and when it must fall back to safer modes
- Record tuning parameters, ordering guarantees, and cleanup expectations
- Capture the minimum diagnostics needed to troubleshoot pipeline failures

### BI-RepoMigrator-005 - Define branch projection and repository reshaping scenarios

Parent: Epic `E-RepoMigrator-001`

- Describe which migration scenarios require subdirectory-to-branch projection
- Clarify the intended relationship between projection during migration and the separate Git branch split tool
- Record the constraints that affect branch naming, depth selection, and overwrite behavior

### BI-RepoMigrator-006 - Expand automated test baseline for migration orchestration and tools

Parent: Epic `E-RepoMigrator-001`

- Identify critical orchestration, provider, parsing, and projection behaviors that require regression coverage
- Keep test work visible for both interactive and command-line slices
- Prepare future feature refinement with explicit test-oriented backlog items

## Risks

- Provider behavior may differ more strongly than the shared abstractions currently suggest
- Credential handling and repository access rules may introduce workflow complexity that is not yet fully represented in planning
- Pipelined execution may create ordering, cleanup, or temporary-storage risks under failure conditions
- Branch transformation scenarios may blur the boundary between core migration and post-processing tools
- Real-world repository sizes may stress current assumptions around progress reporting and operational feedback

## Open Questions

- Which source and target repository combinations are the required MVP scenarios?
- How much native-history transfer support is expected versus snapshot-based migration?
- Which authentication mechanisms must be supported beyond basic username and password flows?
- What is the preferred boundary between the WPF application and the specialized command-line tools?
- Which diagnostics are mandatory for production troubleshooting and auditability?
- Are there packaging, distribution, or environment constraints that should influence early feature ordering?

## Next Refinement Steps

1. Confirm the first supported source and target migration matrix
2. Refine the dedicated feature files `F-RepoMigrator-01` to `F-RepoMigrator-05` and create `F-RepoMigrator-06` only if its scope becomes implementation-relevant
3. Refine `BI-RepoMigrator-001` to `BI-RepoMigrator-003` before implementation planning starts
4. Add dedicated test-oriented backlog items for the first selected delivery slice
5. Revisit open questions around credentials, diagnostics, and tool boundaries after the first refinement pass
