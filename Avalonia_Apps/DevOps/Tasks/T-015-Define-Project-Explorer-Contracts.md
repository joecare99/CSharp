# T-015 Define Project Explorer Contracts

## Parent
B-Project-Explorer | Previous: T-014 | Next: T-016

## Status
Done

## Request
Define capability-oriented project explorer node, hierarchy, selection, expansion, refresh, and open-item contracts. Preserve Planning Explorer types as a separate planning feature.

## Planned tests
- Project, folder, and file node construction.
- Stable identity and deterministic sorting.
- Selection and expansion state transitions.
- Missing, cyclic, and inaccessible path behavior.
- Reference audit confirms no Planning model dependency.

## Delivered
- Added the neutral `Project.Explorer` contract component and dedicated MSTest
  project.
- Defined immutable project, folder, and file hierarchy items with stable
  identities, normalized paths, availability state, and ordered children.
- Defined source refresh, item opening, and UI-independent selection/expansion
  capabilities. `ProjectExplorerState` supplies the default in-memory state.
- The component has no Planning, Avalonia, product model, ConsoleLib, or
  filesystem implementation dependency.

## Validation
- `Project.Explorer.Tests`: 3/3 passed on net8.0, net9.0, and net10.0.
- `dotnet build Libraries\Libraries.sln -f net8.0 --no-restore`: succeeded.

## Version control
- SVN revision: recorded with this completion evidence.

## Handoff
T-016 is In Progress to implement the filesystem source adapter.