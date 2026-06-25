# Task T-Terminal-002 - Implement platform-neutral terminal core

## Status

Done

## Goal

Provide the platform-neutral terminal domain model, buffer, parser, session contracts, and backend factory abstractions.

## Scope

- Add terminal model types for cells, cursor, size, colors, and snapshots
- Add a terminal buffer implementation with resize and scrollback behavior
- Add a small ANSI/VT parser for core text, CR/LF, and a first cursor/control subset
- Add session contracts and backend selection abstractions

## Out of Scope

- Native Windows or Posix bindings
- Avalonia rendering

## Done Criteria

- `Terminal.Core` exposes the required contracts and model types
- A minimal parser updates the buffer from VT text streams
- Factory abstractions support platform-specific session creation
