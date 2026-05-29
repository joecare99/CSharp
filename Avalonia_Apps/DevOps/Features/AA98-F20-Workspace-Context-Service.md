# AA98-F20 Workspace Context Service

## Parent
- Epic: `DevOps/Epics/AA98-E05-Workspace-and-Project-Handling.md`
- Vision: `DevOps/Vision.md`

## Goal
Define the first workspace context service so core application areas can consume coherent workspace state for navigation, editor workflows, and later AI-assisted features.

## Scope
- Define the baseline workspace context model.
- Introduce a service boundary for accessing current workspace state.
- Clarify how editor, shell, and later AI-oriented features consume workspace context.
- Keep the first service simple, explicit, and extensible.

## Included
- Workspace context baseline
- Service abstraction for current workspace state
- Alignment with editor and shell consumption
- Extensibility path for later AI and project-aware features

## Excluded for Now
- Full semantic project graph modeling
- Deep MSBuild-aware context services
- Complex multi-root workspace models
- Advanced cross-process workspace sharing

## Success Indicators
- Core application areas can consume workspace context through a stable abstraction.
- The context service remains understandable and extensible.
- Later editor, component, and AI features can build on the same workspace model.

## Candidate Backlog Items
- Define the first workspace context model
- Introduce a service abstraction for current workspace state
- Align workspace context consumption with shell and editor workflows
- Keep the service ready for future richer context consumers

## Assumptions
- A simple current-workspace service is sufficient before deeper project modeling exists.
- The workspace context model should support future LLM context scoping without overdesigning it now.

## Open Questions
- Which parts of workspace state must be exposed from the first version onward?
- How early should editor selection and active document context become part of the workspace service?

## Status
- Proposed
