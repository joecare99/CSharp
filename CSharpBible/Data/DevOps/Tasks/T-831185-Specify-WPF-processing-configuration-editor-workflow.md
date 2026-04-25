# Task T-831185 - Specify WPF processing configuration editor workflow

## Status

Draft

## Parent

- Backlog Item `BI-830364` - `Create graphical processing configuration editor`

## Goal

Define the first graphical authoring workflow in WPF that helps users create, validate, and save processing configurations.

## Current Domain Context

- Processing configurations are intended to be authored graphically and later executed unchanged by a console-oriented runner
- The processing format now supports both single-output and multi-output operations
- Multi-output examples such as bit extraction and `sin`/`cos` decomposition increase the complexity of the editor workflow
- The first increment should stay discoverable and predictable despite that added flexibility
- The preferred implementation baseline is not a single UI project, but at least a split between non-UI application logic and a dedicated UI project

## Scope

- Define the first editing experience shape, for example wizard, step list, property grid, or hybrid workflow
- Define user flow for selecting source channels and adding processing steps
- Define user flow for naming derived channels and reviewing units or provenance hints
- Define how the editor handles operations with one or more outputs
- Define how semantic output roles such as `sin`, `cos`, or `bit0` are shown and validated
- Define validation feedback behavior for incomplete or inconsistent steps
- Define load, save, and save-as behavior for processing configuration files
- Define how the editor integrates with the main trace-analysis shell without overloading the main window
- Define accessibility and keyboard-navigation expectations for the first increment

## Out of Scope

- Full implementation of the editor
- Final chart interaction design
- Advanced graph-based editing surfaces in the first increment

## Implementation Notes

- Prefer a simple, discoverable workflow over maximum flexibility in the first increment
- Keep editor logic in MVVM-friendly services and view models
- Separate core configuration validation from UI-specific presentation
- Ensure the produced artifact matches the shared processing configuration format
- Avoid a design that assumes every step produces exactly one channel
- Prefer progressive disclosure so multi-output details are visible when needed but do not overwhelm simple single-output authoring flows

## Arguments Against Multi-Output Support in the First Editor Increment

- The editor becomes more complex because users must understand output cardinality per operation
- Validation becomes richer because output-role completeness and duplicate output names must be checked
- The view-model structure becomes less trivial than a simple one-step-one-output form
- Preview UX may be harder when one step produces several new channels at once
- Accessibility and keyboard flow need more care when outputs become nested or repeatable collections

## Arguments For Multi-Output Support in the First Editor Increment

- The processing model matches real signal-processing use cases such as bit decoding and trigonometric decomposition
- The editor avoids an artificial mismatch between UI and execution model
- A uniform `outputs` collection is easier to extend later than retrofitting multi-output support after shipping
- Later operations such as coordinate decomposition, protocol decoding, or vector expansion fit naturally into the same model

## Recommended First-Increment Editor Strategy

- Support multi-output operations in the model and workflow from the start
- Keep the UI simple by distinguishing clearly between:
  - single-output operations
  - fixed multi-output operations
- For fixed multi-output operations, auto-generate default outputs and roles when the operation is selected
- Let users rename generated output channels, but validate required roles and uniqueness
- Defer highly dynamic graph-style editing until a later increment

## Recommended WPF Shell Baseline

The first increment should use a main workbench window with clear, separable regions instead of a monolithic editor surface.

Recommended shell regions:

1. **Trace source region**
   - currently loaded trace source
   - load / reload actions
   - basic source metadata and parse-error indicator
2. **Channel browser region**
   - source and derived channels
   - search, grouping, and selection support
3. **Processing step editor region**
   - editable ordered step list
   - operation details for the selected step
4. **Output definition region**
   - single-output or multi-output rows for the selected step
5. **Validation and diagnostics region**
   - blocking errors
   - warnings
   - informational hints
6. **Preview / summary region**
   - lightweight summary of expected derived outputs
   - optional later preview integration with plot or sample rows

This region-based composition keeps the main window manageable and aligns with later extraction of dedicated widgets.

## Recommended MVVM Baseline

The first increment should keep one class per file and use small focused view models.

Recommended top-level view models:

- `MainWorkbenchViewModel`
  - coordinates file loading, active configuration, and shell state
- `ProcessingConfigurationEditorViewModel`
  - owns the editable processing configuration
- `ProcessingStepListViewModel`
  - manages ordered step collection and selection
- `ProcessingStepDetailViewModel`
  - edits operation, parameters, and input selection for the active step
- `ProcessingOutputsViewModel`
  - edits output rows, roles, names, and units
- `ValidationSummaryViewModel`
  - exposes errors, warnings, and informational validation items

Recommended service boundaries:

- configuration serialization service
- configuration validation service
- operation metadata service
- trace channel discovery service

This keeps UI state separate from processing logic and supports later Avalonia reuse.

## Recommended Project Split Baseline

The first increment should prefer at least two projects:

