# ?? Post-Migration Checklist

Nach der automatischen Migration mit dem Skript müssen Sie möglicherweise noch manuelle Anpassungen vornehmen.

## ? Sofort nach Migration prüfen

### 1. Build-Test durchführen

```powershell
dotnet build <ProjektPfad>/<ProjektName>.csproj
```

**Erwartung:** 0 Fehler (Warnungen sind OK)

---

## ?? Häufige manuelle Anpassungen

### A. MainWindow.axaml Navigation

**Problem:** Frame-Navigation wurde automatisch konvertiert, aber komplexe Navigation benötigt manuelle Anpassung.

**WPF Original:**
```xml
<Frame Source="/Views/MyView.xaml"/>
```

**Automatisch konvertiert zu:**
```xml
<views:MyView/>
```

**Manuelle Anpassung nötig wenn:**
- Dynamische Navigation verwendet wird
- Parameters an Views übergeben werden
- NavigationService verwendet wird

**Lösung:** Verwenden Sie ContentControl mit DataTemplate oder ReactiveUI Router.

---

### B. ValueConverter Signaturen

**Problem:** Nullable-Annotationen können Warnungen verursachen.

**Prüfen Sie:**
```csharp
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
{
    // Implementation
}
```

**Falls Fehler:** Stellen Sie sicher, dass:
- `using Avalonia.Data.Converters;` vorhanden ist
- Rückgabetyp `object?` ist
- Parameter nullable sind

---

### C. App.axaml.cs DI-Konfiguration

**Problem:** Service-Registrierungen wurden automatisch extrahiert, aber möglicherweise fehlen using-Statements.

**Prüfen:**
```csharp
using ${ProjektName}.Models;
using ${ProjektName}.Models.Interfaces;
using ${ProjektName}.Services;
```

**Ergänzen Sie fehlende Namespaces manuell.**

---

### D. Resources.resx Einträge

**Problem:** XAML-Datei-Referenzen wurden entfernt, aber Code-Snippets könnten fehlen.

**Wenn Sie embedded resources (wie Code-Snippets für Anzeige) hatten:**

1. Erstellen Sie separate `.txt` oder `.cs` Dateien
2. Laden Sie diese zur Laufzeit
3. ODER: Verwenden Sie `AvaloniaResource` in `.csproj`

---

### E. Window.DataContext vs Design.DataContext

**Problem:** Das Skript fügt `Design.DataContext` hinzu, aber manchmal muss `Window.DataContext` angepasst werden.

**Best Practice:**
```xml
<Window ...>
    <Design.DataContext>
 <vm:MyViewModel/>
    </Design.DataContext>
    
    <!-- Actual DataContext is set in code-behind or from parent -->
</Window>
```

---

## ?? Spezifische Control-Anpassungen

### ListView / DataGrid

**WPF:**
```xml
<ListView ItemsSource="{Binding Items}">
    <ListView.View>
        <GridView>
            <GridViewColumn Header="Name" DisplayMemberBinding="{Binding Name}"/>
        </GridView>
    </ListView.View>
</ListView>
```

**Avalonia:**
```xml
<DataGrid ItemsSource="{Binding Items}" AutoGenerateColumns="False">
    <DataGrid.Columns>
     <DataGridTextColumn Header="Name" Binding="{Binding Name}"/>
    </DataGrid.Columns>
</DataGrid>
```

---

### Menu & ContextMenu

**WPF:**
```xml
<Menu>
    <MenuItem Header="File">
        <MenuItem Header="Open" Command="{Binding OpenCommand}"/>
    </MenuItem>
</Menu>
```

**Avalonia:** (bleibt meist gleich, aber überprüfen Sie Icons)
```xml
<Menu>
    <MenuItem Header="File">
    <MenuItem Header="Open" Command="{Binding OpenCommand}">
            <MenuItem.Icon>
         <PathIcon Data="{StaticResource OpenIconPath}"/>
     </MenuItem.Icon>
        </MenuItem>
    </MenuItem>
</Menu>
```

---

### Triggers & Styles

**WPF EventTrigger ? Avalonia Interaction:**

```xml
<!-- Avalonia benötigt: -->
<PackageReference Include="Avalonia.Xaml.Interactions" />
<PackageReference Include="Avalonia.Xaml.Interactivity" />
```

```xml
<Interaction.Behaviors>
    <EventTriggerBehavior EventName="PointerPressed">
        <InvokeCommandAction Command="{Binding MyCommand}"/>
    </EventTriggerBehavior>
</Interaction.Behaviors>
```

