# ConsoleLib Showcase delivery

The showcase is a native Windows console application. Run it from a real
console because `ExtendedConsole` requires native console input events:

```powershell
dotnet run --project ConsoleLib.Showcase\ConsoleLib.Showcase.csproj -f net8.0-windows
```

Use the gallery list to inspect the component areas. `Effects` toggles the
animated glyph ramp, `Advance` changes the progress control, and `ConPTY
probe` verifies the showcase-owned Windows terminal bridge with a short shell
command. `Start terminal` opens the live `ConsoleLib.CommonControls.Terminal`
workspace. Output snapshots are rendered into the widget, clicks focus it,
keyboard and advertised SGR mouse input are routed to the session, and console
resizes are forwarded to ConPTY. The bridge is intentionally not part of the
reusable provider-neutral `Libraries\Terminal.Core` project.
