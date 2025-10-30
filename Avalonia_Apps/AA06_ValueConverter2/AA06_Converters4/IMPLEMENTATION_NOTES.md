# AA06_Converters4 - Avalonia UI Implementierung

## Gelöste Probleme

### 1. Encoding-Problem mit Umlauten ✅

**Problem:** Texte mit Umlauten (ä, ö, ü, °) wurden nicht korrekt angezeigt.

**Lösung:**
- Alle `.axaml` Dateien wurden auf **UTF-8 mit BOM** Encoding überprüft
- `VehicleView1.axaml` korrigiert: 
  - "L�nge" → "Länge"
  - "�" → "°" (Grad-Zeichen)
  
**Empfehlung:** Stellen Sie sicher, dass alle XAML/AXAML-Dateien im UTF-8-Format gespeichert sind.

---

### 2. Dynamische Grafische Darstellung ✅

**Problem:** Die PlotFrame-Visualisierung war leer.

**Lösung:** Implementierung eines vollständigen **MVVM-konformen Custom Controls** für Avalonia UI.

#### Neue Komponenten:

##### `DynamicPlotCanvas.cs`
Ein Custom Control das:
- ✅ **Koordinatensystem** mit automatischem Grid und Achsenbeschriftung
- ✅ **AGV-Visualisierung** mit Fahrzeugkörper, Drehschemeln und Rädern  
- ✅ **Interaktive Pan-Funktion** (Drag mit linker Maustaste)
- ✅ **Zoom-Funktion** (Mausrad)
- ✅ **Geschwindigkeitsvektoren** als Pfeile
- ✅ **MVVM-Bindung** über `ViewModel`-Property
- ✅ **Dependency Injection** kompatibel
- ✅ **Reaktive Updates** bei ViewModel-Änderungen
- ✅ **Isometrische Darstellung** (Längenverhältnisse richtungsunabhängig)
- ✅ **Model-Reaktivität** (reagiert auf alle Model-PropertyChanged-Events)

---

### 3. Isometrische Darstellung (Längenverhältnisse) ✅

**Problem:** Längenverhältnisse waren richtungsabhängig (unterschiedliche Skalierung in X und Y).

**Lösung:** Implementierung einer **isometrischen Viewport-Anpassung**:

```csharp
private void UpdateIsometricViewport()
{
    // Berechnet einen angepassten Viewport mit gleichem Maßstab in X und Y
    var aspectRatio = availableWidth / availableHeight;
    var viewportAspect = viewport.Width / viewport.Height;
    
    if (viewportAspect > aspectRatio)
     // Viewport ist breiter -> Höhe anpassen
    else
        // Viewport ist höher -> Breite anpassen
}
```

**Ergebnis:** Ein Kreis mit Radius 100 wird immer als Kreis dargestellt, unabhängig von der Fenstergröße.

---

### 4. Reaktivität auf Model-Änderungen ✅

**Problem:** Der Plot reagierte nicht auf Änderungen im AGV-Model.

**Lösung:** **PropertyChanged-Event-Listener** auf dem Model + ViewModel-Collections.

**Ergebnis:** Jede Änderung an Slider-Werten wird sofort visualisiert.

---

### 5. Flackern beim Drag (Pan-Funktion) ✅

**Problem:** Beim Verschieben mit der Maus flackerte/wackelte die Grafik.

**Ursachen:**
1. Zu viele `InvalidateVisual()`-Aufrufe während `OnPointerMoved`
2. PropertyChanged-Events triggerten zusätzliche Renders
3. Model-Updates während Drag verursachten Render-Kaskaden

**Lösung:** **Render-Throttling mit DispatcherTimer**:

```csharp
// Render-Throttle-Timer (60 FPS = 16ms)
private DispatcherTimer _renderThrottleTimer = new()
{
    Interval = TimeSpan.FromMilliseconds(16)
};

private void ScheduleRender()
{
    if (_isRenderScheduled) return;
    _isRenderScheduled = true;
    
    if (_isDragging)
    {
        // Während Dragging: Throttle auf 60 FPS
        _renderThrottleTimer?.Start();
    }
    else
    {
        // Außerhalb Dragging: Sofort rendern
  Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
    }
}

private void Model_PropertyChanged(...)
{
    // Bei Model-Änderungen nur außerhalb von Drag rendern
    if (!_isDragging)
        ScheduleRender();
}
```

