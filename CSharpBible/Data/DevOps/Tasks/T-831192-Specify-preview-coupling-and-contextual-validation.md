# Task T-831192 - Specify preview coupling and contextual validation

## Status

Draft

## Parent

- Backlog Item `BI-830366` - `Refine workbench primary UX flow`

## Goal

Define how plot or preview feedback and live validation should support the workbench flow without making the UI feel overloaded.

## Current Domain Context

- Preview or plot feedback should only become prominent after the workbench has acquired a usable data basis from a trace file or its header
- The first visible preview should confirm that derived structure such as columns, groups, formats, and time base was understood correctly
- The same preview-coupling concept should remain reusable when the standalone workbench is later composed into a suite shell

## Scope

- Define how channel selection should affect preview or plot feedback
- Define how processing-editor changes should affect preview or summary feedback
- Define how the first preview depends on the acquired data basis from a trace file or header
- Define where live validation appears in relation to the edited controls and the overall shell
- Define which validation messages are blocking, warning, or informational in the standalone flow
- Define the minimum viable preview coupling before richer editor investment continues

## Out of Scope

- Full plotting implementation
- Advanced real-time recomputation strategies
- Final styling details of validation presentation

## Implementation Notes

- Prefer contextual feedback over globally dominant diagnostics when possible
- Make validation useful for the current user task instead of always equally prominent
- Keep the first coupling baseline simple enough to implement incrementally

## Recommended Preview Coupling Baseline

1. Load trace file or header
2. Derive structural basis:
   - columns
   - groups
   - formats
   - time base
3. Reflect this structure immediately in channel browser and source summary
4. Enable preview or plot once one or more channels are selected
5. Reflect processing changes in preview only after the user already sees the raw or source-derived basis

This keeps preview tied to user understanding instead of showing an isolated chart too early.

## Recommended Contextual Validation Baseline

- During data-basis acquisition:
  - show source parsing and structural interpretation issues near the source summary and channel structure
- During channel selection and preview:
  - show warnings that affect preview completeness near the preview or selection context
- During processing editing:
  - show editor-specific validation near the edited control first
  - mirror important issues in a central diagnostics area second

Validation should therefore follow the active user task instead of appearing as one uniformly dominant diagnostics wall.

## Reuse Baseline for a Later Suite

- The suite should reuse the same coupling principle:
  - acquire data basis
  - reveal structure
  - preview selected channels
  - then deepen editing
- A suite shell may place the widgets differently, but should not change this ordering fundamentally

## Test Strategy

- Define example scenarios for channel selection, editor changes, and validation reactions
- Verify the resulting guidance can inform shell and editor refinements directly
- Verify that the preview and validation coupling still makes sense if the same workbench is hosted inside a suite shell

## Done Criteria

- Preview coupling baseline is defined
- Contextual validation baseline is defined
- Priority between preview coupling and deeper editor expansion is documented
- Result can guide subsequent shell and editor tasks
- Dependency on prior data-basis acquisition is documented
