# AA98-F42 Developer Inner Loop

## Parent
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`
- Roadmap: `../Roadmaps/AA98-Linux-Self-Hosting-Roadmap.md`

## Goal
Provide the minimum builder, terminal, and targeted test workflows required to develop AA98 from within AA98 on Linux.

## Scope
- Integrate project inspection and build orchestration through host-neutral builder contracts.
- Provide terminal session access suitable for developer workflows.
- Support targeted test execution or test command handoff for self-hosting work.
- Keep builder and terminal capabilities reusable through micro hosts.

## User or Process Value
- A developer can inspect, build, and test relevant AA98 projects from the workbench.
- Terminal and builder work can be validated independently before full shell integration.
- The inner loop becomes explicit enough for AI-assisted workflow planning.

## Candidate Backlog Items
- `AA98-Bl040 Builder Inner Loop Baseline`
- `AA98-Bl041 Terminal Inner Loop Baseline`

## Assumptions
- Existing builder architecture remains host-neutral and is integrated through wrapper contracts.
- Existing terminal assets should be adapted rather than reimplemented.
- Targeted tests are more important than full solution validation in the early self-hosting loop.

## Open Questions
- Should the first builder workflow run through an existing CLI host or through a direct core integration?
- What is the minimum terminal feature set for self-hosting work?
- How should build/test results be surfaced to AI/tool workflows later?

## Next Refinement Steps
1. Define the first builder integration task and paired test task.
2. Define the first terminal integration task and paired test task.
3. Align result models with later tool-capable command metadata.

## Progress Notes
- The first builder inspection task is complete: AA98 already has solution-level access to the adjacent `Workbench.Builder` projects, and the preferred first integration seam is the host-neutral `Workbench.Builder.Core` inspection service rather than CLI output parsing.
- AA98 now has builder wrapper contracts and a dedicated `AA98.Builder.Host` console micro host backed by a reusable Workbench.Builder adapter for isolated inspection/build validation.
- The builder baseline now also includes deterministic net10.0 tests for the Workbench-backed adapter and the thin builder host, so structured inspection/build output is covered before broader workbench integration.
- Targeted builder test requests now flow through the provider-neutral AA98 testing abstraction instead of returning a placeholder builder result, keeping the builder boundary reusable while deferring runner-specific execution details.
- Terminal source inspection is now complete: the preferred first AA98 seam is a reusable stream-based terminal session wrapper with explicit shell-resolution abstractions, while the existing Avalonia console hosting pattern remains a separate UI adapter and PTY-specific behavior is deferred.
- The first terminal baseline is now implemented and covered by deterministic wrapper/configuration tests in both the shared AA98 test suite and the dedicated terminal-host test project, with only interactive shell smoke behavior left as manual validation.

## Status
- In Progress