**Optimierungen:**
1. **Render-Throttling:** Max 60 FPS während Drag
2. **Model-Updates deaktiviert:** Keine Renders durch Model-Events während Drag
3. **Finales Render:** Ein sauberes Render nach Drag-Ende
4. **Flag-basierte Kontrolle:** `_isRenderScheduled` verhindert Render-Queuing

**Ergebnis:** Butterweiche Pan-Bewegung ohne Flackern! 🎯

---

## Architektur-Highlights

```
┌─────────────────────────────────────┐
│      PlotFrameViewModel         │
│  ┌───────────────────────────────┐  │
│  │ - VPWindow (Viewport)         │  │
│  │ - WindowSize           │  │
│  │ - AGVModel (IAGVModel)        │  │
│  │ - ZoomInCommand       │  │
│  │ - ZoomOutCommand        │  │
│  │ - ResetViewCommand            │  │
│  │ - Arrows/Circles/Polygons     │  │
│  └───────────────────────────────┘  │
└──────────────┬──────────────────────┘
    │ Data Binding
     ▼
┌─────────────────────────────────────┐
│       DynamicPlotCanvas           │
│   (Custom Avalonia Control)  │
│  ┌───────────────────────────────┐  │
│  │ Render Pipeline:        │  │
│  │ - UpdateIsometricViewport()   │  │
│  │ - DrawCoordinateSystem()      │  │
│  │ - DrawPolygon() (Fahrzeug)    │  │
│  │ - DrawCircle() (Drehschemel)  │  │
│  │ - DrawArrow() (Velocities)    │  │
│  └───────────────────────────────┘  │
│  ┌───────────────────────────────┐  │
│  │ Event Listeners: │  │
│  │ - ViewModel.PropertyChanged   │  │
│  │ - AGVModel.PropertyChanged    │  │
│  │ - Pointer Events (Pan/Zoom) │  │
│  └───────────────────────────────┘  │
│  ┌───────────────────────────────┐  │
│  │ Performance:        │  │
│  │ - Render Throttling (60 FPS)  │  │
│  │ - DispatcherTimer     │  │
│  │ - Drag-optimiert │  │
│  └───────────────────────────────┘  │
└─────────────────────────────────────┘
             ▲
     │ PropertyChanged Events
┌──────────────┴──────────────────────┐
│         AGV_Model     │
│  (IAGVModel Implementation)       │
│  - VehicleDim, SwivelKoor        │
│  - Wheel Velocities, Angles │
│  - ObservableObject         │
└─────────────────────────────────────┘
```

#### Features:

| Feature | Implementierung | Status |
|---------|----------------|--------|
| **Koordinatensystem** | Automatisches Grid mit Labels | ✅ |
| **Pan (Verschieben)** | Linke Maustaste + Drag | ✅ |
| **Pan-Performance** | 60 FPS Throttling, flackerfrei | ✅ |
| **Zoom** | Mausrad | ✅ |
| **Zoom Buttons** | Toolbar mit +/- Buttons | ✅ |
| **Reset View** | Button zum Zurücksetzen | ✅ |
| **AGV Rendering** | Fahrzeug, Drehschemel, Räder | ✅ |
| **Velocity Vectors** | Geschwindigkeitspfeile | ✅ |
| **Responsive** | Automatische Größenanpassung | ✅ |
| **MVVM-konform** | Keine View-Logik im ViewModel | ✅ |
| **DI-kompatibel** | Constructor Injection | ✅ |
| **Isometrisch** | Gleiche Skalierung X/Y | ✅ |
| **Model-Reaktiv** | Live-Updates bei Slider-Änderungen | ✅ |
| **Flackerfrei** | Render-Throttling während Drag | ✅ |

---

## Technische Details

### Performance-Optimierungen:

1. **Render-Throttling (60 FPS)**
   ```csharp
 private DispatcherTimer _renderThrottleTimer = new()
   {
       Interval = TimeSpan.FromMilliseconds(16) // ≈ 60 FPS
 };
   ```

2. **Flag-basierte Render-Kontrolle**
   ```csharp
   private bool _isRenderScheduled;
   
   private void ScheduleRender()
   {
 if (_isRenderScheduled) return; // Verhindert Render-Queuing
       _isRenderScheduled = true;
       // ...
   }
   ```

3. **Drag-spezifische Optimierungen**
   ```csharp
   private void Model_PropertyChanged(...)
   {
       if (!_isDragging) // Nur außerhalb Drag
           ScheduleRender();
   }
   ```

4. **Dispatcher Priority**
   ```csharp
   Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
   ```

