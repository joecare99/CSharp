# T-018 Define Designer Consumption

## Parent
B-Designer-Consumption | Previous: T-017 | Next: T-019

## Status
Done

## Request
Document and implement only the shared-contract adapters needed by ConsoleLib.Cxaml.Designer. The designer consumes Config.UI, Property Editor, Project Explorer, and CodeLocation; the shared pool never references designer/ConsoleLib types.

## Required sharing decision
`ConsoleLib.Cxaml.Designer` resides under the sibling `CSharpBible` working
copy. Before adding a dependency on Property.Editor, choose and document one
maintained sharing mechanism: linked canonical sources and tests as used by
Code.Navigation, a versioned package, or an explicit controlled project
reference. Do not duplicate the contract implementation.

## Planned tests
- Project-reference architecture test for one-way dependencies.
- Adapter fixture operates with navigator and property contracts through test doubles.
- AXaml error location maps to CodeLocation.
- The selected cross-working-copy sharing mechanism builds the designer and
  its tests without duplicating Property.Editor sources.

## Sharing decision
`Property.Editor` remains canonical in `Avalonia_Apps\Libraries\Property.Editor`.
The CSharpBible `Property.Editor` and `Property.Editor.Tests` projects link
the canonical production and test sources exactly as its existing
`Code.Navigation` projects do. No independent contract implementation exists.

## Delivered
- The CSharpBible CXAML designer references the linked Property.Editor and
  existing linked Code.Navigation contracts.
- `InspectorPropertyViewModel` is a designer-bound `IPropertyItem` adapter;
  ConsoleLib conversion and CXAML write-back remain in the designer.
- `DesignerViewModel` exposes a selected CXAML source location as
  `CodeLocation` without making the shared contracts depend on ConsoleLib.
- Config.UI and Project Explorer have no designer adapter because no current
  designer behavior requires them; the shared components do not take a
  designer dependency.

## Validation
- Linked `Property.Editor.Tests`: 11/11 passed on net8.0.
- `ConsoleLib.Cxaml.DesignerTests`: 30/30 passed on net8.0, including shared
  property-contract and source-location adapter coverage.

## Version control
- SVN revision: r1899 (`Link shared Property Editor into CXAML designer`).

## Handoff
T-019 is In Progress for complete feature readiness validation.