1. **Core/Application project**
   - processing-configuration editor models
   - services
   - validation logic
   - shell-facing orchestration services
   - optionally non-UI view-model or presentation-neutral state where this improves reuse
2. **WPF UI project**
   - `App`
   - windows
   - views
   - resources
   - UI composition and bindings

Optional later projects:

- widget or controls project for reusable UI widgets
- dedicated view-model project if the solution later benefits from a stricter split

This structure reduces coupling and keeps later Avalonia migration options cleaner.

## Recommended Editing Experience Shape

The first increment should prefer a **hybrid workflow**:

- left side: ordered step list
- center: selected step editor
- right or bottom: outputs and validation summary

This is preferable to a full wizard because:

- users can revisit earlier steps easily
- step order remains visible
- multi-output operations can be edited in context
- save/load behavior stays intuitive for longer configurations

## Proposed Step Authoring Flow

1. User opens or creates a processing configuration
2. User loads or selects a trace source for channel discovery
3. User adds a new step to the ordered list
4. User selects an operation
5. Editor shows input selectors and operation parameters
6. Editor auto-generates default output rows when the operation defines known outputs
7. User adjusts names, units, and optional descriptions
8. Validation updates live in the diagnostics region
9. User saves the configuration once blocking issues are resolved

## Proposed Behavior for Simple Versus Complex Operations

### Simple single-output operations

- show one compact output row
- minimize UI chrome
- keep parameter editing close to the input selectors

### Fixed multi-output operations

- expand an output list automatically
- lock or guide output roles when they are semantically required
- allow renaming of channel names but keep role identities visible

### Future advanced operations

- reserve extensibility for richer parameter editors
- avoid implementing graph-based editing in the first increment

## Recommended Validation Interaction Model

- Validate continuously as users edit
- Show blocking issues inline near the edited control when possible
- Mirror all issues in a central diagnostics list
- Do not block navigation between steps because of errors in one step
- Block save only when blocking validation issues remain

## Save and Reload Baseline

- `New` creates an empty configuration with default metadata
- `Open` loads an existing configuration file
- `Save` writes to the current file path
- `Save As` writes to a new file path
- Reloading should preserve selected step and editor context where practical after successful parse and validation

## Accessibility Baseline

- keyboard-first navigation across step list, inputs, parameters, and outputs
- clear labels for generated output roles
- validation messages reachable without relying on color only
- stable tab order within repeatable output collections

## Open Structural Recommendation

The first project structure derived from this task should likely separate at least:

- one non-UI core/application project containing:
  - `Services`
  - `Models`
  - validation logic
  - reusable orchestration logic
- one WPF UI project containing:
  - `Views`
  - UI-facing `ViewModels` or bindings
  - `Resources`

Additional widget projects are acceptable when a reusable control library becomes beneficial.

Core processing configuration and validation logic should remain outside UI-specific types.

## Proposed Workflow Baseline

1. User adds a processing step
2. User selects an operation
3. Editor determines the operation category:
   - single-output
   - fixed multi-output
4. Editor shows:
   - input channel selectors
   - operation parameters
   - generated output rows
5. User edits output names, optional display names, and units
6. Editor validates:
   - required inputs
   - required parameters
   - required output roles
   - unique output channel names
7. User saves the configuration

## Proposed UI Behavior for Multi-Output Steps

- Show outputs as a reorder-stable list or table within the selected step
- Display an explicit output role column when the operation defines semantic roles
- Pre-populate role-specific outputs for known operations:
  - `sinCos` -> `sin`, `cos`
  - `bitSplit` -> configured or generated bit roles such as `bit0`, `bit1`, `bit2`
- Mark generated outputs as editable where renaming is safe
- Prevent deletion of mandatory outputs for fixed multi-output operations unless the operation parameters are changed accordingly

## Validation Baseline for the Editor

The editor should surface at least:

- missing input selection
- missing parameter values
- duplicate step ids
- duplicate derived output channel names
- incomplete required output-role set
- invalid bit index configuration
- unsupported angle-unit settings for trigonometric decomposition

Validation should distinguish:

- blocking errors
- non-blocking warnings
- informational provenance or unit hints

## Decision Areas

- Pane versus dialog versus dedicated page workflow
- How users preview the effect of a configured step before saving
- Which configuration details require immediate validation versus deferred validation
- Whether advanced text editing is required as an escape hatch in the first increment
- How fixed multi-output steps should generate and maintain default output rows

## Test Strategy

- Define scenario-based workflow tests for create, edit, validate, save, and reload
- Define accessibility and keyboard navigation expectations
- Define compatibility checks against the console runner contract
- Define workflow tests for single-output and multi-output step authoring
- Define validation cases for required output roles and duplicate generated output names

## Done Criteria

- First editor workflow is defined
- Validation interaction is defined
- Save and reload flow is defined
- Shell integration boundaries are defined
- Compatibility with the shared configuration format is documented
- Multi-output authoring behavior is documented
