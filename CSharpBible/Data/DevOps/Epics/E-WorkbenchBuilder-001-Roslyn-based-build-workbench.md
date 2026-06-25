# Epic E-WorkbenchBuilder-001 - Roslyn-based build workbench

## Status

In Progress

## Goal

Establish a provider-neutral, OS-conscious build workbench that can inspect SDK-style `.csproj` files, resolve the data required for compilation, and later compile and emit build artifacts through a Roslyn-centered pipeline.

## Summary

This epic tracks the Workbench.Builder initiative as an incremental build-tool foundation inside the workspace. The current implementation already includes a host-neutral core project, a thin console host, structured inspection models, MSBuild-backed project loading, pragmatic reference resolution, formatter support, a first Roslyn-based compilation and emit slice, position-aware diagnostics, and regression tests. The V1.1 inspection baseline is now established far enough to support the active V1.2 compilation, emit, and debug-symbol work.

The workbench is intended to start small and observable rather than imitating the complete MSBuild engine immediately. Early slices prioritize deterministic project inspection and understandable diagnostics for standard console and library scenarios, while keeping later room for executable output, source generators, and portable debugging data.

## Expected Outcomes

- SDK-style projects can be inspected through a stable structured result model
- Compile items, project references, package references, and resolved references are exposed consistently
- A thin host can render builder output and diagnostics clearly while remaining reusable as a narrow adapter around `Workbench.Builder.Core`
- The implementation remains suitable for later Linux-friendly execution for ordinary console and library projects
- The architecture leaves explicit room for future Roslyn compilation, emit, source-generator support, and Portable PDB generation
- Current and future work is traceable through feature, backlog, and task documents under `Data/DevOps`

## In Scope

- Planning and refinement of the `Workbench.Builder` initiative
- V1.1 project loading, project inspection, reference discovery, and result formatting
- Thin host orchestration for command-line-driven inspection and early compilation output
- Regression coverage for loader, resolver, formatter, host, and inspection orchestration
- Planning and implementation for V1.2 compilation, emit, executable output, and debugging symbols
- Explicit consideration of source-generator-compatible evolution in later slices

## Out of Scope

- Complete parity with all MSBuild behaviors in the first increments
- Immediate support for every project type in the workspace
- Full UI integration into a larger workbench shell in the first slice
- Final production hardening for all multi-targeting and platform-specific workloads
- Advanced IDE-like debugging workflows beyond artifact and symbol generation planning

## Assumptions

- The first practical target remains SDK-style projects that are ordinary console or library workloads
- MSBuild-backed evaluation is acceptable as the project-data acquisition baseline before Roslyn emit is introduced
- The core must stay host-neutral so later console, UI, or automation hosts can reuse it
- Early plain-text and JSON output are sufficient for V1.1 consumers
- Source generators and Portable PDBs are important later requirements and should shape current contracts even when not fully implemented yet
- Windows-only UI technologies are not the main target for the first build slices, while Avalonia remains a plausible future front-end candidate

## Features

### Feature F-WorkbenchBuilder-01 - V1.1 project inspection and reference discovery

Planning file: [F-WorkbenchBuilder-01-V1.1-project-inspection-and-reference-discovery.md](../Features/F-WorkbenchBuilder-01-V1.1-project-inspection-and-reference-discovery.md)

- Stabilize loading of SDK-style project metadata and compile items
- Resolve project, framework, package, metadata, and analyzer references consistently enough for inspection
- Produce a reliable structured inspection result for downstream formatting and later build steps

### Feature F-WorkbenchBuilder-02 - Thin host and inspection output contracts

Planning file: [F-WorkbenchBuilder-02-thin-host-and-inspection-output-contracts.md](../Features/F-WorkbenchBuilder-02-thin-host-and-inspection-output-contracts.md)

- Keep a small command-line-driven host for inspection entry and output selection
- Support readable plain-text output and structured JSON output
- Make diagnostics and failure modes understandable for interactive and automation scenarios

### Feature F-WorkbenchBuilder-03 - V1.2 compilation, emit, and debug symbols

Planning file: [F-WorkbenchBuilder-03-V1.2-compilation-emit-and-debug-symbols.md](../Features/F-WorkbenchBuilder-03-V1.2-compilation-emit-and-debug-symbols.md)

