# AA98-Bl011 Command Contribution Baseline

## Parent
- Feature: `DevOps/Features/AA98-F09-Command-Contribution-Contracts.md`
- Epic: `DevOps/Epics/AA98-E03-Component-Extension-Model.md`
- Vision: `DevOps/Vision.md`

## Scope
Define the first command contribution baseline for `AA98_AvlnCodeStudio` so internal components can provide actions to the workbench through stable contracts instead of direct shell-specific integration.

## Goals
- Define the first abstractions for component-contributed commands.
- Clarify the minimum metadata needed for shell placement and presentation.
- Keep the command contribution model compatible with existing shell command surfaces.
- Prepare the path for later extension by additional internal components.

## Assumptions
- Internal component contributions are the first target, not external plugin scenarios.
- The first command contribution model should favor clarity over dynamic flexibility.
- Command placement can begin with a small baseline and grow later as needed.

## Open Questions
- Which command metadata is essential in the first version?
- Should placement be declarative immediately or phased in after the basic contract is stable?

## Status
- Proposed
