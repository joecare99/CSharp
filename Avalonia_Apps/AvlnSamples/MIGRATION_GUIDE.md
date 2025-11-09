# WPF to Avalonia Migration Guide

## ?? Übersicht

Dieses Dokument beschreibt den Prozess zur Migration von WPF-Projekten zu Avalonia UI unter Beibehaltung von MVVM, Dependency Injection und CommunityToolkit.Mvvm.

## ??? Voraussetzungen

- ? Avalonia BaseLib (`Avln_BaseLib`) ist vorhanden
- ? .NET 8 oder .NET 9 SDK installiert
- ? Visual Studio 2022 oder JetBrains Rider

## ?? Abhängigkeiten

### Avalonia Pakete
- `Avalonia` (11.3.7)
- `Avalonia.Desktop` (11.3.7)
- `Avalonia.Themes.Fluent` (11.3.7)
- `Avalonia.Fonts.Inter` (11.3.7)
- `Avalonia.Diagnostics` (11.3.7) - nur Debug

### Beibehaltene Pakete
- `CommunityToolkit.Mvvm` (8.4.0)
- `Microsoft.Extensions.DependencyInjection` (9.0.10)

## ?? Automatische Migration

### Verwendung des Migrations-Skripts

```powershell
# Einfache Migration
.\Migrate-WpfToAvalonia.ps1 -WpfProjectName "WPF_Sample_Template"

# Migration mit benutzerdefiniertem Namen
.\Migrate-WpfToAvalonia.ps1 -WpfProjectName "WPF_Sample_Template" -AvlnProjectName "Avln_MyApp"

# Migration mit Tests
.\Migrate-WpfToAvalonia.ps1 -WpfProjectName "WPF_Sample_Template" -IncludeTests
```

### Was das Skript tut

1. **Projektstruktur erstellen**
   - Erstellt Ordner: Views, ViewModels, Models, Properties, Assets

2. **C#-Dateien konvertieren**
   - ViewModel-Dateien
   - Model-Dateien
   - Interfaces
   - Namespace-Anpassungen

3. **XAML zu AXAML konvertieren**
   - XML-Namespaces anpassen
   - WPF-Controls zu Avalonia-Controls
   - Page ? UserControl

4. **Projekt-Dateien erstellen**
   - `.csproj` mit Avalonia-Paketen
   - `Program.cs` (Entry Point)
   - `app.manifest`

5. **Resources verarbeiten**
   - `.resx`-Dateien bereinigen
   - Datei-Referenzen entfernen

6. **Assets kopieren**
   - Icons und Bilder

## ?? Manuelle Migrations-Schritte

Falls das Skript nicht verwendet wird oder Nachbearbeitung nötig ist:

### 1. Projekt-Setup

```xml
<Project>
    <Import Project="..\Avln_Samples.props" />
 <PropertyGroup>
        <OutputType>WinExe</OutputType>
      <TargetFrameworks>net8.0;net9.0</TargetFrameworks>
      <Nullable>enable</Nullable>
        <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
    </PropertyGroup>
    <!-- Avalonia Packages -->
</Project>
```

### 2. Namespace-Konvertierungen

| Von (WPF) | Nach (Avalonia) |
|-----------|-----------------|
| `System.Windows` | `Avalonia.Controls` |
| `System.Windows.Controls.Page` | `Avalonia.Controls.UserControl` |
| `MVVM.ViewModel.BaseViewModelCT` | `Avalonia.ViewModels.BaseViewModelCT` |
| `CancelEventArgs` | `WindowClosingEventArgs` |

### 3. XAML-Konvertierungen

**WPF:**
```xml
<Page xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      x:Class="WPF_App.Views.MyView">
    <!-- Content -->
</Page>
```

**Avalonia:**
```xml
<UserControl xmlns="https://github.com/avaloniaui"
         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:vm="using:Avln_App.ViewModels"
 x:Class="Avln_App.Views.MyView"
             x:DataType="vm:MyViewModel">
    <!-- Content -->
</UserControl>
```

### 4. App.xaml ? App.axaml

**WPF App.xaml:**
```xml
<Application x:Class="WPF_App.App"
          xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             StartupUri="MainWindow.xaml">
</Application>
```

**Avalonia App.axaml:**
```xml
<Application xmlns="https://github.com/avaloniaui"
        x:Class="Avln_App.App"
             RequestedThemeVariant="Default">
    <Application.Styles>
        <FluentTheme />
    </Application.Styles>
</Application>
```

### 5. App.xaml.cs ? App.axaml.cs

**WPF:**
```csharp
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
    // DI Setup
 base.OnStartup(e);
    }
}
```

**Avalonia:**
```csharp
public partial class App : Application
{
    public override void Initialize()
    {
 AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
 {
        // DI Setup
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
      {
   desktop.MainWindow = new MainWindow();
        }
        base.OnFrameworkInitializationCompleted();
    }
}
```

### 6. Program.cs erstellen

```csharp
using Avalonia;
using System;

namespace Avln_App;

class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
      .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
      .UsePlatformDetect()
    .WithInterFont()
      .LogToTrace();
}
```

