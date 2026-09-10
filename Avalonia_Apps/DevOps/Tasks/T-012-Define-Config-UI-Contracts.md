# T-012 Define Config UI Contracts

## Parent
B-Config-UI | Previous: T-011 | Next: T-013

## Status
Planned

## Request
Define product-neutral Avalonia-facing section navigation, section metadata, loading, editing, saving, reset, busy, and error contracts. Host registration owns localizable display names and descriptions.

## Planned tests
- Provider order and selected default section.
- Missing description and invalid metadata.
- Read-only, unavailable, loading, and failure states.
- UI contract does not reference RnzTrauer, ConsoleLib, or CodeStudio models.

## Gate and completion
Pass G-Architecture. Before Done: all tests/build/documentation, English SVN commit and revision, T-013 In Progress, and parent statuses updated.