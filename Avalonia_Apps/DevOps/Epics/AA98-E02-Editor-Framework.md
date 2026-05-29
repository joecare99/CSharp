# AA98-E02 Editor Framework

## Parent
- Vision: `DevOps/Vision.md`

## Goal
Create the editor foundation for `AA98_AvlnCodeStudio` as a reusable framework that supports the initial file types and can evolve toward a richer .NET-focused development environment.

## Scope
- Provide document opening, editing, and saving workflows.
- Support initial file types: `C#`, `.axaml`, and `.md`.
- Introduce syntax-highlighting capabilities appropriate to the supported document types.
- Prepare the editor area for future multi-document and richer language tooling increments.
- Prepare editor-facing texts and file-type labels so they can be localized through resources.

## Included Themes
- Document lifecycle
- Editor hosting and document state
- Syntax highlighting
- File-type-aware editor behavior
- Resource-friendly editor text handling

## Excluded for Now
- Full Roslyn-powered code intelligence
- Refactoring features
- Advanced code generation workflows

## Success Indicators
- Users can work productively with the initial supported document types.
- The editor model can be extended without replacing the first implementation.
- The framework supports later additions such as tabs, context actions, and richer language services.
- Editor-facing texts can evolve toward resource-based localization without redesigning the feature set.

## Candidate Child Features
- Multi-document editing
- File-type-aware editor services
- Syntax-highlighting support
- Editor commands and document actions

## Assumptions
- The current small editor backlog item `AA98-Bl001` forms the first vertical slice of this epic.
- Rich language services should come after stable editing fundamentals.

## Open Questions
- Should tabs be introduced before deeper syntax services?
- Should markdown preview be part of this epic or a later feature set?

## Status
- Proposed
