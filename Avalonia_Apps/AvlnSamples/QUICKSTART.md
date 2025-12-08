# ?? Quick Start: WPF zu Avalonia Migration - v2.0

## Schnellstart-Anleitung

### Option 1: Automatische Migration mit PowerShell (Empfohlen)

```powershell
# 1. Navigieren Sie zum AvlnSamples-Verzeichnis
cd C:\Projekte\CSharp\Avalonia_Apps\AvlnSamples

# 2. Führen Sie das verbesserte Migrations-Skript aus
.\Migrate-WpfToAvalonia.ps1 -WpfProjectName "WPF_Sample_Template"

# 3. Build das neue Projekt
dotnet build Avln_Sample_Template\Avln_Sample_Template.csproj

# 4. Prüfen Sie auf Fehler und führen Sie Post-Migration-Checks durch
# Siehe: POST_MIGRATION_CHECKLIST.md

# 5. Führen Sie die App aus
dotnet run --project Avln_Sample_Template\Avln_Sample_Template.csproj
```

### Option 2: Batch-Migration mehrerer Projekte

```powershell
# Migrieren Sie mehrere Projekte nacheinander
$projects = @(
    "WPF_MoveWindow",
    "WPF_ControlsAndLayout",
    "WPF_MasterDetail"
)

foreach ($project in $projects) {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "Migrating $project..." -ForegroundColor Cyan
    Write-Host "========================================`n" -ForegroundColor Cyan
    
    .\Migrate-WpfToAvalonia.ps1 -WpfProjectName $project
    
    $avlnProject = $project -replace "^WPF_", "Avln_"
    Write-Host "`nBuild testing $avlnProject..." -ForegroundColor Yellow
    
    $buildResult = dotnet build "$avlnProject\$avlnProject.csproj" 2>&1
    
  if ($LASTEXITCODE -eq 0) {
        Write-Host "? $avlnProject built successfully!" -ForegroundColor Green
    } else {
        Write-Host "? $avlnProject build failed - manual review needed" -ForegroundColor Red
        Write-Host "See POST_MIGRATION_CHECKLIST.md for common issues" -ForegroundColor Yellow
    }
}
```

### Option 3: Manuelle Migration (Schritt für Schritt)

Siehe [MIGRATION_GUIDE.md](./MIGRATION_GUIDE.md) für detaillierte Anweisungen.

---

## ?? Empfohlene Migrations-Reihenfolge

### ? Abgeschlossen

| # | Projekt | Status | Build | Notizen |
|---|---------|--------|-------|---------|
| 1 | Avln_Hello_World | ? | ? | Template-Projekt |
| 2 | Avln_Sample_Template | ? | ? | Getestet mit Skript v2.0 |

### Phase 1: Einfache Projekte

| # | WPF Projekt | Avalonia Projekt | Priorität | Komplexität |
|---|-------------|------------------|-----------|-------------|
| 3 | WPF_MoveWindow | Avln_MoveWindow | Hoch | Niedrig |
| 4 | WPF_ControlsAndLayout | Avln_ControlsAndLayout | Hoch | Mittel |

### Phase 2: Standard-Projekte

| # | WPF Projekt | Avalonia Projekt | Priorität | Komplexität |
|---|-------------|------------------|-----------|-------------|
| 5 | WPF_Complex_Layout | Avln_Complex_Layout | Mittel | Mittel |
| 6 | WPF_MasterDetail | Avln_MasterDetail | Mittel | Mittel |

### Phase 3: Spezial-Projekte (Animation)

| # | WPF Projekt | Avalonia Projekt | Priorität | Komplexität |
|---|-------------|------------------|-----------|-------------|
| 7 | WPF_AnimationTiming | Avln_AnimationTiming | Niedrig | Hoch |
| 8 | WPF_CustomAnimation | Avln_CustomAnimation | Niedrig | Hoch |

### Phase 4: Komplexe Anwendungen

| # | WPF Projekt | Avalonia Projekt | Priorität | Komplexität |
|---|-------------|------------------|-----------|-------------|
| 9 | WPF_StickyNotesDemo | Avln_StickyNotesDemo | Mittel | Hoch |

### Phase 5: Styling & Templates

| # | WPF Projekt | Avalonia Projekt | Priorität | Komplexität |
|---|-------------|------------------|-----------|-------------|
| 10 | WPF_AlternatingAppearanceOfItems | Avln_AlternatingAppearanceOfItems | Niedrig | Mittel |
| 11 | WPF_ContentControlStyle | Avln_ContentControlStyle | Niedrig | Mittel |
| 12 | WPF_EventTriggers | Avln_EventTriggers | Niedrig | Hoch |

### Phase 6: Grafik & Visualisierung

| # | WPF Projekt | Avalonia Projekt | Priorität | Komplexität |
|---|-------------|------------------|-----------|-------------|
| 13 | WPF_Brushes | Avln_Brushes | Niedrig | Mittel |
| 14 | WPF_Geometry | Avln_Geometry | Niedrig | Hoch |
| 15 | WPF_ImageView | Avln_ImageView | Niedrig | Niedrig |

---

## ??? Nach der Migration

### Schritt 1: Build-Test

```powershell
dotnet build <ProjektPfad>
```

**Erwartetes Ergebnis:** 0 Fehler, <10 Warnungen

### Schritt 2: Code Review

Öffnen Sie die Checkliste:
```powershell
code POST_MIGRATION_CHECKLIST.md
```

Prüfen Sie:
- [ ] App.axaml.cs DI-Konfiguration
- [ ] MainWindow.axaml View-Einbindungen
- [ ] ValueConverter Implementierungen
- [ ] Resources.resx Inhalte

### Schritt 3: Manuelle Anpassungen

**Häufigste Anpassungen:**

1. **MainWindow.axaml Navigation**
   ```xml
   <!-- Falls komplexe Navigation: -->
   <ContentControl Content="{Binding CurrentView}"/>
   ```

2. **Missing using statements in App.axaml.cs**
   ```csharp
   using YourProject.Models;
   using YourProject.Services;
   ```

3. **ValueConverter Null-Checks**
   ```csharp
 if (value == null) return null;
   ```

### Schritt 4: Runtime-Test

```powershell
dotnet run --project <ProjektPfad>
```

**DevTools:** Drücken Sie `F12` für XAML Inspector

### Schritt 5: Unit Tests (falls vorhanden)

```powershell
dotnet test <TestProjektPfad>
```

### Schritt 6: Git Commit

```powershell
git add .
git commit -m "Migrated <ProjektName> to Avalonia UI

