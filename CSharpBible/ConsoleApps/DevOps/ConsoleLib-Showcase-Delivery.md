# ConsoleLib Showcase delivery

## Native gallery

```powershell
dotnet run --project ConsoleLib.Showcase\ConsoleLib.Showcase.csproj -f net8.0-windows
```

Run it from a real console because `ExtendedConsole` requires native console
input events. The default launch now opens the renderer-first virtual desktop.
The former component gallery and its ConPTY bridge remain in the project as
the legacy gallery implementation while the desktop host is validated.

## Portable virtual desktop

The renderer-first desktop is a separate `net8.0` project and is currently
hosted through composition roots that provide the widget set:

```powershell
dotnet test ConsoleLib.Showcase.Desktop.Tests\ConsoleLib.Showcase.Desktop.Tests.csproj --no-restore --disable-build-servers
```

Its application pages are embedded CXAML and render through the canonical
`AttachedRenderService` frame pipeline. The desktop currently exposes:

- Calendar with month navigation and a generated month grid.
- Calculator backed by the shared calculator domain engine.
- Calculator keypad with decimal, sign, backspace, and four operations.
- Notepad with document statistics, explicit clipboard availability, and an
  injected clipboard capability.
- Character Table with ASCII navigation and optional clipboard copy.
- Analog Clock with a live semi-analog face, moving second marker, and alarm controls.
- Terminal page with automatic host-session startup, visible startup/failure status,
  and forwarded shell snapshot output.
- About page describing the renderer-first architecture.

`ConsoleLib.Showcase.Desktop` is provider-neutral. Clipboard, terminal
sessions, alerts, and pointer support are supplied by a selected host through
capability contracts; no Windows APIs or P/Invoke are used by the portable
pages and ViewModels.

The native Showcase composition root now registers the ExtCon capability
adapter. It advertises native mouse and terminal support, keeps clipboard
optional, and maps the clock alarm to an interactive console beep while
remaining silent when output is redirected.

The native entry point explicitly selects UTF-8 for both console input and
output before creating the ExtendedConsole host. ExtendedConsole also applies
the UTF-8 Windows code page and enables processed/VT output on the native
console handle. This is required for the Braille clock renderer
(`U+2800`-`U+28FF`); a legacy Windows code page renders those cells as question
marks. A terminal font with Braille glyph support is also required for visible
dot patterns. If copying the clock into an editor shows the correct Braille
glyphs but the live console does not, the data path is working and the active
console host or its configured font is the remaining limitation; Windows
Terminal with a Unicode font is recommended.

The Clock page now uses a `ConsoleDisplay`-style Picture/Pixel control with a
28x28 pixel buffer rendered in 14 console rows. Each cell uses `▄` with its
foreground and background colors representing the lower and upper pixel. The
ring is yellow/gold, the cardinal markers (`12`, `3`, `6`, `9` positions) are
white, and the four rasterized hands are gray (hour), white (minute), green
(seconds), and red (alarm). This avoids Braille font dependencies while
retaining a high-resolution animation demonstration.

Terminal support is exposed to the portable desktop through
`IShowcaseTerminalCapability`. The ExtCon adapter wraps the existing
Showcase-owned ConPTY service and forwards start/stop, input, resize, and
snapshot text. Hosts without a terminal implementation retain the valid
informative unavailable page.

The desktop shell also clamps open windows and relayouts its taskbar and
status area after host resize events. Undersized consoles receive an explicit
minimum-size message instead of invalid child bounds.

The clock face has golden MSTest coverage in
`ConsoleLib.Showcase.Desktop.Tests\ClockFaceGoldenTests.cs`. The display uses
only the four cardinal numbers (`12`, `3`, `6`, `9`); the final row explicitly
labels the hands as `H` (hour), `M` (minute), and `S` (second), so omitting the
other numbers cannot make the reading ambiguous. The snapshots currently
define these reference cases:

| Time | Expected hand directions |
| --- | --- |
| `00:00:00` | `H:^ M:^` |
| `06:15:15` | `H:v M:/` |
| `12:30:30` | `H:^ M:<` |
| `23:59:59` | `H:^ M:\` |

Every snapshot is exactly nine rows by 32 columns. The cardinal numbers remain
stable while the explicitly labeled hand directions and seconds change.

The clock also exposes a high-resolution Braille rendering path. One terminal
cell represents a 2x4 pixel block, so the circle and hour/minute/second hands
are rasterized rather than approximated with ASCII arrows. The reusable
`BrailleCanvas` control and `BrailleClockRenderer` are covered by focused
tests for dimensions and second-hand changes.

Input and overlap behavior is centralized in the ConsoleLib interaction layer:
mouse clicks select the topmost control and promote focus through its parent
chain, while covered controls are not redrawn over bordered dialogs/windows.
The multiline ExtCon label renderer respects both newline rows and control
height, which is required by the calendar and character table pages.

## Validation

```powershell
dotnet test ConsoleLib.Showcase.Desktop.Tests\ConsoleLib.Showcase.Desktop.Tests.csproj --no-restore --disable-build-servers
dotnet test ConsoleLib.Showcase.Tests\ConsoleLib.Showcase.Tests.csproj --no-restore --disable-build-servers
```

The portable desktop test project also references the existing Posix adapter
and verifies that a loaded desktop CXAML page produces a canonical snapshot
which `AnsiFrameRenderer` consumes and terminates with a clean ANSI reset.
