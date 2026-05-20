# Task T-830360 - Extract reusable trace chart widget and add quick viewer

## Status

Draft

## Parent

- Feature `830360` - `Interactive trace visualization workbench`
- Backlog Item `BI-830362` - `Create WPF trace analysis shell`
- Backlog Item `BI-830366` - `Refine workbench primary UX flow`

## Goal

Extract the trace chart and cursor-analysis widgets into a reusable WPF library, add a larger chart-focused workbench tab, and provide a lightweight quick-viewer application for loading and displaying traces with minimal workflow overhead.

## Current Domain Context

- The first chart increment now visualizes canonical time-series data inside the workbench shell
- User feedback indicates that the embedded chart is helpful as an overview but too small for fast analysis
- The current chart and cursor widgets live directly inside the workbench UI project, which limits reuse
- A lightweight trace viewer is desirable for scenarios where users only want to open a trace file and inspect it without the larger workbench workflow

## Scope

- Create a dedicated reusable WPF chart/widget library project
- Move trace chart and cursor summary controls plus chart-specific view models into that library
- Update the workbench to consume the reusable chart library
- Add a larger chart-oriented tab in the workbench UI for faster analysis
- Create a `TraceAnalysis.Show.Wpf` application for quick trace loading and display
- Support loading a trace file from a command-line parameter in the quick-viewer app

## Out of Scope

- Final pan and zoom mouse interaction design
- Advanced export workflows for chart selections
- Large-dataset virtualization or progressive rendering
- AvaloniaUI viewer implementation

## Acceptance Criteria

- A reusable WPF chart/widget project exists in the workspace
- The workbench uses the reusable chart controls instead of owning chart widgets directly
- The workbench offers a larger chart-focused tab in addition to the overview composition
- A lightweight `TraceAnalysis.Show.Wpf` application exists
- The quick viewer can open a trace from a file picker
- The quick viewer can attempt to open a trace passed as the first command-line argument
- The solution builds successfully after the project split

## Implementation Notes

- Keep chart-state orchestration reusable enough for both the workbench and quick viewer
- Keep non-UI trace loading in the existing core/service layer
- Preserve the current standalone-first workflow while making larger analysis views easier to reach
- Prefer a split where reusable chart widgets stay independent from workbench-specific menu and editor concerns

## Open Questions

- Should the larger analysis tab eventually support multiple synchronized charts instead of one shared large view?
- Should the quick viewer stay intentionally minimal, or should it later gain grouping and export actions from the workbench?
- Which chart interactions should be shared between workbench and quick viewer versus kept app-specific?
