# AA98-Bl012 Dockable UI Contribution Baseline

## Parent
- Feature: `DevOps/Features/AA98-F10-Dockable-UI-Contribution-Contracts.md`
- Epic: `DevOps/Epics/AA98-E03-Component-Extension-Model.md`
- Vision: `DevOps/Vision.md`

## Scope
Define the first dockable or hostable UI contribution baseline for `AA98_AvlnCodeStudio` so internal components can provide workbench UI modules through stable contracts instead of bespoke shell integration.

## Goals
- Define the first abstractions for contributed UI modules.
- Clarify the minimum placement or host metadata required for the workbench.
- Keep the baseline compatible with current shell regions and later docking evolution.
- Prepare the path for internal tool-style UI contributions without requiring full docking support yet.

## Assumptions
- Initial UI contributions are internal and trusted.
- The first baseline can target existing or placeholder shell regions before full docking exists.
- Placement expectations should remain simple in the first step.

## Open Questions
- How much placement detail is needed in the first contribution contract?
- Should the first baseline distinguish tool windows from editor-adjacent content?

## Status
- Proposed
