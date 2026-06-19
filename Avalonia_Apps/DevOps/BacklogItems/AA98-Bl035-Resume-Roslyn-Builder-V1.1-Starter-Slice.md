# AA98-Bl035 Resume Roslyn Builder V1.1 Starter Slice

## Parent
- Feature: `DevOps/Features/AA98-F38-Roslyn-Builder-V1-Inspection-and-Compilation-Baseline.md`
- Epic: `DevOps/Epics/AA98-E10-Workbench-Builder-and-Roslyn-Execution.md`
- Vision: `DevOps/Vision.md`

## Scope
Resume the interrupted `Workbench.Builder.Core` starter slice at the point where the loading models and contracts already existed, but reference resolution, orchestration, tests, and DevOps tracking were still incomplete or inconsistent.

## Goals
- Repair the interrupted loader/model baseline.
- Complete the missing `V1.1` reference resolution and inspection orchestration.
- Add representative SDK-style test data and MSTest coverage.
- Record the resumed execution path in repository-local DevOps artifacts.

## Assumptions
- `V1.1` remains the active slice; `V1.2` is still deferred.
- The first validation host can be the repository test project instead of a dedicated builder console app.
- The production core should stay multi-targeted even if the starter-slice test host is pinned to `net10.0` for stable validation.

## Open Questions
- Should reference resolution later move back from `dotnet msbuild -getItem` to a fully in-process MSBuild strategy?
- When should the first formatter/backing host be added after the structured inspection result is stable?

## Status
- In Progress
