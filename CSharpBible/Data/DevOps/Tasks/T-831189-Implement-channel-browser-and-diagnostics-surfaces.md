# Task T-831189 - Implement channel browser and diagnostics surfaces

## Status

Draft

## Parent

- Backlog Item `BI-830362` - `Create WPF trace analysis shell`

## Goal

Implement the first shell surfaces that expose loaded channels, source metadata, parse errors, and validation diagnostics.

## Scope

- Implement a channel browser surface for source and later derived channels
- Implement basic search or filtering support for channels
- Implement metadata display for the loaded trace source
- Implement diagnostics surfaces for parse errors and validation issues
- Define selection flow from channel browser into later editor or preview regions

## Out of Scope

- Full plot visualization
- Final derived-channel preview behavior
- Full processing editor implementation

## Implementation Notes

- Keep the channel browser view-model-driven and reusable
- Separate source parse errors from processing-validation diagnostics
- Keep diagnostics presentation informative without relying on color only
- Keep grouping support simple in the first increment

## Test Strategy

- Verify channel discovery results can be displayed
- Verify empty, loaded, and error states are rendered
- Verify diagnostics can display blocking errors, warnings, and informational hints

## Done Criteria

- Channel browser surface exists
- Metadata and parse-error display exists
- Validation diagnostics surface exists
- Selection and diagnostics states are represented in a maintainable way
