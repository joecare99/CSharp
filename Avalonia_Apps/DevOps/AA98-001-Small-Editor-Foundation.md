# AA98-001 Small Editor Foundation

## Parent
- Vision: `DevOps/Vision.md`

## Scope
Create a first small file editor prototype for `AA98_AvlnCodeStudio` as a foundation for a later OS-independent IDE.

## Goals
- Split the implementation into three independent projects for UI, model, and base logic.
- Use `Avalonia.AvaloniaEdit` as the editor surface.
- Support basic plain text workflows: new, open, save, and save as.
- Reuse shared libraries from `Libraries/Avln_BaseLib` and `Libraries/Avln_CommonDialogs.Avalonia`.

## Assumptions
- The first increment focuses on a single-document editor.
- Unsaved-change confirmation dialogs are not required in this first step.
- Plain text editing is sufficient as the initial vertical slice.

## Open Questions
- Should later increments add tabs or multiple documents first?
- Should syntax highlighting become file-extension aware in the next increment?

## Status
- Implemented
