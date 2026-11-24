# Avln Image Editor

The reusable Avalonia control and the standalone host implement 90° clockwise and counterclockwise rotation (the **Rotate left** button) and horizontal (left/right) and vertical (top/bottom) flipping, with undo/redo. Brush, eraser, fill and crop are disabled and marked as unimplemented; this is not a full image editor. Select and zoom do not change pixels.

## Run and test

From PowerShell, with .NET 10 SDK installed, replace the example artifact path with a private absolute path for this build. The wrapper's explicit `HostArtifactRoot` must appear after its default argument:

```powershell
$wrapper = 'C:\Projekte\CSharp\Avalonia_Apps\AA98_AvlnCodeStudio\tools\build\dotnet-isolated.ps1'
$root = 'C:\Projekte\CSharp\Avalonia_Apps\Avln_ImageEditor'
$artifacts = '-p:HostArtifactRoot=C:\Projekte\CSharp\Avalonia_Apps\Avln_ImageEditor\.artifacts\shared-editor'
& $wrapper build "$root\Avln_ImageEditor.Host\Avln_ImageEditor.Host.csproj" -f net10.0 '-p:TargetFrameworks=net10.0' $artifacts
& $wrapper test "$root\Avln_ImageEditor.Controls.Tests\Avln_ImageEditor.Controls.Tests.csproj" -f net10.0 '-p:TargetFrameworks=net10.0' $artifacts
& $wrapper run --project "$root\Avln_ImageEditor.Host\Avln_ImageEditor.Host.csproj" -f net10.0 '-p:TargetFrameworks=net10.0' $artifacts
```

The projects also target net8.0 and net9.0, which require their corresponding runtimes/test hosts; net10.0 is the primary tested target. Restore may report NU1701 dependency compatibility warnings; these are distinct from a missing or incompatible test runtime.

The image tests use `HeadlessUnitTestSession` with `AppBuilder.Configure<App>().UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false }).UseSkia()`, and dispatch image/window work to its UI thread. Their asymmetric transparent fixture is encoded as a real PNG and decoded again. Integration tests can check `editor.ActiveDocument!.PixelSize`, call `editor.RotateRight()`/`RotateLeft()`/`FlipHorizontal()`/`FlipVertical()`/`Undo()`/`Redo()` or execute their corresponding commands after `CanExecute(null)`, and compare pixel buffers via `Bitmap.CopyPixels` into a locked `WriteableBitmap`. The headless standalone-window test clicks the editing buttons, tests dirty-close cancellation, and exports programmatically; it does not automate the native file pickers.

In the host, open a PNG, JPEG or BMP, rotate or flip horizontally or vertically, undo/redo and select **Export PNG to new file**. The host asks before replacing or closing a dirty document. Clear is disabled in this host. Save/export is PNG only, including for JPEG and BMP inputs; original JPEG/BMP formats are never written. **PNG re-encoding may discard metadata**, color profiles, EXIF orientation and other ancillary information. It is not a metadata-preserving conversion. The PNG export is available even without editing and *always re-encodes*, including on an unmodified original PNG.

## Integration contract

`Avln_ImageEditor.Controls` does not reference either the standalone host or any IDE. Create `ImageEditorViewModel`, bind it to `ImageEditorControl`, and load using either existing `LoadDocument(byte[], string)` or `LoadDocument(ImageEditorDocument)`. The document overload transfers ownership on success. A successful load replaces the document, clears history, starts clean and resets zoom; an invalid load does not replace the current document. Assigning `ActiveDocument` also loads or clears it. The host must ask about unsaved changes **before** calling either load overload, assigning `ActiveDocument`, or clearing/disposing. Set `AllowClear = false` when the enclosing host owns document state. Its default is true; the command, direct `ClearDocument()` and a null `ActiveDocument` assignment honor it. `Dispose()` always releases the document.

