# Task T-831187 - Create WPF trace analysis project and composition root

## Status

Draft

## Parent

- Backlog Item `BI-830362` - `Create WPF trace analysis shell`

## Goal

Create the initial multi-project WPF application baseline, solution integration, and composition root for the trace analysis workbench.

## Scope

- Create at least two dedicated projects for the trace analysis workbench:
  - one non-UI core/application project for models, services, and reusable orchestration
  - one WPF UI project for application startup, views, resources, and UI composition
- Configure project targeting appropriate for the planned WPF baseline
- Integrate the projects into the existing solution structure
- Establish the application startup path and composition root
- Establish dependency-injection wiring for shell-level services and view models
- Define project references and dependency direction between core/application and UI
- Keep the composition compatible with later extracted widgets and Avalonia-oriented service reuse

## Out of Scope

- Full shell layout implementation
n- Plot rendering implementation
- Full processing editor implementation

## Implementation Notes

- Prefer a clean application bootstrap with explicit service registration
- Keep UI-specific wiring separate from reusable trace-processing and filter services
- Follow MVVM-oriented project structure from the beginning
- Keep one class per file where practical
- Do not collapse core/application logic and WPF UI composition into one project in the first increment

## Recommended Project Structure

- **Core/Application project**
  - `Services`
  - `Models`
  - reusable orchestration logic
  - optionally presentation-neutral state or view-model-adjacent logic
- **WPF UI project**
  - `Views`
  - UI composition
  - `Resources`
  - application bootstrap
- **Optional later project**
  - reusable widget or controls project

## Test Strategy

- Verify project creation and solution integration
- Verify dependency direction between core/application and UI is clean
- Verify application startup and shell composition resolve successfully
- Verify shell-level services can be constructed through the composition root

## Done Criteria

- At least a core/application project and a WPF UI project exist in the workspace
- Projects are integrated into the solution
- Startup path and composition root are defined
- DI baseline is in place for shell-level composition
- Project structure supports later shell and editor tasks
- Project split supports later widget extraction and Avalonia-oriented reuse

## Implementation Status Note

Current workspace baseline created:

- `TraceAnalysis.Workbench.Core`
  - initial models for channels, processing steps, outputs, source state, and validation issues
  - initial session service for shell bootstrap
- `TraceAnalysis.Workbench.Wpf`
  - WPF application bootstrap
  - host-based composition root
  - main workbench window baseline
  - first shell-level view model

This baseline is intentionally minimal and prepares the next steps for shell refinement, channel surfaces, and editor implementation.
