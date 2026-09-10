# B-Code-Navigation

## Parent
F-Shared-CodeStudio-Components

## Status
Done

## Request
Provide a UI-neutral source-location contract and a CodeStudio editor-host implementation able to open, focus, and position the caret in a document.

## Acceptance criteria
- A location represents path plus optional line, column, and offset without AvaloniaEdit dependencies.
- Consumers use an interface rather than direct editor/tab access.
- Invalid paths and coordinates produce defined results and diagnostics.

## Tasks
T-005 Done, T-006 Done, T-007 Done

## Gates
G-Architecture after T-005; G-Component-Quality after T-007.