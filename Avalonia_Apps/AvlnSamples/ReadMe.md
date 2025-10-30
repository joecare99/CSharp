# AvlnSamples - WPF to Avalonia Migration Project

## ?? Overview

This repository contains samples for migrating WPF applications to Avalonia UI, preserving MVVM, Dependency Injection, and CommunityToolkit.Mvvm.

## ??? Project Structure

```
AvlnSamples/
??? Migration/     # Migration Tools (NEW - Modular Structure)
?   ??? Migrate-WpfToAvalonia.ps1   # Main orchestrator script
?   ??? Modules/    # Reusable modules
?   ?   ??? ConversionFunctions.ps1 # C# & XAML conversion
?   ?   ??? ProjectStructure.ps1    # Directory creation
?   ?   ??? FileConverter.ps1       # File conversion logic
?   ?   ??? ProjectFiles.ps1 # .csproj, App.axaml creation
?   ?   ??? ResourceHandler.ps1     # Resources processing
?   ??? Config/
?       ??? MigrationConfig.ps1     # Central configuration
?
??? Avln_*             # Migrated Avalonia projects
?   ??? Avln_Hello_World/           # ? Template project
?   ??? Avln_Sample_Template/       # ? Sample with DI
?   ??? Avln_MoveWindow/            # ??  Needs manual fixes
?
??? WPF_*          # Original WPF projects
?
??? Libraries/
?   ??? Avln_BaseLib/        # Avalonia base library
?   ??? Avln_BaseLibTests/   # Avalonia test base
?
??? Documentation/
?   ??? MIGRATION_GUIDE.md     # Detailed migration guide
?   ??? QUICKSTART.md       # Quick start guide
?   ??? POST_MIGRATION_CHECKLIST.md # Post-migration tasks
?
??? ReadMe.md     # This file
```

## ?? Quick Start

### Prerequisites
- .NET 8 SDK or .NET 9 SDK
- PowerShell 7+ recommended
- Visual Studio 2022 or JetBrains Rider

### Migrate a Project

```powershell
# Navigate to AvlnSamples directory
cd C:\Projekte\CSharp\Avalonia_Apps\AvlnSamples

# Run migration
.\Migration\Migrate-WpfToAvalonia.ps1 -WpfProjectName "WPF_ControlsAndLayout"

# Build the result
dotnet build Avln_ControlsAndLayout\Avln_ControlsAndLayout.csproj

# Run
dotnet run --project Avln_ControlsAndLayout\Avln_ControlsAndLayout.csproj
```

## ?? Modular Migration Framework

### Benefits of Modular Approach

? **Maintainability** - Each module has a single responsibility  
? **Testability** - Modules can be tested independently  
? **Reusability** - Functions can be used in custom scripts  
? **Clarity** - Easy to understand what each part does  
? **Extensibility** - Add new features without modifying existing code  

### Module Descriptions

| Module | Responsibility | Key Functions |
|--------|----------------|---------------|
| **ConversionFunctions** | C# & XAML conversion logic | `Convert-CSharpNamespace`, `Convert-XamlToAxaml` |
| **ProjectStructure** | Directory creation & validation | `New-AvaloniaProjectStructure`, `Test-WpfProject` |
| **FileConverter** | File-by-file conversion | `Convert-CSharpFiles`, `Convert-XamlFiles` |
| **ProjectFiles** | .csproj & App files | `New-AvaloniaProjectFile`, `New-AppAxaml` |
| **ResourceHandler** | Resources & assets | `Convert-ResourceFiles`, `Copy-Assets` |
| **MigrationConfig** | Central configuration | Package versions, namespace mappings |

### Using Modules Independently

```powershell
# Load specific module
. ".\Migration\Modules\ConversionFunctions.ps1"

# Use functions directly
$csContent = Get-Content "MyFile.cs" -Raw
$converted = Convert-CSharpNamespace -Content $csContent `
    -WpfName "WPF_MyApp" -AvlnName "Avln_MyApp"
```

## ?? Migration Status

### ? Successfully Migrated

| Project | Main | Tests | Status |
|---------|------|-------|--------|
| Avln_Hello_World | ? | ? | Template - Production Ready |
| Avln_Sample_Template | ? | ? | Builds successfully |

### ?? Needs Manual Work

| Project | Issue | Resolution |
|---------|-------|------------|
| Avln_MoveWindow | WPF types (Point, Window.Left/Top) | See POST_MIGRATION_CHECKLIST.md |

### ?? Pending Migration

**Priority 1 - Simple:**
- WPF_ControlsAndLayout
- WPF_ImageView
- WPF_Brushes

**Priority 2 - Medium:**
- WPF_Complex_Layout
- WPF_MasterDetail
- WPF_Geometry

**Priority 3 - Complex:**
- WPF_AnimationTiming
- WPF_CustomAnimation
- WPF_StickyNotesDemo

## ?? Documentation

- **[MIGRATION_GUIDE.md](./Documentation/MIGRATION_GUIDE.md)** - Complete migration guide
- **[QUICKSTART.md](./Documentation/QUICKSTART.md)** - Get started quickly
- **[POST_MIGRATION_CHECKLIST.md](./Documentation/POST_MIGRATION_CHECKLIST.md)** - Post-migration tasks

## ??? Configuration

Edit `Migration/Config/MigrationConfig.ps1` to customize:

```powershell
# Package Versions
$PackageVersions = @{
    Avalonia = "11.3.7"
    # ... more versions
}

# Namespace Mappings
$NamespaceMappings = @{
  'MVVM.ViewModel' = 'Avalonia.ViewModels'
    # ... more mappings
}
```

## ?? Best Practices

1. **Start Simple** - Migrate simple projects first to test the framework
2. **Test Early** - Run builds after each migration
3. **Review Changes** - Check converted files before committing
4. **Use Version Control** - Commit after each successful migration
5. **Document Issues** - Note any manual changes needed

## ?? Troubleshooting

### Common Issues

**Issue:** Script not found  
**Solution:** Ensure you're in the `AvlnSamples` directory

**Issue:** Module not loaded  
**Solution:** Check that `Migration/Modules` directory exists

**Issue:** Build errors after migration  
**Solution:** Review POST_MIGRATION_CHECKLIST.md

### Getting Help

1. Check error messages carefully
2. Review the POST_MIGRATION_CHECKLIST.md
3. Compare with Avln_Hello_World (template)
4. Check Avalonia documentation: https://docs.avaloniaui.net/

## ?? Contribution

To add a new conversion feature:

1. Add function to appropriate module in `Migration/Modules/`
2. Update `Migration/Config/MigrationConfig.ps1` if needed
3. Test with a sample project
4. Update documentation

## ?? Version History

- **v2.2** (2025-01-15) - Modular architecture, improved maintainability
- **v2.1** - Added test project support
- **v2.0** - DI fallback, WPF type mappings
- **v1.0** - Initial migration script

## ?? Success Stories

- ? **Avln_Hello_World** - Clean migration, no manual fixes
- ? **Avln_Sample_Template** - DI configuration preserved perfectly
- ?? **Avln_MoveWindow** - Minor manual adjustments for WPF types

---

**License:** MIT  
**Author:** Migration Team  
**Last Updated:** 2025-01-15