- Converted XAML to AXAML
- Updated ViewModels and Models
- Configured DI for Avalonia
- All builds passing"
```

---

## ?? Bekannte Issues & Lösungen

### Issue 1: "Cannot find InitializeComponent"

**Lösung:**
```powershell
dotnet clean
dotnet build
```

### Issue 2: "Cannot resolve type from namespace"

**Prüfen:**
- `xmlns:vm="using:YourNamespace.ViewModels"` korrekt?
- Klasse ist `public`?
- Namespace stimmt überein?

### Issue 3: Bindings funktionieren nicht

**Debug:**
```xml
<!-- DevTools aktivieren (F12) -->
<PackageReference Condition="'$(Configuration)' == 'Debug'" 
                  Include="Avalonia.Diagnostics" Version="11.3.7" />
```

**Oder Compiled Bindings verwenden:**
```xml
<UserControl x:DataType="vm:MyViewModel">
  <TextBlock Text="{Binding MyProperty}"/>
</UserControl>
```

---

## ?? Migrations-Tracker

Verwenden Sie diese Tabelle um Fortschritt zu tracken:

```markdown
| Projekt | Skript | Build | Manual | Test | Status |
|---------|--------|-------|--------|------|--------|
| Avln_Hello_World | ? | ? | ? | ? | ? Done |
| Avln_Sample_Template | ? | ? | ? | ? | ?? Review |
| Avln_MoveWindow | ? | ? | ? | ? | ?? Pending |
```

**Legende:**
- ? Abgeschlossen
- ? In Arbeit
- ? Noch nicht begonnen
- ?? Probleme
- ?? Needs Review

---

## ?? Workflow-Diagramm

```
???????????????????
? WPF Projekt     ?
? auswählen       ?
???????????????????
  ?
  v
???????????????????
? Migrations-     ?
? Skript v2.0     ?
? ausführen       ?
???????????????????
         ?
         v
???????????????????
? dotnet build    ?
? testen          ?
???????????????????
      ?
         v
    ???????????
    ? Fehler? ?
    ???????????
         ?
    ????????????
    ?          ?
JA          NEIN
    ?          ?
    v          v
?????????  ???????????
? Post- ?? Runtime ?
? Check ?  ? Test    ?
? List  ?  ???????????
?????????    ?
    ?  v
    ?     ????????????
    ?????>? Git      ?
          ? Commit   ?
        ????????????
```

---

## ?? Nächster Schritt

**Empfehlung:** Beginnen Sie mit **WPF_MoveWindow**

```powershell
.\Migrate-WpfToAvalonia.ps1 -WpfProjectName "WPF_MoveWindow"
dotnet build Avln_MoveWindow\Avln_MoveWindow.csproj
dotnet run --project Avln_MoveWindow\Avln_MoveWindow.csproj
```

---

## ?? Dokumentation

- **MIGRATION_GUIDE.md** - Detaillierte Migrations-Anleitung
- **POST_MIGRATION_CHECKLIST.md** - Checkliste nach Migration
- **Migrate-WpfToAvalonia.ps1** - Automatisierungs-Skript v2.0

---

## ?? Pro-Tipps

### Tipp 1: Incremental Migration
Migrieren Sie ein Projekt nach dem anderen und testen Sie gründlich.

### Tipp 2: DevTools sind Ihr Freund
`F12` öffnet die Avalonia DevTools - nutzen Sie diese für Debugging!

### Tipp 3: Compiled Bindings
Verwenden Sie `x:DataType` für bessere Performance und Compile-Zeit-Checks.

### Tipp 4: Clean Builds
Bei mysterischen Fehlern: `dotnet clean && dotnet build`

### Tipp 5: Source Control
Commiten Sie nach jedem erfolgreich migrierten Projekt.

---

**Version:** 2.0  
**Letzte Aktualisierung:** 15.01.2025  
**Skript-Version:** Migrate-WpfToAvalonia.ps1 v2.0  
**Getestet mit:** Avln_Hello_World, Avln_Sample_Template
