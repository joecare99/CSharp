# Task T-Terminal-005 - Implement Avalonia terminal user control

## Status

Done

## Goal

Implement an Avalonia `UserControl` that renders the terminal buffer and forwards user input to a terminal session.

## Scope

- Add a terminal view model using CommunityToolkit.Mvvm
- Add an Avalonia user control for rendering rows, colors, and cursor state
- Add focus, keyboard input, resize handling, and session lifecycle hooks

## Out of Scope

- Selection, hyperlinks, tabs, and advanced mouse reporting
- Theming beyond a first default palette

## Done Criteria

- `Terminal.Avalonia` exposes a reusable user control
- The control binds to the terminal view model and session
- Buffer updates can be rendered in Avalonia
