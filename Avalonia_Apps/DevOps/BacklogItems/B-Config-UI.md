# B-Config-UI

## Parent
F-Shared-CodeStudio-Components

## Status
Planned

## Request
Extract or adapt `RnzTrauer.Avalonia.Config` and its tests into a product-neutral, reusable Avalonia configuration UI consuming Config.Service and the shared property editor.

## Open question
The precise source project path and current API must be confirmed by T-011 before extraction decisions are made.

## Acceptance criteria
- Displays registered sections in provider order with localizable display text.
- Supports load, edit, save, reset, busy state, and failure feedback.
- Hosts provide their own registration and localized metadata through DI.

## Tasks
T-011, T-012, T-013, T-014

## Gates
G-Architecture after T-012; G-Integration after T-014.