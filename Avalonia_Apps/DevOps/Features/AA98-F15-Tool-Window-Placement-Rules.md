# AA98-F15 Tool Window Placement Rules

## Parent
- Epic: `DevOps/Epics/AA98-E04-Docking-and-Layout.md`
- Vision: `DevOps/Vision.md`

## Goal
Define baseline placement rules for tool windows and contributed workbench surfaces so the docking model remains predictable and consistent as more components participate.

## Scope
- Define baseline placement expectations for tool windows.
- Clarify default host areas and placement hints.
- Keep placement behavior understandable for internal component contributions.
- Prepare the model for later richer docking and layout customization.

## Included
- Tool window placement baseline
- Default host region expectations
- Placement hints or metadata concepts
- Extensibility path for later layout growth

## Excluded for Now
- Full end-user placement rules editor
- Advanced per-role workspace presets
- Complex adaptive layouts by context
- External extension trust and isolation policies

## Success Indicators
- Tool-like UI contributions can be placed predictably in the workbench.
- Placement rules are stable enough to reduce bespoke integration decisions.
- Later docking customization can evolve without discarding the baseline rules.

## Candidate Backlog Items
- Define default placement rules for tool windows
- Clarify host region expectations for contributed UI
- Introduce baseline placement hints or metadata
- Align placement rules with docking host evolution

## Assumptions
- Predictable defaults are more important than flexibility in early increments.
- Placement guidance should remain simple while the docking model is still maturing.

## Open Questions
- Which placement concepts must be standardized first?
- How closely should placement rules align with component UI contribution contracts from the start?

## Status
- Proposed