Use the view model and callbacks on the Avalonia UI thread, without reentrant mutations from event/property-change handlers. Treat the owned document and exposed image as read-only: external mutation of a caller-supplied bitmap bypasses the editor's history, dirty tracking and event.

* `ContentChanging` is raised immediately before an actual pixel-state change by rotation, horizontal or vertical flip, undo or redo, not for load, clear, zoom, rename, export or a no-op rotation/flip (including images symmetric about the relevant axis).
* `IsDirty` compares the current history state to the last saved state. Undo back to the saved state becomes clean; redo/other edits become dirty. History retains up to nine states including the current state; new edits after undo discard redo. State identity, not pixel-by-pixel comparison with the saved state, determines dirty status.
* `CapturePngSnapshot()` throws if no document is loaded or encoding fails; otherwise it **always encodes the displayed pixels** to PNG. `ImageEditorSaveSnapshot.Bytes` returns a defensive copy. It is bound to the document identity and history revision captured at that instant.
* `IsCurrentDocument(snapshot)` checks that identity, including after edits but not after clear, load or disposal. On the UI thread, check it immediately before assigning a completed Save-As path; do not compare `ActiveDocument` references, which change on undo/redo/rotation.
* Only after completely and successfully writing those exact bytes call `MarkSaved(snapshot)`. A later edit remains dirty; undoing back to that captured revision makes it clean. A snapshot from a replaced/cleared document is ignored. Never modify the byte copy before writing.
* After a successful Save As, `RenameDocument(snapshot, name)` updates the summary without a pixel mutation or dirty change. The snapshot guards document identity, so a replacement document cannot be renamed. It can rename the same document even if its revision changed during writing. Name must be nonempty.

The library deliberately performs no file I/O or conflict handling; integrating hosts choose their own file-writing policy and handle errors. The host-specific public method `MainWindowViewModel.ExportPngToNewFileAsync(path)` creates an unpredictable temporary sibling exclusively, writes and closes it, then moves it to the new local `.png` destination without overwrite before marking/renaming the snapshot. On ordinary write errors, cancellation or move conflicts it deletes its temporary sibling and never marks the snapshot saved. An abrupt process termination, external interference or a cleanup failure can still leave a temporary file; cleanup failures are reported. This is not a host-neutral file service. Picker export rejects nonlocal paths. Existing filenames or other I/O failures are reported in the host status. External processes can still rename, replace or delete paths during/after writing; this is not external-change detection, durable-fsync, or a race-free guarantee. Pick a different new filename to export again.

## Decode and pixel limits

Encoded input is restricted to PNG, JPEG and BMP headers with at most 32 MiB of encoded data, dimensions from 1 through 8192 each, and at most 4,194,304 pixels. `ImageDecodeLimits.ValidateEncoded` checks the PNG IHDR, JPEG first SOF, or BMP DIB declared dimensions before `new Bitmap(stream)` invokes the Avalonia decoder; decoded bitmap dimensions are checked too. Preflight rejects malformed/unsupported headers and over-limit inputs with `InvalidDataException`. Decoder errors propagate with platform-dependent exception types and leave the old document intact. This is not a complete integrity check: the platform decoder decides whether malformed/truncated data is acceptable. The standalone host checks seekable stream length before reading; for nonseekable streams it buffers at most the limit and reads at most one extra byte to detect overflow. A storage provider may perform its own I/O before returning a stream. The preflight limits declared geometry and input size, **not all native decoder allocations or CPU time**, nor behavior on contradictory malicious headers; the platform decoder is not a security sandbox for hostile files. Caller-supplied bitmap documents have already been decoded before this library can check their dimensions.

Rotation and both flips work on BGRA premultiplied-alpha pixels. Visual transparency is retained, but RGB hidden under fully transparent pixels and exact original color/profile/metadata representation are not guaranteed. Platform decoder format support and color handling apply. Decoding/encoding and a bounded undo history use additional memory; limits are not a total-process memory guarantee.
