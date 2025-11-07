# ScreenX.Base

Inhalt
- Reiner Kern ohne UI-Abhängigkeiten
- Datentypen für Geometrie und Farben
- Renderer-Service, der Pixelpuffer (ARGB32) erzeugt

Öffentliche API (geplant)
- `struct ExPoint { double X, Y }`
- `struct ExRect { double X1, Y1, X2, Y2 }`
- `delegate ExPoint DFunction(ExPoint p, ExPoint p0, ref bool shouldBreak)`
- `delegate uint CFunction(ExPoint p)`
- `record RenderOptions(int Width, int Height, ExRect Source, IReadOnlyList<DFunction> Functions, CFunction Colorizer)`
- `record RenderResult(int Width, int Height, uint[] Pixels)`
- `interface IRendererService { RenderResult Render(RenderOptions options, CancellationToken ct = default); }`

Design-Entscheidungen
- Farben als `uint` (ARGB32) statt `System.Drawing.Color` zur Framework-Unabhängigkeit
- Keine Threads im UI-Sinn – Rendering ist synchron; Aufrufer kann in `Task.Run` packen
- Optional spätere Parallelisierung mit `Parallel.For` oder `SIMD`
