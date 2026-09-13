# T-016 Implement Project Explorer Source

## Parent
B-Project-Explorer | Previous: T-015 | Next: T-017

## Status
Done

## Request
Implement a testable project/file-system source adapter and refresh behavior outside the UI layer.

## Planned tests
- Empty solution/project and deeply nested paths.
- Supported and unsupported file types.
- Deterministic sort order.
- Missing, moved, unauthorized, and unreadable paths.
- Refresh produces defined additions, removals, and changes.

## Delivered
- Added `Project.Explorer.FileSystem`, an adapter separate from the neutral
  contracts. It creates deterministic folder-first hierarchies, supports
  optional extension filtering, excludes recursive reparse-point traversal,
  and represents missing or inaccessible directories as unavailable nodes.
- Refresh rebuilds the current hierarchy, thereby representing additions,
  removals, and changes without UI state coupling.

## Validation
- Isolated temporary-filesystem tests passed 2/2 on net8.0, net9.0, and
  net10.0; `Libraries.sln` builds successfully on net8.0.

## Handoff
T-017 is In Progress to provide the Avalonia TreeView consumer.