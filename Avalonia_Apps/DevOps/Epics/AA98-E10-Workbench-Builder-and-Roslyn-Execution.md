# AA98-E10 Workbench Builder and Roslyn Execution

## Parent
- Vision: `DevOps/Vision.md`

## Goal
Establish a host-neutral builder core for the workbench that can inspect SDK-style C# projects first and later compile them through Roslyn without binding the core to a CLI or UI shell.

## Scope
- Add a reusable `Workbench.Builder.Core` project-inspection baseline.
- Prepare the path from project inspection to Roslyn-based compilation and emit.
- Keep the builder Linux-capable and workbench-consumable.
- Separate builder-core concerns from presentation and host concerns.

## Included
- V1.1 project inspection
- Reference resolution and test-project detection
- Structured diagnostics and inspection result models
- Starter-slice validation and test data
- First formatter and thin-host integration slice
- DevOps tracking for resumed builder work

## Excluded for Now
- Full V1.2 emit and portable-PDB pipeline
- Source-generator execution
- Avalonia- or MAUI-specific build targets
- Packaging and publish workflows

## Success Indicators
- The builder core can inspect normal SDK-style console and library projects.
- The workbench can consume stable structured inspection results later without reworking the contract.
- A first thin host can emit readable plain-text and JSON inspection output.
- Starter-slice tests validate the current baseline in the repository.

## Candidate Features
- V1.1 project inspection baseline
- V1.2 Roslyn compilation and emit baseline
- Workbench-facing formatter and integration layer
- V2-ready analyzer and generator metadata support

## Status
- In Progress
