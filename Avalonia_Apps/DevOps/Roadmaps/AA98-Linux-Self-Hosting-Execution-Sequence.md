# AA98 Linux Self-Hosting Execution Sequence

## Purpose

This document orders the planned AA98 self-hosting tasks so an implementation agent can work through them step by step toward the Linux self-hosting target.

## Execution Rules for Agents

- Work on one task at a time.
- Read the parent backlog item and feature before implementing a task.
- Prefer small, validated changes over broad rewrites.
- Keep provider-neutral contracts separate from Azure DevOps, GitHub, Copilot, or local-provider adapters.
- Keep executable micro hosts thin; core logic belongs in reusable components or contracts.
- Add or update tests for every implementation task that changes code.
- Do not store runtime or provider-owned state in the `DevOps` planning directory.
- Use explicit C# `using` directives; do not introduce global or implicit usings.

## Phase 1 - Linux Workbench Base

### Shell Startup

1. `../Tasks/AA98-T042-Inspect-Linux-Shell-Startup-Path.md`
2. `../Tasks/AA98-T043-Implement-Linux-Shell-Startup-Fixes.md`
3. `../Tasks/AA98-T044-Add-Linux-Shell-Startup-Validation.md`

### Editor and Workspace

4. `../Tasks/AA98-T045-Inspect-Linux-Workspace-and-Editor-Path-Handling.md`
5. `../Tasks/AA98-T046-Implement-Linux-Editor-and-Workspace-Fixes.md`
6. `../Tasks/AA98-T047-Add-Editor-and-Workspace-Validation.md`

## Phase 2 - Developer Inner Loop

### Builder

7. `../Tasks/AA98-T048-Inspect-Builder-Integration-Sources.md`
8. `../Tasks/AA98-T049-Implement-Builder-Wrapper-Contracts.md`
9. `../Tasks/AA98-T050-Create-Builder-Micro-Host.md`
10. `../Tasks/AA98-T051-Add-Builder-Inner-Loop-Tests.md`

### Terminal

11. `../Tasks/AA98-T052-Inspect-Terminal-Integration-Sources.md`
12. `../Tasks/AA98-T053-Implement-Terminal-Component-Wrapper.md`
13. `../Tasks/AA98-T054-Create-Terminal-Micro-Host.md`
14. `../Tasks/AA98-T055-Add-Terminal-Inner-Loop-Tests.md`

## Phase 3 - Repository and Planning Workflows

### Local Repository

15. `../Tasks/AA98-T056-Define-Local-Repository-Contracts.md`
16. `../Tasks/AA98-T057-Implement-Local-Repository-Inspection.md`
17. `../Tasks/AA98-T058-Add-Repository-Workflow-Tests.md`

### Local DevOps Planning Model

18. `../Tasks/AA98-T059-Define-Local-Planning-Model-Contracts.md`
19. `../Tasks/AA98-T060-Implement-Markdown-Planning-Reader.md`
20. `../Tasks/AA98-T061-Add-Planning-Model-Validation-Tests.md`

### DevOps Planning UI

21. `../Tasks/AA98-T062-Implement-Planning-Explorer-ViewModel.md`
22. `../Tasks/AA98-T063-Create-DevOps-Planning-Micro-Host.md`
23. `../Tasks/AA98-T064-Add-Planning-UI-Tests.md`

## Phase 4 - Provider-Neutral External Planning Foundation

24. `../Tasks/AA98-T065-Define-Provider-Neutral-Planning-Adapter-Contracts.md`
25. `../Tasks/AA98-T066-Add-Planning-Adapter-Contract-Tests.md`

## Phase 5 - Azure DevOps and GitHub Provider Skeletons

### Azure DevOps

26. `../Tasks/AA98-T067-Refine-Azure-DevOps-Field-Mapping.md`
27. `../Tasks/AA98-T068-Implement-Azure-DevOps-Adapter-Skeleton.md`
28. `../Tasks/AA98-T069-Add-Azure-DevOps-Adapter-Contract-Tests.md`

### GitHub

29. `../Tasks/AA98-T070-Refine-GitHub-Planning-Mapping.md`
30. `../Tasks/AA98-T071-Implement-GitHub-Adapter-Skeleton.md`
31. `../Tasks/AA98-T072-Add-GitHub-Adapter-Contract-Tests.md`

## Phase 6 - Micro-Host Foundation

32. `../Tasks/AA98-T073-Define-Micro-Host-Project-Pattern.md`
33. `../Tasks/AA98-T074-Create-Shell-and-Editor-Micro-Hosts.md`
34. `../Tasks/AA98-T075-Add-Micro-Host-Smoke-Tests.md`

## Phase 7 - Copilot-Assisted Tool Workflows

### Tool-Capable Commands

35. `../Tasks/AA98-T076-Define-Tool-Capable-Command-Metadata.md`
36. `../Tasks/AA98-T077-Implement-Tool-Command-Descriptor-Contracts.md`
37. `../Tasks/AA98-T078-Add-Tool-Command-Contract-Tests.md`

### First AI-Assisted Workflow

38. `../Tasks/AA98-T079-Select-First-Copilot-Assisted-Workflow.md`
39. `../Tasks/AA98-T080-Implement-First-AI-Tool-Workflow-Skeleton.md`
40. `../Tasks/AA98-T081-Add-AI-Workflow-Consent-and-Boundary-Tests.md`

## Milestone Checks

### Minimal Linux Workbench Check

Complete after tasks 1-6:

- AA98 shell startup path is Linux-ready or blockers are explicit.
- Workspace and editor flows are Linux-portable for self-hosting source files.

### Developer Inner Loop Check

Complete after tasks 7-14:

- Builder and terminal workflows have isolated validation paths.
- Micro-host candidates are ready for shell integration.

### Local Self-Hosting Workflow Check

Complete after tasks 15-23:

- Repository context and local planning workflows are available without external providers.
- Planning model and UI can operate over repository markdown artifacts.

### Provider Adapter Readiness Check

Complete after tasks 24-31:

- Provider-neutral adapter contracts exist.
- Azure DevOps and GitHub skeletons are isolated behind adapters.

### Tool-Assisted Self-Hosting Check

Complete after tasks 32-40:

- Micro-host pattern is established.
- Commands are tool-capable.
- First AI-assisted workflow skeleton respects consent and context boundaries.

## Notes

The sequence is intentionally conservative. Provider skeleton work may be postponed if the project chooses to reach minimal Linux self-hosting before external provider preparation.
