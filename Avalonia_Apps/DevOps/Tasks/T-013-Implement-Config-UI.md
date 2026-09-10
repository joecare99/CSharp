# T-013 Implement Shared Config UI

## Parent
B-Config-UI | Previous: T-012 | Next: T-014

## Status
Planned

## Request
Extract or adapt the existing UI into a reusable Avalonia component with View, CommunityToolkit.Mvvm ViewModels, commands, DI registration, Config.Service integration, and shared PropertiesPanel hosting.

## Planned tests
- First provider selection and section switching.
- Load, save, reset, reload, busy, and error commands.
- Unsaved-change policy during section changes.
- Property validation blocks persistence by documented policy.
- ViewModel substitutes and Avalonia binding/control behavior.

## Gate and completion
Before Done: tests on success/error/regression paths pass, net8.0 build passes, English SVN commit/revision recorded, T-014 In Progress, all statuses updated.