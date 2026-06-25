# Task T-Terminal-003 - Implement Windows ConPTY backend

## Status

Done

## Goal

Implement a Windows terminal backend based on ConPTY for the terminal session abstraction.

## Scope

- Add ConPTY P/Invoke definitions
- Add process creation and pipe management for hosted shells
- Expose read, write, resize, and shutdown operations through the session contract

## Out of Scope

- Full terminal emulation in the backend
- Advanced Windows-specific shell profiles

## Done Criteria

- `Terminal.Backends.Windows` builds successfully
- A ConPTY session type implements the terminal session contract
- The backend can be selected by the platform-aware factory
