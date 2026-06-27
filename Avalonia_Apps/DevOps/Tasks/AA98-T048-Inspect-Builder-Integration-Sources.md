# AA98-T048 Inspect Builder Integration Sources

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl040-Builder-Inner-Loop-Baseline.md`

## Goal
Inspect existing builder sources and decide the first AA98 integration boundary.

## Scope
- Review `Workbench.Builder.Core`, analysis, host, and CLI sources where available.
- Identify reusable contracts and result models.
- Decide whether the first slice should call an existing CLI or integrate core services directly.

## Execution Notes
1. Compare current builder architecture with AA98 layering rules.
2. Record integration options and recommended first slice.
3. Do not move builder logic into AA98 shell projects.

## Acceptance Criteria
- First builder integration approach is documented.
- `AA98-T049` has clear contract targets.

## Validation
- Inspection only; no code change required.

## Findings
- `AA98_AvlnCodeStudio.slnx` already includes `Workbench.Builder.Core`, `Workbench.Builder.Core.Tests`, and `Workbench.Builder.Host` from the linked `../CSharpBible/Data/Workbench.Builder` workspace slice. The builder is therefore available to the AA98 solution as an adjacent implementation source instead of an AA98-local reimplementation.
- `Workbench.Builder.Core/Abstractions/IProjectInspectionService.cs` provides the clearest first reusable boundary for AA98 because it exposes host-neutral project inspection through a single structured service contract.
- `Workbench.Builder.Core/Models/Inspection/ProjectInspectionResult.cs` already carries structured project, compile item, project reference, package reference, resolved reference, diagnostic, and test-project information that AA98 later needs for tool-capable workflows.
- `Workbench.Builder.Core/Services/Inspection/ProjectInspectionService.cs` confirms that the current V1.1 slice already composes project loading, test-project detection, and reference resolution without shell dependencies.
- `Workbench.Builder.Analysis/AnalysisApplication.cs` and `Workbench.Builder.Host/HostApplication.cs` show that existing thin hosts consume the core directly and use CLI parsing only as an outer application concern.
- `Workbench.Builder.Host/ServiceRegistration.cs` demonstrates that current host composition is DI-friendly and keeps builder orchestration outside UI projects.

## Integration Options
1. Call an existing CLI or host executable and parse its text or JSON output.
2. Wrap `Workbench.Builder.Core` directly through AA98-facing contracts and translate structured core results into AA98 result models.

## Recommended First Slice
- Prefer direct integration of `Workbench.Builder.Core` through an AA98 wrapper boundary.
- Keep CLI or host execution as a later micro-host and troubleshooting path, not as the primary AA98 shell integration seam.
- Let `AA98-T049` target wrapper contracts around project inspection first, using `IProjectInspectionService` and `ProjectInspectionResult` as the upstream source boundary.
- Defer direct compilation-host wrapping until the first inspection wrapper is stable, because the current developer-inner-loop acceptance starts with inspect/build/test capability but does not require the shell to parse host console output.

## Status
- Completed
