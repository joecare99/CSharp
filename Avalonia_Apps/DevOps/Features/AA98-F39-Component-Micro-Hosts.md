# AA98-F39 Component Micro Hosts

## Parent
- Epic: `../Epics/AA98-E11-Linux-Self-Hosting.md`
- Epic: `../Epics/AA98-E12-DevOps-Planning-Workbench.md`
- Roadmap: `../Roadmaps/AA98-Linux-Self-Hosting-Roadmap.md`

## Goal
Define a repeatable strategy for small dedicated host applications that allow AA98 component areas to be developed, tested, and demonstrated independently.

## Scope
- Define what qualifies as a component micro host.
- Map major component areas to candidate host applications.
- Keep host apps thin and aligned with the shared layering strategy.
- Make micro hosts part of the Linux self-hosting path instead of ad-hoc demos.
- Identify validation value, not only UI value, for each host.

## User or Process Value
- Developers can work on a component without loading the entire workbench.
- Linux-specific behavior can be validated in smaller slices.
- Component contracts become easier to stabilize through isolated usage.
- Demonstration and troubleshooting scenarios become simpler.

## Host Principles
- A micro host should exercise one primary component area.
- A micro host should depend on reusable contracts and implementations rather than duplicating them.
- A micro host should stay thin in composition and UI surface.
- A micro host should support targeted testing and exploratory validation.
- A micro host should not become a parallel product line.

## Candidate Host Mapping
- `AA98.Shell.Host` -> shell composition, command surfaces, docking baseline
- `AA98.Editor.Host` -> editor lifecycle and file-type-aware editing
- `AA98.Terminal.Host` -> terminal session hosting and interaction validation
- `AA98.Builder.Host` -> project inspection, build, and test orchestration
- `AA98.AI.Host` -> provider-neutral AI interaction and tool invocation flows
- `AA98.DevOpsPlanning.Host` -> local planning hierarchy, editing, and validation
- `AA98.Repository.Host` -> repository context and local Git-oriented workflows
- `AA98.Settings.Host` -> settings registration, storage abstraction, and settings UI
- `AA98.ImageEditor.Host` -> visual editor component validation
- `AA98.ResourceEditor.Host` -> resource and localization editing workflows

## Architectural Boundaries
- Core and contract logic remain outside the micro hosts.
- Avalonia-specific reusable UI pieces should stay in reusable UI layers where possible.
- The host application provides startup, composition, and the minimum surface required to exercise the target component.
- Shared service abstractions should be reused instead of introducing host-specific shortcuts.

## Validation Expectations
- Each micro host should have a clearly stated developer scenario.
- Each major coding slice should later derive a dedicated test task as required by `DevOps/.info.md`.
- Linux execution should be considered a first-class validation scenario where applicable.
- Hosts intended for self-hosting milestones should support targeted smoke validation.

## Naming Convention
- Prefer the form `AA98.<Component>.Host` for executable host applications.
- Keep component libraries distinct from their host applications.
- Reserve workbench-wide executable naming for the main AA98 shell.

## Assumptions
- Thin hosts provide real architectural value and are not only temporary prototypes.
- Some hosts may share infrastructure but should still keep their primary focus narrow.
- Existing external component sources such as terminal or builder infrastructure can be wrapped under the host pattern.

## Open Questions
- Which micro hosts are mandatory before the first Linux self-hosting milestone?
- Should all micro hosts be desktop-first Avalonia apps, or should some remain console-oriented thin hosts?
- Which hosts require their own CI smoke validation earliest?

## Next Refinement Steps
1. Derive backlog items for the first required micro hosts in the Linux self-hosting path.
2. Pair each implementation-oriented host slice with a dedicated test task.
3. Decide the earliest host set required for shell, editor, terminal, builder, and planning validation.

## Status
- Proposed
