# AA98-Bl007 Initial Syntax Highlighting for Core Formats

## Parent
- Feature: `DevOps/Features/AA98-F07-Syntax-Highlighting-Support.md`
- Epic: `DevOps/Epics/AA98-E02-Editor-Framework.md`
- Vision: `DevOps/Vision.md`

## Scope
Add the first syntax-highlighting support for the initial core formats in `AA98_AvlnCodeStudio` so the editor becomes more practical for `C#`, `.axaml`, and `.md` editing workflows.

## Goals
- Define the initial highlighting mappings for the supported core file types.
- Apply syntax highlighting automatically based on document type or file extension.
- Keep the highlighting selection logic extensible for later formats.
- Validate that the user-visible highlighting behavior improves the first editing experience.

## Assumptions
- Basic highlighting provides strong early value without requiring deeper language intelligence.
- The editor control capabilities should be used pragmatically before introducing custom highlighting infrastructure.
- The highlighting model should remain compatible with future additions such as `json`.

## Open Questions
- Should `json` be included immediately once the mapping mechanism is in place, or remain a separate follow-up?
- How much theme-awareness should be part of this first highlighting increment?

## Status
- Proposed
