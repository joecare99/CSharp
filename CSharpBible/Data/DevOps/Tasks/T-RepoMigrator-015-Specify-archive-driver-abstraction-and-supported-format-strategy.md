# Task T-RepoMigrator-015 - Specify archive driver abstraction and supported format strategy

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-010` - `Plan driver-based archive extraction and metadata inspection`

## Goal

Define the archive-driver contract and the first supported-format strategy for archive-backed snapshot imports.

## Scope

- Draft driver responsibilities for probing, metadata inspection, and extraction
- Compare library or tooling options for `.zip`, `.7z`, `.tar`, and `.tar.gz`
- Record timestamp metadata expectations per format
- Define safe failure behavior for unsupported or damaged archives

## Deliverables

- A proposed archive-driver interface model
- A support matrix for the initial archive formats
- A dependency and compatibility note for the current solution target frameworks

## Open Questions

- Is a managed implementation available for every planned format across the relevant target frameworks?
- Should metadata inspection and extraction be separated into different abstractions?
