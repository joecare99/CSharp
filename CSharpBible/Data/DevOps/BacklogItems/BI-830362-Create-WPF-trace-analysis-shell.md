# Backlog Item BI-830362 - Create WPF trace analysis shell

## Status

Draft

## Parent

- Feature `830360` - `Interactive trace visualization workbench`

## Goal

Create the first WPF application shell that can load trace datasets and expose the main analysis regions for channels, plots, metadata, and errors.

## Description

The first UI increment should establish the WPF shell, `MVVM` structure, and dependency-injection composition needed for a long-lived trace analysis workbench. The shell should load trace data through the existing filter pipeline and provide a clean main-window composition for later plotting and processing features.

The implementation should not start as a single monolithic WPF project. The preferred baseline is at least a split between:

- one non-UI project for models, services, and where useful view-model-adjacent application logic
- one UI-focused WPF project that contains views, application bootstrap, resources, and UI composition

Additional widget-oriented projects are acceptable when they help keep the shell modular.

The initial window should keep main UI regions simple and separated into dedicated widgets or components, for example a channel browser, a plot area, a metadata or diagnostics pane, and a future processing-configuration editor surface.

User feedback from the first shell baseline indicates that the current UI can feel confusing when editing surfaces are visible before the main load-and-inspect workflow is obvious. The next shell refinement should therefore emphasize a standalone-first workflow in which trace loading, channel selection, and immediate preview or plotting are easier to understand than deeper configuration editing.

## Acceptance Criteria

- A WPF application shell exists in the workspace
- The WPF baseline uses at least a core/application project plus a UI project
- The shell uses MVVM-compatible structure
- Trace loading through shared filters is possible
- Main UI regions for channels, plot, and diagnostics are defined
- A keyboard-oriented main menu exists or is explicitly planned as part of the shell baseline
- The shell makes the primary standalone workflow understandable before advanced editor interaction is required
- The shell leaves a clear integration path for a graphical processing-configuration editor
- Parse errors and metadata have visible UI space
- The composition stays compatible with later extracted widgets and AvaloniaUI-oriented service reuse

## Dependencies

- `830329` - `Filter-based trace intake and export foundation`

## Candidate Tasks

- Create WPF application project and composition root
- Define the initial multi-project split between core/application logic and UI composition
- Define main window layout and region responsibilities
- Integrate trace-loading service and diagnostics surface
- Implement a keyboard-oriented main menu with context-sensitive command behavior
- Refine shell navigation so preview and plot feedback are more prominent than deep editing at first glance
- Reserve composition boundaries for a later graphical processing editor and configuration persistence

## Open Questions

- Which charting control should be used for the first plot region placeholder?
- Should the processing editor open as a pane, dialog, or dedicated workflow page?
- Should the first shell minimize or defer editor emphasis until after channels and preview are visible?
