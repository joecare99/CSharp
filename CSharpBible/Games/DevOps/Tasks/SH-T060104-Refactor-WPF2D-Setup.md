# SH-T060104 - Refactor WPF2D Setup

## Work Item Type
Task

## Parent
`SH-BI0601 - Add Shared Application Orchestration`

## Goal
Move WPF2D gameplay setup to the shared orchestration service.

## Scope
- Replace duplicated dependency construction in WPF2D factory.
- Preserve WPF-specific hosting and rendering.
- Keep DI integration clean.

## Done
- WPF2D uses shared gameplay setup.
- WPF-specific code remains UI-local.
