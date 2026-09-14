# ExtendedConsoleSim

`RedirectedConsole` is the stdio-backed `IExtendedConsole` implementation used
when a process has redirected output. It never accesses native console window
properties.

Input uses standard terminal sequences:

- printable characters, Enter, Tab, Backspace and Escape become key events;
- `CSI A/B/C/D`, `CSI H/F`, `CSI 3~`, `CSI 5~` and `CSI 6~` map to navigation
  keys;
- SGR mouse sequences (`CSI <button;x;yM`/`m`) become mouse events;
- `CSI 8;rows;columnst` reports a simulated resize;
- EOF raises `IInputEndNotification.EndOfInput`, allowing the host to stop
  deterministically.

Initial dimensions come from `COLUMNS` and `LINES`, with 80x25 defaults.
