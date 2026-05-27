# AA98-E04 Docking and Layout

## Parent
- Vision: `DevOps/Vision.md`

## Goal
Provide a user-adjustable workbench layout with docking as a central interaction concept so the IDE can evolve into a flexible desktop workspace instead of a fixed editor window.

## Scope
- Introduce dockable workbench regions and tool surfaces.
- Support user-adjustable layout behavior.
- Prepare layout persistence and restore workflows.
- Define how editor and tool windows coexist inside the workbench.

## Included Themes
- Docking model
- Layout state
- Layout persistence
- Workbench arrangement

## Excluded for Now
- Complex multi-monitor layout scenarios
- Cloud-synced layout roaming
- Full workspace templates for every user role

## Success Indicators
- Users can rearrange core workbench elements.
- Layout behavior is understandable and stable.
- Future components can participate in the docking model.

## Candidate Child Features
- Initial docking host integration
- Layout save and restore
- Tool window placement rules
- Editor/document region layout behavior

## Assumptions
- Docking should be introduced early enough to shape the shell correctly.
- Persistence may begin with a simple local format.

## Open Questions
- Which docking library or approach best fits Avalonia and the planned workbench model?
- How early should layout persistence be introduced relative to the first docking feature?

## Status
- Proposed
