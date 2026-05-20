# Libraries.Package

Dieses Projekt erzeugt ein gemeinsames NuGet-Paket für die Bibliotheken dieser Solution.

## Zweck
- baut nur Bibliotheksprojekte
- schließt Testprojekte und Console-Apps aus
- sammelt die erzeugten Bibliotheks-Assemblies in einem Paket

## Verwendung
```powershell
dotnet build .\Libraries.Package\Libraries.Package.csproj -c Release -t:CreateLibrariesPackage
```

Intern wird dafür die testfreie `Libraries.Package.sln` gebaut. Danach werden nur die Bibliotheksausgaben in ein gemeinsames ZIP-Archiv unter `artifacts` geschrieben.

## Enthaltene Projekte
- BaseLib
- CommonDialogs
- CommonDialogs_net
- ConsoleDisplay
- ConsoleLib
- ConsoleLib_net
- ExtendedConsole
- GenInterfaces
- MathLibrary
- MVVM_BaseLib
- WFSystem.Windows.Data

## Nicht enthalten
- alle *Tests*-Projekte
- TestConsole und TestConsole_net

## Enthaltene Hilfsdateien
- `Libraries.Package.sln` enthält nur Bibliotheksprojekte
- `Libraries.Package.csproj` baut die testfreie Solution und archiviert die Ergebnisse
