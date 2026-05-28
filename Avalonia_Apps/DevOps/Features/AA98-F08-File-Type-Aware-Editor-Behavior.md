# AA98-F08 File-Type-Aware Editor Behavior

## Parent
- Epic: `DevOps/Epics/AA98-E02-Editor-Framework.md`
- Vision: `DevOps/Vision.md`

## Goal
Introduce file-type-aware editor behavior so the framework can apply suitable handling, services, and defaults based on the kind of document being edited.

## Scope
- Define how the editor determines document type from file context.
- Introduce a baseline model for file-type-specific behavior selection.
- Separate shared editor behavior from file-type-specific contributions.
- Prepare the framework for later richer per-type services beyond syntax highlighting.
- Prepare file-type-facing labels and descriptions so they can come from resources.

## Included
- Document type identification rules
- File-type-aware behavior selection
- Shared versus specialized editor responsibilities
- Extensibility path for future editor services
- Resource-friendly file-type presentation

## Excluded for Now
- Full language server integration
- Advanced refactorings or code fixes
- Markdown preview and design-time tooling
- Rich per-type settings UI

## Success Indicators
- The editor framework can choose baseline behavior according to document type.
- File-type-specific behavior remains extensible and does not overcomplicate the shared editor core.
- Later language and tooling services can attach through defined type-aware seams.

## Candidate Backlog Items
- Define document type identification and mapping rules
- Introduce file-type-aware behavior contracts or abstractions
- Separate shared editor baseline from specialized behavior
- Prepare extension seams for future editor services per file type

## Assumptions
- File-type-aware behavior should begin with lightweight rules rather than complex provider systems.
- The shared editor core should stay small and stable while type-specific behavior grows incrementally.

## Open Questions
- Should markdown preview remain outside the initial type-aware behavior baseline?
- How early should type-specific commands be modeled separately from shared editor commands?

## Status
- Proposed
