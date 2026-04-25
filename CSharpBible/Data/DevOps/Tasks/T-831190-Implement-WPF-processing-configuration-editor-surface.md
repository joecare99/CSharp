# Task T-831190 - Implement WPF processing configuration editor surface

## Status

Draft

## Parent

- Backlog Item `BI-830364` - `Create graphical processing configuration editor`

## Goal

Implement the first WPF editor surface for creating, editing, validating, and saving processing configurations.

## Scope

- Implement the step-list-based editor surface inside the WPF workbench
- Implement selected-step editing for operation choice, inputs, parameters, and outputs
- Implement support for single-output and fixed multi-output operations
- Implement generated output rows with semantic output-role visibility
- Implement save, load, and save-as interactions for configuration files
- Integrate validation feedback into the editor workflow

## Out of Scope

- Full advanced graph-based authoring
- Rich preview plotting of editor changes
- Console-runner implementation

## Implementation Notes

- Prefer a hybrid workflow with step list plus selected-step detail surface
- Auto-generate known outputs for fixed multi-output operations
- Keep output-role identity visible even when users rename output channels
- Reuse shared configuration serialization and validation services where available

## Test Strategy

- Verify create, edit, save, and reload workflows
- Verify validation for duplicate outputs, missing roles, and incomplete parameters
- Verify multi-output editing for operations such as `sinCos` and `bitSplit`
- Verify saved artifacts remain compatible with the console-runner contract

## Done Criteria

- Editor surface exists in the WPF shell
- Step editing flow works for first-increment operations
- Save/load behavior exists for configuration artifacts
- Validation feedback is integrated into the editing flow
- Multi-output authoring is supported in the first increment
