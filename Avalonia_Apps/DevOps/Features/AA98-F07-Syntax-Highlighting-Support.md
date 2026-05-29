# AA98-F07 Syntax Highlighting Support

## Parent
- Epic: `DevOps/Epics/AA98-E02-Editor-Framework.md`
- Vision: `DevOps/Vision.md`

## Goal
Provide syntax-highlighting support for the initial document types so the first editor framework becomes more practical for real development-oriented editing workflows.

## Scope
- Define syntax-highlighting support for `C#`, `.axaml`, and `.md`.
- Clarify how highlighting is selected based on document type.
- Keep the highlighting architecture extensible for later file types.
- Align highlighting behavior with the chosen editor control capabilities.

## Included
- Highlighting selection rules
- Initial language/file-type mappings
- Extensible highlighting registration approach
- Baseline user-visible highlighting behavior

## Excluded for Now
- Semantic highlighting
- Deep Roslyn-backed classification
- Full theme customization model
- Rich language service integration

## Success Indicators
- Initial supported file types display appropriate syntax highlighting.
- Highlighting behavior can be extended without replacing the editor foundation.
- File-type-aware services can build on the same type resolution approach.

## Candidate Backlog Items
- Define syntax-highlighting mappings for initial file types
- Implement highlighting selection in the editor workflow
- Prepare extensible registration for future highlight definitions
- Validate highlighting behavior for the first supported formats

## Assumptions
- Basic syntax highlighting provides immediate value without requiring deep language tooling.
- The highlighting strategy should remain simple and incremental in early phases.

## Open Questions
- Should `json` be included as an early follow-up once the highlighting model exists?
- How much theme-awareness is needed in the first usable highlighting increment?

## Status
- Proposed
