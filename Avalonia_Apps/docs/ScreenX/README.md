# ScreenX – Portierung von Delphi nach C#

Ziel
- Portierung des Pascal/Delphi-Projekts „ScreenX“ (siehe `Unt_RenderTask.pas`) nach C#.
- Trennung in zwei Projekte:
 - ScreenX.Base: Rechenkern (ohne UI-Framework-Abhängigkeiten)
 - AA14_ScreenX: Avalonia UI (Darstellung, User-Interaktion)

Vorgaben / Patterns
-1 Klasse pro Datei (klare Zuständigkeiten, kleine Artefakte)
- MVVM im UI-Projekt, ViewModels mit CommunityToolkit.Mvvm
- DI mit Microsoft.Extensions.DependencyInjection
- TDD: erst Tests im Kern (ScreenX.Base), dann UI

Architektur
- ScreenX.Base enthält:
 - Grundtypen: `ExPoint` (double X,Y), `ExRect` (double X1,Y1,X2,Y2)
 - Delegates: `DFunction` (p,p0,ref shouldBreak) ? `ExPoint`; `CFunction` (p) ? `uint` (ARGB32)
 - Farbtyp: `uint` (ARGB32) + Hilfsfunktionen (`Color32.Pack`, `FromRgb`, etc.)
 - Optionen: `RenderOptions` (Width,Height,Source, Functions, Colorizer)
 - Ergebnis: `RenderResult` (Width,Height,Pixels:uint[])
 - Service: `IRendererService` + `RendererService`
- AA14_ScreenX enthält:
 - ViewModels (z. B. `RenderViewModel`) mit bindbaren `RenderOptions`
 - DI-Komposition (Registrierung von `IRendererService`)
 - Konvertierung von `RenderResult.Pixels` (ARGB32) nach Avalonia `WriteableBitmap`

Migrationsnotizen (Delphi ? C#)
- `TExPoint` ? `ExPoint` (struct), `extended` ? `double`
- `TExRect` ? `ExRect` (struct)
- `TColor` ? `uint` (ARGB32) im Kern (kein System.Drawing)
- `TDFunktion`/`TCFunktion` ? C#-Delegates
- `TRenderThread` ? CPU-gebundene Berechnung via `Task`/`Parallel` (kein UI-Typ im Kern)

Threading/Performance
- Kern erzeugt nur ein `uint[]`-Pixelpuffer (ARGB32) und gibt ihn zurück
- Iteration: x ? [0..Width-1], y ? [0..Height-1]
 - p0.x = X1 + (X2 - X1) * (x/Width)
 - p0.y = Y1 + (Y2 - Y1) * (y/Height)
 - p = p0; for f in Functions: p = f(p,p0,ref shouldBreak); optional vorzeitig beenden
 - color = Colorizer(p) ? Pixels[y*Width + x] = color
- Optional: Parallelisierung (später), CancellationToken-Unterstützung

TDD-Plan
- Unit-Tests in `ScreenX.BaseTests`:
 - Identity-Funktion + einfacher Colorizer ? erwartbares Gradientenmuster
 - Funktionskette-Komposition
 - Korrekte Abbildung von Koordinaten nach Pixel

UI-Plan (AA14_ScreenX)
- Avalonia App mit DI-Setup (ServiceCollection)
- `RenderViewModel`: Bindbare `RenderOptions`, `RenderCommand`
- Anzeige über `WriteableBitmap` (keine System.Drawing-Abhängigkeit)

Fahrplan
1) Doku (dieses Dokument + Projekt-READMEs)
2) ScreenX.Base erstellen (Types, Delegates, Optionen, Service)
3) Tests erstellen und grün bekommen
4) AA14_ScreenX Grundgerüst (MVVM, DI), Anzeige des RenderResult
5) Erweiterungen (Parallel, Cancel, Presets, UI-Controls)
