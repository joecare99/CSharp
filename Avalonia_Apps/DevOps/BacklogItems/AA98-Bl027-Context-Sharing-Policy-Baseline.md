# AA98-Bl027 Context Sharing Policy Baseline

## Parent
- Feature: `DevOps/Features/AA98-F28-Context-Sharing-Policy-Model.md`
- Epic: `DevOps/Epics/AA98-E07-Privacy-Security-and-Consent.md`
- Vision: `DevOps/Vision.md`

## Scope
Define the first context sharing policy baseline for `AA98_AvlnCodeStudio` so editor, workspace, and AI-related features can determine what information may be shared with external providers under explicit user control.

## Goals
- Define baseline rules for context sharing with external providers.
- Clarify how file, selection, and workspace context are governed.
- Align sharing decisions with consent levels and provider disclosure.
- Keep the model explicit and easy to reason about.

## Assumptions
- Explicit user control is required for context sharing.
- Initial rules should favor simplicity and transparency over breadth.
- AI-related features will enforce sharing rules consistently once the policy exists.

## Open Questions
- Which context categories are allowed by default before provider setup?
- How should context sharing interact with future chat and selection actions?

## Status
- Proposed
