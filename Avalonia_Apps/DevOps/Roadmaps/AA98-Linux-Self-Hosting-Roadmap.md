# AA98 Linux Self-Hosting Roadmap

## Purpose

This roadmap describes how AA98 should evolve into a Linux-first self-hosting development environment with Copilot-assisted workflows, reusable component boundaries, and thin host applications for isolated development and validation.

## Related Planning Items

- Vision: `../Vision.md`
- Component catalog: `../AA98_WorkbenchComponentCatalog.Info.md`
- Component extension model: `../Epics/AA98-E03-Component-Extension-Model.md`
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`
- Feature: `../Features/AA98-F39-Component-Micro-Hosts.md`
- Feature: `../Features/AA98-F40-Copilot-Assisted-Workflow.md`
- Execution sequence: `./AA98-Linux-Self-Hosting-Execution-Sequence.md`
- Feature: `../Features/AA98-F41-Linux-Workbench-Base.md`
- Feature: `../Features/AA98-F42-Developer-Inner-Loop.md`
- Feature: `../Features/AA98-F43-Repository-and-Planning-Workflows.md`
- Feature: `../Features/AA98-F44-External-DevOps-and-Repository-Providers.md`

## Target Outcome

AA98 should become usable on Linux as a daily driver for its own development in incremental stages.

The target state is not a monolithic IDE rewrite. Instead, AA98 should grow as a set of reusable components with:

- a Linux-first Avalonia workbench shell,
- editor, terminal, builder, repository, AI, settings, and DevOps planning components,
- machine-invokable commands that can also be surfaced as UI actions,
- thin component host apps for isolated development and exploratory testing,
- provider-neutral integration boundaries for Copilot-like services, Azure DevOps, GitHub, and local tools.

## Minimal Self-Hosting Definition

AA98 is considered minimally self-hosting on Linux when all of the following are true:

1. The shell starts on Linux without platform-specific manual patching.
2. A developer can open the AA98 workspace, inspect files, and edit core source files.
3. A developer can run builds and targeted tests from within AA98.
4. Basic repository workflows are available for local Git-based work.
5. AI-assisted workflows can operate through explicit consent and tool-safe command boundaries.
6. At least the most important AA98 components can be developed and validated independently through dedicated host apps.

## Architecture Direction

### Layering

- `BaseLib` remains the place for low-level reusable abstractions.
- `Avln_BaseLib` remains the place for Avalonia- and MVVM-oriented reusable UI foundations.
- `AppKomponentBaseLib` remains the place for reusable application-component contracts, shared context objects, and later tool-oriented command metadata when it is no longer AA98-specific.
- `AA98_AvlnCodeStudio.Base` remains the AA98-specific workbench contract layer.
- UI implementations and executable hosts stay outside the contract/core layers.

### Component Host Strategy

Each major workbench component should have a small dedicated host application for separate development, testing, and demonstrations.

Candidate host apps:

- `AA98.Shell.Host`
- `AA98.Editor.Host`
- `AA98.Terminal.Host`
- `AA98.Builder.Host`
- `AA98.AI.Host`
- `AA98.DevOpsPlanning.Host`
- `AA98.Repository.Host`
- `AA98.Settings.Host`
- `AA98.ImageEditor.Host`
- `AA98.ResourceEditor.Host`

These hosts should stay thin. They are not forks of the workbench. They exist to exercise one component area at a time with minimal composition overhead.

### AI and Copilot Support

Copilot support should be treated as one provider path inside a provider-neutral AI architecture.

The command and workflow model should support:

- machine-readable command metadata,
- explicit parameter and result descriptions,
- safety and consent boundaries,
- context-sharing policies,
- local and remote provider adapters,
- later interoperability with tool-oriented protocols such as MCP where useful.

## Roadmap Phases

The concrete task execution order is maintained in `./AA98-Linux-Self-Hosting-Execution-Sequence.md`.

## Phase 1 - Workbench Base on Linux

Focus:

- Make the shell, composition baseline, and editor framework reliable on Linux.
- Keep startup, file opening, basic editing, and save workflows stable.
- Verify that the workbench can host commands, settings, and component contributions without Windows-only assumptions.

Expected outputs:

- Linux startup baseline for AA98 shell.
- Stable editor and shell integration for core source formats.
- Initial workspace open/close and file tree support.

Primary planning items:

- Feature: `../Features/AA98-F41-Linux-Workbench-Base.md`
- Backlog Item: `../BacklogItems/AA98-Bl038-Linux-Shell-Startup-Baseline.md`
- Backlog Item: `../BacklogItems/AA98-Bl039-Linux-Editor-and-Workspace-Baseline.md`

## Phase 2 - Developer Inner Loop

Focus:

- Add builder, terminal, and targeted test execution workflows suitable for developing AA98 itself.
- Keep these capabilities reusable outside the shell through dedicated thin hosts.

Expected outputs:

- Builder integration that can inspect and build AA98-relevant projects.
- Terminal integration for development sessions.
- Thin host apps for shell, editor, terminal, and builder components.

Primary planning items:

- Feature: `../Features/AA98-F42-Developer-Inner-Loop.md`
- Backlog Item: `../BacklogItems/AA98-Bl040-Builder-Inner-Loop-Baseline.md`
- Backlog Item: `../BacklogItems/AA98-Bl041-Terminal-Inner-Loop-Baseline.md`
- Backlog Item: `../BacklogItems/AA98-Bl048-Component-Micro-Host-Foundation.md`

## Phase 3 - Repository and Planning Workflows

Focus:

- Introduce repository workflows and a DevOps planning component as first-class workbench areas.
- Start with local planning artifacts and local Git-oriented repository workflows.

Expected outputs:

- DevOps planning component for `Epic -> Feature -> Backlog Item -> Task` work.
- Repository component for local Git workflows and repository context.
- Thin host apps for planning and repository areas.

Primary planning items:

- Feature: `../Features/AA98-F43-Repository-and-Planning-Workflows.md`
- Backlog Item: `../BacklogItems/AA98-Bl042-Local-Repository-Workflow-Baseline.md`
- Backlog Item: `../BacklogItems/AA98-Bl043-DevOps-Planning-Local-Model-Baseline.md`
- Backlog Item: `../BacklogItems/AA98-Bl044-DevOps-Planning-UI-Baseline.md`

## Phase 4 - Copilot-Assisted Self-Hosting

Focus:

- Make AI-assisted workflows practical for daily AA98 development under explicit consent and traceable boundaries.
- Commands should become tool-usable by AI components in addition to UI-invokable by users.

Expected outputs:

- AI provider abstraction suitable for Copilot-like services and local models.
- Context-aware, tool-friendly commands.
- User-visible consent and disclosure model integration.

Primary planning items:

- Feature: `../Features/AA98-F40-Copilot-Assisted-Workflow.md`
- Backlog Item: `../BacklogItems/AA98-Bl049-Tool-Capable-Command-Contracts.md`
- Backlog Item: `../BacklogItems/AA98-Bl050-First-Copilot-Assisted-Self-Hosting-Workflow.md`

## Phase 5 - External DevOps and Repository Providers

Focus:

- Extend the local planning and repository model with external provider adapters.
- Keep Azure DevOps and GitHub outside the neutral planning model until the local component is stable.

Expected outputs:

- Provider-neutral planning contracts.
- Azure DevOps planning adapter.
- GitHub issues/projects adapter where useful.
- Repository workflow bridges that do not leak provider-specific assumptions into the core model.

Primary planning items:

- Feature: `../Features/AA98-F44-External-DevOps-and-Repository-Providers.md`
- Backlog Item: `../BacklogItems/AA98-Bl045-Provider-Neutral-Planning-Adapter-Contracts.md`
- Backlog Item: `../BacklogItems/AA98-Bl046-Azure-DevOps-Planning-Adapter-Baseline.md`
- Backlog Item: `../BacklogItems/AA98-Bl047-GitHub-Planning-Adapter-Baseline.md`

## Planning Component Direction

The DevOps planning component should start as a local workbench component that understands the repository's own markdown planning structure.

Initial scope:

- browse and edit planning items,
- navigate hierarchy and cross-links,
- validate planning conventions,
- support refinement from epic to implementation-oriented items.

Later extensions:

- Azure DevOps synchronization or import/export,
- GitHub issue/project synchronization or import/export,
- cross-repository planning support,
- workflow automation through AI/tool-capable commands.

## Assumptions

- Linux is the primary validation target for self-hosting readiness.
- Avalonia remains the UI platform for the AA98 shell and thin hosts.
- Self-hosting should be achieved incrementally, not through a single large migration.
- Existing reusable sources such as builder, terminal, and repository tooling should be wrapped or adapted instead of copied.
- Runtime data and provider data should be stored in runtime-defined or provider-owned locations, not in the `DevOps` planning directory.

## Risks

- Linux-specific UI, file-system, terminal, or process behavior may expose hidden Windows assumptions.
- Copilot-like integrations may pressure the architecture toward provider-specific shortcuts.
- Component host apps can drift into parallel applications if their boundaries are not kept thin.
- Planning synchronization with Azure DevOps or GitHub can prematurely distort the local planning model.
- Credential and token handling can compromise portability if not abstracted early.

## Open Questions

- Which component host apps are mandatory before AA98 can be called minimally self-hosting?
- Should the first Linux self-hosting milestone include repository commit workflows or only repository inspection?
- Which external provider should be integrated first after the local planning component stabilizes: Azure DevOps or GitHub?
- Which command metadata belongs in shared contracts versus AA98-specific workbench contracts?
- What is the minimum Copilot-assisted workflow that provides visible user value without overcommitting the first self-hosting milestone?

## Success Indicators

- AA98 can start and be used on Linux for its own incremental development.
- Core component areas can be validated through dedicated thin host apps.
- Planning, repository, builder, terminal, and AI workflows can evolve independently without collapsing boundaries.
- Copilot-assisted workflows are possible through explicit, consent-aware, tool-capable command contracts.
- External provider integrations remain adapters instead of reshaping the core architecture prematurely.

## Next Refinement Steps

1. Refine the Linux self-hosting epic into feature-level slices.
2. Refine the DevOps planning component into thin backlog items before implementation starts.
3. Decide the minimum set of thin host apps required for the first self-hosting milestone.
4. Define the neutral versus AA98-specific boundary for tool-oriented command metadata.
5. Add dedicated test tasks once implementation planning begins for each major coding slice.
