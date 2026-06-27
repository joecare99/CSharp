# Avln Image Editor

`Avln_ImageEditor` contains an initial reusable Avalonia image editing control and a small desktop host for manual testing.

## Projects

- `Avln_ImageEditor.Controls` provides the OS-agnostic control, document model, editor tool state, and ViewModel.
- `Avln_ImageEditor.Host` provides a desktop shell that uses Avalonia storage APIs to select an image and load it into the control.
- `Avln_ImageEditor.Controls.Tests` validates the host-independent editor state.

## Current Scope

The first slice focuses on a clean component boundary rather than full Paint.NET feature parity. It includes:

- Image document loading from encoded bytes.
- A reusable editor surface with zoom controls.
- Initial tool selection state for select, brush, eraser, fill, and crop.
- A desktop host with rudimentary image file selection.

## Architecture Notes

The control library does not use platform-specific file system dialogs. Hosts are responsible for acquiring files or streams and passing image bytes to the editor ViewModel. This keeps the editor reusable for desktop, browser, mobile, and future suite-host scenarios.
