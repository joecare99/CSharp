# AA14_ScreenX (Avalonia)

Inhalt
- UI für ScreenX mit MVVM und DI
- Anzeigen eines `RenderResult` als `WriteableBitmap`

Bausteine (geplant)
- `App`: DI-Setup mit `Microsoft.Extensions.DependencyInjection`
- `MainWindow` + `RenderView`
- `RenderViewModel` (CommunityToolkit.Mvvm)
- Konverter/Adapter: `Argb32PixelBufferToWriteableBitmap`

Interaktion
- Benutzer setzt Parameter (Source, Width, Height, Funktionen)
- `RenderCommand` startet Rendering via `IRendererService`
- Ergebnis wird als Bitmap gebunden angezeigt
