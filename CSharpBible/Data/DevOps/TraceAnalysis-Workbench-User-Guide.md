# TraceAnalysis Workbench User Guide

## Status

Draft

## Purpose

The `TraceAnalysis Workbench` is intended to support interactive trace inspection and later trace processing in a standalone-capable desktop application.

The current product direction is:

- the workbench must be usable as a standalone app,
- the same app or reusable modules should later be composable into a broader analysis suite,
- the standalone and suite variants should not be developed as separate duplicated products.

## Primary User Goal

The first understandable user journey should be:

1. load a trace file,
2. derive the structural data basis,
3. inspect channels and metadata,
4. review the currently selected processing step,
5. understand the expected outputs,
6. refine the processing configuration,
7. save or export later results.

The workbench should therefore make the data basis visible early and keep deeper editing contextual.

## Data Basis First

Before analysis or processing becomes useful, the workbench needs a data basis from the trace source.

The first load step should provide enough information to derive at least:

- columns,
- column groups,
- field formats,
- field types,
- time base.

Where possible, the workbench may derive this from a trace header or equivalent source metadata before full deep processing is required.

## Current Main Areas of the Workbench

The current WPF shell is organized into the following main regions.

### 1. Trace Source Summary

The top area summarizes the currently loaded source.

It shows:

- configuration title,
- source file or source identifier,
- parse error summary,
- derived time base,
- expandable data-basis table.

Use the **Load Trace** button to open a supported trace file.

### Main Menu

For keyboard-oriented use, the workbench should provide a **MainMenu**.

The menu should:

- expose important commands without requiring mouse interaction,
- provide a stable top-level command structure,
- support standard keyboard navigation,
- adapt its available actions to the currently active widget or context.

This means the menu structure should stay recognizable, while individual commands, enable states, and context-sensitive actions may change depending on whether the user is currently focused on for example:

- the trace source area,
- the channel browser,
- the processing-step list,
- the current-step editor,
- the preview area.

The long-term goal is that the menu becomes one of the primary accessibility and productivity surfaces of the workbench.

### 2. Channel Browser

The left area lists the available channels derived from the currently loaded trace data basis.

It is intended to answer:

- which signals exist,
- how they are grouped,
- which structure was derived from the source.

### 3. Processing Steps

The center-left area lists the processing steps of the current processing configuration.

It allows the user to:

- manage the current processing configuration as a whole,
- review available steps,
- select the current step,
- edit the step id,
- edit the operation name,
- enable or disable a step,
- inspect currently defined outputs.

The selected step is visually highlighted.

The file actions conceptually belongs to this processing-configuration scope rather than to the currently selected step only.

The intended meaning of the file actions is:

- **New** = create a new empty processing configuration
- **Open** = open an existing processing configuration
- **Save** = save the current processing configuration
- **Save As** = save the current processing configuration under a new file name

This does **not** mean:

- `New Step`
- `Add Step`

Those step-specific actions should later be exposed separately and named explicitly.

### 4. Current Step

The center-right area shows the properties of the currently selected step.

This area is intentionally closer to the step list so selection and editing stay visually connected.

The section is currently organized with expanders so multiple aspects remain available without forcing the user into tabs.

It contains:

- step header information,
- **Inputs**,
- **Parameters**,
- **Outputs**.

Note: if file actions are visually shown near the current-step area in an early UI baseline, they should still be understood as configuration-level actions, not as actions for one specific step.

### 5. Preview / Summary

Below the current-step editor, the workbench shows a lightweight preview summary.

At the current implementation level, this summary is not a full chart yet. Instead it helps the user understand:

- which step is selected,
- which operation is configured,
- which outputs are expected.

The long-term intent is to evolve this area into stronger plot or preview feedback after channel selection and processing edits.

### 6. Validation and Diagnostics

The right area shows diagnostic information.

This area is intended to separate:

- source-related issues,
- processing-related issues.

The long-term UX direction is that validation should also appear contextually near the edited control, while important issues remain mirrored in a central diagnostics area.

## How to Use the Workbench

### Load a Trace

1. Start the workbench.
2. Use **Load Trace**.
3. Select a supported trace file.
4. Review the top summary area.
5. Expand **Data Basis** when you need to inspect the derived structure in detail.

After loading, verify that:

- the source is correct,
- the parse error summary is acceptable,
- the time base looks plausible,
- the derived field structure matches the expected trace content.

### Inspect the Derived Structure

After loading a trace, inspect:

- the channel list,
- the group information,
- type and format information in the data-basis table.

This step is important because later processing and preview behavior depend on the correctness of the derived structure.

### Work with Processing Steps

Before editing individual steps, understand that the processing-step list belongs to one processing configuration.

Configuration-level actions mean:

- **New** creates a new empty configuration
- **Open** loads an existing configuration
- **Save** stores the current configuration
- **Save As** stores the current configuration under a new file name

Step-level actions should later be named separately, for example:

- `Add Step`
- `Remove Step`
- `Duplicate Step`

Where suitable, these actions should also be reachable from the main menu when the processing-step area is the active context.

1. Select a step from the **Processing Steps** list.
2. Review the **Current Step** section.
3. Expand or collapse `Inputs`, `Parameters`, and `Outputs` as needed.
4. Edit the current step values.
5. Use the configuration-level file actions to create, open, or save the full processing configuration.

### Review the Expected Result

After selecting or editing a step, review the **Preview / Summary** area.

At the current stage, this preview is descriptive rather than graphical. It is meant to explain the expected effect of the selected step before a richer preview or plot is introduced.

## Current Limitations

The current implementation is still an early baseline.

Important current limitations include:

- the preview is still a summary rather than a real plot,
- input editing is still simplified,
- live validation is not yet fully contextual,
- the visual placement of file actions may still suggest a step-local meaning even though they belong to the full configuration,
- not all editor behaviors are finalized,
- the UX will continue to be refined so the primary workflow becomes easier to understand.

## Intended UX Direction

The intended next UX refinements are:

1. keep the data-basis acquisition clearly visible,
2. strengthen the coupling between channel selection and preview,
3. improve contextual validation,
4. keep processing editing understandable without overloading the screen,
5. integrate a keyboard-friendly main menu with context-sensitive actions,
6. preserve reusability for later suite composition.

## Main Menu Direction

The main menu should later be structured so that users can discover commands by category first and by active context second.

Candidate top-level categories include:

- `File`
- `Trace`
- `View`
- `Processing`
- `Help`

Context adaptation should then refine:

- which commands are enabled,
- which commands are visible,
- which commands refer to the active widget,
- which shortcuts are currently meaningful.

The menu should therefore behave as a shell-level command surface, not as a hard-coded static list.

## Recommended First-Time User Flow

For first-time use, follow this order:

1. load a trace,
2. inspect the data basis,
3. inspect channels,
4. select a processing step,
5. review the current step,
6. review the preview summary,
7. save the configuration if needed.

## Notes for Later Suite Integration

When the workbench is later hosted inside a larger suite, the suite should still preserve the same core workflow:

1. acquire a data basis,
2. reveal the trace structure,
3. inspect channels,
4. review or edit processing,
5. review the result.

Only the surrounding shell and navigation should change, not the core user journey.
