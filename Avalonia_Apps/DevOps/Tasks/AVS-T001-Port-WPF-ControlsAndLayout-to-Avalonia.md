# Task AVS-T001 - Port WPF ControlsAndLayout sample to Avalonia

## Status
Done

## Goal
Create a new Avalonia.UI sample project `Avln_ControlsAndLayout` based on `AvlnSamples/WPF_ControlsAndLayout/WPF_ControlsAndLayout_net.csproj`.

## Scope
- Added `AvlnSamples/Avln_ControlsAndLayout/Avln_ControlsAndLayout.csproj`.
- Added Avalonia bootstrap files, main window, sample browser view, view models, model records, and sample source snippets.
- Ported common WPF layout/control examples to Avalonia equivalents.
- Omitted WPF-only samples that do not have direct Avalonia controls, including `InkCanvas`, `FlowDocumentPageViewer`, `Table`, and `RichTextBox`.
- Added Avalonia-specific examples for `SplitView`, `ToggleSwitch`, `CalendarDatePicker`, `TransitioningContentControl`, and `NativeMenu`.
- Added `AvlnSamples/Avln_ControlsAndLayoutTests/Avln_ControlsAndLayoutTests.csproj` with MSTest coverage for the runtime AXAML pipeline and sample snippets.

## Validation
- `dotnet build C:\Projekte\CSharp\Avalonia_Apps\AvlnSamples\Avln_ControlsAndLayout\Avln_ControlsAndLayout.csproj --configuration Debug` succeeded for `net8.0`, `net9.0`, and `net10.0`.
- `dotnet test C:\Projekte\CSharp\Avalonia_Apps\AvlnSamples\Avln_ControlsAndLayoutTests\Avln_ControlsAndLayoutTests.csproj --configuration Debug` succeeded: 24 total, 24 passed, 0 failed, 0 skipped across `net8.0`, `net9.0`, and `net10.0`.
- A full workspace build was not used as the acceptance gate because existing unrelated solution errors remain in iOS/Android target framework configuration and in the old WPF sample project reference.

## Notes
- The runtime preview uses a single pipeline: editable AXAML text is parsed through `AvaloniaRuntimeXamlLoader.Parse(...)`, and the parsed control is displayed.
- The old WPF `XmlDataProvider`/`Frame` pattern was replaced with MVVM-friendly typed sample metadata plus runtime AXAML rendering.
