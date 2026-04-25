# DevOps Planning

This directory contains local planning artifacts that mirror an Azure DevOps backlog structure.

## Structure

- `Epics` contains large work packages
- `Features` contains slices of epic scope
- `BacklogItems` contains actionable backlog entries
- `Tasks` contains implementation-level work items

## Current Focus

- Epic `E-DC-FRONIUS-001` - Deliver Fronius inverter monitoring with reusable data access and a dedicated dashboard UI

## Active Refinement Sequence

1. Clarify the target Fronius endpoint contract and access constraints
2. Extract reusable inverter access services from the current console-only prototype
3. Add robust polling, mapping, and error handling for inverter, battery, and energy-flow state
4. Create a separate MVVM-based UI project for a modern monitoring dashboard
5. Validate the solution with automated tests and a manual device integration pass
