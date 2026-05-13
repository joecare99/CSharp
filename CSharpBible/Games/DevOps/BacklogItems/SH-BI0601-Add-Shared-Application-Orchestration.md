# SH-BI0601 - Add Shared Application Orchestration

## Work Item Type
Backlog Item

## Parent
`SH-F06 - UI and Player Experience`

## Description
Remove duplicated session setup from UI projects by introducing shared orchestration services.

## Scope
- Extract duplicated setup from console and WPF2D.
- Keep UI-specific rendering in UI projects.
- Use dependency injection where practical.

## Acceptance Criteria
- Console and WPF2D create equivalent sessions through shared setup.
- Tests can instantiate setup with substitutes.
- No UI project owns core gameplay defaults exclusively.

## Child Tasks
- `SH-T060101 - Identify Shared Setup Dependencies`
- `SH-T060102 - Create Application Setup Service`
- `SH-T060103 - Refactor Console Setup`
- `SH-T060104 - Refactor WPF2D Setup`
- `SH-T060105 - Test Shared Orchestration`
