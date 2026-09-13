# B-Project-Explorer

## Parent
F-Shared-CodeStudio-Components

## Status
Done

## Request
Build a general project/file explorer separate from the existing Planning Explorer, with neutral hierarchy and opening capabilities and an Avalonia UI consumer.

## Acceptance criteria
- Planning-specific tree models remain unchanged.
- Explorer supports projects, folders, files, sorting, expansion, selection, refresh, and inaccessible paths.
- Selecting a file delegates opening to the code navigation/document capability.

## Tasks
T-015, T-016, T-017

## Gates
G-Architecture passed after T-015; G-Integration passed after T-017.

## Current handoff
T-015 through T-017 are Done. The shared explorer has neutral contracts, a
filesystem source, a reusable Avalonia consumer, and CodeStudio navigation
integration without affecting the Planning Explorer.