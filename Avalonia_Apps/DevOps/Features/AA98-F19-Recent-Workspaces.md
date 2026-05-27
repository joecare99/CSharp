# AA98-F19 Recent Workspaces

## Parent
- Epic: `DevOps/Epics/AA98-E05-Workspace-and-Project-Handling.md`
- Vision: `DevOps/Vision.md`

## Goal
Introduce a recent workspaces concept so users can return quickly to previously used workspace contexts and maintain continuity across sessions.

## Scope
- Define the first recent workspaces model.
- Record recently opened workspaces.
- Provide a baseline way to reopen a recent workspace.
- Keep the first implementation lightweight and local.

## Included
- Recent workspaces baseline
- Recording recent entries
- Reopen workflow
- Local persistence expectations

## Excluded for Now
- Cloud-synced recents
- Cross-device roaming
- Advanced ranking or recommendation behavior
- Complex workspace metadata enrichment

## Success Indicators
- Users can quickly reopen previously used workspaces.
- The recent workspaces model remains small and reliable.
- Later startup and workspace UX features can build on the same baseline.

## Candidate Backlog Items
- Define the recent workspaces model
- Record recently opened workspace entries
- Provide a baseline reopen workflow
- Keep the feature aligned with future startup UX improvements

## Assumptions
- A local recent list is sufficient for the first increment.
- Recent workspaces should remain simple and predictable in behavior.

## Open Questions
- How many recent entries should be retained in the first version?
- Should recent workspaces be exposed in startup UI, menu surfaces, or both?

## Status
- Proposed
