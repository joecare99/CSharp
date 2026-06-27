# Avln Image Editor

## Overview

Create a reusable OS-agnostic image editing control inspired by Paint.NET and a host application that can exercise the control with rudimentary file selection.

## Architecture Decision

The reusable editor is implemented in `Avln_ImageEditor.Controls`. It owns editor state, tool selection, zoom state, and the loaded image document. It does not own platform file selection.

The desktop test host is implemented in `Avln_ImageEditor.Host`. It uses Avalonia's storage provider abstraction to select local image files and passes encoded bytes to the reusable control ViewModel.

## Implemented Task

Status: Done

- Created the `Avln_ImageEditor` project group.
- Added `Avln_ImageEditor.Controls` as the OS-agnostic control library.
- Added `Avln_ImageEditor.Host` as a desktop test host.
- Added `Avln_ImageEditor.Controls.Tests` for initial host-independent ViewModel tests.
- Registered the new projects in the solution.

## Backlog

- Add a layered image model with explicit layer ordering and visibility.
- Add command-based edit operations with undo and redo support.
- Add real brush, eraser, crop, and fill implementations.
- Add image export through host-owned storage providers.
- Add integration tests for the Avalonia control surface where practical.
