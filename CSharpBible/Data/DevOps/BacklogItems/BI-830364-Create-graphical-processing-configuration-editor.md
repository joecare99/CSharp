# Backlog Item BI-830364 - Create graphical processing configuration editor

## Status

Draft

## Parent

- Feature `830360` - `Interactive trace visualization workbench`

## Goal

Provide a graphical authoring aid in the WPF workbench that helps users create, inspect, validate, and save processing-model configurations.

## Description

Users should not be required to hand-author processing configurations as raw JSON or another low-level representation. The workbench should therefore provide an editor experience that guides channel selection, operation setup, output naming, and unit-related metadata decisions.

The editor should produce a persisted configuration artifact that can later be executed by a console-oriented processor against trace files without requiring the UI. The first increment can focus on a step-by-step or list-based authoring experience instead of a full node graph.

Current shell feedback suggests that the editor should not dominate the product story before users can clearly see what a loaded trace looks like and how channel selection affects a preview or plot. The editor remains important, but its UX should be coupled more visibly to preview feedback and lightweight validation rather than treated as the first focal point of the standalone workbench.

## Acceptance Criteria

- A graphical editing flow for processing configurations is defined
- The editor can create and save a processing configuration artifact
- Validation feedback exists for incomplete or inconsistent processing steps
- Editor feedback can be understood in the context of channel or preview results rather than in isolation only
- The saved configuration is suitable for non-UI console execution
- The editor design remains compatible with later richer visual authoring styles

## Dependencies

- `BI-830362` - `Create WPF trace analysis shell`
- `BI-830363` - `Create canonical trace processing model`

## Candidate Tasks

- Define the editor workflow and view-model structure
- Define validation behavior for channel references, operation parameters, and units
- Couple editor changes to plot or preview feedback earlier in the user flow
- Define save/load interaction for processing configuration files
- Test compatibility of saved configurations with the console runner

## Open Questions

- Should the first editor use a wizard, a property grid, a reorderable step list, or a hybrid approach?
- Which advanced editing scenarios require a future textual escape hatch?
- How much live validation is needed before preview coupling becomes more important than richer editor controls?
