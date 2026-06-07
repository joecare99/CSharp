# ConsoleLib.WinForms

WinForms-backed widget host for `ConsoleLib`.

## Purpose
- Materialize real `System.Windows.Forms` controls for existing `ConsoleLib` control descriptions.
- Keep activation IoC-based through `ConsoleFramework.WidgetSet`.
- Provide a host `Form` / `UserControl` for `ConsoleLib.CommonControls.Application`.

## Current Slice
- Project scaffold for the WinForms widget set.
- References `ConsoleLib` and targets classic .NET Framework plus modern `-windows` TFMs.
- Native host, registry, and control adapters will be added incrementally.
- `MenuBar` / `MenuItem` are mapped through a native WinForms menu path (`MenuStrip` / `ToolStripItem`) instead of only generic `Control` widgets.
