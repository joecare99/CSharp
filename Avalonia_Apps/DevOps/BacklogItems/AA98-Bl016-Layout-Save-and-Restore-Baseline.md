# AA98-Bl016 Layout Save and Restore Baseline

## Parent
- Feature: `DevOps/Features/AA98-F14-Layout-Save-and-Restore.md`
- Epic: `DevOps/Epics/AA98-E04-Docking-and-Layout.md`
- Vision: `DevOps/Vision.md`

## Scope
Define and introduce the first layout save and restore baseline for `AA98_AvlnCodeStudio` so users can persist a usable workbench arrangement across sessions.

## Goals
- Define the first layout state model for the workbench.
- Persist layout state in a simple local format.
- Restore a previously saved layout in a reliable way.
- Keep the persistence model small, understandable, and extensible.

## Assumptions
- A simple local persistence format is sufficient for the first increment.
- Layout persistence should follow a stable docking host baseline.
- Advanced roaming, templates, and recovery scenarios belong to later increments.

## Open Questions
- Should layout restore happen automatically on startup in the first version?
- How much validation is needed for invalid or outdated saved layouts?

## Status
- Proposed
