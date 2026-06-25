# Task T-Terminal-004 - Implement Posix PTY backend

## Status

Done

## Goal

Implement a Linux/macOS terminal backend based on a Posix PTY for the terminal session abstraction.

## Scope

- Add PTY and process-launch interop declarations
- Add read, write, resize, and shutdown behavior through the session contract
- Keep the implementation compile-safe on non-Posix hosts

## Out of Scope

- Full shell profile management
- Deep terminal feature negotiation

## Done Criteria

- `Terminal.Backends.Posix` builds successfully
- A Posix session type implements the terminal session contract
- The backend can be selected by the platform-aware factory
