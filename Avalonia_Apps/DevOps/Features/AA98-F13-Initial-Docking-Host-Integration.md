# AA98-F13 Initial Docking Host Integration

## Parent
- Epic: `DevOps/Epics/AA98-E04-Docking-and-Layout.md`
- Vision: `DevOps/Vision.md`

## Goal
Introduce the first docking host integration for `AA98_AvlnCodeStudio` so the workbench can evolve from fixed shell regions toward a user-adjustable layout model.

## Scope
- Define the first docking host concept for the workbench.
- Integrate dockable hosting into the shell without replacing existing structure prematurely.
- Prepare the workbench for hosted editor and tool surfaces.
- Keep the first integration simple enough to validate early.

## Included
- Docking host baseline
- Workbench integration path
- Hosted surface expectations
- Extensibility path for later docking growth

## Excluded for Now
- Full advanced docking customization
- Multi-monitor docking scenarios
- Complex user-defined workspace templates
- External extension docking isolation

## Success Indicators
- The workbench can host dockable regions through a defined baseline.
- Docking integration remains understandable and stable.
- Later tool and editor layout features can build on the same host model.

## Candidate Backlog Items
- Define the first docking host baseline
- Integrate docking host with the current workbench shell
- Prepare hosted regions for editor and tool surfaces
- Validate the baseline docking behavior in the workbench

## Assumptions
- Docking should shape the shell early enough to avoid later redesign.
- The first integration can remain intentionally limited while proving the hosting model.

## Open Questions
- Which Avalonia-compatible docking approach best fits the planned workbench model?
- How much shell restructuring should happen in the first docking increment?

## Status
- Proposed
