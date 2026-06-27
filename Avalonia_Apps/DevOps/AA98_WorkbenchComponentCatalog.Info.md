# AA98 Workbench Component Catalog

## Purpose

This document defines the intended AA98 workbench component areas and maps them to already existing implementations where they are available in this or sibling repositories. It serves as the planning bridge between the AA98 extension model and reusable implementation sources.

## Principles

- `AA98_AvlnCodeStudio.Base` owns workbench-specific contracts and adapters that remain specific to the AA98 host.
- Reusable application-component contracts should move to shared `Libraries` layers once they are no longer AA98-specific only.
- UI-specific implementations stay outside the core and are adapted through dedicated host layers.
- Thin executable host applications should be planned per major component area so isolated Linux validation and focused development remain possible.
- Existing sibling-repository implementations should be reused where their boundaries already fit the target architecture.
- Low-level cross-domain abstractions that are not workbench-specific may later move to `Libraries/BaseLib`.

## Shared Library Layering

The reusable foundation should be layered explicitly so that AA98 can act as a reference application while shared infrastructure remains available for other applications.

### BaseLib

`Libraries/BaseLib` is the candidate home for generic low-level abstractions that are not workbench-specific and not Avalonia-specific.

- File system abstractions such as `IFile` already fit this role.
- Future low-level contracts such as `IMessaging` should be evaluated here first when they are broadly reusable.

### Avln_BaseLib

`Libraries/Avln_BaseLib` is the candidate home for Avalonia, MVVM, and UI-adjacent base types that are still reusable across multiple Avalonia applications.

- Avalonia-facing helpers, base view models, and UI infrastructure belong here more naturally than in `BaseLib`.
- Workbench-specific shell or component contracts should still stay outside this library.

### AppKomponentBaseLib

A future `AppKomponentBaseLib` inside `Libraries` is a plausible layer for reusable application-component concerns that are broader than AA98 but higher-level than `BaseLib`.

- Candidate responsibilities include component descriptors, shared shell/component coordination patterns, preference contribution contracts, menu/toolbar/popup contribution concepts, and docking participation contracts when those concepts are no longer AA98-specific only.
- AA98 can then remain the first reference host and workbench implementation on top of this layer.

### Current Slice Decision

The next shared-foundation slice should establish `Libraries/AppKomponentBaseLib` and place the first neutral application-component contracts there.

- Component identity and component descriptor contracts belong in `AppKomponentBaseLib` when they do not encode AA98-only shell assumptions.
- Configuration and preference contribution contracts belong in `AppKomponentBaseLib` when they are reusable across multiple component-based applications.
- Neutral context object and selection metadata for context-sensitive commands belong in `AppKomponentBaseLib` when they can be reused across hosts.
- Workbench-specific popup menu routing, popup targets, and command-surface interpretation stay in `AA98_AvlnCodeStudio.Base`.
- `AA98_AvlnCodeStudio.Base` should consume these shared contracts as a reference host layer instead of re-declaring them.

## Component Areas

| Component Area | AA98 Role | Primary Epic | Current State | Existing Implementation Sources |
| --- | --- | --- | --- | --- |
| Shell and command surfaces | Main window shell, menu, toolbar, status, composition entry points | `AA98-E01` | Planned / partial AA98 implementation | `AA98_AvlnCodeStudio.UI`, `AA98_AvlnCodeStudio.Base.Components.Commands`, `Libraries/AppKomponentBaseLib.Context` |
| Editor framework | Text document lifecycle, editor hosting, file-type-aware editing | `AA98-E02` | Partial AA98 implementation exists | `AA98_AvlnCodeStudio.Editor`, `AA98_AvlnCodeStudio.UI`, `AA98_AvlnCodeStudio.Model` |
| Component extension model | Command, configuration, UI, and registration contributions | `AA98-E03` | In progress | `AA98_AvlnCodeStudio.Base.Components.Commands`, `Libraries/AppKomponentBaseLib`, `Libraries/AppKomponentBaseLib.Context` |
| Docking and layout | Dockable workbench areas and persistent arrangement rules | `AA98-E04` | Planned | No selected implementation yet |
| Workspace and project handling | Workspace context, file navigation, project and solution context | `AA98-E05` | Planned | Future alignment with builder and explorer slices |
| AI integration | Provider-neutral AI contracts and interaction flows | `AA98-E06` | Baseline partial implementation exists; Copilot-assisted workflow planning added | `AA98_AvlnCodeStudio.Base.AI`, `DevOps/Features/AA98-F40-Copilot-Assisted-Workflow.md` |
| Privacy and consent | Consent level model and data-sharing boundaries | `AA98-E07` | Planned | No dedicated implementation cataloged yet |
| Settings and configuration | Component-contributed settings and preference persistence | `AA98-E08` | Started at contract level | Planned `Libraries/AppKomponentBaseLib` configuration slice |
| Testing and engineering baseline | Validation, regression protection, and architecture guardrails | `AA98-E09` | Partial implementation exists | `AA98_AvlnCodeStudio.Tests`, base test projects |
| Builder and project inspection | Host-neutral SDK-style project inspection and later Roslyn execution | `AA98-E10` | Existing implementation source available | `C:\Projekte\CSharp\CSharpBible\Data\Workbench.Builder\Workbench.Builder.Core`, `Workbench.Builder.Analysis`, `Workbench.Builder.Host`, `Workbench.Builder.Cli` |
| Linux self-hosting | Linux-first AA98 usage as its own development environment | `AA98-E11` | Planned | `DevOps/Roadmaps/AA98-Linux-Self-Hosting-Roadmap.md`, thin host strategy, shell/editor/builder/terminal/repository/planning slices |
| DevOps planning workbench | Local planning workflows with later Azure DevOps and GitHub adapters | `AA98-E12` | Planned | `DevOps/.info.md`, `DevOps/Epics/AA98-E12-DevOps-Planning-Workbench.md`, future `AA98.DevOpsPlanning.Host` |
| Component micro hosts | Thin dedicated host applications per major component area | `AA98-E11`, `AA98-E12` | Planned | `DevOps/Features/AA98-F39-Component-Micro-Hosts.md`, future `AA98.<Component>.Host` executables |
| Developer terminal | Embedded developer terminal component for workbench sessions | `AA98-E03`, `AA98-E01` | Existing implementation source available | `C:\Projekte\CSharp\CSharpBible\Libraries\Terminal.Core`, `C:\Projekte\CSharp\CSharpBible\Libraries\Terminal.Avalonia` |
| Version control and repository migration | VCS-facing workflows, migration tooling, and provider-oriented repository services | `AA98-E03`, `AA98-E05` | Existing implementation source available | `C:\Projekte\CSharp\CSharpBible\Data\RepoMigrator\RepoMigrator.Core`, provider projects, and tool projects |
| Image editor | Future workbench-hosted visual editor component | `AA98-E03`, `AA98-E08` | Early standalone slice exists | `Avln_ImageEditor\Avln_ImageEditor.Controls`, `Avln_ImageEditor\Avln_ImageEditor.Host` |