- Define the transition from inspection-only flows to Roslyn compilation and emit
- Support executable output for non-library, non-test projects where appropriate
- Plan Portable PDB generation and source-generator-compatible build orchestration

### Feature F-WorkbenchBuilder-04 - Source-generator and analyzer-aware compilation pipeline

Planning file: [F-WorkbenchBuilder-04-source-generator-and-analyzer-aware-compilation-pipeline.md](../Features/F-WorkbenchBuilder-04-source-generator-and-analyzer-aware-compilation-pipeline.md)

- Integrate source generators and analyzers explicitly into the compilation pipeline
- Make generated-source handling and diagnostics observable for hosts and future UI consumers
- Clarify which generator-dependent scenarios belong to the first supported build tiers

### Feature F-WorkbenchBuilder-05 - Incremental build orchestration and artifact caching

Planning file: [F-WorkbenchBuilder-05-incremental-build-orchestration-and-artifact-caching.md](../Features/F-WorkbenchBuilder-05-incremental-build-orchestration-and-artifact-caching.md)

- Define deterministic incremental build boundaries and cacheable inputs or outputs
- Avoid full recompilation when inputs, references, and configuration state have not changed
- Keep cache invalidation and artifact ownership explicit instead of implicit host behavior

### Feature F-WorkbenchBuilder-06 - Builder diagnostics, observability, and workbench integration

Planning file: [F-WorkbenchBuilder-06-builder-diagnostics-observability-and-workbench-integration.md](../Features/F-WorkbenchBuilder-06-builder-diagnostics-observability-and-workbench-integration.md)

- Expand diagnostics into a more observable build pipeline with phases, timings, and categorized messages
- Prepare host and later UI integration around stable machine-readable execution data
- Clarify the transition from thin console host to broader Workbench-facing consumers

## Prioritized First Increment

The first increment focuses on completing and hardening the V1.1 inspection baseline so the builder can explain a project deterministically before attempting to compile it.

### Objective

Make `Workbench.Builder` reliable enough to load an SDK-style project, classify its shape, resolve the relevant references, and present the result through a stable host-facing output contract.

### Why this increment comes first

- It provides observable truth about project structure before adding compilation complexity
- It keeps build-pipeline decisions grounded in a stable data model
- It reduces risk for later source-generator and emit work by clarifying input contracts early
- It creates a testable baseline that can be expanded incrementally instead of replaced wholesale later

### Planned Features

- [`F-WorkbenchBuilder-01`](../Features/F-WorkbenchBuilder-01-V1.1-project-inspection-and-reference-discovery.md) - `V1.1 project inspection and reference discovery`
- [`F-WorkbenchBuilder-02`](../Features/F-WorkbenchBuilder-02-thin-host-and-inspection-output-contracts.md) - `Thin host and inspection output contracts`

## Initial Backlog Candidates

### BI-WorkbenchBuilder-001 - Complete V1.1 inspection pipeline

Parent: Epic `E-WorkbenchBuilder-001`

- Review the loader, resolver, detector, formatter, and inspection service as one coherent V1.1 pipeline
- Identify correctness gaps for target framework handling, diagnostics, and resolved reference shape
- Clarify what inspection success means before V1.2 compile and emit work starts

### BI-WorkbenchBuilder-002 - Stabilize thin host and output contracts

Parent: Epic `E-WorkbenchBuilder-001`

- Refine command-line entry behavior and output-format handling
- Keep plain-text output readable and JSON output stable enough for automation
- Record error-reporting expectations for invalid arguments and execution failures

### BI-WorkbenchBuilder-003 - Add regression coverage for builder inspection slices

Parent: Epic `E-WorkbenchBuilder-001`

- Expand tests for loading, reference resolution, formatting, host orchestration, and sample projects
- Keep coverage visible while the architecture is still changing quickly
- Reduce the chance that future emit work breaks the established V1.1 contract silently

### BI-WorkbenchBuilder-004 - Plan V1.2 compilation, emit, and Portable PDB support

Parent: Epic `E-WorkbenchBuilder-001`

- Define the first compilation target state after V1.1 inspection is stable
- Clarify executable-versus-library emit behavior and debug-symbol expectations
- Record the baseline requirements for source generators and future Linux-friendly operation

### BI-WorkbenchBuilder-005 - Plan source-generator-aware compilation support

Parent: Epic `E-WorkbenchBuilder-001`

