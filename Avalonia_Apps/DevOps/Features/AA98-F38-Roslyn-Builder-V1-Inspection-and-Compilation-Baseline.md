# AA98-F38 Roslyn Builder V1 Inspection and Compilation Baseline

## Parent
- Epic: `DevOps/Epics/AA98-E10-Workbench-Builder-and-Roslyn-Execution.md`
- Vision: `DevOps/Vision.md`

## Goal
Deliver the first builder baseline in two slices: `V1.1` for SDK-style project inspection and `V1.2` for later Roslyn compilation and emit.

## Scope
- Stabilize `V1.1` models, loading, reference resolution, and inspection orchestration.
- Add pragmatic reference resolution for restored SDK-style projects.
- Add test data and MSTest coverage for the first inspection slice.
- Keep the public result shape suitable for later workbench integration.

## Included
- `Workbench.Builder.Core`
- `Workbench.Builder.Core.Tests`
- A first thin builder host for inspection output
- Stable inspection models and contracts
- Starter-slice task resumption after the interrupted agent run

## Excluded for Now
- `V1.2` emit implementation
- Multi-target inspection orchestration beyond the selected target framework
- Source-generator execution

## Success Indicators
- The builder core loads and inspects SDK-style sample projects.
- Reference resolution returns usable structured references for the starter slice.
- The starter-slice tests run green in the chosen validation host.

## Candidate Backlog Items
- Resume and complete the `V1.1` starter slice
- Add `V1.1` formatter output for host consumption
- Add the first thin inspection host
- Prepare `V1.2` compilation and emit contracts

## Assumptions
- The workbench will consume structured builder results directly instead of console-formatted output.
- `V1.1` can stay pragmatic if diagnostics remain visible and the contracts stay stable.

## Open Questions
- Should later host-facing reporting add Markdown in addition to plain-text and JSON?
- Which `V1.2` emit seams should be created now versus delayed until Roslyn compilation starts?

## Status
- In Progress
