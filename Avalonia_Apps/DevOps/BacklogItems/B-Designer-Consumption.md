# B-Designer-Consumption

## Parent
F-Shared-CodeStudio-Components

## Status
Planned

## Request
Prepare the later AXaml/CXaml designer to consume shared configuration UI, explorer, property editor, and source navigation without moving ConsoleLib-specific rendering into shared components.

## Acceptance criteria
- Allowed dependencies are documented and tested.
- AXaml diagnostics can map to shared source locations.
- Preview renderer, hit-testing, toolbox, mappings, and AXaml write-back remain ConsoleLib.Cxaml.Designer responsibilities.

## Tasks
T-018, T-019

## Gates
G-Architecture after T-018; G-Release-Readiness after T-019.