# AA00-T003 Native WinForms Menu Mapping for ConsoleLib.WinForms

## Scope
Introduce a backend-specific native menu path in `ConsoleLib.WinForms` so that `MenuBar` and `MenuItem` are represented by WinForms menu components instead of generic panel/button controls.

## Parent
- AA00 - ConsoleLib / WinForms widget host ongoing implementation slice

## Summary
`ConsoleLib.WinForms` now contains a dedicated menu mapping path:
- `MenuBar` -> `MenuStrip`
- normal `MenuItem` -> `ToolStripMenuItem`
- separator `MenuItem` -> `ToolStripSeparator`
- `MenuPopup` -> structural dropdown representation via parent `ToolStripMenuItem.DropDownItems`

## Assumptions
- Native WinForms menus are backend-specific and do not need to preserve the console layout behavior one-to-one.
- `MenuPopup` is treated as a structural menu node in the WinForms backend rather than as a normal `Control`.
- Existing non-menu ConsoleLib controls continue to use the prior `Control` registry path.

## Notes
- The current implementation keeps menu registrations local to `WinFormsWidgetSet` instead of widening `NativeWidgetRegistry` beyond `Control` objects.
- A later refinement may extract native menu registration into a dedicated helper if more hosted non-control widget types are introduced.
- `ConsoleLib` core has since been slimmed down further, and the concrete ExtendedConsole-backed console widget set is being hosted separately in `ConsoleLib.ExtCon`.

## Validation
- File-level validation completed for the changed WinForms widget-set file.
- Full project build should be re-run in a follow-up validation pass because one direct terminal build was cancelled.
