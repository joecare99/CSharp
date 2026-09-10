# T-016 Implement Project Explorer Source

## Parent
B-Project-Explorer | Previous: T-015 | Next: T-017

## Status
Planned

## Request
Implement a testable project/file-system source adapter and refresh behavior outside the UI layer.

## Planned tests
- Empty solution/project and deeply nested paths.
- Supported and unsupported file types.
- Deterministic sort order.
- Missing, moved, unauthorized, and unreadable paths.
- Refresh produces defined additions, removals, and changes.

## Gate and completion
Before Done: isolated temporary-file-system tests and net8.0 build pass; English SVN commit/revision, T-017 In Progress, and status updates are mandatory.