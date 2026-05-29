# AA98-Bl021 Workspace Context Service Baseline

## Parent
- Feature: `DevOps/Features/AA98-F20-Workspace-Context-Service.md`
- Epic: `DevOps/Epics/AA98-E05-Workspace-and-Project-Handling.md`
- Vision: `DevOps/Vision.md`

## Scope
Define and introduce the first workspace context service baseline for `AA98_AvlnCodeStudio` so core application areas can consume current workspace state through a stable abstraction.

## Goals
- Define the first workspace context model.
- Introduce a service abstraction for accessing current workspace state.
- Align workspace context consumption with shell and editor workflows.
- Keep the service explicit, understandable, and extensible for later AI and project-aware features.

## Assumptions
- A simple current-workspace service is sufficient before deeper project modeling exists.
- The workspace context model should support future LLM context scoping without overdesigning it now.
- Rich semantic project graph concerns belong to later increments.

## Open Questions
- Which parts of workspace state must be exposed from the first version onward?
- How early should active document and selection context become part of the workspace service?

## Status
- Proposed
