# AA98-F06 Multi-Document Editing Baseline

## Parent
- Epic: `DevOps/Epics/AA98-E02-Editor-Framework.md`
- Vision: `DevOps/Vision.md`

## Goal
Prepare the editor framework for multiple open documents so the workbench can evolve from a single-document editor into a practical IDE-style editing environment.

## Scope
- Define the baseline model for handling multiple open documents.
- Introduce document selection and active-document concepts.
- Clarify how the editor host should evolve toward tabbed or equivalent multi-document presentation.
- Keep the initial design compatible with the existing small editor baseline.

## Included
- Multi-document state model
- Active document handling
- Document collection management
- Baseline presentation expectations for multiple documents

## Excluded for Now
- Advanced tab management customization
- Split views or side-by-side editor groups
- Document pinning and advanced tab behaviors
- Session restore across many documents

## Success Indicators
- The editor framework can represent more than one open document coherently.
- The active document concept is clear for shell, command, and status interactions.
- Later tab, layout, and docking features can use the same editor model.

## Candidate Backlog Items
- Define multi-document state and active document model
- Introduce baseline document collection handling
- Prepare workbench/editor host for multi-document presentation
- Align command and status interactions with active document state

## Assumptions
- Multi-document support should follow stable single-document fundamentals.
- A simple tab-oriented mental model is likely the best first evolution path.

## Open Questions
- Should tabs be the first concrete UI for multi-document handling?
- How early should document reordering or persistence of open documents be considered?

## Status
- Proposed