---

## ?? Test-Checkliste

### Funktionale Tests

- [ ] Applikation startet
- [ ] Hauptfenster wird angezeigt
- [ ] Navigation funktioniert
- [ ] Bindings aktualisieren sich
- [ ] Commands funktionieren
- [ ] ValueConverter liefern korrekte Werte
- [ ] DI liefert Services korrekt

### UI-Tests

- [ ] Layouts sehen korrekt aus
- [ ] Controls sind anklickbar
- [ ] Scrolling funktioniert
- [ ] Tooltips werden angezeigt
- [ ] Icons/Bilder werden geladen

---

## ?? Dokumentierte Probleme & Lösungen

### Problem: "InitializeComponent not found"

**Ursache:** AXAML-Datei ist nicht im Projekt enthalten oder falsch benannt.

**Lösung:**
1. Prüfen Sie, ob `.axaml` Datei existiert
2. Clean & Rebuild: `dotnet clean && dotnet build`
3. Prüfen Sie, dass Dateiname und `x:Class` übereinstimmen

---

### Problem: "Cannot resolve type 'XXX' from namespace 'using:YYY'"

**Ursache:** Namespace-Referenz ist falsch oder Klasse existiert nicht.

**Lösung:**
1. Prüfen Sie `xmlns:vm="using:Namespace"` Deklaration
2. Prüfen Sie, ob Klasse existiert und `public` ist
3. Clean & Rebuild

---

### Problem: Binding funktioniert nicht

**Ursache:** Mehrere mögliche Ursachen in Avalonia.

**Lösung:**
1. Aktivieren Sie Binding-Diagnostics:
```xml
<Window xmlns:diagnostics="clr-namespace:Avalonia.Diagnostics;assembly=Avalonia.Diagnostics"
        ...>
```

2. Starten Sie mit F12 (DevTools) und prüfen Sie Console

3. Verwenden Sie `x:DataType` für Compiled Bindings:
```xml
<UserControl x:DataType="vm:MyViewModel">
```

---

### Problem: ValueConverter gibt Fehler

**Ursache:** Avalonia's IValueConverter hat andere Signatur als WPF.

**Lösung:**
```csharp
// Avalonia Version
public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
{
    // Null-Checks hinzufügen
  if (value == null) return null;
    
 // Rest der Implementierung
}
```

---

## ?? Debug-Tipps

### DevTools verwenden

```xml
<!-- In Debug-Builds verfügbar -->
<PackageReference Condition="'$(Configuration)' == 'Debug'" 
   Include="Avalonia.Diagnostics" Version="11.3.7" />
```

**Verwendung:** Drücken Sie `F12` im laufenden Fenster

**Features:**
- XAML Tree Inspector
- Property Values
- Binding Diagnostics
- Performance Profiler

---

### Logging aktivieren

```csharp
// In Program.cs
public static AppBuilder BuildAvaloniaApp()
    => AppBuilder.Configure<App>()
        .UsePlatformDetect()
  .WithInterFont()
        .LogToTrace()  // ? Aktiviert Trace-Logging
        .LogToTrace(LogEventLevel.Debug);  // ? Für verbose logging
```

---

## ?? Migrations-Status Tracking

| Komponente | Status | Notizen |
|------------|--------|---------|
| ? Projekt erstellt | ? | |
| ? Build erfolgreich | ? | |
| ? App startet | ? | |
| ? Views angezeigt | ? | |
| ? Navigation | ? | |
| ? Bindings | ? | |
| ? Commands | ? | |
| ? ValueConverter | ? | |
| ? Styles | ? | |
| ? Tests | ? | |

---

## ?? Weiterführende Ressourcen

- [Avalonia Bindings](https://docs.avaloniaui.net/docs/basics/data/data-binding)
- [Control Catalog](https://github.com/AvaloniaUI/Avalonia.Samples/tree/main/src/Avalonia.Samples)
- [WPF Developers Guide](https://docs.avaloniaui.net/docs/get-started/wpf)
- [Styling Guide](https://docs.avaloniaui.net/docs/basics/user-interface/styling)

---

**Version:** 2.0  
**Letzte Aktualisierung:** $(Get-Date -Format "dd.MM.yyyy")  
**Für Projekte migriert mit:** Migrate-WpfToAvalonia.ps1 v2.0