- Define how source generators and analyzers enter the builder pipeline
- Record generated-source visibility, diagnostics, and execution constraints
- Split minimum viable generator support from later advanced scenarios

### BI-WorkbenchBuilder-006 - Define incremental build and artifact caching strategy

Parent: Epic `E-WorkbenchBuilder-001`

- Define cache keys and invalidation boundaries for compilation inputs and outputs
- Clarify ownership of intermediate artifacts, generated files, and emitted binaries
- Keep incremental behavior deterministic enough for local and automated use

### BI-WorkbenchBuilder-007 - Define builder diagnostics, observability, and execution telemetry

Parent: Epic `E-WorkbenchBuilder-001`

- Define how build phases, durations, and categorized diagnostics are exposed
- Clarify what machine-readable execution data future hosts and UI consumers require
- Keep observability orthogonal to core compilation logic where practical

## Roadmap Waves

### Wave 1 - Inspection baseline hardening

Status: Completed

Primary focus:

- Complete and stabilize V1.1 project inspection and reference discovery
- Lock down the first thin-host contract for plain-text and JSON output
- Expand regression coverage around current builder slices

Primary planning artifacts:

- `F-WorkbenchBuilder-01`
- `F-WorkbenchBuilder-02`
- `BI-WorkbenchBuilder-001`
- `BI-WorkbenchBuilder-002`
- `BI-WorkbenchBuilder-003`

### Wave 2 - First Roslyn emit baseline

Status: In Progress

Primary focus:

- Turn stable inspection output into Roslyn compilation inputs
- Emit the first supported library and executable artifacts
- Generate Portable PDBs for the supported baseline scenarios

Primary planning artifacts:

- `F-WorkbenchBuilder-03`
- `BI-WorkbenchBuilder-004`

### Wave 3 - Generator-aware and analyzer-aware builds

Primary focus:

- Introduce explicit support for source generators and analyzer-sensitive compilation
- Keep generated-source diagnostics and outputs inspectable
- Separate minimum supported generator scenarios from later advanced coverage

Primary planning artifacts:

- `F-WorkbenchBuilder-04`
- `BI-WorkbenchBuilder-005`

### Wave 4 - Incremental execution and caching

Primary focus:

- Reduce repeated work through deterministic incremental build behavior
- Define cacheable build artifacts and invalidation boundaries
- Keep runtime behavior understandable across local and automated execution contexts

Primary planning artifacts:

- `F-WorkbenchBuilder-05`
- `BI-WorkbenchBuilder-006`

### Wave 5 - Diagnostics, observability, and broader workbench integration

Primary focus:

- Expose richer build-pipeline diagnostics and execution telemetry
- Prepare stable machine-readable execution data for future UI or automation consumers
- Clarify how the thin host evolves into broader Workbench integration points

Primary planning artifacts:

- `F-WorkbenchBuilder-06`
- `BI-WorkbenchBuilder-007`

## Risks

- MSBuild evaluation and CLI-based reference discovery may behave differently across target frameworks and environments
- Multi-targeting, restore state, or platform-specific references may produce edge cases not yet covered by the current tests
- Source-generator support may require additional contracts beyond the current inspection models
- Later emit work may reveal missing metadata that was not captured during the first inspection slices
- The current host contract may need refinement if automation scenarios require more explicit error or diagnostic structure

## Open Questions

- Which project categories are explicitly in scope for the first V1.2 emit slice?
- How much of reference resolution should remain MSBuild-driven versus being derived from restore artifacts or Roslyn inputs directly?
- Which diagnostics should be treated as informational versus warning-level in V1.1 output?
- What is the exact source-generator baseline for the first compilation slice?
- Which host contract should later UI integration consume directly, and which concerns should remain host-specific?

## Next Refinement Steps

1. Reconcile the planning artifacts with the now-completed V1.1 baseline and the active V1.2 implementation state
2. Continue hardening compilation and emit behavior for the supported baseline scenarios
3. Expand regression coverage where V1.2 introduces new diagnostics, artifact, or runtime-asset behavior
4. Revisit source-generator and Portable PDB requirements before the next compilation slice broadens
6. Define the first generator-aware compilation support slice after the emit baseline is stable
7. Specify deterministic incremental-build and caching boundaries before optimization work expands
8. Define richer diagnostics and execution telemetry before broader Workbench integration starts
