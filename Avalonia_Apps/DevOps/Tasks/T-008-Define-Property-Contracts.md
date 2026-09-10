# T-008 Define Property Contracts

## Parent
B-Property-Editor | Previous: T-007 | Next: T-009

## Status
In Progress

## Request
Consolidate existing property item patterns into a neutral contract for category, display name, type, value, editability, options, validation, and change notification. No ConsoleLib reflection or product model type may enter the contract.

## Planned tests
- Scalar, nullable, Boolean, and enum values.
- Read-only values and empty option sets.
- Category/property deterministic ordering.
- Invalid update does not replace the last valid value.
- Contract reference audit.

## Gate and completion
Pass G-Architecture. Before Done: document contract, run MSTest/build, make English SVN commit, record revision/results, set T-009 In Progress, and update B-Property-Editor and feature status.