# AA21_Buttons Migration: WPF ? Avalonia.UI

## Migrationsstatus

Diese Migration hat die Grundstruktur für die Konvertierung des WPF-Projekts AA21_Buttons zu Avalonia.UI mit folgenden Komponenten erstellt:

## ? Abgeschlossene Schritte

1. **Projektdatei** (`AA21_Buttons.csproj`)
   - Konfiguriert für Avalonia 11.3.8
   - .NET 8.0-windows und .NET 9.0-windows Targets
   - UTF-8 Encoding
   - Zentrale Paketverwaltung

2. **ViewModel Migration** (`ViewModels/ButtonsViewViewModel.cs`)
   - Migriert zu MVVM Community Toolkit
   - `ObservableObject` statt BaseViewModel
   - `[RelayCommand]` statt DelegateCommand
   - `[ObservableProperty]` für Properties mit automatischem ChangeNotification

3. **Views** (Avalonia.axaml)
   - `MainWindow.axaml` - Hauptfenster mit TabControl
   - `ButtonsView.axaml` - 3x3 Button Grid
   - Vereinfachtes Binding-System für Avalonia

4. **Converter** (`Converters/BoolToColorConverter.cs`)
   - Portiert zu Avalonia `IValueConverter`
   - `SolidColorBrush` statt WPF Brush
   - Unterstützung für Array-Element-Konvertierung

5. **Dependency Injection**
   - `App.xaml.cs` mit `ServiceCollection`
   - `MainWindowViewModel` als ViewModel-Container
   - Singleton-Registrierung für ViewModels

6. **Entry Point** (`Program.cs`)
   - Avalonia `AppBuilder` Setup
   - `StartWithClassicDesktopLifetime` für Desktop-Apps

## ?? Datei-Übersicht

```
AA21_Buttons/
??? AA21_Buttons/
?   ??? AA21_Buttons.csproj              (aktualisiert)
?   ??? App.axaml                        (neu)
?   ??? App.xaml.cs                      (migriert)
?   ??? MainWindow.axaml                 (neu)
?   ??? MainWindow.xaml.cs               (migriert)
?   ??? Program.cs                       (neu)
?   ??? ViewModels/
?   ?   ??? ButtonsViewViewModel.cs      (migriert zu MVVM Toolkit)
?   ?   ??? MainWindowViewModel.cs       (neu)
?   ??? Views/
?   ?   ??? ButtonsView.axaml            (neu)
?   ?   ??? ButtonsView.xaml.cs          (migriert)
?   ??? Converters/
?   ?   ??? BoolToColorConverter.cs      (migriert zu Avalonia)
?   ??? Properties/
?       ??? Resources.resx               (vereinfacht)
??? Directory.Packages.props             (zentrale Paketverwaltung)
```

## ?? Nachbearbeitung erforderlich

Da NuGet-Restore in dieser Umgebung Probleme verursacht, führe folgende Schritte lokal aus:

```bash
# 1. Solution in Visual Studio öffnen
# 2. NuGet Package Manager öffnen (Tools ? NuGet Package Manager)
# 3. Package Manager Console ausführen:

Update-Package -Reinstall Avalonia
Update-Package -Reinstall Avalonia.Desktop
Update-Package -Reinstall Avalonia.Themes.Fluent
Update-Package -Reinstall CommunityToolkit.Mvvm
Update-Package -Reinstall Microsoft.Extensions.DependencyInjection

# 4. Projekt neu bauen
Build ? Rebuild Solution

# 5. Testen
Ctrl+F5 zum Starten
```

## ?? Zentrale Pakete

Die erforderlichen Pakete sind in `AA21_Buttons/Directory.Packages.props` definiert:
- **Avalonia 11.3.8** - UI Framework
- **Avalonia.Desktop 11.3.8** - Desktop-Unterstützung
- **Avalonia.Themes.Fluent 11.3.8** - Fluent Theme
- **Avalonia.Fonts.Inter 11.3.8** - Inter Font
- **CommunityToolkit.Mvvm 8.4.0** - MVVM Pattern
- **Microsoft.Extensions.DependencyInjection 9.0.10** - DI Container

## ?? Spiellogik

Das Spiel funktioniert wie folgt:
- 3×3 Grid mit Buttons (1-9)
- Klick auf Button: Toggles Button + benachbarte Buttons
- Reset-Button: Setzt Spiel zurück (nur Button 1 aktiv)
- Farbcodierung: Grün (aktiv) / Dunkelrot (inaktiv)

## ?? MVVM-Architektur

- **MainWindowViewModel**: Container für andere ViewModels
- **ButtonsViewViewModel**: Spiellogik mit RelayCommands
- **BoolToColorConverter**: Konvertiert bool[]/bool zu SolidColorBrush
- **Dependency Injection**: Alle ViewModels sind Singletons

## ? Best Practices implementiert

? Eine Klasse pro Datei  
? UTF-8 Encoding auf allen Dateien  
? MVVM Community Toolkit  
? Dependency Injection  
? Namespacing nach Verzeichnisstruktur  
? XML-Dokumentation (/// <summary>)  
? Avalonia statt WPF-Binding  
? Keine hardgecodeten Strings  

## ?? Nächste Schritte

1. Lokal NuGet-Pakete installieren
2. Projekt neu aufbauen
3. App starten und Spiellogik testen
4. Optional: Styling erweitern (aktuell minimal)
5. Optional: Tests schreiben