## ?? Control-Mapping

| WPF Control | Avalonia Control | Bemerkungen |
|-------------|------------------|-------------|
| `Window` | `Window` | Gleich |
| `Page` | `UserControl` | Änderung nötig |
| `Frame` | `ContentControl` oder `TransitioningContentControl` | Navigation anders |
| `ListView` | `ListBox` oder `DataGrid` | Je nach Verwendung |
| `DataGrid` | `DataGrid` | Ähnlich, aber andere API |
| `TextBlock` | `TextBlock` | Gleich |
| `TextBox` | `TextBox` | Gleich |
| `Button` | `Button` | Gleich |
| `ComboBox` | `ComboBox` | Gleich |
| `CheckBox` | `CheckBox` | Gleich |

## ?? Compiled Bindings

Avalonia unterstützt Compiled Bindings für bessere Performance:

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:vm="using:Avln_App.ViewModels"
             x:DataType="vm:MyViewModel">
    <TextBlock Text="{Binding Title}" />
</UserControl>
```

Um dies zu aktivieren:
```xml
<PropertyGroup>
  <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
</PropertyGroup>
```

## ?? Test-Projekt Migration

Test-Projekte benötigen zusätzliche Anpassungen:

```xml
<PackageReference Include="Avalonia.Headless.XUnit" Version="11.3.7" />
```

BaseTestViewModel bleibt kompatibel durch Avln_BaseLibTests.

## ?? Häufige Probleme und Lösungen

### Problem 1: Resources.resx enthält XAML-Datei-Referenzen

**Lösung:** Entfernen Sie `<data type="System.Resources.ResXFileRef">` Einträge aus der `.resx`-Datei.

### Problem 2: Namespace-Konflikte

**Lösung:** Systematisch alle `using`-Anweisungen prüfen und WPF-Namespaces ersetzen.

### Problem 3: CancelEventArgs nicht gefunden

**Lösung:** Ersetzen durch `WindowClosingEventArgs` aus `Avalonia.Controls`.

### Problem 4: Frame-Navigation funktioniert nicht

**Lösung:** Avalonia verwendet ein anderes Navigations-Modell. Verwenden Sie `ContentControl` mit DataTemplate-Switching oder eine Router-Bibliothek.

## ?? Migrations-Checkliste

- [ ] Projekt-Ordnerstruktur erstellt
- [ ] `.csproj` mit Avalonia-Paketen erstellt
- [ ] `Program.cs` hinzugefügt
- [ ] `App.axaml` und `App.axaml.cs` konvertiert
- [ ] Alle `.xaml` zu `.axaml` umbenannt und konvertiert
- [ ] Namespaces in C#-Dateien aktualisiert
- [ ] `Resources.resx` bereinigt
- [ ] `BaseViewModelCT` Namespace geändert
- [ ] Event-Handler aktualisiert (OnClosing)
- [ ] Build-Test durchgeführt
- [ ] Laufzeit-Test durchgeführt
- [ ] Test-Projekt migriert (falls vorhanden)

## ?? Weiterführende Ressourcen

- [Avalonia UI Dokumentation](https://docs.avaloniaui.net/)
- [WPF zu Avalonia Migrations-Guide](https://docs.avaloniaui.net/guides/platforms/wpf)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [Avalonia Samples](https://github.com/AvaloniaUI/Avalonia.Samples)

## ?? Best Practices

1. **Schrittweise Migration:** Beginnen Sie mit einfachen Projekten
2. **Template-Projekt:** Verwenden Sie Avln_Hello_World als Referenz
3. **Testen Sie früh:** Führen Sie nach jedem Migrations-Schritt einen Build durch
4. **Git-Commits:** Commiten Sie nach jedem erfolgreichen Teilschritt
5. **Dokumentation:** Dokumentieren Sie projektspezifische Anpassungen

## ?? Projekt-Prioritäten

Empfohlene Migrations-Reihenfolge:

1. ? **Avln_Hello_World** (Abgeschlossen - Template)
2. **Avln_Sample_Template** - Einfaches Template
3. **Avln_MoveWindow** - Einfache Window-Manipulation
4. **Avln_ControlsAndLayout** - Control-Tests
5. **Avln_Complex_Layout** - Komplexere Layouts
6. **Avln_MasterDetail** - Master-Detail Pattern
7. **Avln_AnimationTiming** - Animations (aufwändiger)
8. **Avln_CustomAnimation** - Custom Animations
9. **Avln_StickyNotesDemo** - Komplexe Anwendung

## ?? Tipps

- **Designer-Support:** Avalonia hat einen XAML-Previewer, aber er unterscheidet sich von WPF
- **Hot Reload:** Funktioniert in Rider und VS 2022 (mit Avalonia Extension)
- **Styles:** Avalonia verwendet ein anderes Styling-System (ResourceDictionaries funktionieren ähnlich)
- **Themes:** FluentTheme ist modern und gut dokumentiert

---

**Version:** 1.0  
**Letzte Aktualisierung:** 15.01.2025  
**Autor:** Migration Team
