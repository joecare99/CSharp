# AA98-Bl040 Builder Inner Loop Baseline

## Parent
- Feature: `../Features/AA98-F42-Developer-Inner-Loop.md`
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`

## Value
A developer can inspect, build, and test relevant AA98 projects from AA98 or a dedicated builder host.

## Scope
- Reuse host-neutral builder contracts and existing builder sources where applicable.
- Provide a thin builder host path for isolated validation.
- Surface build and targeted test results in a structured form.
- Avoid moving builder logic into the shell.

## Acceptance Criteria
- A builder integration path is documented and implemented through a reusable boundary.
- A thin builder host can run the first inspection/build scenario.
- Build/test results are suitable for later AI/tool consumption.

## Implementation Tasks
- `AA98-T048 Inspect Builder Integration Sources`
- `AA98-T049 Implement Builder Wrapper Contracts`
- `AA98-T050 Create Builder Micro Host`
- `AA98-T051 Add Builder Inner Loop Tests`

## Assumptions
- `Workbench.Builder.Core` remains the preferred reusable core.

## Open Questions
- Should the first execution call the existing CLI or integrate the core directly?

## Next Refinement Steps
1. Inspect builder sources before adding contracts.
2. Create tests before broad shell integration.

## Discovery Notes
- The AA98 solution already references the adjacent `Workbench.Builder` projects from `../CSharpBible/Data/Workbench.Builder`, including `Workbench.Builder.Core`, `Workbench.Builder.Core.Tests`, and `Workbench.Builder.Host`.
- `Workbench.Builder.Core.Abstractions.IProjectInspectionService` is the clearest first reusable integration boundary because it exposes structured project inspection without shell or CLI coupling.
- `Workbench.Builder.Core.Models.Inspection.ProjectInspectionResult` already carries the structured result shape needed for later AI/tool-oriented workbench workflows.
- Existing `Workbench.Builder.Analysis` and `Workbench.Builder.Host` applications consume the core directly and keep command-line parsing as an outer host concern.
- The existing host service registration is DI-friendly, which supports an AA98 wrapper layer without moving builder logic into shell projects.

## Implementation Progress
- `AA98-T048` completed the builder-source inspection and recommends that AA98 integrate builder inspection through direct wrapper contracts over `Workbench.Builder.Core` instead of parsing CLI or host output.
- `AA98-T049` defined AA98-facing builder contracts for inspection, build, diagnostics, and targeted test result shapes.
- `AA98-T050` added a thin console-oriented builder micro host plus a reusable net10.0 Workbench.Builder adapter while keeping the contract layer multi-target.
- `AA98-T051` added deterministic net10.0 tests for the Workbench-backed builder adapter and the thin builder host, including inspection/build result formatting and the current targeted-test placeholder path.
- The Workbench-backed builder adapter now forwards targeted test requests through the provider-neutral AA98 testing boundary and maps the resulting summary back into structured builder test results.

## Task Status Snapshot
- `AA98-T048 Inspect Builder Integration Sources` - Completed
- `AA98-T049 Implement Builder Wrapper Contracts` - Completed
- `AA98-T050 Create Builder Micro Host` - Completed
- `AA98-T051 Add Builder Inner Loop Tests` - Completed

## Status
- In Progress