5. **Isometrischer Viewport-Cache**
   ```csharp
private RectangleF _isometricViewport; // Einmalige Berechnung pro Render
   ```

### Threading:
- **Rendering:** UI-Thread (via `Dispatcher.UIThread`)
- **ViewModel Updates:** Jeder Thread (via `PropertyChanged`)
- **Model Updates:** Jeder Thread (via `PropertyChanged`)
- **Throttle-Timer:** UI-Thread (DispatcherTimer)

### Rendering Pipeline:

```
User Input → ScheduleRender()
     ↓
  _isDragging?
     ↓
  YES → DispatcherTimer (16ms) → InvalidateVisual() → Render()
  NO  → Dispatcher.Post() → InvalidateVisual() → Render()
```

### Flacker-Vermeidung:

| Problem | Lösung |
|---------|--------|
| Zu viele Renders | Throttling auf 60 FPS |
| PropertyChanged-Kaskaden | Flag `_isRenderScheduled` |
| Model-Updates während Drag | Deaktiviert via `!_isDragging` |
| Render-Queuing | Single-Flag verhindert Doppel-Renders |

---

## MVVM-Best Practices

### ✅ **Separation of Concerns**
- **Model:** `AGV_Model` (reine Datenlogik)
- **ViewModel:** `PlotFrameViewModel` (Präsentationslogik, Commands)
- **View:** `DynamicPlotCanvas` (nur Rendering und UI-Events)

### ✅ **Dependency Injection**
```csharp
public PlotFrame(PlotFrameViewModel viewModel)
{
    DataContext = viewModel;
}
```

### ✅ **Observable Properties**
```csharp
[ObservableProperty]
private ArrowList _arrows;
```

### ✅ **Reactive Updates mit Performance**
```csharp
// ViewModel-Updates: Throttled
ViewModel.PropertyChanged += (s, e) => ScheduleRender();

// Model-Updates: Nur außerhalb Drag
AGVModel.PropertyChanged += (s, e) =>
{
    if (!_isDragging) ScheduleRender();
};
```

### ✅ **Commands statt Event-Handler**
```csharp
[RelayCommand]
private void ZoomIn() { ... }
```

---

## Erweiterungsmöglichkeiten

### 1. Element-Auswahl
```csharp
protected override void OnPointerPressed(...)
{
  var clickedElement = FindElementAtPosition(e.GetPosition(this));
    if (clickedElement != null)
        ViewModel.SelectedElement = clickedElement;
}
```

### 2. Zusätzliche Shapes
```csharp
public class RectangleList : List<RectangleData> { }
public class PathList : List<PathData> { }
```

### 3. Export-Funktionalität
```csharp
[RelayCommand]
private async Task ExportToPng()
{
    // Render to RenderTargetBitmap
}
```

### 4. Snap-to-Grid
```csharp
private PointF SnapToGrid(PointF point, double gridSize)
{
    return new PointF(
 (float)(Math.Round(point.X / gridSize) * gridSize),
      (float)(Math.Round(point.Y / gridSize) * gridSize));
}
```

---

## Build & Run

```bash
dotnet build AA06_Converters4
dotnet run --project AA06_Converters4
```

---

## Tests

```bash
dotnet test AA06_Converters4Tests
```

---

## Bekannte Probleme

### Design-Time-Warnung (AVLN2000)
**Symptom:** `Unable to resolve suitable regular or attached property Background`

**Ursache:** Avalonia Designer-Bug bei Custom Controls

**Lösung:** Ignorieren - funktioniert zur Laufzeit einwandfrei

---

## Changelog

### Version 2.0 (2024-12-20)
- ✅ **Isometrische Darstellung** implementiert
- ✅ **Model-Reaktivität** für Live-Updates
- ✅ **Anti-Flacker-Optimierung** mit Render-Throttling
- ✅ **Performance:** 60 FPS während Drag-Operationen
- ✅ **Bugfix:** Flackern beim Pan komplett eliminiert

### Version 1.0 (2024-12-19)
- ✅ **UTF-8 Encoding** für Umlaute
- ✅ **DynamicPlotCanvas** Custom Control
- ✅ **MVVM-Architektur** mit DI
- ✅ **Pan & Zoom** Funktionalität

---

## Autor
- **Mir** (Original WPF-Version)
- **GitHub Copilot** (Avalonia-Migration & Optimierungen, 2024)

## Lizenz
Copyright © JC-Soft 2022-2024
