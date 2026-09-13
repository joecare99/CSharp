# B-Property-Editor

## Parent
F-Shared-CodeStudio-Components

## Status
Done

## Request
Create a shared property editing capability for configuration and future designer adapters, based on neutral property contracts rather than ConsoleLib reflection or product models.

## Acceptance criteria
- Categories, names, values, editability, options, validation, and change notifications are modeled.
- Avalonia UI renders scalar, nullable scalar, Boolean, and enum editing.
- Read-only and invalid values have defined behavior.

## Tasks
T-008, T-009, T-010

## Current handoff
- `T-008 Define Property Contracts`: Done. `Property.Editor` is the sole
  public property-item contract; the Planning adapter composes it while
  retaining only Planning-specific mutation and display conversion.
- `T-009 Implement Properties Panel`: Done; the reusable Avalonia component
  replaces the Planning-specific layout.
- `T-010 Validate Property Editor`: Done; host-independent fixture and
  cross-framework component-quality evidence passed.

## Gates
G-Architecture after T-008; G-Component-Quality after T-010.