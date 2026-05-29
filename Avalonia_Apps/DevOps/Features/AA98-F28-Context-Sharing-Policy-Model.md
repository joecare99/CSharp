# AA98-F28 Context Sharing Policy Model

## Parent
- Epic: `DevOps/Epics/AA98-E07-Privacy-Security-and-Consent.md`
- Vision: `DevOps/Vision.md`

## Goal
Define the first context sharing policy model so editor, workspace, and AI-related features can determine what information may be shared with external providers under explicit user control.

## Scope
- Define baseline rules for context sharing with external providers.
- Clarify how file, selection, and workspace context are governed.
- Align sharing decisions with consent levels and provider disclosure.
- Keep the first model explicit and easy to reason about.

## Included
- Context sharing policy baseline
- File, selection, and workspace context categories
- Consent-aware sharing rules
- Extensibility path for later fine-grained policies

## Excluded for Now
- Fully automated background sharing
- Complex policy rule authoring UI
- Organization-wide policy enforcement
- Deep audit trail management

## Success Indicators
- Users can understand what context may be shared.
- AI-related features can enforce sharing rules consistently.
- The policy model remains compatible with later consent and provider controls.

## Candidate Backlog Items
- Define baseline context sharing categories
- Align sharing decisions with consent and provider disclosure
- Clarify file, selection, and workspace context rules
- Keep the model easy to reason about and extend

## Assumptions
- Explicit user control is required for context sharing.
- Initial rules should favor simplicity and transparency over breadth.

## Open Questions
- Which context categories are allowed by default before provider setup?
- How should context sharing interact with future chat and selection actions?

## Status
- Proposed
