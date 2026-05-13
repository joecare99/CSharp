# SH-T040103 - Move Combat Message Formatting

## Work Item Type
Task

## Parent
`SH-BI0401 - Replace Simple Combat With Structured Combat Results`

## Goal
Move human-readable combat summaries out of low-level combat logic.

## Scope
- Format combat messages in session, ViewModel, or UI-facing formatter.
- Keep internationalization in mind.
- Ensure messages still reach existing logs.

## Done
- Combat system no longer owns final UI wording.
- Existing message log behavior remains understandable.
