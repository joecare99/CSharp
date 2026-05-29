# AA98-Bl008 Multi-Document State and Active Document Baseline

## Parent
- Feature: `DevOps/Features/AA98-F06-Multi-Document-Editing-Baseline.md`
- Epic: `DevOps/Epics/AA98-E02-Editor-Framework.md`
- Vision: `DevOps/Vision.md`

## Scope
Establish the first multi-document baseline for `AA98_AvlnCodeStudio`, including document collection handling and a clear active-document concept that can support later tabbed editor presentation and shell interactions.

## Goals
- Define the baseline model for multiple open documents.
- Introduce a clear active-document concept for shell, editor, and status interactions.
- Prepare the editor host to evolve from the current single-document baseline toward a practical multi-document workflow.
- Keep the design simple enough to extend later with tabs and richer layout behavior.

## Assumptions
- Stable single-document fundamentals should remain the base for multi-document support.
- A tab-oriented mental model is likely the most practical first evolution path, even if the first abstraction remains presentation-neutral.
- Session restore and advanced tab behaviors are later increments and should not complicate the first baseline.

## Open Questions
- Should the first concrete UI expression of multi-document support already be tabs?
- How early should document ordering or persistence of open documents be considered?

## Status
- Proposed
