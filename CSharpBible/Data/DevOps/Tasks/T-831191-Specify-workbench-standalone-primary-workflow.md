# Task T-831191 - Specify workbench standalone primary workflow

## Status

Draft

## Parent

- Backlog Item `BI-830366` - `Refine workbench primary UX flow`

## Goal

Define the first standalone workflow of the workbench so users understand the product quickly after startup.

## Current Domain Context

- The workbench should be usable as a standalone app, but not developed as a separate product line from a future suite
- A later analysis suite should compose the same standalone-capable apps or reusable modules instead of duplicating workflows
- The first workflow should begin with acquisition of a trace data basis, because later UI structure depends on it
- In many cases it should be sufficient to load a trace header or equivalent structural metadata first, before full trace processing is required

## Scope

- Define the first screen emphasis of the standalone workbench
- Define the expected user journey from loading a trace or trace header to selecting channels and seeing preview feedback
- Define where the processing editor fits into the primary workflow
- Define when validation should be shown and how prominent it should be
- Define which shell areas should be visually primary versus secondary
- Define how the same workflow remains valid when the app is later hosted as one module in a suite

## Out of Scope

- Full visual design system work
- Final chart implementation details
- Full suite-shell integration design

## Implementation Notes

- Prefer a simple and explainable primary workflow over exposing all capabilities equally
- Use preview or plot feedback to help users understand the value of channel selection and processing edits
- Keep the standalone story clear while still designing it to be composable into a later suite

## Recommended Workflow Baseline

1. Open a trace file or load enough of its header to acquire the structural data basis
2. Derive and display:
   - columns
   - column groups
   - formats
   - time base
3. Show the resulting channels and structure in a way that confirms the data basis is understood
4. Allow channel selection from that derived structure
5. Show preview or plot feedback for the selected channels
6. Offer processing configuration editing as a secondary step after the data basis and preview are visible
7. Allow export of the current or processed result

## Product Composition Baseline

- The standalone workbench should define the canonical user workflow
- A future suite should host or compose this workflow rather than redefine it separately
- Navigation and shell integration may differ in the suite, but the core user journey from data basis acquisition to preview should stay recognizable

## Data Basis Baseline

The first workflow should treat data-basis acquisition as explicit, not implicit.

Minimum expected derived structure:

- columns
- column groups
- display or parse formats
- time base

Where practical, loading only the trace header or equivalent metadata should already be enough to populate this structure before deeper processing begins.

## Test Strategy

- Review the documented workflow against current shell implementation feedback
- Verify that shell priorities can be derived from the workflow without ambiguity
- Verify that the workflow can be reused unchanged in principle when the workbench is embedded into a later suite shell

## Done Criteria

- Primary standalone workflow is documented
- Relative emphasis of source, preview, and editor surfaces is documented
- Validation visibility baseline is documented
- Relationship to later suite integration is noted
- Data-basis acquisition as the first workflow step is documented