## Existing Implementation Notes

### Developer Terminal

The terminal area already has a useful split between host-neutral and Avalonia-facing code.

- `Terminal.Core` contains reusable terminal session and buffer contracts and logic.
- `Terminal.Avalonia` contains Avalonia controls and view models.

This makes the terminal a strong candidate for future AA98 component wrapping rather than reimplementation.

### Builder

The builder area already follows the intended architecture especially well.

- `Workbench.Builder.Core` provides host-neutral builder logic.
- `Workbench.Builder.Analysis` provides structured analysis output.
- `Workbench.Builder.Host` and `Workbench.Builder.Cli` act as thin hosts.

This aligns well with the AA98 requirement that builder concerns remain host-neutral and workbench-consumable.

### Thin Component Hosts

Linux-first self-hosting should be supported by thin executable hosts per major component area.

- A thin host validates one component area with minimal composition overhead.
- It should reuse component contracts and implementations rather than clone them.
- It acts as a focused development, smoke-test, and troubleshooting surface.
- It should not grow into a second full workbench.

The current planning references are:

- Roadmap: `DevOps/Roadmaps/AA98-Linux-Self-Hosting-Roadmap.md`
- Feature: `DevOps/Features/AA98-F39-Component-Micro-Hosts.md`

### DevOps Planning Workbench

The DevOps planning area is now planned as a first-class AA98 component, starting with local markdown planning and later adapter-based external providers.

- The local source of truth remains the repository planning hierarchy in `DevOps`.
- Azure DevOps and GitHub should arrive as later adapters instead of shaping the local planning model too early.
- Planning commands should be compatible with both user-driven and AI-driven workflows.

The current planning references are:

- Epic: `DevOps/Epics/AA98-E12-DevOps-Planning-Workbench.md`
- Feature: `DevOps/Features/AA98-F40-Copilot-Assisted-Workflow.md`

### Version Control and Repository Migration

`RepoMigrator` contains provider-oriented repository and migration logic that may later inform or supply AA98 workbench components.

- `RepoMigrator.Core` is the most relevant reusable core candidate.
- Provider projects show how VCS-specific capabilities can stay separated.
- Tool projects illustrate host-specific command workflows that should not be moved directly into `AA98_AvlnCodeStudio.Base`.

## Low-Level Interface Direction

Not every reusable abstraction should live in AA98-specific projects. Some low-level contracts may be general enough for `Libraries/BaseLib`.

### Candidate BaseLib-Level Abstractions

- `IMessaging` or a similarly named neutral message publishing contract.
- Possibly other foundational non-UI, non-workbench abstractions that are broadly reusable across repositories.

### Boundary Rule

If an interface is generic infrastructure and not inherently tied to workbench concepts such as commands, component descriptors, shell regions, or workspace context, it should be evaluated for `BaseLib` instead of `AA98_AvlnCodeStudio.Base`.

## Immediate Planning Consequences

- Future AA98 component planning should reference this catalog before creating new contracts.
- The next AA98 slices should continue with configuration contribution contracts, component descriptor/registration contracts, and tool-capable command metadata.
- The first shared implementation step for reusable command-tool metadata should land in `Libraries/AppKomponentBaseLib` once it is no longer AA98-specific.
- Context-sensitive command work should split neutral context data into `AppKomponentBaseLib` and keep popup/workbench semantics in `AA98_AvlnCodeStudio.Base`.
- Terminal, builder, and repository migration areas should be integrated through AA98 wrapper contracts instead of copied directly into the shell.
- Linux self-hosting planning should favor thin host applications per component area instead of driving every validation scenario through the main shell only.
- The DevOps planning component should start from the repository's local markdown planning model before Azure DevOps or GitHub adapters are introduced.
- Shared abstractions must now be evaluated against three layers: `BaseLib`, `Avln_BaseLib`, and `AppKomponentBaseLib`.
