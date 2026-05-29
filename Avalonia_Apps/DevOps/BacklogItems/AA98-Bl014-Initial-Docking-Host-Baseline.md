# AA98-Bl014 Initial Docking Host Baseline

## Parent
- Feature: `DevOps/Features/AA98-F13-Initial-Docking-Host-Integration.md`
- Epic: `DevOps/Epics/AA98-E04-Docking-and-Layout.md`
- Vision: `DevOps/Vision.md`

## Scope
Define and introduce the first docking host baseline for `AA98_AvlnCodeStudio` so the workbench can move from fixed shell regions toward a user-adjustable hosting model for editor and tool surfaces.

## Goals
- Define the first docking host concept for the workbench.
- Integrate the docking host with the current shell structure without unnecessary redesign.
- Prepare hosted regions for editor and tool surfaces.
- Keep the first docking baseline small, understandable, and testable.

## Assumptions
- Docking should influence the shell structure early enough to avoid later rework.
- The first docking baseline can remain intentionally limited while validating the hosting model.
- Advanced docking customization and multi-monitor scenarios belong to later increments.

## Open Questions
- Which Avalonia-compatible docking approach best fits the planned workbench model?
- How much shell restructuring is acceptable in the first docking increment?

## Status
- Proposed
